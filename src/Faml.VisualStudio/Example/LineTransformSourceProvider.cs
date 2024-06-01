//***************************************************************************
// 
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;

namespace Faml.VisualStudio.Example {
    /// <summary>
    /// This class implements a connector that produces the Faml LineTransformSourceProvider, used to make
    /// source lines taller if needed to render examples.
    /// </summary>
    [Export(typeof(ILineTransformSourceProvider))]
    [ContentType(FamlPackage.FamlContentType)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class LineTransformSourceProvider : ILineTransformSourceProvider {
        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public ILineTransformSource Create(IWpfTextView textView) {
            return textView.Properties.GetOrCreateSingletonProperty(delegate {
                var famlModuleBuffer = FamlModuleBuffer.GetOrCreateFromTextBuffer(textView.TextBuffer);
                return new LineTransformSource(famlModuleBuffer);
            });
        }
    }
}

