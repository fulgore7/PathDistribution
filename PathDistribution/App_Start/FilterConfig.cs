using PathDistribution.Attributes;
using System.Web.Mvc;

namespace PathDistribution
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new PDHandleErrorAttribute());
            filters.Add(new OutputCacheAttribute()
            {
                NoStore = true,
                Duration = 0,
                VaryByParam = "*",
                Location = System.Web.UI.OutputCacheLocation.None
            });
            //filters.Add(new CompressContentAttribute());
            //filters.Add(new MinifyHtmlAttribute());
            //filters.Add(new MinifyXhtmlAttribute());
            //filters.Add(new MinifyXmlAttribute());
        }
    }
}
