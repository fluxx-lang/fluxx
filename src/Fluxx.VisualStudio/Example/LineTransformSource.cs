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

using Faml.Syntax;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace Faml.VisualStudio.Example {
    public class LineTransformSource : ILineTransformSource {
        private readonly FamlModuleBuffer _famlBuffer;

        public LineTransformSource(FamlModuleBuffer famlBuffer) {
            _famlBuffer = famlBuffer;
        }

        /// <summary>Add additional height below example lines, when needed, to provide room to draw the example adornment</summary>
        /// <returns>The line transform for that line.</returns>
        /// <param name="line">The line for which to calculate the line transform</param>
        /// <param name="yPosition">The y-coordinate of the line</param>
        /// <param name="placement">The placement of the line with respect to <paramref name="yPosition" /></param>
        public LineTransform GetLineTransform(ITextViewLine line, double yPosition, ViewRelativePosition placement) {
            if (line.IsLastTextViewLineForSnapshotLine) {
                //int lineNumber = line.Snapshot.GetLineNumberFromPosition(line.Start);

                ExampleDefinitionSyntax exampleDefinition = _famlBuffer.FamlModule?.ModuleSyntax.GetExampleDefinitionAtSourcePosition(line.Start);
                if (exampleDefinition != null) {
                    int exampleEndPosition = exampleDefinition.Span.End;
                    if (exampleEndPosition <= line.EndIncludingLineBreak) {
                        int exampleIndex = exampleDefinition.ExampleIndex;

                        RenderedExample renderedExample = _famlBuffer.ExampleManager.Get(exampleIndex);
                        if (renderedExample == null || ! renderedExample.IsPresentationAvailable)
                            renderedExample = _famlBuffer.ExampleManager.GetPrevious(exampleIndex);

                        if (renderedExample != null) {
                            double height = renderedExample.HeightNeeded;
                            if (height > 0.0)
                                 return new LineTransform(0.0, height, 1.0);
                        }
                    }
                }
            }

            // No transformation needed
            return new LineTransform(1.0);
        }
    }
}
