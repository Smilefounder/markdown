using MarkdigEngine;
using Microsoft.DocAsCode.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MarkdigEngineTest
{
    public class YamlHeaderTest
    {
        private static MarkupResult SimpleMarkup(string source)
        {
            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);
            return service.Markup(source, "Topic.md");
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestDfm_InvalidYamlHeader_YamlUtilityThrowException()
        {
            var source = @"---
- Jon Schlinkert
- Brian Woodward

---";
            var expected = @"<hr />
<ul>
<li>Jon Schlinkert</li>
<li>Brian Woodward</li>
</ul>
<hr />
";
            var marked = SimpleMarkup(source);
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }


        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestDfmYamlHeader_YamlUtilityReturnNull()
        {
            var source = @"---

### /Unconfigure

---";
            var expected = @"<hr />
<h3 id=""unconfigure"">/Unconfigure</h3>
<hr />
";
            var marked = SimpleMarkup(source);
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }
        
        [Fact]
        public void CodeSnippetGeneral()
        {
            //arange
            var content = @"---
title: ""如何使用 Visual C++ 工具集报告问题 | Microsoft Docs""
ms.custom: 
ms.date: 11/04/2016
ms.reviewer: 
ms.suite: 
ms.technology:
- cpp
ms.tgt_pltfrm: 
ms.topic: article
dev_langs:
- C++
ms.assetid: ec24a49c-411d-47ce-aa4b-8398b6d3e8f6
caps.latest.revision: 8
author: corob-msft
ms.author: corob
manager: ghogen
translation.priority.mt:
- cs-cz
- pl-pl
- pt-br
- tr-tr
translationtype: Human Translation
ms.sourcegitcommit: 5c6fbfc8699d7d66c40b0458972d8b6ef0dcc705
ms.openlocfilehash: 2ea129ac94cb1ddc7486ba69280dc0390896e088
---";
            // act
            var parameter = new MarkdownServiceParameters
            {
                BasePath = ".",
            };
            var service = new MarkdigMarkdownService(parameter);
            var marked = service.Markup(content, "Topic.md");

            // assert
            var expected = @"<yamlheader>title: &quot;如何使用 Visual C++ 工具集报告问题 | Microsoft Docs&quot;
ms.custom: 
ms.date: 11/04/2016
ms.reviewer: 
ms.suite: 
ms.technology:
- cpp
ms.tgt_pltfrm: 
ms.topic: article
dev_langs:
- C++
ms.assetid: ec24a49c-411d-47ce-aa4b-8398b6d3e8f6
caps.latest.revision: 8
author: corob-msft
ms.author: corob
manager: ghogen
translation.priority.mt:
- cs-cz
- pl-pl
- pt-br
- tr-tr
translationtype: Human Translation
ms.sourcegitcommit: 5c6fbfc8699d7d66c40b0458972d8b6ef0dcc705
ms.openlocfilehash: 2ea129ac94cb1ddc7486ba69280dc0390896e088</yamlheader>";
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }
    }
}
