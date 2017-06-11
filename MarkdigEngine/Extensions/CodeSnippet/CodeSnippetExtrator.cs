﻿using Microsoft.DocAsCode.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdigEngine
{
    public class CodeSnippetExtrator
    {
        private readonly string StartLineTemplate;
        private readonly string EndLineTemplate;
        private readonly bool IsEndLineContainsTagName;
        public const string TagNamePlaceHolder = "{tagname}";

        public CodeSnippetExtrator(string startLineTemplate, string endLineTemplate, bool isEndLineContainsTagName = true)
        {
            this.StartLineTemplate = startLineTemplate;
            this.EndLineTemplate = endLineTemplate;
            this.IsEndLineContainsTagName = isEndLineContainsTagName;
        }

        public Dictionary<string, CodeRange> GetAllTags(string[] lines)
        {
            var result = new Dictionary<string, CodeRange>(StringComparer.OrdinalIgnoreCase);
            var tagStack = new Stack<string>();

            for(int index = 0; index < lines.Length; index++)
            {
                var line = lines[index];

                string tagName;

                if(MatchTag(line, EndLineTemplate, out tagName))
                {
                    if(!IsEndLineContainsTagName)
                    {
                        tagName = tagStack.Pop();
                    }

                    if(!result.ContainsKey(tagName))
                    {
                        Logger.LogError($"Can't find startTag {tagName}");
                    }
                    else
                    {
                        result[tagName].End = index;
                    }

                    continue;
                }

                if (MatchTag(line, StartLineTemplate, out tagName))
                {
                    result[tagName] = new CodeRange { Start = index + 2 };
                    tagStack.Push(tagName);
                }

            }

            return result;
        }

        private bool MatchTag(string line, string template, out string tagName)
        {
            tagName = string.Empty;
            if (string.IsNullOrEmpty(line) || string.IsNullOrEmpty(template)) return false;

            var splitedTemplate = template.Split(new[] { TagNamePlaceHolder }, StringSplitOptions.None);
            var beforeTagName = splitedTemplate[0];
            var afterTagName = splitedTemplate.Length == 2 ? splitedTemplate[1] : string.Empty;

            int column = 0;
            int index = 0;

            // match before
            while (column < line.Length && index < beforeTagName.Length)
            {
                if (line[column] != ' ')
                {
                    if (char.ToLower(line[column]) != beforeTagName[index]) return false;
                    index++;
                }
                column++;
            }

            if (index != beforeTagName.Length) return false;

            //match tagname
            var sb = new StringBuilder();
            while(column < line.Length && (afterTagName == string.Empty || line[column] != afterTagName[0]))
            {
                sb.Append(line[column]);
                column++;
            }
            tagName = sb.ToString().Trim().ToLower();

            //match after tagname
            index = 0;
            while(column < line.Length && index < afterTagName.Length)
            {
                if (line[column] != ' ')
                {
                    if (char.ToLower(line[column]) != afterTagName[index]) return false;
                    index++;
                }
                column++;
            }

            if (index != afterTagName.Length) return false;
            while (column < line.Length && line[column] == ' ') column++;
            
            return column == line.Length;
        }
    }
}
