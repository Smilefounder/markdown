using System.Collections.Generic;

using Newtonsoft.Json;

namespace MarkdigEngine
{
    internal class MarkdownSytleDefinition
    {
        public const string MarkdownStyleDefinitionFilePostfix = ".md.style";
        public const string MarkdownStyleDefinitionFolderName = "md.styles";

        [JsonProperty("tagRules")]
        public Dictionary<string, MarkdownTagValidationRule> TagRules { get; set; }
    }
}
