﻿#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Caching;

using DotNetNuke.Collections.Internal;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.ClientCapability;
using DotNetNuke.Services.Log.EventLog;

#endregion

namespace DotNetNuke.Services.Mobile
{
	public class RedirectionController : IRedirectionController
    {
        #region Constants
        private const string DisableMobileRedirectCookieName = "disablemobileredirect";
		private const string DisableRedirectPresistCookieName = "disableredirectpresist";
        private const string DisableMobileRedirectQueryStringName = "nomo"; //google uses the same name nomo=1 means do not redirect to mobile
        #endregion

        #region "Private Properties"

        private const string UrlsCacheKey = "MobileRedirectAllUrls";
		private const string RedirectionUrlCacheKey = "RedirectionUrl_{0}_{1}_{2}";
		private const string FullSiteUrlCacheKey = "FullSiteUrl_{0}_{1}";
		private const string MobileSiteUrlCacheKey = "MobileSiteUrl_{0}_{1}";
		private const int UrlsCacheTimeout = 60;

		#endregion

		#region "Public Methods"

        /// <summary>
        /// Is Redirection Allowed for the session. Method analyzes the query string for special parameters to enable / disable redirects.
        /// Cookie is created to temporarily store those parameters so that they remain available till the interactions are active.
        /// </summary>
        /// <returns>boolean - True if redirection </returns>
        /// <param name="app">app - HttpApplication. Request and Response properties are used</param>
        public bool IsRedirectAllowedForTheSession(HttpApplication app)
        {
            bool allowed = true;

            //Check for the existence of special query string to force enable / disable of redirect
            if (app.Request.QueryString[DisableMobileRedirectQueryStringName] != null)
            {
                int val;
                if (int.TryParse(app.Request.QueryString[DisableMobileRedirectQueryStringName], out val))
                {
                    if (val == 0) //forced enable. clear any cookie previously set
                    {
                        if (app.Response.Cookies[DisableMobileRedirectCookieName] != null)
                        {
                            HttpCookie cookie = new HttpCookie(DisableMobileRedirectCookieName);
                            cookie.Expires = DateTime.Now.AddMinutes(-1);
                            app.Response.Cookies.Add(cookie);      
                        }

						if (app.Response.Cookies[DisableRedirectPresistCookieName] != null)
						{
							HttpCookie cookie = new HttpCookie(DisableRedirectPresistCookieName);
							cookie.Expires = DateTime.Now.AddMinutes(-1);
							app.Response.Cookies.Add(cookie);
						}   
                    }
                    else if (val == 1) //forced disable. need to setup cookie
                    {
                        allowed = false;
                    }                                        
                }                
            }
			else if (app.Request.Cookies[DisableMobileRedirectCookieName] != null && app.Request.Cookies[DisableRedirectPresistCookieName] != null) //check for cookie
            {
                allowed = false;
            }


            if (!allowed) //redirect is not setup to be allowed, keep the cookie alive
            {
				//this cookie is set to re-enable redirect after 20 minutes
				HttpCookie presistCookie = new HttpCookie(DisableRedirectPresistCookieName);
				presistCookie.Expires = DateTime.Now.AddMinutes(20);
				app.Response.Cookies.Add(presistCookie);

				//this cookie is set to re-enable redirect after close browser.
				HttpCookie cookie = new HttpCookie(DisableMobileRedirectCookieName);
				app.Response.Cookies.Add(cookie);    
            }

            return allowed;
        }
        
