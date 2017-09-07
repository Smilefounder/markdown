using System;
using System.Collections.Immutable;
using System.Linq;

using Markdig.Syntax;
using Microsoft.DocAsCode.Common;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    internal class TagValidator
    {
        public TagValidator(ICompositionContainer container, ImmutableList<MarkdownTagValidationRule> validators)
        {
            Container = container;
            Validators = validators;
        }

        public ICompositionContainer Container { get; }

        public ImmutableList<MarkdownTagValidationRule> Validators { get; }

        public void Validate(IMarkdownObject markdownObject)
        {
            var tags = Tag.Convert(markdownObject);
            if (tags == null)
            {
                return;
            }

            foreach (var tag in tags)
            {
                foreach (var validator in Validators)
                {
                    ValidateOne(tag, validator);
                }
            }
        }

        private void ValidateOne(Tag tag, MarkdownTagValidationRule validator)
        {
            if (tag.IsOpening || !validator.OpeningTagOnly)
            {
                var hasTagName = validator.TagNames.Any(tagName => string.Equals(tagName, tag.Name, StringComparison.OrdinalIgnoreCase));
                if (hasTagName ^ (validator.Relation == TagRelation.NotIn))
                {
                    ValidateCore(tag, validator);
                }
            }
        }

        private void ValidateCore(Tag tag, MarkdownTagValidationRule validator)
        {
            switch (validator.Behavior)
            {
                case TagValidationBehavior.Warning:
                    Logger.LogWarning(string.Format(validator.MessageFormatter, tag.Name, tag.Content), line: tag.Line.ToString());
                    return;
                case TagValidationBehavior.Error:
                    Logger.LogError(string.Format(validator.MessageFormatter, tag.Name, tag.Content), line: tag.Line.ToString());
                    return;
                case TagValidationBehavior.None:
                default:
                    return;
            }
        }
    }
}
