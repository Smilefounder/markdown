using System;

using Markdig.Syntax;

namespace MarkdigEngine
{
    public class MarkdownDocumentRewriter : IMarkdownObjectRewriter
    {
        private readonly IMarkdownObjectRewriter _rewriter;

        public MarkdownDocumentRewriter(IMarkdownObjectRewriter rewriter)
        {
            _rewriter = rewriter;
        }

        public IMarkdownObject Rewrite(IMarkdownObject token)
        {
            throw new NotImplementedException();
        }

        public void Rewrite(MarkdownDocument document)
        {
            throw new NotImplementedException();
        }
    }
}