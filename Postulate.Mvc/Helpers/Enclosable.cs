using System;
using System.Linq;
using System.Web.Mvc;

namespace Postulate.Mvc.Helpers
{
    /// <summary>
    /// Base class for creating Html helper methods that insert custom content into one or more fixed enclosing tags
    /// </summary>
    public class Enclosable : IDisposable
    {
        private HtmlHelper _html;
        private readonly string _closingTags;

        public Enclosable(HtmlHelper html, params TagBuilder[] tags)
        {
            _html = html;
            _closingTags = string.Join("", tags.Select(t => t.ToString(TagRenderMode.EndTag)));
        }

        public Enclosable(HtmlHelper html, string closingTags)
        {
            _html = html;
            _closingTags = closingTags;
        }

        public void Dispose()
        {
            _html.ViewContext.Writer.Write(_closingTags);
        }
    }
}