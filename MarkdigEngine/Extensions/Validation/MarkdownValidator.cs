using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

using Markdig.Syntax;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    internal class MarkdownValidator
    {
        private static readonly Regex OpeningTag = new Regex(@"^\<(\w+)((?:""[^""]*""|'[^']*'|[^'"">])*?)\>$", RegexOptions.Compiled);
        private static readonly Regex ClosingTag = new Regex(@"^\</(\w+)((?:""[^""]*""|'[^']*'|[^'"">])*?)\>$", RegexOptions.Compiled);
        private static readonly Regex OpeningTagMatcher = new Regex(@"^\<(\w+)((?:""[^""]*""|'[^']*'|[^'"">])*?)\>", RegexOptions.Compiled);

        public ICompositionContainer Container { get; }
        public IImmutableList<MarkdownTagValidationRule> Validators { get; }

        public MarkdownValidator(ICompositionContainer container, ImmutableList<MarkdownTagValidationRule> validators)
        {
            Container = container;
            Validators = validators;
        }

        public void Validate(MarkdownDocument document)
        {
            throw new NotImplementedException();
        }
    }
}
