// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;

namespace TypeTooling.Xaml.CustomAttributes {
    /// <summary>
    /// An instance of this class is passed to callback delegates to lazily
    /// populate the attributes for a type.
    /// </summary>
    public sealed class DesignAttributeCallbackBuilder {
        private MutableDesignAttributeTable table;
        private string callbackTypeIdentifier;

        internal DesignAttributeCallbackBuilder(MutableDesignAttributeTable table, string callbackTypeIdentifier) {
            this.table = table;
            this.callbackTypeIdentifier = callbackTypeIdentifier;
        }

        /// <summary>
        /// The type this callback is being invoked for.
        /// </summary>
        public string CallbackTypeIdentifier {
            get { return this.callbackTypeIdentifier; }
        }

        /// <summary>
        /// Adds the contents of the provided attributes to this builder.  
        /// Conflicts are resolved with a last-in-wins strategy.
        /// </summary>
        /// <param name="attributes">The new attributes to add.</param>
        /// <exception cref="ArgumentNullException">if type or attributes is null</exception>
        public void AddCustomAttributes(params Attribute[] attributes) {
            if (attributes == null) {
                throw new ArgumentNullException(nameof(attributes));
            }
            this.table.AddCustomAttributes(this.callbackTypeIdentifier, attributes);
        }

        /// <summary>
        /// Adds attributes to the member with the given name.  The member can be a property
        /// or an event.  The member is evaluated on demand when the user queries
        /// attributes on a given property or event.
        /// </summary>
        /// <param name="memberName">
        /// The member to add attributes for.  Only property and event members are supported;
        /// all others will be ignored.  For attached properties and events, use the
        /// owner type and the property or event name.
        /// </param>
        /// <param name="attributes">The new attributes to add.</param>
        public void AddCustomAttributes(string memberName, params Attribute[] attributes) {
            if (memberName == null) {
                throw new ArgumentNullException(nameof(memberName));
            }
            if (attributes == null) {
                throw new ArgumentNullException(nameof(attributes));
            }
            this.table.AddCustomAttributes(this.callbackTypeIdentifier, memberName, attributes);
        }
    }
}
