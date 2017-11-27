using System.Collections.Generic;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine.Extensions
{
    public interface IMarkdigCompositor
    {
        string Markup(MarkdownContext context, MarkdownServiceParameters parameters);
        void ReportDependency(string file);
    }
}
