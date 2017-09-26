using System.Composition;
using System.Collections.Immutable;

using MarkdigEngine.Plugin;

using Markdig.Syntax;
using MarkdigEngine.Extensions;
using Microsoft.DocAsCode.Common;

namespace MarkdigEngine.Test
{
    [Export(ContractName, typeof(IMarkdownObjectValidatorProvider))]
    public class HtmlMarkdownObjectValidatorProvider : IMarkdownObjectValidatorProvider
    {
        public const string ContractName = "Html";

        public const string WarningMessage = "Html Tag!";

        ImmutableArray<IMarkdownObjectValidator> IMarkdownObjectValidatorProvider.GetValidators()
        {
            return ImmutableArray.Create(
                MarkdownObjectValidatorFactory.FromLambda<HtmlBlock>(
                    block => Logger.LogWarning(WarningMessage)));
        }
    }
}
