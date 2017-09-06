using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

using Markdig.Syntax;
using Microsoft.DocAsCode.Plugins;
using Microsoft.DocAsCode.Common;
using System.Linq;
using MarkdigEngine.Plugin;

namespace MarkdigEngine
{
    internal class MarkdownValidatorBuilder
    {
        private readonly List<RuleWithId<MarkdownValidationRule>> _validators =
            new List<RuleWithId<MarkdownValidationRule>>();
        private readonly List<RuleWithId<MarkdownTagValidationRule>> _tagValidators =
            new List<RuleWithId<MarkdownTagValidationRule>>();
        private readonly Dictionary<string, MarkdownValidationRule> _globalValidators =
            new Dictionary<string, MarkdownValidationRule>();
        private readonly List<MarkdownValidationSetting> _settings =
            new List<MarkdownValidationSetting>();

        public const string DefaultValidatorName = "default";
        public const string MarkdownValidatePhaseName = "Markdown style";
        public ICompositionContainer Container { get; }

        public MarkdownValidatorBuilder(ICompositionContainer container)
        {
            Container = container;
        }

        public static MarkdownValidatorBuilder Create(ICompositionContainer container, MarkdownServiceParameters parameters)
        {
            var builder = new MarkdownValidatorBuilder(container);
            LoadValidatorConfig(parameters.BasePath, parameters.TemplateDir, builder);

            return builder;
        }

        public IMarkdownObjectRewriter CreateRewriter()
        {
            return MarkdownObjectRewriterFactory.FromValidators(
                    GetEnabledRules().Concat(
                        new[]
                        {
                            MarkdownObjectValidatorFactory.FromLambda<IMarkdownObject>(ValidateTagRules)
                        }));
        }

        public void ValidateTagRules(IMarkdownObject markdownObject)
        {
            // TODO: implement
            throw new NotImplementedException();
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
                    ContractName = DefaultValidatorName
                };
            }
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

        private IEnumerable<IMarkdownObjectValidator> GetEnabledRules()
        {
            HashSet<string> enabledContractName = new HashSet<string>();
            foreach (var item in _validators)
            {
                if (IsDisabledBySetting(item) ?? item.Rule.Disable)
                {
                    enabledContractName.Remove(item.Rule.ContractName);
                }
                else
                {
                    enabledContractName.Add(item.Rule.ContractName);
                }
            }
            foreach (var pair in _globalValidators)
            {
                if (pair.Value.Disable)
                {
                    enabledContractName.Remove(pair.Value.ContractName);
                }
                else
                {
                    enabledContractName.Add(pair.Value.ContractName);
                }
            }
            if (Container == null)
            {
                return Enumerable.Empty<IMarkdownObjectValidator>();
            }
            return from name in enabledContractName
                   from vp in Container.GetExports<IMarkdownObjectValidatorProvider>(name)
                   from v in vp.GetValidators()
                   select v;
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
