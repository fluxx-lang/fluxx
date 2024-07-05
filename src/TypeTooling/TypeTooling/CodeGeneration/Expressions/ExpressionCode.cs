namespace TypeTooling.CodeGeneration.Expressions
{
    public class ExpressionCode
    {
        private string? _comment = null;

        public string? Comment
        {
            get => this._comment;
            set => this._comment = value;
        }
    }
}
