using System.Composition;
using System.Collections.Immutable;

using MarkdigEngine.Extensions;

using Markdig.Syntax;
using Microsoft.DocAsCode.Common;

namespace MarkdigEngine.Tests
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
