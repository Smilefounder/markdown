using System;
using System.Collections.Generic;
using System.IO;

using Markdig.Parsers;
using Markdig.Syntax;
using Microsoft.DocAsCode.Plugins;
using Microsoft.DocAsCode.Common;
using System.Collections.Immutable;

namespace MarkdigEngine
{
    internal class MarkdownValidatorBuilder
    {
        private readonly List<RuleWithId<MarkdownTagValidationRule>> _tagValidators =
            new List<RuleWithId<MarkdownTagValidationRule>>();
        private readonly Dictionary<string, MarkdownValidationRule> _globalValidators =
            new Dictionary<string, MarkdownValidationRule>();
        private readonly List<MarkdownValidationSetting> _settings =
            new List<MarkdownValidationSetting>();

        public const string DefaultValidatorName = "default";
        public const string MarkdownValidatePhaseName = "Markdown style";

        public ICompositionContainer Container { get; }

        public static MarkdownValidatorBuilder Create(ICompositionContainer container, string baseDir, string templateDir)
        {
            var builder = new MarkdownValidatorBuilder(container);
            LoadValidatorConfig(baseDir, templateDir, builder);
            return builder;
        }

        public MarkdownValidatorBuilder(ICompositionContainer container)
        {
            Container = container;
        }

        public ProcessDocumentDelegate CreateValidation()
        {
            return document => Validate(document);
        }

        public void AddTagValidators(MarkdownTagValidationRule[] validators)
        {
            if (validators == null)
            {
                return;
            }

            foreach (var item in validators)
            {
                _tagValidators.Add(new RuleWithId<MarkdownTagValidationRule>
                {
                    Category = null,
                    Id = null,
                    Rule = item
                });
            }
        }

        public void AddTagValidators(string category, Dictionary<string, MarkdownTagValidationRule> validators)
        {
            if (validators == null)
            {
                return;
            }

            foreach (var pair in validators)
            {
                _tagValidators.Add(new RuleWithId<MarkdownTagValidationRule>
                {
                    Category = category,
                    Id = pair.Key,
                    Rule = pair.Value,
                });
            }
        }

        public void AddSettings(MarkdownValidationSetting[] settings)
        {
            if (settings == null)
            {
                return;
            }
            foreach (var setting in settings)
            {
                _settings.Add(setting);
            }
        }

        public void EnsureDefaultValidator()
        {
            if (!_globalValidators.ContainsKey(DefaultValidatorName))
            {
                _globalValidators[DefaultValidatorName] = new MarkdownValidationRule
                {
                    ContractName = DefaultValidatorName,
                };
            }
        }

        private void Validate(MarkdownDocument document)
        {
            var tagRules = GetEnabledTagRules().ToImmutableList();
            var validator = new MarkdownValidator(Container, tagRules);
            validator.Validate(document);
        }

        private static void LoadValidatorConfig(string baseDir, string templateDir, MarkdownValidatorBuilder builder)
        {
            if (string.IsNullOrEmpty(baseDir))
            {
                return;
            }

            if (templateDir != null)
            {
                var configFolder = Path.Combine(templateDir, MarkdownSytleDefinition.MarkdownStyleDefinitionFolderName);
                if (Directory.Exists(configFolder))
                {
                    LoadValidatorDefinition(configFolder, builder);
                }
            }

            var configFile = Path.Combine(baseDir, MarkdownSytleConfig.MarkdownStyleFileName);
            if (EnvironmentContext.FileAbstractLayer.Exists(configFile))
            {
                var config = JsonUtility.Deserialize<MarkdownSytleConfig>(configFile);
                builder.AddTagValidators(config.TagRules);
                builder.AddSettings(config.Settings);
            }
            builder.EnsureDefaultValidator();
        }

        private static void LoadValidatorDefinition(string mdStyleDefPath, MarkdownValidatorBuilder builder)
        {
            if (Directory.Exists(mdStyleDefPath))
            {
                foreach (var configFile in Directory.GetFiles(mdStyleDefPath, "*" + MarkdownSytleDefinition.MarkdownStyleDefinitionFilePostfix))
                {
                    var fileName = Path.GetFileName(configFile);
                    var category = fileName.Remove(fileName.Length - MarkdownSytleDefinition.MarkdownStyleDefinitionFilePostfix.Length);
                    var config = JsonUtility.Deserialize<MarkdownSytleDefinition>(configFile);
                    builder.AddTagValidators(category, config.TagRules);
                }
            }
        }

        private IEnumerable<MarkdownTagValidationRule> GetEnabledTagRules()
        {
            foreach (var item in _tagValidators)
            {
                if (IsDisabledBySetting(item) ?? item.Rule.Disable)
                {
                    continue;
                }
                yield return item.Rule;
            }
        }

        private bool? IsDisabledBySetting<T>(RuleWithId<T> item)
        {
            bool? categoryDisable = null;
            bool? idDisable = null;
            if (item.Category != null)
            {
                foreach (var setting in _settings)
                {
                    if (setting.Category == item.Category)
                    {
                        if (setting.Id == null)
                        {
                            categoryDisable = setting.Disable;
                        }
                        else if (setting.Id == item.Id)
                        {
                            idDisable = setting.Disable;
                        }
                    }
                }
            }
            return idDisable ?? categoryDisable;
        }

        #region Nested Classes
        private sealed class RuleWithId<T>
        {
            public T Rule { get; set; }
            public string Category { get; set; }
            public string Id { get; set; }
        }
        #endregion
    }
}
