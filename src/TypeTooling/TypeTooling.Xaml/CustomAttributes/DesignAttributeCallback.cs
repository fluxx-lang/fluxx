// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

namespace TypeTooling.Xaml.CustomAttributes {
    /// <summary>
    /// Called when attributes are needed for a type.
    /// </summary>
    /// <param name="builder">A builder that can be used to 
    /// add attributes.  AttributeCallbackBuilders can only build attributes
    /// for the type that is requesting metadata.</param>
    public delegate void DesignAttributeCallback(DesignAttributeCallbackBuilder builder);
}
