using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CoreJob.Web.Dashboard.Helpers
{
    public class MenuLinkTagHelper : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            context.AllAttributes.TryGetAttribute("asp-action", out var action);
            context.AllAttributes.TryGetAttribute("asp-controller", out var controller);
            var url = $"/{controller.Value}/{action.Value}";

            string classes = "item";
            if (controller.Value.ToString().ToLower() == ViewContext.RouteData.Values["controller"].ToString().ToLower())
                classes += " active";

            var content = output.GetChildContentAsync().Result.GetContent();

            output.TagName = "a";
            output.Attributes.SetAttribute("href", url);
            output.Attributes.SetAttribute("class", classes);
            output.Content.SetContent(content);
        }
    }
}
