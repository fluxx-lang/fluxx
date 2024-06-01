// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TypeTooling.Xaml.CustomAttributes {
    //
    // This class is used by the attribute table builder to
    // add attributes.  It is then handed to AttributeTable
    // and accessed in a read-only fashion.
    //
    internal class MutableDesignAttributeTable {
        private Dictionary<string, AttributeList> assemblyIdentifierAttributes;
        private Dictionary<string, TypeMetadata> typeIdentifierMetadata;
        private object syncLock = new object();
        private static object[] empty = new object[0];

        internal MutableDesignAttributeTable() {
            this.assemblyIdentifierAttributes = new Dictionary<string, AttributeList>();
            this.typeIdentifierMetadata = new Dictionary<string, TypeMetadata>();
        }

        //
        // Returns the types we're handing metadata for
        //
        internal IEnumerable<string> AttributedTypeIdentifiers {
            get { return this.typeIdentifierMetadata.Keys; }
        }

        //
        // Private helper to add a portion of an existing table.
        //
        private static void AddAttributeMetadata(TypeMetadata newMetadata, TypeMetadata existingMetadata) {
            if (newMetadata.TypeAttributes != null) {
                if (existingMetadata.TypeAttributes != null) {
                    existingMetadata.TypeAttributes.AddRange(newMetadata.TypeAttributes);
                }
                else {
                    existingMetadata.TypeAttributes = newMetadata.TypeAttributes;
                }
            }
        }

        //
        // Helper to add a enum of attributes ot an existing list
        //
        private static void AddAttributes(AttributeList list, IEnumerable<object> attributes) {
            // Attributes are ordered so those at the end of the
            // list take precedence over those at the front.
            list.AddRange(attributes);
        }

        internal void AddCallback(string typeIdentifier, DesignAttributeCallback callback) {
            Debug.Assert(typeIdentifier != null && callback != null);
            AttributeList list = this.GetTypeList(typeIdentifier);
            list.Add(callback);
        }

        internal void AddAssemblyCustomAttributes(string assemblyIdentifier, IEnumerable<object> attributes) {
            Debug.Assert(assemblyIdentifier != null && attributes != null);
            MutableDesignAttributeTable.AddAttributes(this.GetAssemblyList(assemblyIdentifier), attributes);
        }

        //
        // Adds custom attrs for a type
        //
        internal void AddCustomAttributes(string typeIdentifier, IEnumerable<object> attributes) {
            Debug.Assert(typeIdentifier != null && attributes != null);
            MutableDesignAttributeTable.AddAttributes(this.GetTypeList(typeIdentifier), attributes);
        }

        //
        // Adds custom attrs for a member name
        //
        internal void AddCustomAttributes(string ownerTypeIdentifier, string memberName, IEnumerable<object> attributes) {
            Debug.Assert(ownerTypeIdentifier != null && memberName != null && attributes != null);
            MutableDesignAttributeTable.AddAttributes(this.GetMemberList(ownerTypeIdentifier, memberName), attributes);
        }

        //
        // Private helper to add a portion of an existing table.
        //
        private static void AddMemberMetadata(TypeMetadata newMetadata, TypeMetadata existingMetadata) {
            if (newMetadata.MemberAttributes != null) {
                if (existingMetadata.MemberAttributes != null) {
                    foreach (KeyValuePair<string, AttributeList> kv in newMetadata.MemberAttributes) {
                        AttributeList existing;
                        if (existingMetadata.MemberAttributes.TryGetValue(kv.Key, out existing)) {
                            existing.AddRange(kv.Value);
                        }
                        else {
                            existingMetadata.MemberAttributes.Add(kv.Key, kv.Value);
                        }
                    }
                }
                else {
                    existingMetadata.MemberAttributes = newMetadata.MemberAttributes;
                }
            }
        }

        //
        // Adds an existing table.
        //
        internal void AddTable(MutableDesignAttributeTable table) {
            Debug.Assert(table != null);
            foreach (KeyValuePair<string, TypeMetadata> kv in table.typeIdentifierMetadata) {
                this.AddTypeMetadata(kv.Key, kv.Value);
            }
            foreach (KeyValuePair<string, AttributeList> kv in table.assemblyIdentifierAttributes) {
                this.AddAssemblyMetadata(kv.Key, kv.Value);
            }
        }

        //
        // Private helper to add a portion of an existing table.
        //
        private void AddTypeMetadata(string typeIdentifier, TypeMetadata md) {
            TypeMetadata existing;
            if (this.typeIdentifierMetadata.TryGetValue(typeIdentifier, out existing)) {
                MutableDesignAttributeTable.AddAttributeMetadata(md, existing);
                MutableDesignAttributeTable.AddMemberMetadata(md, existing);
            }
            else {
                this.typeIdentifierMetadata.Add(typeIdentifier, md);
            }
        }

        //
        // Private helper to add assembly attributes to an existing
        // table
        //
        private void AddAssemblyMetadata(string assemblyIdentifier, AttributeList attributes) {
            AttributeList existing;
            if (this.assemblyIdentifierAttributes.TryGetValue(assemblyIdentifier, out existing)) {
                if (!attributes.IsExpanded) {
                    existing.IsExpanded = false;
                }
                existing.AddRange(attributes);
            }
            else {
                this.assemblyIdentifierAttributes.Add(assemblyIdentifier, attributes);
            }
        }

        //
        // Returns true if this table contains attributes for the
        // given type
        //
        internal bool ContainsAttributes(string typeIdentifier) {
            Debug.Assert(typeIdentifier != null);
            return (this.typeIdentifierMetadata.ContainsKey(typeIdentifier));
        }

        //
        // Helper method that walks through an attribute list and expands all callbacks
        // within it.  Callers should lock before calling this because it
        // modifies the provided attribute list.
        //
        private void ExpandAttributes(string typeIdentifier, AttributeList attributes) {
            if (!attributes.IsExpanded) {
                // First, expand all the callbacks. This may add more attributes into our list
                for (int idx = 0; idx < attributes.Count; idx++) {
                    DesignAttributeCallback? callback = attributes[idx] as DesignAttributeCallback;
                    while (callback != null) {
                        attributes.RemoveAt(idx);
                        DesignAttributeCallbackBuilder builder = new DesignAttributeCallbackBuilder(this, typeIdentifier);
                        callback(builder);

                        if (idx < attributes.Count) {
                            callback = attributes[idx] as DesignAttributeCallback;
                        }
                        else {
                            callback = null;
                        }
                    }
                }
            }
        }

        internal IEnumerable<string> GetAttributedMembers(string typeIdentifier) {
            Debug.Assert(typeIdentifier != null);
            TypeMetadata md;
            if (!this.typeIdentifierMetadata.TryGetValue(typeIdentifier, out md) || md?.MemberAttributes == null) {
                return Enumerable.Empty<string>();
            }
            return md.MemberAttributes.Keys;
        }

        //
        // Returns custom attributes for the assembly.
        //
        internal IEnumerable GetAssemblyCustomAttributes(string assemblyIdentifier) {
            Debug.Assert(assemblyIdentifier != null);
            // Assemblies can't have attribute callback builders:they're
            // always expanded.
            AttributeList list;
            if (this.assemblyIdentifierAttributes.TryGetValue(assemblyIdentifier, out list)) {
                return list.AsReadOnly();
            }
            return MutableDesignAttributeTable.empty;
        }

        //
        // Returns custom attributes for the type.
        //
        internal IEnumerable<object> GetCustomAttributes(string typeIdentifier) {
            Debug.Assert(typeIdentifier != null);

            AttributeList? attributes = GetExpandedAttributes(typeIdentifier, null, delegate (string typeToGet, object? callbackParam) {
                TypeMetadata md;
                if (this.typeIdentifierMetadata.TryGetValue(typeToGet, out md)) {
                    return md.TypeAttributes;
                }
                return null;
            });

            if (attributes != null) {
                return attributes.AsReadOnly();
            }

            return MutableDesignAttributeTable.empty;
        }

        //
        // Returns custom attributes for the member.
        //
        internal IEnumerable<object> GetCustomAttributes(string ownerTypeIdentifier, string memberName) {
            Debug.Assert(ownerTypeIdentifier != null && memberName != null);

            AttributeList? attributes = GetExpandedAttributes(ownerTypeIdentifier, memberName, delegate (string typeToGet, object? callbackParam) {
                string name = (string)callbackParam!;
                TypeMetadata md;

                if (this.typeIdentifierMetadata.TryGetValue(typeToGet, out md)) {

                    // If member attributes are null but type attributes are not,
                    // it is possible that expanding type attributes could cause
                    // member attributes to be added.  Check.

                    if (md.MemberAttributes == null && md.TypeAttributes != null && !md.TypeAttributes.IsExpanded) {
                        lock (this.syncLock) {
                            this.ExpandAttributes(ownerTypeIdentifier, md.TypeAttributes);
                            md.TypeAttributes.IsExpanded = true;
                        }
                    }

                    if (md.MemberAttributes != null) {
                        AttributeList list;
                        if (md.MemberAttributes.TryGetValue(name, out list)) {
                            return list;
                        }
                    }
                }

                return null;
            });

            if (attributes != null) {
                return attributes.AsReadOnly();
            }

            return MutableDesignAttributeTable.empty;
        }

        //
        // Helper to demand create the attribute list for a dependency property.
        //
        private AttributeList GetMemberList(string ownerTypeIdentifier, string memberName) {
            Debug.Assert(ownerTypeIdentifier != null && memberName != null);
            TypeMetadata md = this.GetTypeMetadata(ownerTypeIdentifier);

            if (md.MemberAttributes == null) {
                md.MemberAttributes = new Dictionary<string, AttributeList>();
            }

            AttributeList list;
            if (!md.MemberAttributes.TryGetValue(memberName, out list)) {
                list = new AttributeList();
                md.MemberAttributes.Add(memberName, list);
            }

            return list;
        }

        //
        // Returns the attribute list for the assembly
        //
        private AttributeList GetAssemblyList(string assemblyIdentifier) {
            AttributeList list;
            if (!this.assemblyIdentifierAttributes.TryGetValue(assemblyIdentifier, out list)) {
                list = new AttributeList();
                this.assemblyIdentifierAttributes.Add(assemblyIdentifier, list);
            }
            return list;
        }

        //
        // Expands a type attribute table for use.
        // Attribute tables only contain attributes for
        // the given type, and may have callbacks embedded
        // within them.
        //
        private AttributeList? GetExpandedAttributes(string typeIdentifier, object? callbackParam, GetTypeIdentifierAttributesCallback callback) {
            // Do we have attributes to expand?
            AttributeList? attributes = callback(typeIdentifier, callbackParam);
            if (attributes != null) {
                // If these attributes haven't been expanded yet, do that now.
                if (!attributes.IsExpanded) {
                    // We have a lock here because multiple people could be
                    // surfing type information at the same time from multiple
                    // threads.  While we are read only once we are expanded,
                    // we do modify the list here to expand the callbacks and 
                    // merge.  Therefore, we need to acquire a lock.
                    lock (attributes) {
                        if (!attributes.IsExpanded) {
                            lock (this.syncLock) {
                                this.ExpandAttributes(typeIdentifier, attributes);
                                attributes.IsExpanded = true;
                            }
                        }
                    }
                }
            }

            return attributes;
        }

        //
        // Helper to demand create the attribute list for a type.
        //
        private AttributeList GetTypeList(string typeIdentifier) {
            Debug.Assert(typeIdentifier != null);
            TypeMetadata md = this.GetTypeMetadata(typeIdentifier);
            if (md.TypeAttributes == null) {
                md.TypeAttributes = new AttributeList();
            }
            return md.TypeAttributes;
        }

        //
        // Helper to demand create the type metadata.
        //
        private TypeMetadata GetTypeMetadata(string typeIdentifier) {
            Debug.Assert(typeIdentifier != null);
            TypeMetadata md;
            if (!this.typeIdentifierMetadata.TryGetValue(typeIdentifier, out md)) {
                md = new TypeMetadata();
                this.typeIdentifierMetadata.Add(typeIdentifier, md);
            }
            return md;
        }

        //
        // All metadata for a type is stored here.
        //
        private class TypeMetadata {
            internal AttributeList TypeAttributes;
            internal Dictionary<string, AttributeList> MemberAttributes;
        }

        //
        // Individual attributes for a member or type are stored
        // here.  Attribute lists can be "expanded", so their
        // callbacks are evaluated and their attributes are
        // merged with their base attribute list.
        //
        private class AttributeList : List<object> {
            internal bool IsExpanded {
                get; set;
            }
        }

        //
        // We have a generic attribute expansion routine
        // that relies on someone else providing a mechanism
        // for returning the base attribute list.  If there
        // is no base list, this callback can return null.
        //
        private delegate AttributeList? GetTypeIdentifierAttributesCallback(string typeIdentifier, object? callbackParam);
    }
}
