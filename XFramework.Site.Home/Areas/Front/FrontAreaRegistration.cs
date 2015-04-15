using System.Web.Mvc;
using System;

namespace XFramework.Site.Home
{
    public class FrontAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Front";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            #region == Survey ==
            context.MapRoute(
                "Survey_default",
                "survey/{file}.html",
                new { controller = "Survey", action = "Show" }
            );
            context.MapRoute(
                "en_survey_default",
                "en/survey/{file}.html",
                new { controller = "Survey", action = "Show" }
            );
            #endregion

            #region == Channel ==
            context.MapRoute(
                "Channel_default",
                "Channel/{cat}.html",
                new { controller = "Channel", action = "Show" }
            );
            context.MapRoute(
                "EnChannel_default",
                "en/channel/{cat}.html",
                new { controller = "Channel", action = "Show" }
            );
            #endregion

            #region == ArticleShow ==
            //context.MapRoute(
            //    "ArticleShow",
            //    "{year}-{month}-{day}/{id}.html",
            //    new { controller = "Article", action = "Show", id = @"\d+", year = @"\d+", month = @"\d+", day = @"\d+", data = @"\d{0,4}-\d{0,2}-\d{0,2}" }
            //);
            //context.MapRoute(
            //    "EnArticleShow",
            //    "en/{year}-{month}-{day}/{id}.html",
            //    new { controller = "Article", action = "Show", id = @"\d+", year = @"\d+", month = @"\d+", day = @"\d+", data = @"\d{0,4}-\d{0,2}-\d{0,2}" }
            //);
            context.MapRoute(
                "ArticleShow",
                "n/{guid}.html",
                new { controller = "Article", action = "Show"}
            );
            context.MapRoute(
                "EnArticleShow",
                "en/n/{guid}.html",
                new { controller = "Article", action = "Show" }
            );
            #endregion

            #region == Search ==
            context.MapRoute(
                "search_default",
                "search",
                new { controller = "Home", action = "Search" }
            );
            context.MapRoute(
                "en_search_default",
                "en/search",
                new { controller = "Home", action = "Search" }
            );
            #endregion

            #region == Default ==
            context.MapRoute(
                "EnDefault", // Route name
                "en", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
            context.MapRoute(
                "Default", // Route name
                "{controller}/{action}", // URL with parameters
                new { controller = "Home", action = "Index" }, // Parameter defaults
                new { controller = @"Home|Article|Channel|Download|Feedback|Survey|Product" }
            );
            #endregion
        }
    }
}
