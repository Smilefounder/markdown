using System;

using MarkdigEngine.Plugin;

using Markdig.Syntax;

namespace MarkdigEngine
{
    public static class MarkdownObjectValidatorFactory
    {
        public static IMarkdownObjectValidator FromLambda<TObject>(Action<TObject> validator)
                where TObject : class, IMarkdownObject
        {
            return new MarkdownLambdaObjectValidator<TObject>(validator);
        }
    }
}
