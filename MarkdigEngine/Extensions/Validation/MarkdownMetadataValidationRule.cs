using System.ComponentModel;

using Newtonsoft.Json;

namespace MarkdigEngine
{
    public class MarkdownMetadataValidationRule
    {
        /// <summary>
        /// The contract name of rule.
        /// </summary>
        [JsonProperty("contractName", Required = Required.Always)]
        public string ContractName { get; set; }

        /// <summary>
        /// Whether to disable this rule by default.
        /// </summary>
        [DefaultValue(false)]
        [JsonProperty("disable")]
        public bool Disable { get; set; }

        public static explicit operator MarkdownMetadataValidationRule(string contractName)
        {
            return new MarkdownMetadataValidationRule { ContractName = contractName };
        }
    }
}