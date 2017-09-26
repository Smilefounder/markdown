using Newtonsoft.Json;

namespace MarkdigEngine.Extensions
{
    internal class MarkdownSytleConfig
    {
        public const string MarkdownStyleFileName = "md.style";

        [JsonProperty("metadataRules")]
        public MarkdownMetadataValidationRule[] MetadataRules { get; set; }
        [JsonProperty("rules")]
        public MarkdownValidationRule[] Rules { get; set; }
        [JsonProperty("tagRules")]
        public MarkdownTagValidationRule[] TagRules { get; set; }
        [JsonProperty("settings")]
        public MarkdownValidationSetting[] Settings { get; set; }
    }
}
