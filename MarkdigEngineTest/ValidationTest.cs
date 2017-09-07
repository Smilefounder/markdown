﻿using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;

using Markdig;
using Microsoft.DocAsCode.Common;
using Xunit;

namespace MarkdigEngine.Test
{
    public class ValidationTest
    {
        [Fact]
        [Trait("Related", "Validation")]
        public void TestHtmlBlockTagValidation()
        {
            var content = @"
<div class='a'>
    <i>x</i>
    <EM>y</EM>
    <h1>
        z
        <pre><code>
            a*b*c
        </code></pre>
    </h1>
</div>
<script>alert(1);</script>";

            var builder = MarkdownValidatorBuilder.Create(
                new CompositionContainer(
                    new ContainerConfiguration()
                        .WithAssembly(typeof(ValidationTest).Assembly)
                        .CreateContainer()), null);

            builder.AddTagValidators(new[]
            {
                new MarkdownTagValidationRule
                {
                    TagNames = new List<string> { "em", "div" },
                    MessageFormatter = "Invalid tag({0})!",
                    Behavior = TagValidationBehavior.Error,
                    OpeningTagOnly = true,
                },
                new MarkdownTagValidationRule
                {
                    TagNames = new List<string> { "h1" },
                    MessageFormatter = "Warning tag({0})!",
                    Behavior = TagValidationBehavior.Warning,
                },
                new MarkdownTagValidationRule
                {
                    TagNames = new List<string> { "script" },
                    MessageFormatter = "Warning tag({0})!",
                    Behavior = TagValidationBehavior.Warning,
                    OpeningTagOnly = true
                },
                new MarkdownTagValidationRule
                {
                    TagNames = new List<string> { "pre" },
                    MessageFormatter = "Warning tag({0})!",
                    Behavior = TagValidationBehavior.Warning,
                }
            });

            builder.AddValidators(new[]
            {
                new MarkdownValidationRule
                {
                    ContractName =  HtmlMarkdownObjectValidatorProvider.ContractName,
                }
            });

            var listener = new TestLoggerListener(logItem => logItem.LogLevel >= LogLevel.Warning);
            var html = Markup(content, builder, listener);

            Assert.Equal(@"<div class='a'>
    <i>x</i>
    <EM>y</EM>
    <h1>
        z
        <pre><code>
            a*b*c
        </code></pre>
    </h1>
</div>
<script>alert(1);</script>
".Replace("\r\n", "\n"), html);
            Assert.Equal(8, listener.Items.Count);
            Assert.Equal(new[]
            {
                "Html Tag!",
                "Invalid tag(div)!",
                "Invalid tag(EM)!",
                "Warning tag(h1)!",
                "Warning tag(pre)!",
                "Warning tag(pre)!",
                "Warning tag(h1)!",
                "Warning tag(script)!"
            }, from item in listener.Items select item.Message);
        }

        [Fact]
        [Trait("Related", "Validation")]
        public void TestHtmlBlockTagNotInRelationValidation()
        {
            var content = @"
<div class='a'>
    <i>x</i>
    <EM>y</EM>
    <h1>
        z
        <pre><code>
            a*b*c
        </code></pre>
    </h1>
</div>
<script>alert(1);</script>";

            var builder = MarkdownValidatorBuilder.Create(null, null);
            builder.AddTagValidators(new[]
            {
                new MarkdownTagValidationRule
                {
                    TagNames = new List<string> { "h1", "code", "pre", "div" },
                    MessageFormatter = "Invalid tag({0})!",
                    Behavior = TagValidationBehavior.Error,
                    OpeningTagOnly = true,
                    Relation = TagRelation.NotIn
                }
            });

            var listener = new TestLoggerListener(logItem => logItem.LogLevel >= LogLevel.Warning);
            var html = Markup(content, builder, listener);

            Assert.Equal(@"<div class='a'>
    <i>x</i>
    <EM>y</EM>
    <h1>
        z
        <pre><code>
            a*b*c
        </code></pre>
    </h1>
</div>
<script>alert(1);</script>
".Replace("\r\n", "\n"), html);
            Assert.Equal(3, listener.Items.Count);
            Assert.Equal(new[]
            {
                "Invalid tag(i)!",
                "Invalid tag(EM)!",
                "Invalid tag(script)!"
            }, from item in listener.Items select item.Message);
        }

        [Fact]
        [Trait("Related", "Validation")]
        public void TestHtmlInlineTagValidation()
        {
            var content = @"This is inline html: <div class='a'><i>x</i><EM>y</EM><h1>z<pre><code>a*b*c</code></pre></h1></div>

<script>alert(1);</script> end.";

            var builder = MarkdownValidatorBuilder.Create(
                new CompositionContainer(
                    new ContainerConfiguration()
                        .WithAssembly(typeof(ValidationTest).Assembly)
                        .CreateContainer()), null);

            builder.AddTagValidators(new[]
            {
                new MarkdownTagValidationRule
                {
                    TagNames = new List<string> { "em", "div" },
                    MessageFormatter = "Invalid tag({0})!",
                    Behavior = TagValidationBehavior.Error,
                    OpeningTagOnly = true,
                },
                new MarkdownTagValidationRule
                {
                    TagNames = new List<string> { "h1" },
                    MessageFormatter = "Warning tag({0})!",
                    Behavior = TagValidationBehavior.Warning,
                },
                new MarkdownTagValidationRule
                {
                    TagNames = new List<string> { "script" },
                    MessageFormatter = "Warning tag({0})!",
                    Behavior = TagValidationBehavior.Warning,
                    OpeningTagOnly = true
                },
                new MarkdownTagValidationRule
                {
                    TagNames = new List<string> { "pre" },
                    MessageFormatter = "Warning tag({0})!",
                    Behavior = TagValidationBehavior.Warning,
                },
            });

            var listener = new TestLoggerListener(logItem => logItem.LogLevel >= LogLevel.Warning);
            var html = Markup(content, builder, listener);

            Assert.Equal(@"<p>This is inline html: <div class='a'><i>x</i><EM>y</EM><h1>z<pre><code>a<em>b</em>c</code></pre></h1></div></p>
<script>alert(1);</script> end.
".Replace("\r\n", "\n"), html);
            Assert.Equal(7, listener.Items.Count);
            Assert.Equal(new[]
            {
                "Invalid tag(div)!",
                "Invalid tag(EM)!",
                "Warning tag(h1)!",
                "Warning tag(pre)!",
                "Warning tag(pre)!",
                "Warning tag(h1)!",
                "Warning tag(script)!"
            }, from item in listener.Items select item.Message);
        }

        private string Markup(string content, MarkdownValidatorBuilder builder, TestLoggerListener listener)
        {
            var pipelineBuilder = new MarkdownPipelineBuilder();
            var rewriter = builder.CreateRewriter();
            var documentRewriter = new MarkdownDocumentVisitor(rewriter);
            pipelineBuilder.DocumentProcessed += document => documentRewriter.Visit(document);

            var pipeline = pipelineBuilder.Build();

            Logger.RegisterListener(listener);
            var html = MarkdigMarked.Markup(content, pipeline);
            Logger.UnregisterListener(listener);

            return html;
        }
    }
}