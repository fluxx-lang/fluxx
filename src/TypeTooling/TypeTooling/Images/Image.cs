namespace TypeTooling.Images
{
    public abstract class Image
    {
        protected Image(string? automationName)
        {
            AutomationName = automationName;
        }

        private string? AutomationName { get; }
    }
}
