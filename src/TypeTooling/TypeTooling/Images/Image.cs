namespace TypeTooling.Images
{
    public abstract class Image
    {
        protected Image(string? automationName)
        {
            this.AutomationName = automationName;
        }

        private string? AutomationName { get; }
    }
}
