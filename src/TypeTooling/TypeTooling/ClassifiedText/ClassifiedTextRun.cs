namespace TypeTooling.ClassifiedText
{
    public class ClassifiedTextRun
    {
        public ClassifiedTextRun(string classificationTypeName, string text)
        {
            this.ClassificationTypeName = classificationTypeName;
            this.Text = text;
        }

        public string ClassificationTypeName { get; }

        public string Text { get; }
    }
}
