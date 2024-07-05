namespace TypeTooling.Images
{
    public class PngImage : Image
    {
        public PngImage(byte[] pngData, string? automationName) : base(automationName)
        {
            this.PngData = pngData;
        }

        public byte[] PngData { get; }
    }
}
