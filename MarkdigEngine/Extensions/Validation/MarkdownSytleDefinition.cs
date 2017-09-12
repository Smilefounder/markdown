using System.Collections.Generic;

using Newtonsoft.Json;

namespace MarkdigEngine
{
    public class MarkdownSytleDefinition
    {
        public const string MarkdownStyleDefinitionFilePostfix = ".md.style";
        public const string MarkdownStyleDefinitionFolderName = "md.styles";

        [JsonProperty("metadataRules")]
        public Dictionary<string, MarkdownMetadataValidationRule> MetadataRules { get; set; }
        [JsonProperty("rules")]
        public Dictionary<string, MarkdownValidationRule> Rules { get; set; }
        [JsonProperty("tagRules")]
        public Dictionary<string, MarkdownTagValidationRule> TagRules { get; set; }
    }
}
