using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdigEngine
{
	public class LineNumberExtension
	{
		public const string EnableSourceInfo = "EnableSourceInfo";

		public static ProcessDocumentDelegate GetProcessDocumentDelegate(MarkdownContext context)
		{
			ProcessDocumentDelegate result = delegate (MarkdownDocument document) 
			{
				AddSourceInfoInDataEntry(document, context);
			};

			return result;
		}

		/// <summary>
		/// if context.EnableSourceInfo is true: add sourceFile, sourceStartLineNumber, sourceEndLineNumber in each MarkdownObject
		/// </summary>
		/// <param name="markdownObject"></param>
		/// <param name="context"></param>
		public static void AddSourceInfoInDataEntry(MarkdownObject markdownObject, MarkdownContext context)
		{
			if (markdownObject == null || context == null || context.EnableSourceInfo == false) return;

			// set linenumber for its children recursively
			if (markdownObject is ContainerBlock)
			{
				var containerBlock = (ContainerBlock)markdownObject;
				foreach (var subBlock in containerBlock)
				{
					AddSourceInfoInDataEntry(subBlock, context);
				}
			}
			else if (markdownObject is LeafBlock)
			{
				var leafBlock = (LeafBlock)markdownObject;
				if (leafBlock.Inline != null)
				{
					foreach (var subInline in leafBlock.Inline)
					{
						AddSourceInfoInDataEntry(subInline, context);
					}
				}
			}
			else if (markdownObject is ContainerInline)
			{
				var containerInline = (ContainerInline)markdownObject;
				foreach (var subInline in containerInline)
				{
					AddSourceInfoInDataEntry(subInline, context);
				}
			}

			// set linenumber for this object
			var htmlAttributes = markdownObject.GetAttributes();
			htmlAttributes.AddPropertyIfNotExist("sourceFile", context.FilePath);
			htmlAttributes.AddPropertyIfNotExist("sourceStartLineNumber", markdownObject.Line + 1);
			htmlAttributes.AddPropertyIfNotExist("sourceEndLineNumber", context.GetLineNumber(markdownObject.Span.End, markdownObject.Line) + 1);
		}
	}
}
