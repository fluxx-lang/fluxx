using System.Collections.Generic;
using TypeTooling.ClassifiedText;
using VSAdornments = Microsoft.VisualStudio.Text.Adornments;

namespace Fluxx.VisualStudio {
    public static class VsClassifiedTextConverter {
        public static object ToVsContent(ClassifiedTextMarkup classifiedTextMarkup) {
            return ToVs(classifiedTextMarkup.ContainerElement);
        }

        public static VSAdornments.ContainerElement ToVs(ContainerElement containerElement) {
            List<object> vsElements = new List<object>();
            foreach (object element in containerElement.Elements) {
                if (element is ClassifiedTextElement textElement)
                    vsElements.Add(ToVs(textElement));
            }

            return new VSAdornments.ContainerElement(ToVs(containerElement.Style), vsElements);
        }

        public static VSAdornments.ContainerElementStyle ToVs(ContainerElementStyle style) {
            VSAdornments.ContainerElementStyle vsStyle = 0;
            if ((style & ContainerElementStyle.Stacked) != 0)
                vsStyle |= VSAdornments.ContainerElementStyle.Stacked;
            if ((style & ContainerElementStyle.VerticalPadding) != 0)
                vsStyle |= VSAdornments.ContainerElementStyle.VerticalPadding;
            return vsStyle;
        }

        public static VSAdornments.ClassifiedTextElement ToVs(ClassifiedTextElement textElement) {
            List<VSAdornments.ClassifiedTextRun> vsRuns = new List<VSAdornments.ClassifiedTextRun>();
            foreach (ClassifiedTextRun run in textElement.Runs)
                vsRuns.Add(ToVs(run));

            return new VSAdornments.ClassifiedTextElement(vsRuns);
        }

        public static VSAdornments.ClassifiedTextRun ToVs(ClassifiedTextRun textRun) =>
            new VSAdornments.ClassifiedTextRun(textRun.ClassificationTypeName, textRun.Text);
    }
}
