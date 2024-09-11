using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace WebHalk.Helpers
{
    public static class HtmlHelpers
    {
        public static IHtmlContent SelectedAttribute(this IHtmlHelper htmlHelper, int value, int selectedValue)
        {
            return new HtmlString(value == selectedValue ? "selected" : "");
        }

        public static IHtmlContent PageSizeDropDown(this IHtmlHelper htmlHelper, int currentPageSize)
        {
            var selectList = new SelectList(new[]
            {
            new { Value = 5, Text = "5" },
            new { Value = 10, Text = "10" },
            new { Value = 20, Text = "20" },
            new { Value = 50, Text = "50" }
        }, "Value", "Text", currentPageSize);

            return htmlHelper.DropDownList("pageSize", selectList, new { @class = "form-control", onchange = "this.form.submit()" });
        }

        public static IHtmlContent CategoryDropDown(this IHtmlHelper htmlHelper, IEnumerable<WebHalk.Areas.Admin.Models.Category.CategoryItemViewModel> categories, int? selectedCategoryId)
        {
            var selectList = new SelectList(categories, "Id", "Name", selectedCategoryId);

            return htmlHelper.DropDownList("categoryFilter", selectList, "Всі категорії", new { @class = "form-control" });
        }
    }

}