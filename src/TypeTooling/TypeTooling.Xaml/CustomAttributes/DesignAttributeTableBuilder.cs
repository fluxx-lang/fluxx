// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;

namespace TypeTooling.Xaml.CustomAttributes {
    /// <summary>
    /// An attribute table is a read only blob of data.  How 
    /// do you create one?  We will have a class called an 
    /// Attribute Builder that can be used to create an attribute 
    /// table.  Attribute builders have methods you can call to 
    /// add metadata.  When you?re finished, you can produce an 
    /// attribute table from the builder.  Builder methods also 
    /// support callback delegates so the entire process can be 
    /// deferred until needed.
    /// </summary>
    public class DesignAttributeTableBuilder {
        private MutableDesignAttributeTable table = new MutableDesignAttributeTable();
        private bool cloneOnUse;

        //
        // Returns an attribute table we can make changes to
        //
        private MutableDesignAttributeTable MutableTable {
            get {
                if (this.cloneOnUse) {
                    MutableDesignAttributeTable clone = new MutableDesignAttributeTable();
                    clone.AddTable(this.table);
                    this.table = clone;
                    this.cloneOnUse = false;
                }

                return this.table;
            }
        }

        /// <summary>
        /// Adds a callback that will be invoked when metadata for the 
        /// given type is needed.  The callback can add metadata to
        /// to the attribute table on demand, which is much more efficient
        /// than adding metadata up front.
        /// </summary>
        /// <param name="type">The type this callback is added for.</param>
        /// <param name="callback">The callback to add metadata for this type.</param>
        public void AddCallback(string typeIdentifier, DesignAttributeCallback callback) {
            if (typeIdentifier == null) {
                throw new ArgumentNullException(nameof(typeIdentifier));
            }
            if (callback == null) {
                throw new ArgumentNullException(nameof(callback));
            }
            this.MutableTable.AddCallback(typeIdentifier, callback);
        }

        /// <summary>
        /// Adds the contents of the provided attributes to this builder for
        /// the given assembly.
        /// Conflicts are resolved with a last-in-wins strategy.
        /// </summary>
        /// <param name="assembly">The assembly to add assembly-level attributes to.</param>
        /// <param name="attributes">
        /// The new attributes to add.
        /// </param>
        /// <exception cref="ArgumentNullException">if assembly or attributes is null</exception>
        public void AddAssemblyCustomAttributes(string assemblyIdentifier, params Attribute[] attributes) {
            if (assemblyIdentifier == null) {
                throw new ArgumentNullException(nameof(assemblyIdentifier));
            }
            if (attributes == null) {
                throw new ArgumentNullException(nameof(attributes));
            }
            this.MutableTable.AddAssemblyCustomAttributes(assemblyIdentifier, attributes);
        }

        /// <summary>
        /// Adds the contents of the provided attributes to this builder.  
        /// Conflicts are resolved with a last-in-wins strategy.  When 
        /// building a large attribute table it is best to use AddCallback 
        /// to defer the work of creating attributes until they are needed.
        /// </summary>
        /// <param name="type">The type to add class-level attributes to.</param>
        /// <param name="attributes">
        /// The new attributes to add.
        /// </param>
        /// <exception cref="ArgumentNullException">if type or attributes is null</exception>
        public void AddCustomAttributes(string typeIdentifier, params Attribute[] attributes) {
            if (typeIdentifier == null) {
                throw new ArgumentNullException(nameof(typeIdentifier));
            }
            if (attributes == null) {
                throw new ArgumentNullException(nameof(attributes));
            }
            this.MutableTable.AddCustomAttributes(typeIdentifier, attributes);
        }

        /// <summary>
        /// Adds attributes to the member with the given name.  The member can be a property
        /// or an event.  The member is evaluated on demand when the user queries
        /// attributes on a given property or event.
        /// </summary>
        /// <param name="ownerType">
        /// The type the member lives on.
        /// </param>
        /// <param name="memberName">
        /// The member to add attributes for.  For attached properties or events, the
        /// member name is the name of the "Get" method or "Add" handler.
        /// </param>
        /// <param name="attributes">
        /// The new attributes to add.
        /// </param>
        public void AddCustomAttributes(string ownerTypeIdentifier, string memberName, params Attribute[] attributes) {
            if (ownerTypeIdentifier == null) {
                throw new ArgumentNullException(nameof(ownerTypeIdentifier));
            }
            if (memberName == null) {
                throw new ArgumentNullException(nameof(memberName));
            }
            if (attributes == null) {
                throw new ArgumentNullException(nameof(attributes));
            }
            this.MutableTable.AddCustomAttributes(ownerTypeIdentifier, memberName, attributes);
        }

        /// <summary>
        /// Adds the contents of the provided attribute table to 
        /// this builder.  Conflicts are resolved with a last-in-wins 
        /// strategy. 
        /// </summary>
        /// <param name="table">An existing attribute table.</param>
        /// <exception cref="ArgumentNullException">if table is null</exception>
        public void AddTable(DesignAttributeTable table) {
            if (table == null) {
                throw new ArgumentNullException(nameof(table));
            }
            this.MutableTable.AddTable(table.MutableTable);
        }

        /// <summary>
        /// Creates an attribute table that contains all of the attribute 
        /// definitions provided through AddAttribute calls.  The table is 
        /// a snapshot of the current state of the attribute builder; any 
        /// subsequent AddAttribute calls are not included in the table.
        /// 
        /// If callback methods were used to declare attributes, those methods 
        /// will not be evaluated during CreateTable.  Instead, the table will 
        /// contain those callbacks and will evaluate them as needed.
        /// </summary>
        /// <returns>
        /// An attribute table that can be passed to the metadata store.
        /// </returns>
        public DesignAttributeTable CreateTable() {
            this.cloneOnUse = true;
            return new DesignAttributeTable(this.table);
        }
    }
}
