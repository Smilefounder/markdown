using System;
using System.Collections.Generic;

using Markdig.Parsers;
using Markdig.Syntax;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    internal class MarkdownValidatorBuilder
    {
        private readonly List<RuleWithId<MarkdownTagValidationRule>> _tagValidators =
            new List<RuleWithId<MarkdownTagValidationRule>>();

        public ICompositionContainer Container { get; }

        public MarkdownValidatorBuilder(ICompositionContainer container)
        {
            Container = container;
        }

        public ProcessDocumentDelegate CreateValidation()
        {
            return document => Validate(document);
        }

        private void Validate(MarkdownDocument document)
        {
            throw new NotImplementedException();
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
