namespace TypeTooling.ClassifiedText
{
    public class ClassifiedTextRun {
        public ClassifiedTextRun(string classificationTypeName, string text) {
            ClassificationTypeName = classificationTypeName;
            Text = text;
        }

        public string ClassificationTypeName { get; }

        public string Text { get; }
    }
}
