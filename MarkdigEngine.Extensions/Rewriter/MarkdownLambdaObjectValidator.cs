using System;

using MarkdigEngine.Plugin;

using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    internal class MarkdownLambdaObjectValidator<TObject> : IMarkdownObjectValidator where TObject : class, IMarkdownObject
    {
        private Action<TObject> _validator;

        public MarkdownLambdaObjectValidator(Action<TObject> validator)
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