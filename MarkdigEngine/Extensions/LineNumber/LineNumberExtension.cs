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

        internal static ProcessDocumentDelegate GetProcessDocumentDelegate(LineNumberExtensionHelper helper)
        {
            ProcessDocumentDelegate result = delegate (MarkdownDocument document) 
            {
                AddSourceInfoInDataEntry(document, helper);
            };

            return result;
        }

        /// <summary>
        /// if context.EnableSourceInfo is true: add sourceFile, sourceStartLineNumber, sourceEndLineNumber in each MarkdownObject
        /// </summary>
        /// <param name="markdownObject"></param>
        /// <param name="context"></param>
        private static void AddSourceInfoInDataEntry(MarkdownObject markdownObject, LineNumberExtensionHelper helper)
        {
            if (markdownObject == null || helper == null) return;

            // set linenumber for its children recursively
            if (markdownObject is ContainerBlock)
            {
                var containerBlock = (ContainerBlock)markdownObject;
                foreach (var subBlock in containerBlock)
                {
                    AddSourceInfoInDataEntry(subBlock, helper);
                }
            }
            else if (markdownObject is LeafBlock)
            {
                var leafBlock = (LeafBlock)markdownObject;
                if (leafBlock.Inline != null)
                {
                    foreach (var subInline in leafBlock.Inline)
                    {
                        AddSourceInfoInDataEntry(subInline, helper);
                    }
                }
            }
            else if (markdownObject is ContainerInline)
            {
                var containerInline = (ContainerInline)markdownObject;
                foreach (var subInline in containerInline)
                {
                    AddSourceInfoInDataEntry(subInline, helper);
                }
            }

            // set linenumber for this object
            var htmlAttributes = markdownObject.GetAttributes();
            htmlAttributes.AddPropertyIfNotExist("sourceFile", helper.FilePath);
            htmlAttributes.AddPropertyIfNotExist("sourceStartLineNumber", markdownObject.Line + 1);
            htmlAttributes.AddPropertyIfNotExist("sourceEndLineNumber", helper.GetLineNumber(markdownObject.Span.End, markdownObject.Line) + 1);
        }
    }

    public class LineNumberExtensionHelper
    {
        // This two private members are used for quickly getting the line number of one charactor
        // lineEnds[5] = 255 means the 6th lines ends at the 255th character of the text
        private int previousLineNumber;
        private List<int> lineEnds;

        internal string FilePath { get; private set; }

        static internal LineNumberExtensionHelper Create(string content, string absolutefilePath, string relativeFilePath)
        {
            var instance = new LineNumberExtensionHelper();
            instance.FilePath = relativeFilePath;

            if (string.IsNullOrEmpty(content))
            {
                if (File.Exists(absolutefilePath))
                {
                    instance.ResetlineEnds(File.ReadAllText(absolutefilePath));
                }
            }
            else
            {
                instance.ResetlineEnds(content);
            }

            return instance;
        }

        private void ResetlineEnds(string text)
        {
            previousLineNumber = 0;
            lineEnds = new List<int>();
            for (int position = 0; position < text.Length; position++)
            {
                var c = text[position];
                if (c == '\r' || c == '\n')
                {
                    if (c == '\r' && position + 1 < text.Length && text[position + 1] == '\n')
                    {
                        position++;
                    }
                    lineEnds.Add(position);
                }
            }
            lineEnds.Add(text.Length - 1);
        }

        /// <summary>
        /// Should call ResetlineEnds() first, and call GetLineNumber with an incremental position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        internal int GetLineNumber(int position, int start)
        {
            int lineNumber = start > previousLineNumber ? start : previousLineNumber;

            if (lineEnds == null || lineNumber >= lineEnds.Count)
            {
                previousLineNumber = start;
                return start;
            }

            for (; lineNumber < lineEnds.Count; lineNumber++)
            {
                if (position <= lineEnds[lineNumber])
                {
                    previousLineNumber = lineNumber;
                    return lineNumber;
                }
            }

            previousLineNumber = start;
            return start;
        }
    }
}
