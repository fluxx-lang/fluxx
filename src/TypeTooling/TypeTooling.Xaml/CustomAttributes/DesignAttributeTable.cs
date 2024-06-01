// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace TypeTooling.Xaml.CustomAttributes {
    /// <summary>
    /// Attribute tables are essentially read-only dictionaries, but the keys 
    /// and values are computed separately.  It is very efficient to ask an 
    /// attribute table if it contains attributes for a particular type.  
    /// The actual set of attributes is demand created. 
    /// </summary>
    public sealed class DesignAttributeTable {
        private MutableDesignAttributeTable attributes;

        //
        // Creates a new attribute table given dictionary information
        // from the attribute table builder.
        //
        internal DesignAttributeTable(MutableDesignAttributeTable attributes) {
            Debug.Assert(attributes != null);
            this.attributes = attributes;
        }

        /// <summary>
        /// Returns an enumeration of all types that have attribute overrides
        /// of some kind (on a property, on the type itself, etc).  This can be
        /// used to determine what types will be refreshed when this attribute
        /// table is added to the metadata store.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> AttributedTypeIdentifiers {
            get { return this.attributes.AttributedTypeIdentifiers; }
        }

        //
        // Returns our internal mutable table.  This is used
        // by AttributeTableBuilder's AddTable method.
        //
        internal MutableDesignAttributeTable MutableTable {
            get { return this.attributes; }
        }

        /// <summary>
        /// Returns true if this table contains any metadata for the given type.  
        /// The metadata may be class-level metadata or metadata associated with 
        /// a DepenendencyProperty or MemberDescriptor.  The AttributeStore uses 
        /// this method to identify loaded types that need a Refresh event raised 
        /// when a new attribute table is added, and to quickly decide which 
        /// tables should be further queried during attribute queries.
        /// </summary>
        /// <param name="typeIdentifier">The type to check.</param>
        /// <returns>true if the table contains attributes for the given type.</returns>
        /// <exception cref="ArgumentNullException">if type is null</exception>
        public bool ContainsAttributes(string typeIdentifier) {
            if (typeIdentifier == null) {
                throw new ArgumentNullException(nameof(typeIdentifier));
            }
            return this.attributes.ContainsAttributes(typeIdentifier);
        }

        /// <summary>
        /// Gets the names of all attributed members for the given type
        /// contained in the table.
        /// </summary>
        /// <param name="typeIdentifier">The type to check.</param>
        /// <returns>The names of the attributed members, or an empty enumerable if no attributes are found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the type parameter is null.</exception>
        public IEnumerable<string> GetAttributedMembers(string typeIdentifier) {
            if (typeIdentifier == null) {
                throw new ArgumentNullException(nameof(typeIdentifier));
            }
            return this.attributes.GetAttributedMembers(typeIdentifier);
        }

        /// <summary>
        /// Returns an enumeration of all attributes provided for the 
        /// given argument.  This will never return a null enumeration.
        /// </summary>
        /// <param name="assemblyIdentifier">The assembly to get assembly-level attributes for.</param>
        /// <returns>An enumeration of attributes.</returns>
        /// <exception cref="ArgumentNullException">if assembly is null</exception>
        public IEnumerable GetAssemblyCustomAttributes(string assemblyIdentifier) {
            if (assemblyIdentifier == null) {
                throw new ArgumentNullException(nameof(assemblyIdentifier));
            }
            return this.attributes.GetCustomAttributes(assemblyIdentifier);
        }

        /// <summary>
        /// Returns an enumeration of all attributes provided for the 
        /// given argument.  This will never return a null enumeration.
        /// </summary>
        /// <param name="typeIdentifier">The type to get class-level attributes for.</param>
        /// <returns>An enumeration of attributes.</returns>
        /// <exception cref="ArgumentNullException">if type is null</exception>
        public IEnumerable<object> GetCustomAttributes(string typeIdentifier) {
            if (typeIdentifier == null) {
                throw new ArgumentNullException(nameof(typeIdentifier));
            }
            return this.attributes.GetCustomAttributes(typeIdentifier);
        }

        /// <summary>
        /// Returns an enumeration of all attributes provided for the 
        /// given argument.  This will never return a null enumeration.
        /// </summary>
        /// <param name="ownerTypeIdentifier">The owner type of the dependency property.</param>
        /// <param name="memberName">The name of the member to provide attributes for.</param>
        /// <returns>An enumeration of attributes.</returns>
        /// <exception cref="ArgumentNullException">if ownerType or member is null</exception>
        public IEnumerable<object> GetCustomAttributes(string ownerTypeIdentifier, string memberName) {
            if (ownerTypeIdentifier == null) {
                throw new ArgumentNullException(nameof(ownerTypeIdentifier));
            }
            if (memberName == null) {
                throw new ArgumentNullException(nameof(memberName));
            }
            return this.attributes.GetCustomAttributes(ownerTypeIdentifier, memberName);
        }
    }
}
