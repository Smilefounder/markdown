using Newtonsoft.Json;

namespace MarkdigEngine
{
    internal class MarkdownSytleConfig
    {
        public const string MarkdownStyleFileName = "md.style";

        [JsonProperty("tagRules")]
        public MarkdownTagValidationRule[] TagRules { get; set; }

        [JsonProperty("settings")]
        public MarkdownValidationSetting[] Settings { get; set; }
    }
}
