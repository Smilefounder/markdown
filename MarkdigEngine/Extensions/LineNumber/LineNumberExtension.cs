using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdigEngine
{
    public class LineNumberExtension
    {
        public const string EnableSourceInfo = "EnableSourceInfo";

        internal static ProcessDocumentDelegate GetProcessDocumentDelegate(LineNumberExtensionContext lineNumberContext)
        {
            ProcessDocumentDelegate result = delegate (MarkdownDocument document) 
            {
                AddSourceInfoInDataEntry(document, lineNumberContext);
            };

            return result;
        }

        /// <summary>
        /// if context.EnableSourceInfo is true: add sourceFile, sourceStartLineNumber, sourceEndLineNumber in each MarkdownObject
        /// </summary>
        /// <param name="markdownObject"></param>
        /// <param name="context"></param>
        private static void AddSourceInfoInDataEntry(MarkdownObject markdownObject, LineNumberExtensionContext lineNumberContext)
        {
            if (markdownObject == null || lineNumberContext == null) return;

            // set linenumber for its children recursively
            if (markdownObject is ContainerBlock)
            {
                var containerBlock = (ContainerBlock)markdownObject;
                foreach (var subBlock in containerBlock)
                {
                    AddSourceInfoInDataEntry(subBlock, lineNumberContext);
                }
            }
            else if (markdownObject is LeafBlock)
            {
                var leafBlock = (LeafBlock)markdownObject;
                if (leafBlock.Inline != null)
                {
                    foreach (var subInline in leafBlock.Inline)
                    {
                        AddSourceInfoInDataEntry(subInline, lineNumberContext);
                    }
                }
            }
            else if (markdownObject is ContainerInline)
            {
                var containerInline = (ContainerInline)markdownObject;
                foreach (var subInline in containerInline)
                {
                    AddSourceInfoInDataEntry(subInline, lineNumberContext);
                }
            }

            // set linenumber for this object
            var htmlAttributes = markdownObject.GetAttributes();
            htmlAttributes.AddPropertyIfNotExist("sourceFile", lineNumberContext.FilePath);
            htmlAttributes.AddPropertyIfNotExist("sourceStartLineNumber", markdownObject.Line + 1);
            htmlAttributes.AddPropertyIfNotExist("sourceEndLineNumber", lineNumberContext.GetLineNumber(markdownObject.Span.End, markdownObject.Line) + 1);
        }
    }
}
