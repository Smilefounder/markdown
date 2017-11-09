using System;

using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    public static class MarkdownObjectValidatorFactory
    {
        public static IMarkdownObjectValidator FromLambda<TObject>(
            Action<TObject> validator,
            Action<IMarkdownObject> preAction = null,
            Action<IMarkdownObject> postAction = null)
                where TObject : class, IMarkdownObject
        {
            return new MarkdownLambdaObjectValidator<TObject>(validator, preAction, postAction);
        }
    }
}
