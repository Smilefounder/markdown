using System;
using System.Collections.Generic;

using MarkdigEngine.Plugin;

namespace MarkdigEngine
{
    public static class MarkdownObjectRewriterFactory
    {
        public static IMarkdownObjectRewriter FromValidators(IEnumerable<IMarkdownObjectValidator> validators)
        {
            if (validators == null)
            {
                throw new ArgumentNullException(nameof(validators));
            }

            return new MarkdownTokenValidatorAdapter(validators);
        }
    }
}
