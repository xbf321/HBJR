using System.Text.RegularExpressions;

using XFramework.Model;

namespace XFramework.Site.Home.Models
{

    public sealed class XFrontContext
    {
        private const string URLPATTERN = @"/en(/)?";
        private XFrontContext()
        {
            string path = Goodspeed.Web.UrlHelper.Current.Path;         //当前URL
            Language = WebLanguage.zh_cn;
            Regex r = new Regex(URLPATTERN);
            Match m = r.Match(path);
            if (m.Success)
            {
                Language = WebLanguage.en;
            }
        }
        public static XFrontContext Current
        {
            get
            {
                return new XFrontContext();
            }
        }
        public WebLanguage Language { get; set; }
    }
}