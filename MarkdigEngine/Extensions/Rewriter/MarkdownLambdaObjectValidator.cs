using System;

using MarkdigEngine.Plugin;

using Markdig.Syntax;

namespace MarkdigEngine
{
    internal class MarkdownLambdaTokenValidator<TObject> : IMarkdownObjectValidator where TObject : class, IMarkdownObject
    {
        private Action<TObject> _validator;

        public MarkdownLambdaTokenValidator(Action<TObject> validator)
        {
            _validator = validator;
        }

        public void Validate(IMarkdownObject markdownObject)
        {
            if (markdownObject is TObject obj)
            {
                _validator(obj);
            }
        }
    }
}