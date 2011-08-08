// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpUrlBuilder.cs" company="--">
//   Copyright � -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   HttpUrlBuilder
//   This Class is Responsible for all the Urls in Appleseed to prevent
//   hardcoded urls.
//   This makes it easier to update urls across the multiple portals
//   Original ideas from John Mandia, Cory Isakson, Jes and Manu.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Web;

    using Appleseed.Framework.Settings;
    using Appleseed.Framework.Site.Configuration;
    using Appleseed.Framework.Web;
    using System.Text.RegularExpressions;

    /// <summary>
    /// HttpUrlBuilder
    ///     This Class is Responsible for all the Urls in Appleseed to prevent
    ///     hardcoded urls. 
    ///     This makes it easier to update urls across the multiple portals
    ///     Original ideas from John Mandia, Cory Isakson, Jes and Manu.
    /// </summary>
    [History("jminond", "2005/03/10", "Tab to page conversion")]
    [History("john.mandia@whitelightsolutions.com", "2004/09/2", 
        "Introduced the provider pattern for UrlBuilder so that people can implement their rules for how urls should be built")]
    [History("john.mandia@whitelightsolutions.com", "2003/08/13", 
        "Removed Keywords splitter - rebuilt handler code to use a rules engine and changed code on url builder to make it cleaner and compatible")]
    [History("Jes1111", "2003/03/18", "Added Keyword Splitter feature, see explanation in web.config")]
    [History("Jes1111", "2003/04/24", "Fixed problem with '=' in Keyword Splitter")]
    public class HttpUrlBuilder
    {
        #region Constants and Fields

        /// <summary>
        /// The provider.
        /// </summary>
        private static readonly UrlBuilderProvider Provider = UrlBuilderProvider.Instance();

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the default page.
        /// </summary>
        /// <value>The default page.</value>
        public static string DefaultPage
        {
            get
            {
                // UrlBuilderProvider provider = UrlBuilderProvider.Instance();
                return Provider.DefaultPage;
            }
        }

        /// <summary>
        ///     Gets the default splitter.
        /// </summary>
        /// <value>The default splitter.</value>
        public static string DefaultSplitter
        {
            get
            {
                // UrlBuilderProvider provider = UrlBuilderProvider.Instance();
                return Provider.DefaultSplitter;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Builds the url for get to current portal home page
        ///     containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <returns>
        /// The build url.
        /// </returns>
        public static string BuildUrl()
        {
            return BuildUrl("~/" + DefaultPage, 0, 0, null, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Builds the url for get to current portal home page
        ///     containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">
        /// Linked page
        /// </param>
        /// <returns>
        /// The build url.
        /// </returns>
        public static string BuildUrl(string targetPage)
        {
            return BuildUrl(targetPage, 0, 0, null, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Builds the url for get to current portal home page
        ///     containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">
        /// Linked page
        /// </param>
        /// <param name="customAttributes">
        /// Any custom attribute that can be needed
        /// </param>
        /// <returns>
        /// The build url.
        /// </returns>
        public static string BuildUrl(string targetPage, string customAttributes)
        {
            return BuildUrl(targetPage, 0, 0, null, customAttributes, string.Empty, string.Empty);
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page
        ///     containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="pageId">
        /// ID of the tab
        /// </param>
        /// <returns>
        /// The build url.
        /// </returns>
        public static string BuildUrl(int pageId)
        {
            return BuildUrl("~/" + DefaultPage, pageId, 0, null, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page
        ///     containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="pageId">
        /// ID of the page
        /// </param>
        /// <param name="urlKeywords">
        /// Add some keywords to uniquely identify this tab. Usual source is UrlKeyword from TabSettings.
        /// </param>
        /// <returns>
        /// The build url.
        /// </returns>
        public static string BuildUrl(int pageId, string urlKeywords)
        {
            return BuildUrl("~/" + DefaultPage, pageId, 0, null, string.Empty, string.Empty, urlKeywords);
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        ///     containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">
        /// Linked page
        /// </param>
        /// <param name="pageId">
        /// ID of the page
        /// </param>
        /// <returns>
        /// The build url.
        /// </returns>
        public static string BuildUrl(string targetPage, int pageId)
        {
            return BuildUrl(targetPage, pageId, 0, null, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        ///     containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">
        /// Linked page
        /// </param>
        /// <param name="pageId">
        /// ID of the page
        /// </param>
        /// <param name="customAttributes">
        /// Any custom attribute that can be needed
        /// </param>
        /// <returns>
        /// The build url.
        /// </returns>
        public static string BuildUrl(string targetPage, int pageId, string customAttributes)
        {
            return BuildUrl(targetPage, pageId, 0, null, customAttributes, string.Empty, string.Empty);
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        ///     containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">
        /// Linked page
        /// </param>
        /// <param name="pageId">
        /// ID of the page
        /// </param>
        /// <param name="customAttributes">
        /// Any custom attribute that can be needed
        /// </param>
        /// <param name="currentAlias">
        /// Current Alias
        /// </param>
        /// <returns>
        /// The build url.
        /// </returns>
        public static string BuildUrl(string targetPage, int pageId, string customAttributes, string currentAlias)
        {
            return BuildUrl(targetPage, pageId, 0, null, customAttributes, currentAlias, string.Empty);
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        ///     containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">
        /// Linked page
        /// </param>
        /// <param name="pageId">
        /// ID of the page
        /// </param>
        /// <param name="culture">
        /// Client culture
        /// </param>
        /// <param name="customAttributes">
        /// Any custom attribute that can be needed
        /// </param>
        /// <param name="currentAlias">
        /// Current Alias
        /// </param>
        /// <returns>
        /// The build url.
        /// </returns>
        public static string BuildUrl(
            string targetPage, int pageId, CultureInfo culture, string customAttributes, string currentAlias)
        {
            return BuildUrl(targetPage, pageId, 0, culture, customAttributes, currentAlias, string.Empty);
        }

        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        ///     containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">
        /// Linked page
        /// </param>
        /// <param name="pageId">
        /// ID of the page
        /// </param>
        /// <param name="modId">
        /// ID of the module
        /// </param>
        /// <returns>
        /// The build url.
        /// </returns>
        public static string BuildUrl(string targetPage, int pageId, int modId)
        {
            return BuildUrl(targetPage, pageId, modId, null, string.Empty, string.Empty, string.Empty);
        }


                
        /// <summary>
        /// Takes a Tab ID and builds the url for get the desidered page (non default)
        ///     containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="targetPage">
        /// Linked page
        /// </param>
        /// <param name="pageId">
        /// ID of the page
        /// </param>
        /// <param name="modId">
        /// ID of the module
        /// </param>
        /// <param name="culture">
        /// Client culture
        /// </param>
        /// <param name="customAttributes">
        /// Any custom attribute that can be needed. Use the following format...single attribute: paramname--paramvalue . Multiple attributes: paramname--paramvalue/paramname2--paramvalue2/paramname3--paramvalue3
        /// </param>
        /// <param name="currentAlias">
        /// Current Alias
        /// </param>
        /// <param name="urlKeywords">
        /// Add some keywords to uniquely identify this tab. Usual source is UrlKeyword from TabSettings.
        /// </param>
        /// <returns>
        /// The build url.
        /// </returns>
        public static string BuildUrl(
            string targetPage, 
            int pageId, 
            int modId, 
            CultureInfo culture, 
            string customAttributes, 
            string currentAlias, 
            string urlKeywords)
        {
            PortalSettings currentSetting = null;

            if (HttpContext.Current.Items["PortalSettings"] != null)
            {
                currentSetting = (PortalSettings)HttpContext.Current.Items["PortalSettings"];
            }

            if (culture == null)
            {
                culture = currentSetting != null ? currentSetting.PortalContentLanguage : Thread.CurrentThread.CurrentUICulture;
            }

            if (string.IsNullOrEmpty(currentAlias))
            {
                // jes1111 - currentAlias = ConfigurationSettings.AppSettings["DefaultPortal"];
                currentAlias = currentSetting != null ? currentSetting.PortalAlias : Config.DefaultPortal;
            }

            // prepare for additional querystring values
            var completeCustomAttributes = customAttributes;

            /*

            // Start of John Mandia's UrlBuilder Enhancement - Uncomment to test (see history for details)
            // prepare the customAttributes so that they may include any additional existing parameters
            
            // get the current tab id
            int currentTabID = 0;
            if (HttpContext.Current.Request.Params["tabID"] != null)
                currentTabID = Int32.Parse(HttpContext.Current.Request.Params["tabID"]);

            if(tabID == currentTabID)
            {
                // this link is being generated for the current page the user is on
                foreach(string name in HttpContext.Current.Request.QueryString)
                {
                    if((HttpContext.Current.Request.QueryString[ name ].Length != 0) && (HttpContext.Current.Request.QueryString[ name ] != null) && (name != null))
                    {
                            // do not add any of the common parameters
                            if((name.ToLower() !="tabid") && (name.ToLower() != "mid") && (name.ToLower() != "alias") && (name.ToLower() != "lang") && (name.ToLower() != "returntabid") && (name != null))
                            {
                                if(!(customAttributes.ToLower().IndexOf(name.ToLower()+"=")> -1))
                                {
                                    completeCustomAttributes += "&" + name + "=" + HttpContext.Current.Request.QueryString[ name ];
                                }
                            }
                    }
                
            }
            
            */
            
            return Provider.BuildUrl(
                targetPage, pageId, modId, culture, completeCustomAttributes, currentAlias, urlKeywords);
        }

        /// <summary>
        /// Clears any Url Elements e.g IsPlaceHolder, TabLink, UrlKeywords and PageName etc
        ///     that may be stored (either in cache, xml etc depending on the UrlBuilder implementation
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        public static void Clear(int pageId)
        {
            Provider.Clear(pageId);
        }

        /// <summary>
        /// 2_aug_2004 Cory Isakson enhancement
        ///     Determines if a tab is simply a placeholder in the navigation
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified page ID is placeholder; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPlaceholder(int pageId)
        {
            // UrlBuilderProvider provider = UrlBuilderProvider.Instance();
            return Provider.IsPlaceholder(pageId);
        }

        /// <summary>
        /// 2_aug_2004 Cory Isakson enhancement
        ///     Returns the URL for a tab that is a link only.
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <returns>
        /// The tab link.
        /// </returns>
        public static string TabLink(int pageId)
        {
            // UrlBuilderProvider provider = UrlBuilderProvider.Instance();
            return Provider.TabLink(pageId);
        }

        /// <summary>
        /// Builds the url for get to current portal home page
        ///     containing the application path, portal alias, tab ID, and language.
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <returns>
        /// The url keyword.
        /// </returns>
        public static string UrlKeyword(int pageId)
        {
            // UrlBuilderProvider provider = UrlBuilderProvider.Instance();
            return Provider.UrlKeyword(pageId);
        }

        /// <summary>
        /// Returns the page name that has been specified.
        /// </summary>
        /// <param name="pageId">
        /// The page ID.
        /// </param>
        /// <returns>
        /// The url page name.
        /// </returns>
        public static string UrlPageName(int pageId)
        {
            // UrlBuilderProvider provider = UrlBuilderProvider.Instance();
            return Provider.UrlPageName(pageId);
        }

        /// <summary>
        /// Webs the path combine.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The web path combine.
        /// </returns>
        [Obsolete("Please use the new Appleseed.Framework.Settings.Path.WebPathCombine()")]
        public static string WebPathCombine(params string[] values)
        {
            return Path.WebPathCombine(values);
        }

        public static bool ValidateProperUrl(Uri url, int pageId)
        {
            string query = url.Query;
            query = Regex.Replace(query, @"\+", "%20");
            int index = query.IndexOf('&');
            if (index > 0) {
                // Removing the first element that its the pageId, to only add the other querys
                var customAttributes = query.Substring(index + 1, query.Length - index - 1);
                return url.AbsolutePath.Equals(BuildUrl("~/" + DefaultPage, pageId, customAttributes));
            }
            else
                return url.AbsolutePath.Equals(BuildUrl(pageId));
        }

        public static string getProperUrl(Uri url, int pageId) {
            string query = url.Query;
            query = Regex.Replace(query, @"\+", "%20");
            int index = query.IndexOf('&');
            if (index > 0) {
                
                var customAttributes = query.Substring(index + 1, query.Length - index - 1);
                return BuildUrl("~/" + DefaultPage, pageId, customAttributes);
            } else
                return BuildUrl(pageId);
        }


        #endregion
    }
}