		/// <summary>
        /// Get Redirection Url based on UserAgent.         
        /// </summary>
        /// <returns>string - Empty if redirection rules are not defined or no match found</returns>
        /// <param name="userAgent">User Agent - used for client capability detection.</param>
        public string GetRedirectUrl(string userAgent)
        {            
            var portalSettings = PortalController.GetCurrentPortalSettings();
            if (portalSettings != null && portalSettings.ActiveTab != null)
            {
                string redirectUrl = GetRedirectUrl(userAgent, portalSettings.PortalId, portalSettings.ActiveTab.TabID);
                if (!string.IsNullOrEmpty(redirectUrl) && string.Compare(redirectUrl, portalSettings.ActiveTab.FullUrl, true, CultureInfo.InvariantCulture) != 0)
                {
                    return redirectUrl;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Get Redirection Url based on Http Context and Portal Id.         
        /// </summary>
        /// <returns>string - Empty if redirection rules are not defined or no match found</returns>
        /// <param name="userAgent">User Agent - used for client capability detection.</param>
        /// <param name="portalId">Portal Id from which Redirection Rules should be applied.</param>
        /// <param name="currentTabId">Current Tab Id that needs to be evaluated.</param>        
        public string GetRedirectUrl(string userAgent, int portalId, int currentTabId)
        {
            Requires.NotNull("userAgent", userAgent);

			string redirectUrl = string.Empty;

			IList<IRedirection> redirections = GetRedirectionsByPortal(portalId);
			//check for redirect only when redirect rules are defined
			if (redirections == null || redirections.Count == 0)
			{
                return redirectUrl;
			}

            //try to get content from cache
        	var cacheKey = string.Format(RedirectionUrlCacheKey, userAgent, portalId, currentTabId);
            redirectUrl = GetUrlFromCache(cacheKey);
            if (!string.IsNullOrEmpty(redirectUrl)) 
            {
                return redirectUrl;
            }
			
			var clientCapability = ClientCapabilityProvider.Instance().GetClientCapability(userAgent);
			var tabController = new TabController();
			foreach (var redirection in redirections)
			{
				if (redirection.Enabled)
				{
					bool checkFurther = false;
					//redirection is based on source tab
					if (redirection.SourceTabId != Null.NullInteger)
					{
						//source tab matches current tab
						if (currentTabId == redirection.SourceTabId)
						{
							checkFurther = true;
						}
							//is child tabs to be included as well
						else if (redirection.IncludeChildTabs)
						{
							//Get all the descendents of the source tab and find out if current tab is in source tab's hierarchy or not.
							foreach (var childTab in tabController.GetTabsByPortal(portalId).DescendentsOf(redirection.SourceTabId))
							{
								if (childTab.TabID == currentTabId)
								{
									checkFurther = true;
									break;
								}
							}
						}
					}
						//redirection is based on portal
					else if (redirection.SourceTabId == Null.NullInteger)
					{
						checkFurther = true;
					}

					if (checkFurther)
					{
						//check if client capability matches with this rule
						if (DoesCapabilityMatchWithRule(clientCapability, redirection))
						{
							//find the redirect url
							redirectUrl = GetRedirectUrlFromRule(redirection, portalId, currentTabId);

                            //update cache content
                            SetUrlInCache(cacheKey, redirectUrl);
							break;
						}
					}
				}
			}

        	return redirectUrl;
        }

        /// <summary>
        /// Get Url for the equivalent full site based on the current page of the mobile site
        /// </summary>
        /// <returns>string - Empty if redirection rules are not defined or no match found</returns>
        public string GetFullSiteUrl()
        {
            var portalSettings = PortalController.GetCurrentPortalSettings();
            if (portalSettings != null && portalSettings.ActiveTab != null)
            {
                string fullSiteUrl = GetFullSiteUrl(portalSettings.PortalId, portalSettings.ActiveTab.TabID);
                if (!string.IsNullOrEmpty(fullSiteUrl) && string.Compare(fullSiteUrl, portalSettings.ActiveTab.FullUrl, true, CultureInfo.InvariantCulture) != 0)
                {                    
                    return fullSiteUrl;
                }
            }

            return string.Empty; 
        }

        /// <summary>
        /// Get Url for the equivalent full site based on the current page of the mobile site
        /// </summary>
        /// <returns>string - Empty if redirection rules are not defined or no match found</returns>        
        /// <param name="portalId">Portal Id from which Redirection Rules should be applied.</param>
        /// <param name="currentTabId">Current Tab Id that needs to be evaluated.</param>        
        public string GetFullSiteUrl(int portalId, int currentTabId)
        {
            string fullSiteUrl = string.Empty;

            IList<IRedirection> redirections = GetAllRedirections();
            //check for redirect only when redirect rules are defined
            if (redirections == null || redirections.Count == 0)
            {
                return fullSiteUrl;
            }

			//try to get content from cache
			var cacheKey = string.Format(FullSiteUrlCacheKey, portalId, currentTabId);
            fullSiteUrl = GetUrlFromCache(cacheKey);
            if (!string.IsNullOrEmpty(fullSiteUrl))
            {
                return fullSiteUrl;
            }			

			bool foundRule = false;
			foreach (var redirection in redirections)
			{
				if (redirection.Enabled)
				{
					if (redirection.TargetType == TargetType.Tab) //page within same site
					{
						int targetTabId = int.Parse(redirection.TargetValue.ToString());
						if (targetTabId == currentTabId) //target tab is same as current tab
						{
							foundRule = true;
						}
					}
					else if (redirection.TargetType == TargetType.Portal) //home page of another portal
					{
						int targetPortalId = int.Parse(redirection.TargetValue.ToString());
						if (targetPortalId == portalId) //target portal is same as current portal
						{
							foundRule = true;
						}
					}

					//found the rule, let's find the url now
					if (foundRule)
					{
						////redirection is based on tab
						//Following are being commented as NavigateURL method does not return correct url for a tab in a different portal
						//always point to the home page of the other portal
						//if (redirection.SourceTabId != Null.NullInteger)
						//{
						//    fullSiteUrl = Globals.NavigateURL(redirection.SourceTabId);
						//}
						//else //redirection is based on portal
						{
							var portalSettings = new PortalSettings(redirection.PortalId);
							if (portalSettings.HomeTabId != Null.NullInteger && portalSettings.HomeTabId != currentTabId) //ensure it's not redirecting to itself
							{
								fullSiteUrl = GetPortalHomePageUrl(portalSettings);
							}
						}
						break;
					}
				}
			}

            //append special query string            
            if (!string.IsNullOrEmpty(fullSiteUrl))
                fullSiteUrl += string.Format("{0}{1}=1", fullSiteUrl.Contains("?") ? "&" : "?", DisableMobileRedirectQueryStringName);

			//update cache content
            SetUrlInCache(cacheKey, fullSiteUrl);

        	return fullSiteUrl;
        }

        /// <summary>
        /// Get Url for the equivalent mobile site based on the current page of the full site
        /// </summary>
        /// <returns>string - Empty if redirection rules are not defined or no match found</returns>
        public string GetMobileSiteUrl()
        {
            var portalSettings = PortalController.GetCurrentPortalSettings();
            if (portalSettings != null && portalSettings.ActiveTab != null)
            {
                string fullSiteUrl = GetMobileSiteUrl(portalSettings.PortalId, portalSettings.ActiveTab.TabID);
                if (!string.IsNullOrEmpty(fullSiteUrl) && string.Compare(fullSiteUrl, portalSettings.ActiveTab.FullUrl, true, CultureInfo.InvariantCulture) != 0)
                {
                    return fullSiteUrl;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Get Url for the equivalent mobile site based on the current page of the full site
        /// </summary>
        /// <returns>string - Empty if redirection rules are not defined or no match found</returns>        
        /// <param name="portalId">Portal Id from which Redirection Rules should be applied.</param>
        /// <param name="currentTabId">Current Tab Id that needs to be evaluated.</param>        
        public string GetMobileSiteUrl(int portalId, int currentTabId)
        {
            string mobileSiteUrl = string.Empty;
            IList<IRedirection> redirections = GetRedirectionsByPortal(portalId);
            //check for redirect only when redirect rules are defined
            if (redirections == null || redirections.Count == 0)
            {
                return mobileSiteUrl;
            }

			//try to get content from cache
			var cacheKey = string.Format(MobileSiteUrlCacheKey, portalId, currentTabId);
            mobileSiteUrl = GetUrlFromCache(cacheKey);
            if (!string.IsNullOrEmpty(mobileSiteUrl))
            {
                return mobileSiteUrl;
            }

			//let's try to find if this tab has any specifc rules
			foreach (var redirection in redirections)
			{
				if (redirection.Enabled)
				{
					if (redirection.SourceTabId != Null.NullInteger && currentTabId == redirection.SourceTabId)
					{
						mobileSiteUrl = GetRedirectUrlFromRule(redirection, portalId, currentTabId);
						break;
					}
				}
			}

			//tab has no specific rule, we can select the first rule
			if (string.IsNullOrEmpty(mobileSiteUrl))
			{
				var firstRedirection = redirections.FirstOrDefault(r => r.Enabled);
				if (firstRedirection != null)
				{
					mobileSiteUrl = GetRedirectUrlFromRule(firstRedirection, portalId, currentTabId);
				}
			}

            //append special query string
            if (!string.IsNullOrEmpty(mobileSiteUrl))
                mobileSiteUrl += string.Format("{0}{1}=0", mobileSiteUrl.Contains("?") ? "&" : "?", DisableMobileRedirectQueryStringName);            

			//update cache content
            SetUrlInCache(cacheKey, mobileSiteUrl);

        	return mobileSiteUrl;
        }

		/// <summary>
		/// save a redirection. If redirection.Id equals Null.NullInteger(-1), that means need to add a new redirection;
		/// otherwise will update the redirection by redirection.Id.
		/// </summary>
		/// <param name="redirection">redirection object.</param>
		public void Save(IRedirection redirection)
		{
			if(redirection.Id == Null.NullInteger || redirection.SortOrder == 0)
			{
				redirection.SortOrder = GetRedirectionsByPortal(redirection.PortalId).Count + 1;
			}

			int id = DataProvider.Instance().SaveRedirection(redirection.Id,
			                                        redirection.PortalId,
			                                        redirection.Name,
			                                        (int) redirection.Type,
			                                        redirection.SortOrder,
			                                        redirection.SourceTabId,
													redirection.IncludeChildTabs,
			                                        (int) redirection.TargetType,
			                                        redirection.TargetValue,
													redirection.Enabled,
			                                        UserController.GetCurrentUserInfo().UserID);

			foreach (IMatchRule rule in redirection.MatchRules)
			{
				DataProvider.Instance().SaveRedirectionRule(rule.Id, id, rule.Capability, rule.Expression);
			}

            var logContent = string.Format("'{0}' {1}", redirection.Name, redirection.Id == Null.NullInteger ? "Added" : "Updated");
			AddLog(logContent);

			ClearCache(redirection.PortalId);
		}

        /// <summary>
        /// Deletes all redirection rules that were set for pages that have been soft or hard deleted.
        /// </summary>
        /// <param name="portalId"></param>
        public void PurgeInvalidRedirections(int portalId)
        {
            var allTabs = new TabController().GetTabsByPortal(portalId);
            var redirects = GetRedirectionsByPortal(portalId);

            //remove rules for deleted source tabs
            foreach (var r in redirects.Where(r => r.SourceTabId != Null.NullInteger && allTabs.Where(t => t.Key == r.SourceTabId).Count() < 1))
            {
                Delete(portalId, r.Id);
            }
            
            //remove rules for deleted target tabs
            redirects = GetRedirectionsByPortal(portalId); //fresh get of rules in case some were deleted above                       
            foreach (var r in redirects.Where(r => r.TargetType == TargetType.Tab && allTabs.Where(t => t.Key == int.Parse(r.TargetValue.ToString())).Count() < 1))
            {
                Delete(portalId, r.Id);
            }

            //remove rules for deleted target portals
            redirects = GetRedirectionsByPortal(portalId); //fresh get of rules in case some were deleted above                       
            var allPortals = new PortalController().GetPortals();            
            foreach (var r in redirects.Where(r => r.TargetType == TargetType.Portal))
            {
                bool found = false;
                foreach (PortalInfo portal in allPortals)
                {
                    if (portal.PortalID == int.Parse(r.TargetValue.ToString()))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)                               
                    Delete(portalId, r.Id);
            }
        }

		/// <summary>
		/// delete a redirection.
		/// </summary>
		/// <param name="portalId">Portal's id.</param>
		/// <param name="id">the redirection's id.</param>
		public void Delete(int portalId, int id)
		{
			var delRedirection = GetRedirectionById(portalId, id);
			if (delRedirection != null)
			{
				//update the list order
				GetRedirectionsByPortal(portalId).Where(p => p.SortOrder > delRedirection.SortOrder).ToList().ForEach(p =>
				                                                                                              	{
				                                                                                              		p.SortOrder--;
				                                                                                              		Save(p);
				                                                                                              	});
				DataProvider.Instance().DeleteRedirection(id);

				var logContent = string.Format("Id '{0}' Deleted", id);
				AddLog(logContent);

				ClearCache(portalId);
			}
		}

		/// <summary>
		/// delete a redirection's match rule.
		/// </summary>
		/// <param name="portalId">Portal's id.</param>
		/// <param name="redirectionId">the redirection's id.</param>
		/// <param name="ruleId">the rule's id.</param>
		public void DeleteRule(int portalId, int redirectionId, int ruleId)
		{
			DataProvider.Instance().DeleteRedirectionRule(ruleId);

            var logContent = string.Format("Id '{0}' Removed from Redirection Id '{1}'", ruleId, redirectionId);
			AddLog(logContent);

			ClearCache(portalId);
		}

        /// <summary>
        /// get all redirections defined in system.
        /// </summary>        
        /// <returns>List of redirection.</returns>
        public IList<IRedirection> GetAllRedirections()
        {            
            var cacheArg = new CacheItemArgs(AllRedirectionsCacheKey, DataCache.RedirectionsCacheTimeOut, DataCache.RedirectionsCachePriority, "");
            return CBO.GetCachedObject<IList<IRedirection>>(cacheArg, GetAllRedirectionsCallBack);
        }

		/// <summary>
		/// get a redirection list for portal.
		/// </summary>
		/// <param name="portalId">redirection id.</param>
		/// <returns>List of redirection.</returns>
		public IList<IRedirection> GetRedirectionsByPortal(int portalId)
		{
			string cacheKey = string.Format(DataCache.RedirectionsCacheKey, portalId);
			var cacheArg = new CacheItemArgs(cacheKey, DataCache.RedirectionsCacheTimeOut, DataCache.RedirectionsCachePriority, portalId);
			return CBO.GetCachedObject<IList<IRedirection>>(cacheArg, GetRedirectionsByPortalCallBack);
		}

		/// <summary>
		/// get a specific redirection by id.
		/// </summary>
		/// <param name="portalId">the redirection belong's portal.</param>
		/// <param name="id">redirection's id.</param>
		/// <returns>redirection object.</returns>
		public IRedirection GetRedirectionById(int portalId, int id)
		{
			return GetRedirectionsByPortal(portalId).Where(r => r.Id == id).FirstOrDefault();
		}

        /// <summary>
        /// returns a target URL for the specific redirection
        /// </summary>
        /// <param name="redirection"></param>
        /// <param name="portalId"></param>
        /// <param name="currentTabId"></param>
        /// <returns></returns>
        public string GetRedirectUrlFromRule(IRedirection redirection, int portalId, int currentTabId)
        {
            string redirectUrl = string.Empty;

            if (redirection.TargetType == TargetType.Url) //independent url base
            {
                redirectUrl = redirection.TargetValue.ToString();
            }
            else if (redirection.TargetType == TargetType.Tab) //page within same site
            {
                int targetTabId = int.Parse(redirection.TargetValue.ToString());
                if (targetTabId != currentTabId) //ensure it's not redirecting to itself
                {
                    var tabController = new TabController();
                    var tab = tabController.GetTab(targetTabId, portalId, false);
                    if (tab != null && !tab.IsDeleted)
                    {
                        redirectUrl = Globals.NavigateURL(targetTabId);
                    }
                }
            }
            else if (redirection.TargetType == TargetType.Portal) //home page of another portal
            {
                int targetPortalId = int.Parse(redirection.TargetValue.ToString());
                if (targetPortalId != portalId) //ensure it's not redirecting to itself
                {
                    //check whethter the target portal still exists
                    var portalController = new PortalController();
                    if (portalController.GetPortals().Cast<PortalInfo>().Any(p => p.PortalID == targetPortalId))
                    {
                        var portalSettings = new PortalSettings(targetPortalId);
                        if (portalSettings.HomeTabId != Null.NullInteger && portalSettings.HomeTabId != currentTabId) //ensure it's not redirecting to itself
                        {
                            redirectUrl = GetPortalHomePageUrl(portalSettings);
                        }
                    }
                }
            }

            return redirectUrl;
        }

		#endregion

		#region "Private Methods"

        private string GetPortalHomePageUrl(PortalSettings portalSettings)
        {
            //the commented line doesn't work because the call to NavigateUrl returns url from the current portal not target portal
            //possible issue in FriendlyUrlProvider.GetFriendlyAlias method
            //redirectUrl = Globals.NavigateURL(portalSettings.HomeTabId, portalSettings, Null.NullString, null);
            return Globals.AddHTTP(portalSettings.DefaultPortalAlias);            
        }

		private IList<IRedirection> GetAllRedirectionsCallBack(CacheItemArgs cacheItemArgs)
		{			
			return CBO.FillCollection<Redirection>(DataProvider.Instance().GetAllRedirections()).Cast<IRedirection>().ToList();
		}

        private IList<IRedirection> GetRedirectionsByPortalCallBack(CacheItemArgs cacheItemArgs)
        {
            int portalId = (int)cacheItemArgs.ParamList[0];
            return CBO.FillCollection<Redirection>(DataProvider.Instance().GetRedirections(portalId)).Cast<IRedirection>().ToList();
        }

		private void ClearCache(int portalId)
		{
			DataCache.RemoveCache(string.Format(DataCache.RedirectionsCacheKey, portalId));
            DataCache.RemoveCache(AllRedirectionsCacheKey);
			DataCache.RemoveCache(UrlsCacheKey);
		}        

        private string GetUrlFromCache(string cacheKey)
        {
            var cachedUrls = DataCache.GetCache<SharedDictionary<string, string>>(UrlsCacheKey);

            if (cachedUrls != null)
            {
                using (cachedUrls.GetReadLock())
                {                
                    if (cachedUrls.ContainsKey(cacheKey))
                    {
                        return cachedUrls[cacheKey];                
                    }				    
                }
            }            

            return string.Empty;
        }

        private void SetUrlInCache(string cacheKey, string url)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                return;
            }

            var cachedUrls = DataCache.GetCache<SharedDictionary<string, string>>(UrlsCacheKey);

            if (cachedUrls == null)
            {
                cachedUrls = new SharedDictionary<string, string>();
            }
            using (cachedUrls.GetWriteLock())
            {
                cachedUrls[cacheKey] = url;
            }

            DataCache.SetCache(UrlsCacheKey, cachedUrls, TimeSpan.FromMinutes(UrlsCacheTimeout));
        }

		private void AddLog(string logContent)
		{
			var objEventLog = new EventLogController();
            objEventLog.AddLog("Site Redirection Rule", logContent, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, EventLogController.EventLogType.ADMIN_ALERT);
		}

        private bool DoesCapabilityMatchWithRule(IClientCapability clientCapability, IRedirection redirection)
        {
            bool match = false;            
            if (redirection.Type == RedirectionType.Tablet && clientCapability.IsTablet)
            {
                match = true;
            }
            else if (redirection.Type == RedirectionType.MobilePhone && clientCapability.IsMobile)
            {
                match = true;
            }
            else if (redirection.Type == RedirectionType.AllMobile && (clientCapability.IsMobile || clientCapability.IsTablet))
            {
                match = true;
            }
            else if (redirection.Type == RedirectionType.Other)
            {
                //match all the capabilities defined in the rule
                int matchCount = 0;
                foreach (IMatchRule rule in redirection.MatchRules)
                {
                    if (clientCapability.Capabilities != null && clientCapability.Capabilities.ContainsKey(rule.Capability))
                    {
                        if (clientCapability.Capabilities[rule.Capability].Equals(rule.Expression, StringComparison.InvariantCultureIgnoreCase))
                        {
                            matchCount++;
                        }
                    }
                }
                if(matchCount > 0 && matchCount == redirection.MatchRules.Count)
                {
                    match = true;
                }
            }
                           
            return match;
        }

		#endregion
	
        #region Private Properties
	    private string AllRedirectionsCacheKey
	    {
            get
            {
                return string.Format(DataCache.RedirectionsCacheKey, "All");
            }
	    }
        #endregion
    }
}
