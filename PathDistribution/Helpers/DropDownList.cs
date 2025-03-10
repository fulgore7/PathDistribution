using PathDistribution.Models;
using PathDistribution.Models.DAL;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace PathDistribution.Helpers
{
    public static class DropDownList
    {
        public static MvcHtmlString OffTypeDDL(this HtmlHelper htmlHelper, string name)
        {
            //Created a select element which renderes the dropdown.
            TagBuilder dropdown = new TagBuilder("select");
            //Assigning the name and id attribute.
            dropdown.Attributes.Add("name", name);
            dropdown.Attributes.Add("id", name);

            DistDAL dal = new DistDAL();

            List<OffType> offTypes = dal.GetOffTypes();

            //Created StringBuilder object to store option data fetched oen by one from list.
            StringBuilder options = new StringBuilder();
            //Iterated over the IEnumerable list.
            foreach (OffType item in offTypes)
            {
                //Each option represents a value in dropdown. For each element in the list, option element is created and appended to the stringBuilder object.
                options.Append("<option value='" + item.chrAbbr + "' data-IsFullOff='" + item.bitIsFullOff.ToString().ToLower() + "'>" + item.chrAbbr+ "</option>");
            }
            //assigned all the options to the dropdown using innerHTML property.    
            dropdown.InnerHtml = options.ToString();

            //Returning the entire select or dropdown control in HTMLString format.
            return MvcHtmlString.Create(dropdown.ToString(TagRenderMode.Normal));
        }
    }
}
