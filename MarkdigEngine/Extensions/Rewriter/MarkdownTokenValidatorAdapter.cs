using System.Collections.Generic;
using System.Collections.Immutable;

using MarkdigEngine.Plugin;

using Markdig.Syntax;

namespace MarkdigEngine
{
    internal class MarkdownTokenValidatorAdapter : IMarkdownObjectRewriter
    {
        public ImmutableArray<IMarkdownObjectValidator> Validators { get; }

        public MarkdownTokenValidatorAdapter(IEnumerable<IMarkdownObjectValidator> validators)
        {
            Validators = validators.ToImmutableArray();
        }

        public IMarkdownObject Rewrite(IMarkdownObject markdownObject)
        {
            foreach(var validator in Validators)
            {
                validator.Validate(markdownObject);
            }

            return markdownObject;
        }
    }
}