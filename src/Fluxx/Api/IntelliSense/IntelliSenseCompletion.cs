using System;
using TypeTooling.Images;

namespace Faml.Api.IntelliSense
{
    [Serializable]
    public class IntelliSenseCompletion
    {
        public CompletionType Type { get; }
        public string DisplayText { get; }
        public string? InsertText { get; }
        public Image? Icon { get; }

        /// <summary>
        /// The data is sent back in the GetDescriptionAsync call.
        /// </summary>
        public object? Data { get; }

        public IntelliSenseCompletion(CompletionType type, string displayText, string? insertText = null, Image? icon = null, object? data = null)
        {
            this.Type = type;
            this.DisplayText = displayText;

            if (insertText == null)
            {
                insertText = displayText;
            }

            this.InsertText = insertText;

            this.Icon = icon;

            this.Data = data;
        }
    }
}
