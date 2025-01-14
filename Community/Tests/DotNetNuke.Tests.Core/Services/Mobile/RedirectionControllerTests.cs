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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;
using System.Web;

using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.ClientCapability;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mobile;
using DotNetNuke.Tests.Instance.Utilities;
using DotNetNuke.Tests.Utilities.Mocks;

using Moq;

using NUnit.Framework;

namespace DotNetNuke.Tests.Core.Services.Mobile
{
	/// <summary>
	///   Summary description for RedirectionControllerTests
	/// </summary>
	[TestFixture]
	public class RedirectionControllerTests
	{
		#region "Private Properties"

		private Mock<DataProvider> _dataProvider;
		private RedirectionController _redirectionController;
        private Mock<ClientCapabilityProvider> _clientCapabilityProvider;	    

		private DataTable _dtRedirections;
		private DataTable _dtRules;
		
        public const string iphoneUserAgent = "Mozilla/5.0 (iPod; U; CPU iPhone OS 4_0 like Mac OS X; en-us) AppleWebKit/532.9 (KHTML, like Gecko) Version/4.0.5 Mobile/8A293 Safari/6531.22.7";
        public const string wp7UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows Phone OS 7.0; Trident/3.1; IEMobile/7.0) Asus;Galaxy6";
        public const string msIE8UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; InfoPath.3; Creative AutoUpdate v1.40.02)";
        public const string msIE9UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
        public const string msIE10UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
        public const string fireFox5NT61UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:5.0) Gecko/20110619 Firefox/5.0";
        public const string iPadTabletUserAgent = "Mozilla/5.0 (iPad; U; CPU OS 3_2 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Version/4.0.4 Mobile/7B334b Safari/531.21.10";
        public const string samsungGalaxyTablet = "Mozilla/5.0 (Linux; U; Android 2.2; en-gb; SAMSUNG GT-P1000 Tablet Build/MASTER) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1";
        public const string winTabletPC = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705; Tablet PC 2.0)";
        public const string htcDesireVer1Sub22UserAgent = "Mozilla/5.0 (Linux; U; Android 2.2; sv-se; Desire_A8181 Build/FRF91) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1";
        public const string blackBerry9105V1 = "BlackBerry9105/5.0.0.696 Profile/MIDP-2.1 Configuration/CLDC-1.1 VendorID/133";
        public const string motorolaRIZRSymbianOSOpera865 = "MOTORIZR-Z8/46.00.00 Mozilla/4.0 (compatible; MSIE 6.0; Symbian OS; 356) Opera 8.65 [it] UP.Link/6.3.0.0.0";

	    public const int Portal0 = 0;
        public const int Portal1 = 1;
        public const int Portal2 = 2;
        public const int Page1 = 1;
        public const int Page2 = 2;
        public const int Page3 = 3;
	    public const int SortOrder1 = 1;
        public const int SortOrder2 = 1;
        public const int SortOrder3 = 1;
		public const string PortalAlias0 = "www.portal0.com";
		public const string PortalAlias1 = "www.portal1.com";
        public const int AnotherPageOnSamePortal = 56;
        public const int DeletedPageOnSamePortal = 59;
        public const int DeletedPageOnSamePortal2 = 94;
        public const int HomePageOnPortal0 = 55;
        public const int HomePageOnPortal1 = 57;
        public const int MobileLandingPage = 91;
        public const int TabletLandingPage = 92;
        public const int AllMobileLandingPage = 93;
	    public const bool EnabledFlag = true;
        public const bool DisabledFlag = false;
        public const bool IncludeChildTabsFlag = true;
	    public const string ExternalSite = "http://www.dotnetnuke.com";

		private const string DisableMobileRedirectCookieName = "disablemobileredirect";
		private const string DisableRedirectPresistCookieName = "disableredirectpresist";
		private const string DisableMobileRedirectQueryStringName = "nomo";

		#endregion

		#region "Set Up"

		[SetUp]
		public void SetUp()
		{
			ComponentFactory.Container = new SimpleContainer();
			_dataProvider = MockComponentProvider.CreateDataProvider();
			MockComponentProvider.CreateDataCacheProvider();
			MockComponentProvider.CreateEventLogController();
            _clientCapabilityProvider = MockComponentProvider.CreateNew<ClientCapabilityProvider>();

			_redirectionController = new RedirectionController();

			SetupDataProvider();
			SetupClientCapabilityProvider();
			SetupRoleProvider();
		}

		#endregion

		#region "Tests"

		#region "CURD API Tests"

		[Test]
		public void RedirectionController_Save_Valid_Redirection()
		{
			var redirection = new Redirection { Name = "Test R", PortalId = Portal0, SortOrder = 1, SourceTabId = -1, Type = RedirectionType.MobilePhone, TargetType = TargetType.Portal, TargetValue = Portal1 };
			_redirectionController.Save(redirection);

			var dataReader = _dataProvider.Object.GetRedirections(Portal0);
			var affectedCount = 0;
			while (dataReader.Read())
			{
				affectedCount++;
			}
			Assert.AreEqual(1, affectedCount);
		}

		[Test]
		public void RedirectionController_Save_ValidRedirection_With_Rules()
		{
			var redirection = new Redirection { Name = "Test R", PortalId = Portal0, SortOrder = 1, SourceTabId = -1, IncludeChildTabs = true, Type = RedirectionType.Other, TargetType = TargetType.Portal, TargetValue = Portal1 };
			redirection.MatchRules.Add(new MatchRule { Capability = "Platform", Expression = "IOS" });
			redirection.MatchRules.Add(new MatchRule { Capability = "Version", Expression = "5" });
			_redirectionController.Save(redirection);

			var dataReader = _dataProvider.Object.GetRedirections(Portal0);
			var affectedCount = 0;
			while (dataReader.Read())
			{
				affectedCount++;
			}
			Assert.AreEqual(1, affectedCount);

			var getRe = _redirectionController.GetRedirectionsByPortal(Portal0)[0];
			Assert.AreEqual(2, getRe.MatchRules.Count);
		}

		[Test]
		public void RedirectionController_GetRedirectionsByPortal_With_Valid_PortalID()
		{
			PrepareData();

			IList<IRedirection> list = _redirectionController.GetRedirectionsByPortal(Portal0);

			Assert.AreEqual(7, list.Count);
		}

		[Test]
		public void RedirectionController_Delete_With_ValidID()
		{
			PrepareData();
			_redirectionController.Delete(Portal0, 1);

			IList<IRedirection> list = _redirectionController.GetRedirectionsByPortal(Portal0);

			Assert.AreEqual(6, list.Count);
		}

        [Test]
        public void RedirectionController_PurgeInvalidRedirections_DoNotPurgeRuleForNonDeletetedSource()
        {
            _dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, SortOrder1, HomePageOnPortal0, IncludeChildTabsFlag, (int)TargetType.Tab, AnotherPageOnSamePortal, EnabledFlag);
            _redirectionController.PurgeInvalidRedirections(0);
            Assert.AreEqual(1, _redirectionController.GetRedirectionsByPortal(0).Count);
        }

        [Test]
        public void RedirectionController_PurgeInvalidRedirections_DoPurgeRuleForDeletetedSource()
        {
            _dtRedirections.Rows.Add(new object[] { 1, Portal0, "R1", (int)RedirectionType.MobilePhone, SortOrder1, DeletedPageOnSamePortal2, IncludeChildTabsFlag, (int)TargetType.Tab, AnotherPageOnSamePortal, EnabledFlag });
            _redirectionController.PurgeInvalidRedirections(0);
            Assert.AreEqual(0, _redirectionController.GetRedirectionsByPortal(0).Count);
        }

        [Test]
        public void RedirectionController_PurgeInvalidRedirections_DoPurgeRuleForDeletetedTargetPortal()
        {
            _dtRedirections.Rows.Add(new object[] { 1, Portal0, "R1", (int)RedirectionType.MobilePhone, SortOrder1, HomePageOnPortal0, IncludeChildTabsFlag, (int)TargetType.Portal, Portal2, EnabledFlag });
            _redirectionController.PurgeInvalidRedirections(0);
            Assert.AreEqual(0, _redirectionController.GetRedirectionsByPortal(0).Count);
        }

        [Test]
        public void RedirectionController_PurgeInvalidRedirections_DoPurgeRuleForDeletetedTargetTab()
        {
            _dtRedirections.Rows.Add(new object[] { 1, Portal0, "R1", (int)RedirectionType.MobilePhone, SortOrder1, HomePageOnPortal0, IncludeChildTabsFlag, (int)TargetType.Tab, DeletedPageOnSamePortal2, EnabledFlag });
            _redirectionController.PurgeInvalidRedirections(0);
            Assert.AreEqual(0, _redirectionController.GetRedirectionsByPortal(0).Count);
        }

		#endregion

		#region "Get Redirections URL Tests"

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RedirectionController_GetRedirectionUrl_Throws_On_Null_UserAgent()
        {
			_redirectionController.GetRedirectUrl(null, Portal0, 0);
        }
        
        [Test]
        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_Redirection_IsNotSet()
		{
			Assert.AreEqual(string.Empty, _redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, HomePageOnPortal0));
		}

        [Test]
        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_Redirection_IsNotEnabled()
        {
            PrepareSingleDisabledRedirectionRule();
			Assert.AreEqual(string.Empty, _redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, HomePageOnPortal0));
        }        

        [Test]
        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_UserAgent_Is_Desktop()
        {
            PrepareData();
			Assert.AreEqual(string.Empty, _redirectionController.GetRedirectUrl(msIE9UserAgent, Portal0, HomePageOnPortal0));
        }

        [Test]
        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_CurrentPage_IsSameAs_TargetPage_OnMobile()
        {
            PreparePortalToAnotherPageOnSamePortal();
			Assert.AreEqual(string.Empty, _redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, AnotherPageOnSamePortal));
        }

        [Test]
        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_TargetPage_IsDeleted()
        {
            //prepare rule to a deleted tab on the same portal
            _dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, AnotherPageOnSamePortal, EnabledFlag, (int)TargetType.Tab, DeletedPageOnSamePortal, 1);
            Assert.AreEqual(string.Empty, _redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, AnotherPageOnSamePortal));
        }

        [Test]
        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_CurrentPortal_IsSameAs_TargetPortal_OnMobile()
        {
            PrepareSamePortalToSamePortalRedirectionRule();
			Assert.AreEqual(string.Empty, _redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, AnotherPageOnSamePortal));
        }

        [Test]
        public void RedirectionController_GetRedirectionUrl_Returns_TargetPageOnSamePortal_When_Surfing_HomePage_OnMobile()
        {
            PreparePortalToAnotherPageOnSamePortal();
			Assert.AreEqual(NavigateUrl(AnotherPageOnSamePortal), _redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 1));
        }

        //[Test]
        //public void RedirectionController_GetRedirectionUrl_Returns_HomePageOfOtherPortal_When_Surfing_AnyPageOfCurrentPortal_OnMobile()
        //{
        //    PrepareHomePageToHomePageRedirectionRule();
        //    Assert.AreEqual(DotNetNuke.Common.Globals.AddHTTP(PortalAlias1), _redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 1));
        //    Assert.AreEqual(DotNetNuke.Common.Globals.AddHTTP(PortalAlias1), _redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 2));            
        //}

        [Test]
        public void RedirectionController_GetRedirectionUrl_Returns_ExternalSite_When_Surfing_AnyPageOfCurrentPortal_OnMobile()
        {
            PrepareExternalSiteRedirectionRule();
			Assert.AreEqual(ExternalSite, _redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 1));
			Assert.AreEqual(ExternalSite, _redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 2));
        }

        [Test]
        public void RedirectionController_GetRedirectionUrl_Returns_MobileLanding_ForMobile_And_TabletLanding_ForTablet()
        {
            PrepareMobileAndTabletRedirectionRuleWithMobileFirst();
			Assert.AreEqual(NavigateUrl(MobileLandingPage), _redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 1));
			Assert.AreEqual(NavigateUrl(TabletLandingPage), _redirectionController.GetRedirectUrl(iPadTabletUserAgent, Portal0, 1));                        
        }

        [Test]
        public void RedirectionController_GetRedirectionUrl_Returns_TabletLanding_ForTablet_And_MobileLanding_ForMobile()
        {
            PrepareMobileAndTabletRedirectionRuleWithAndTabletRedirectionRuleTabletFirst();
            Assert.AreEqual(NavigateUrl(MobileLandingPage), _redirectionController.GetRedirectUrl(iphoneUserAgent, 0, 1));
            Assert.AreEqual(NavigateUrl(TabletLandingPage), _redirectionController.GetRedirectUrl(iPadTabletUserAgent, 0, 1));
        }

        [Test]
        public void RedirectionController_GetRedirectionUrl_Returns_SameLandingPage_For_AllMobile()
        {
            PrepareAllMobileRedirectionRule();
			string mobileLandingPage = _redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 1);
			string tabletLandingPage = _redirectionController.GetRedirectUrl(iPadTabletUserAgent, Portal0, 1);
            Assert.AreEqual(NavigateUrl(AllMobileLandingPage), mobileLandingPage);
            Assert.AreEqual(NavigateUrl(AllMobileLandingPage), tabletLandingPage);
            Assert.AreEqual(mobileLandingPage, tabletLandingPage);
        }

        [Test]
        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_Capability_DoesNot_Match()
        {
            PrepareOperaBrowserOnSymbianOSRedirectionRule();
			Assert.AreEqual(string.Empty, _redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 1));
        }

        [Test]
        public void RedirectionController_GetRedirectionUrl_Returns_ValidUrl_When_Capability_Matches()
        {
            PrepareOperaBrowserOnSymbianOSRedirectionRule();
			Assert.AreEqual(NavigateUrl(AnotherPageOnSamePortal), _redirectionController.GetRedirectUrl(motorolaRIZRSymbianOSOpera865, Portal0, 1));
        }

        [Test]
        public void RedirectionController_GetRedirectionUrl_Returns_EmptyString_When_NotAll_Capability_Matches()
        {
            PrepareOperaBrowserOnIPhoneOSRedirectionRule();
			Assert.AreEqual(string.Empty, _redirectionController.GetRedirectUrl(iphoneUserAgent, Portal0, 1));
        }
                

		#endregion

		#region "Get FullSite Url Tests"

		[Test]
        public void RedirectionController_GetFullSiteUrl_With_NoRedirections()
		{
			var url = _redirectionController.GetFullSiteUrl(Portal0, HomePageOnPortal0);

			Assert.AreEqual(string.Empty, url);
		}

        //[Test]
        //public void RedirectionController_GetFullSiteUrl_When_Redirect_Between_Different_Portals()
        //{
        //    _dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, -1, EnabledFlag, (int)TargetType.Portal, "1", 1);

        //    var url = _redirectionController.GetFullSiteUrl(Portal1, HomePageOnPortal1);
			
        //    Assert.AreEqual(Globals.AddHTTP(PortalAlias0), url);
        //}

        //[Test]
        //public void RedirectionController_GetFullSiteUrl_When_Redirect_In_Same_Portal()
        //{
        //    _dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, HomePageOnPortal0, EnabledFlag, (int)TargetType.Tab, AnotherPageOnSamePortal, 1);

        //    var url = _redirectionController.GetFullSiteUrl(Portal1, AnotherPageOnSamePortal);

        //    //Assert.AreEqual(string.Empty, url);
        //}

		[Test]
		public void RedirectionController_GetFullSiteUrl_When_Redirect_To_DifferentUrl()
		{
			_dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, HomePageOnPortal0, EnabledFlag, (int)TargetType.Url, ExternalSite, 1);

			var url = _redirectionController.GetFullSiteUrl(Portal1, AnotherPageOnSamePortal);

			Assert.AreEqual(string.Empty, url);
		}

		#endregion

        #region "Get MobileSite Url Tests"

        [Test]
        public void RedirectionController_GetMobileSiteUrl_With_NoRedirections()
        {
            var url = _redirectionController.GetMobileSiteUrl(Portal0, HomePageOnPortal0);

            Assert.AreEqual(string.Empty, url);
        }

        [Test]
        public void RedirectionController_GetMobileSiteUrl_Returns_Page_Specific_Url_When_Multiple_PageLevel_Redirects_Defined()
        {
            string redirectUrlPage1 = "m.yahoo.com";
            string redirectUrlPage2 = "m.cnn.com";

            //first page goes to one url
            _dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, Page1, EnabledFlag, (int)TargetType.Url, redirectUrlPage1, 1);

            //second page goes to another url (this is Tablet - it should not matter)
            _dtRedirections.Rows.Add(2, Portal0, "R2", (int)RedirectionType.Tablet, 2, Page2, EnabledFlag, (int)TargetType.Url, redirectUrlPage2, 1);

            var mobileUrlForPage1 = _redirectionController.GetMobileSiteUrl(Portal0, Page1);
            var mobileUrlForPage2 = _redirectionController.GetMobileSiteUrl(Portal0, Page2);
            var mobileUrlForPage3 = _redirectionController.GetMobileSiteUrl(Portal0, Page3);

            //First Page returns link to first url
            Assert.AreEqual(String.Format("{0}?nomo=0",redirectUrlPage1), mobileUrlForPage1);

            //Second Page returns link to second url
            Assert.AreEqual(String.Format("{0}?nomo=0", redirectUrlPage2), mobileUrlForPage2);
            
            //Third Page returns link to first url - as this is the first found url and third page has no redirect defined
            Assert.AreEqual(mobileUrlForPage3, String.Format("{0}?nomo=0", redirectUrlPage1));
        }

        //[Test]
        //public void RedirectionController_GetMobileSiteUrl_Works_When_Page_Redirects_To_Another_Portal()
        //{
        //    //first page goes to one second portal
        //    _dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, -1, EnabledFlag, (int)TargetType.Portal, Portal1, 1);            

        //    var mobileUrlForPage1 = _redirectionController.GetMobileSiteUrl(Portal0, Page1);

        //    //First Page returns link to home page of other portal
        //    Assert.AreEqual(Globals.AddHTTP(PortalAlias1), mobileUrlForPage1);
        //}

        [Test]
        public void RedirectionController_GetMobileSiteUrl_When_Redirect_To_DifferentUrl()
        {
            _dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, HomePageOnPortal0, EnabledFlag, (int)TargetType.Url, ExternalSite, 1);

            var url = _redirectionController.GetMobileSiteUrl(Portal1, AnotherPageOnSamePortal);

            Assert.AreEqual(string.Empty, url);
        }

        #endregion

		#region "Redirect Enable/Disable Tests"

		[Test]
		public void RedirectionController_IsRedirectAllowedForTheSession_In_Normal_Action()
		{
			var app = GenerateApplication();

			Assert.IsTrue(_redirectionController.IsRedirectAllowedForTheSession(app));
		}

		[Test]
		public void RedirectionController_IsRedirectAllowedForTheSession_With_Nonmo_Param_Set_To_1()
		{
			var app = GenerateApplication();
			app.Context.Request.QueryString.Add(DisableMobileRedirectQueryStringName, "1");
			
			Assert.IsFalse(_redirectionController.IsRedirectAllowedForTheSession(app));
			Assert.IsNotNull(app.Request.Cookies[DisableMobileRedirectCookieName]);
			Assert.IsNotNull(app.Request.Cookies[DisableRedirectPresistCookieName]);
		}

		[Test]
		public void RedirectionController_IsRedirectAllowedForTheSession_With_Nonmo_Param_Set_To_0()
		{
			var app = GenerateApplication();
			app.Context.Request.QueryString.Add(DisableMobileRedirectQueryStringName, "0");

			Assert.IsTrue(_redirectionController.IsRedirectAllowedForTheSession(app));
		}

		[Test]
		public void RedirectionController_IsRedirectAllowedForTheSession_With_Nonmo_Param_Set_To_1_And_Then_Setback_To_0()
		{
			var app = GenerateApplication();
			app.Context.Request.QueryString.Add(DisableMobileRedirectQueryStringName, "1");
			Assert.IsFalse(_redirectionController.IsRedirectAllowedForTheSession(app));

			app.Context.Request.QueryString.Add(DisableMobileRedirectQueryStringName, "0");
			Assert.IsTrue(_redirectionController.IsRedirectAllowedForTheSession(app));
		}

		#endregion

		#endregion

		#region "Private Methods"

		private void SetupDataProvider()
		{
			_dataProvider.Setup(d => d.GetProviderPath()).Returns("");

			_dtRedirections = new DataTable("Redirections");
			var pkCol = _dtRedirections.Columns.Add("Id", typeof(int));
			_dtRedirections.Columns.Add("PortalId", typeof(int));
			_dtRedirections.Columns.Add("Name", typeof(string));
			_dtRedirections.Columns.Add("Type", typeof(int));
			_dtRedirections.Columns.Add("SortOrder", typeof(int));
			_dtRedirections.Columns.Add("SourceTabId", typeof(int));
			_dtRedirections.Columns.Add("IncludeChildTabs", typeof(bool));
			_dtRedirections.Columns.Add("TargetType", typeof(int));
			_dtRedirections.Columns.Add("TargetValue", typeof(object));
			_dtRedirections.Columns.Add("Enabled", typeof(bool));

			_dtRedirections.PrimaryKey = new[] { pkCol };

			_dtRules = new DataTable("Rules");
			var pkCol1 = _dtRules.Columns.Add("Id", typeof(int));
			_dtRules.Columns.Add("RedirectionId", typeof(int));
			_dtRules.Columns.Add("Capability", typeof(string));
			_dtRules.Columns.Add("Expression", typeof(string));

			_dtRules.PrimaryKey = new[] { pkCol1 };

			_dataProvider.Setup(d =>
								d.SaveRedirection(It.IsAny<int>(),
								It.IsAny<int>(),
								It.IsAny<string>(),
								It.IsAny<int>(),
								It.IsAny<int>(),
								It.IsAny<int>(),
								It.IsAny<bool>(),
								It.IsAny<int>(),
								It.IsAny<object>(),
								It.IsAny<bool>(),
								It.IsAny<int>())).Returns<int, int, string, int, int, int, bool, int, object, bool, int>(
															(id, portalId, name, type, sortOrder, sourceTabId, includeChildTabs, targetType, targetValue, enabled, userId) =>
															{
																if (id == -1)
																{
																	if (_dtRedirections.Rows.Count == 0)
																	{
																		id = 1;
																	}
																	else
																	{
																		id = Convert.ToInt32(_dtRedirections.Select("", "Id Desc")[0]["Id"]) + 1;
																	}

																	var row = _dtRedirections.NewRow();
																	row["Id"] = id;
																	row["PortalId"] = portalId;
																	row["name"] = name;
																	row["type"] = type;
																	row["sortOrder"] = sortOrder;
																	row["sourceTabId"] = sourceTabId;
																	row["includeChildTabs"] = includeChildTabs;
																	row["targetType"] = targetType;
																	row["targetValue"] = targetValue;
																	row["enabled"] = enabled;

																	_dtRedirections.Rows.Add(row);
																}
																else
																{
																	var rows = _dtRedirections.Select("Id = " + id);
																	if (rows.Length == 1)
																	{
																		var row = rows[0];

																		row["name"] = name;
																		row["type"] = type;
																		row["sortOrder"] = sortOrder;
																		row["sourceTabId"] = sourceTabId;
																		row["includeChildTabs"] = includeChildTabs;
																		row["targetType"] = targetType;
																		row["targetValue"] = targetValue;
																		row["enabled"] = enabled;
																	}
																}

																return id;
															});

			_dataProvider.Setup(d => d.GetRedirections(It.IsAny<int>())).Returns<int>(GetRedirectionsCallBack);
			_dataProvider.Setup(d => d.DeleteRedirection(It.IsAny<int>())).Callback<int>((id) =>
			{
				var rows = _dtRedirections.Select("Id = " + id);
				if (rows.Length == 1)
				{
					_dtRedirections.Rows.Remove(rows[0]);
				}
			});

			_dataProvider.Setup(d => d.SaveRedirectionRule(It.IsAny<int>(),
				It.IsAny<int>(),
				It.IsAny<string>(),
				It.IsAny<string>())).Callback<int, int, string, string>((id, rid, capbility, expression) =>
				{
					if (id == -1)
					{
						if (_dtRules.Rows.Count == 0)
						{
							id = 1;
						}
						else
						{
							id = Convert.ToInt32(_dtRules.Select("", "Id Desc")[0]["Id"]) + 1;
						}

                        var row = _dtRules.NewRow();
						row["Id"] = id;
						row["RedirectionId"] = rid;
						row["capability"] = capbility;
						row["expression"] = expression;

						_dtRules.Rows.Add(row);
					}
					else
					{
						var rows = _dtRules.Select("Id = " + id);
						if (rows.Length == 1)
						{
							var row = rows[0];

							row["capability"] = capbility;
							row["expression"] = expression;
						}
					}
				});

			_dataProvider.Setup(d => d.GetRedirectionRules(It.IsAny<int>())).Returns<int>(GetRedirectionRulesCallBack);
			_dataProvider.Setup(d => d.DeleteRedirectionRule(It.IsAny<int>())).Callback<int>((id) =>
			{
				var rows = _dtRules.Select("Id = " + id);
				if (rows.Length == 1)
				{
					_dtRules.Rows.Remove(rows[0]);
				}
			});

            _dataProvider.Setup(d => d.GetPortals(It.IsAny<string>())).Returns<string>(GetPortalsCallBack);
			_dataProvider.Setup(d => d.GetPortal(It.IsAny<int>(), It.IsAny<string>())).Returns<int, string>(GetPortalCallBack);
			_dataProvider.Setup(d => d.GetTabs(It.IsAny<int>())).Returns<int>(GetTabsCallBack);
			_dataProvider.Setup(d => d.GetTabModules(It.IsAny<int>())).Returns<int>(GetTabModulesCallBack);
			_dataProvider.Setup(d => d.GetPortalSettings(It.IsAny<int>(), It.IsAny<string>())).Returns<int, string>(GetPortalSettingsCallBack);
			_dataProvider.Setup(d => d.GetAllRedirections()).Returns(GetAllRedirectionsCallBack);

			var portalDataService = MockComponentProvider.CreateNew<DotNetNuke.Entities.Portals.Data.IDataService>();
			portalDataService.Setup(p => p.GetPortalGroups()).Returns(GetPortalGroupsCallBack);
		}

		private void SetupClientCapabilityProvider()
		{		    
            _clientCapabilityProvider.Setup(p => p.GetClientCapability(It.IsAny<string>())).Returns<string>(GetClientCapabilityCallBack);
		}

		private void SetupRoleProvider()
		{
			var mockRoleProvider = MockComponentProvider.CreateNew<RoleProvider>();

			mockRoleProvider.Setup(p => p.GetRole(It.IsAny<int>(), It.IsAny<int>())).Returns<int, int>((portalId, roleId) =>
			{
				RoleInfo roleInfo = new RoleInfo();
				roleInfo.RoleID = roleId;
				roleInfo.PortalID = portalId;
				if (roleId == 1)
				{
					roleInfo.RoleName = "Administrators";
				}

				return roleInfo;
			});
		}

		private IDataReader GetRedirectionsCallBack(int portalId)
		{
			var dtCheck = _dtRedirections.Clone();
			foreach (var row in _dtRedirections.Select("PortalId = " + portalId))
			{
				dtCheck.Rows.Add(row.ItemArray);
			}

			return dtCheck.CreateDataReader();
		}

		private IDataReader GetRedirectionRulesCallBack(int rid)
		{
			var dtCheck = _dtRules.Clone();
			foreach (var row in _dtRules.Select("RedirectionId = " + rid))
			{
				dtCheck.Rows.Add(row.ItemArray);
			}

			return dtCheck.CreateDataReader();
		}

        private IDataReader GetPortalsCallBack(string culture)
        {
            return GetPortalCallBack(Portal0, DotNetNuke.Services.Localization.Localization.SystemLocale);
        }

		private IDataReader GetPortalCallBack(int portalId, string culture)
		{
			DataTable table = new DataTable("Portal");

			var cols = new string[]
			           	{
			           		"PortalID", "PortalGroupID", "PortalName", "LogoFile", "FooterText", "ExpiryDate", "UserRegistration", "BannerAdvertising", "AdministratorId", "Currency", "HostFee",
			           		"HostSpace", "PageQuota", "UserQuota", "AdministratorRoleId", "RegisteredRoleId", "Description", "KeyWords", "BackgroundFile", "GUID", "PaymentProcessor", "ProcessorUserId",
			           		"ProcessorPassword", "SiteLogHistory", "Email", "DefaultLanguage", "TimezoneOffset", "AdminTabId", "HomeDirectory", "SplashTabId", "HomeTabId", "LoginTabId", "RegisterTabId",
			           		"UserTabId", "SearchTabId", "SuperTabId", "CreatedByUserID", "CreatedOnDate", "LastModifiedByUserID", "LastModifiedOnDate", "CultureCode"
			           	};

			foreach (var col in cols)
			{
				table.Columns.Add(col);
			}

		    int homePage = 55;
            if (portalId == Portal0)
                homePage = HomePageOnPortal0;
            else if (portalId == Portal1)
                homePage = HomePageOnPortal1;

            table.Rows.Add(portalId, null, "My Website", "Logo.png", "Copyright 2011 by DotNetNuke Corporation", null, "2", "0", "2", "USD", "0", "0", "0", "0", "0", "1", "My Website", "DotNetNuke, DNN, Content, Management, CMS", null, "1057AC7A-3C08-4849-A3A6-3D2AB4662020", null, null, null, "0", "admin@change.me", "en-US", "-8", "58", "Portals/0", null, homePage.ToString(), null, null, "57", "56", "7", "-1", "2011-08-25 07:34:11", "-1", "2011-08-25 07:34:29", culture);

			return table.CreateDataReader();
		}

		private IDataReader GetTabsCallBack(int portalId)
		{
			DataTable table = new DataTable("Tabs");

			var cols = new string[]
			           	{
							"TabID","UniqueId","VersionGuid","DefaultLanguageGuid","LocalizedVersionGuid","TabOrder","PortalID","TabName","IsVisible","ParentId","Level","IconFile","IconFileLarge","DisableLink","Title","Description","KeyWords","IsDeleted","SkinSrc","ContainerSrc","TabPath","StartDate","EndDate","Url","HasChildren","RefreshInterval","PageHeadText","IsSecure","PermanentRedirect","SiteMapPriority","ContentItemID","Content","ContentTypeID","ModuleID","ContentKey","Indexed","CultureCode","CreatedByUserID","CreatedOnDate","LastModifiedByUserID","LastModifiedOnDate"
			           	};

			foreach (var col in cols)
			{
				table.Columns.Add(col);
			}

            if(portalId == Portal1)
            {
                table.Rows.Add(HomePageOnPortal1, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "3", "0", "HomePageOnPortal1", true, null, "0", null, null, false, "", "", "", false, "[G]Skins/DarkKnight/Home-Mega-Menu.ascx", "[G]Containers/DarkKnight/SubTitle_Grey.ascx", "//HomePageOnPortal1", null, null, "", false, null, null, false, false, "0.5", "89", "HomePageOnPortal1", "1", "-1", null, false, null, "-1", DateTime.Now, "-1", DateTime.Now);    
            }
            else if (portalId == Portal0)
            {
                table.Rows.Add(HomePageOnPortal0, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "3", "0", "HomePageOnPortal0", true, null, "0", null, null, false, "", "", "", false, "[G]Skins/DarkKnight/Home-Mega-Menu.ascx", "[G]Containers/DarkKnight/SubTitle_Grey.ascx", "//HomePageOnPortal0", null, null, "", false, null, null, false, false, "0.5", "89", "HomePageOnPortal0", "1", "-1", null, false, null, "-1", DateTime.Now, "-1", DateTime.Now);
                table.Rows.Add(AnotherPageOnSamePortal, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "4", "0", "AnotherPageOnSamePortal", true, null, "0", null, null, false, "", "", "", false, "[G]Skins/DarkKnight/Home-Mega-Menu.ascx", "[G]Containers/DarkKnight/SubTitle_Grey.ascx", "//AnotherPageOnSamePortal", null, null, "", false, null, null, false, false, "0.5", "89", "HomePageOnPortal0", "1", "-1", null, false, null, "-1", DateTime.Now, "-1", DateTime.Now);
                table.Rows.Add(MobileLandingPage, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "5", "0", "MobileLandingPage", true, null, "0", null, null, false, "", "", "", false, "[G]Skins/DarkKnight/Home-Mega-Menu.ascx", "[G]Containers/DarkKnight/SubTitle_Grey.ascx", "//MobileLandingPage", null, null, "", false, null, null, false, false, "0.5", "89", "HomePageOnPortal0", "1", "-1", null, false, null, "-1", DateTime.Now, "-1", DateTime.Now);
                table.Rows.Add(TabletLandingPage, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "6", "0", "TabletLandingPage", true, null, "0", null, null, false, "", "", "", false, "[G]Skins/DarkKnight/Home-Mega-Menu.ascx", "[G]Containers/DarkKnight/SubTitle_Grey.ascx", "//TabletLandingPage", null, null, "", false, null, null, false, false, "0.5", "89", "HomePageOnPortal0", "1", "-1", null, false, null, "-1", DateTime.Now, "-1", DateTime.Now);
                table.Rows.Add(AllMobileLandingPage, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "7", "0", "AllMobileLandingPage", true, null, "0", null, null, false, "", "", "", false, "[G]Skins/DarkKnight/Home-Mega-Menu.ascx", "[G]Containers/DarkKnight/SubTitle_Grey.ascx", "//AllMobileLandingPage", null, null, "", false, null, null, false, false, "0.5", "89", "HomePageOnPortal0", "1", "-1", null, false, null, "-1", DateTime.Now, "-1", DateTime.Now);    
                table.Rows.Add(DeletedPageOnSamePortal, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), "8", "0", "A Deleted Page", true, null, "0", null, null, false, "", "", "", true, "[G]Skins/DarkKnight/Home-Mega-Menu.ascx", "[G]Containers/DarkKnight/SubTitle_Grey.ascx", "//DeletedPage", null, null, "", false, null, null, false, false, "0.5", "90", "Deleted Page", "1", "-1", null, false, null, "-1", DateTime.Now, "-1", DateTime.Now);
            }			
            
			return table.CreateDataReader();
		}

		private IDataReader GetTabModulesCallBack(int tabId)
		{
			DataTable table = new DataTable("TabModules");

			var cols = new string[]
			           	{
							"PortalID","TabID","TabModuleID","ModuleID","ModuleDefID","ModuleOrder","PaneName","ModuleTitle","CacheTime","CacheMethod","Alignment","Color","Border","IconFile","AllTabs","Visibility","IsDeleted","Header","Footer","StartDate","EndDate","ContainerSrc","DisplayTitle","DisplayPrint","DisplaySyndicate","IsWebSlice","WebSliceTitle","WebSliceExpiryDate","WebSliceTTL","InheritViewPermissions","DesktopModuleID","DefaultCacheTime","ModuleControlID","BusinessControllerClass","IsAdmin","SupportedFeatures","ContentItemID","Content","ContentTypeID","ContentKey","Indexed","CreatedByUserID","CreatedOnDate","LastModifiedByUserID","LastModifiedOnDate","LastContentModifiedOnDate","UniqueId","VersionGuid","DefaultLanguageGuid","LocalizedVersionGuid","CultureCode"
			           	};

			foreach (var col in cols)
			{
				table.Columns.Add(col);
			}
			table.Columns["ModuleID"].DataType = typeof (int);

			var portalId = tabId == HomePageOnPortal0 ? Portal0 : Portal1;

			table.Rows.Add(portalId, tabId, 51, 362, 117, 1, "ContentPane", "DotNetNuke® Enterprise Edition", "3600", "FileModuleCachingProvider", "left", null, null, null, false, "2", false, null, null, null, null, "[G]Containers/DarkKnight/Banner.ascx", true, false, false, false, null, null, "0", true, "75", "1200", "240", "DotNetNuke.Modules.HtmlPro.HtmlTextController", false, "7", "90", "DotNetNuke® Enterprise Edition", "2", null, false, "-1", DateTime.Now, "-1", DateTime.Now, DateTime.Now, Guid.NewGuid(), Guid.NewGuid(), null, Guid.NewGuid(), null);

			return table.CreateDataReader();
		}

		private IDataReader GetPortalSettingsCallBack(int portalId, string culture)
		{
			DataTable table = new DataTable("PortalSettings");

			var cols = new string[]
			           	{
							"SettingName","SettingValue","CreatedByUserID","CreatedOnDate","LastModifiedByUserID","LastModifiedOnDate","CultureCode"
			           	};

			foreach (var col in cols)
			{
				table.Columns.Add(col);
			}

			var alias = portalId == Portal0 ? PortalAlias0 : PortalAlias1;

			table.Rows.Add("DefaultPortalAlias", alias, "-1", DateTime.Now, "-1", DateTime.Now, "en-us");

			return table.CreateDataReader();
		}

		private IDataReader GetPortalGroupsCallBack()
		{
			DataTable table = new DataTable("PortalGroups");

			var cols = new string[]
			           	{
							"PortalGroupID","MasterPortalID","PortalGroupName","PortalGroupDescription","AuthenticationDomain","CreatedByUserID","CreatedOnDate","LastModifiedByUserID","LastModifiedOnDate"
			           	};

			foreach (var col in cols)
			{
				table.Columns.Add(col);
			}

			table.Rows.Add(1, 0, "Portal Group", "", "", -1, DateTime.Now, -1, DateTime.Now);

			return table.CreateDataReader();
		}

		private IClientCapability GetClientCapabilityCallBack(string userAgent)
		{
            IClientCapability clientCapability = new DotNetNuke.Services.ClientCapability.ClientCapability();
            if (userAgent == iphoneUserAgent)
            {
                clientCapability.IsMobile = true;
                clientCapability.Capabilities.Add("mobile_browser", "Safari");
                clientCapability.Capabilities.Add("device_os", "iPhone OS");
            }
            else if (userAgent == iPadTabletUserAgent)
            {
                clientCapability.IsTablet = true;
                clientCapability.Capabilities.Add("mobile_browser", "Safari");
                clientCapability.Capabilities.Add("device_os", "iPhone OS");
            }
            else if (userAgent == motorolaRIZRSymbianOSOpera865)
            {
                clientCapability.IsMobile  = true;
                clientCapability.Capabilities.Add("mobile_browser", "Opera Mini");
                clientCapability.Capabilities.Add("device_os", "Symbian OS");
            }
            
            return clientCapability;
		}

		private IDataReader GetAllRedirectionsCallBack()
		{
			return _dtRedirections.CreateDataReader();
		}

		private void PrepareData()
		{
			//id, portalId, name, type, sortOrder, sourceTabId, includeChildTabs, targetType, targetValue, enabled
            _dtRedirections.Rows.Add(1, Portal0, "R4", (int)RedirectionType.Other, 4, -1, DisabledFlag, (int)TargetType.Portal, "1", EnabledFlag);
            _dtRedirections.Rows.Add(2, Portal0, "R2", (int)RedirectionType.Tablet, 2, -1, DisabledFlag, (int)TargetType.Portal, "1", EnabledFlag);
            _dtRedirections.Rows.Add(3, Portal0, "R3", (int)RedirectionType.AllMobile, 3, -1, DisabledFlag, (int)TargetType.Portal, "1", EnabledFlag);
            _dtRedirections.Rows.Add(4, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, -1, DisabledFlag, (int)TargetType.Portal, "1", EnabledFlag);
            _dtRedirections.Rows.Add(5, Portal0, "R5", (int)RedirectionType.MobilePhone, 5, HomePageOnPortal0, EnabledFlag, (int)TargetType.Portal, "1", EnabledFlag);
            _dtRedirections.Rows.Add(6, Portal0, "R6", (int)RedirectionType.MobilePhone, 6, -1, DisabledFlag, (int)TargetType.Tab, HomePageOnPortal0, EnabledFlag);
            _dtRedirections.Rows.Add(7, Portal0, "R7", (int)RedirectionType.MobilePhone, 7, -1, DisabledFlag, (int)TargetType.Url, ExternalSite, EnabledFlag);

			//id, redirectionId, capability, expression
			_dtRules.Rows.Add(1, 1, "mobile_browser", "Safari");
			_dtRules.Rows.Add(2, 1, "device_os_version", "4.0");

			_dtRedirections.Rows.Add(8, Portal1, "R8", (int)RedirectionType.MobilePhone, 1, -1, EnabledFlag, (int)TargetType.Portal, 2, true);
			_dtRedirections.Rows.Add(9, Portal1, "R9", (int)RedirectionType.Tablet, 1, -1, EnabledFlag, (int)TargetType.Portal, 2, true);
			_dtRedirections.Rows.Add(10, Portal1, "R10", (int)RedirectionType.AllMobile, 1, -1, EnabledFlag, (int)TargetType.Portal, 2, true);
		}

        private void PrepareOperaBrowserOnSymbianOSRedirectionRule()
        {
            _dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.Other, 1, -1, DisabledFlag, (int)TargetType.Tab, AnotherPageOnSamePortal, EnabledFlag);

            //id, redirectionId, capability, expression
            _dtRules.Rows.Add(1, 1, "mobile_browser", "Opera Mini");
            _dtRules.Rows.Add(2, 1, "device_os", "Symbian OS");
        }

        private void PrepareOperaBrowserOnIPhoneOSRedirectionRule()
        {
            _dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.Other, 1, -1, DisabledFlag, (int)TargetType.Tab, AnotherPageOnSamePortal, EnabledFlag);

            //id, redirectionId, capability, expression
            _dtRules.Rows.Add(1, 1, "mobile_browser", "Opera Mini");
            _dtRules.Rows.Add(2, 1, "device_os", "iPhone OS");
        }

        private void PreparePortalToAnotherPageOnSamePortal()
        {
            _dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, -1, DisabledFlag, (int)TargetType.Tab, AnotherPageOnSamePortal, EnabledFlag);
        }

        private void PrepareSamePortalToSamePortalRedirectionRule()
        {
			_dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, -1, DisabledFlag, (int)TargetType.Portal, Portal0, 1);
        }

        private void PrepareHomePageToHomePageRedirectionRule()
        {
			_dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, -1, DisabledFlag, (int)TargetType.Portal, Portal1, 1);
        }

        private void PrepareExternalSiteRedirectionRule()
        {
			_dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 7, -1, DisabledFlag, (int)TargetType.Url, ExternalSite, 1);
        }

        private void PrepareMobileAndTabletRedirectionRuleWithMobileFirst()
        {
            _dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.MobilePhone, 1, -1, DisabledFlag, (int)TargetType.Tab, MobileLandingPage, EnabledFlag);
            _dtRedirections.Rows.Add(2, Portal0, "R2", (int)RedirectionType.Tablet, 2, -1, DisabledFlag, (int)TargetType.Tab, TabletLandingPage, EnabledFlag);
        }

        private void PrepareMobileAndTabletRedirectionRuleWithAndTabletRedirectionRuleTabletFirst()
        {
            _dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.Tablet, 1, -1, DisabledFlag, (int)TargetType.Tab, TabletLandingPage, EnabledFlag);
            _dtRedirections.Rows.Add(2, Portal0, "R2", (int)RedirectionType.MobilePhone, 2, -1, DisabledFlag, (int)TargetType.Tab, MobileLandingPage, EnabledFlag);
            
        }

        private void PrepareAllMobileRedirectionRule()
        {
			_dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.AllMobile, 1, -1, DisabledFlag, (int)TargetType.Tab, AllMobileLandingPage, EnabledFlag);            
        }

        private void PrepareSingleDisabledRedirectionRule()
        {
			_dtRedirections.Rows.Add(1, Portal0, "R1", (int)RedirectionType.AllMobile, 1, -1, DisabledFlag, (int)TargetType.Tab, AllMobileLandingPage, DisabledFlag);
        }

		private HttpApplication GenerateApplication()
		{
			UnitTestHelper.SetHttpContextWithSimulatedRequest("localhost", "dnn", "c:\\", "default.aspx");
			var app = new HttpApplication();

			var requestProp = typeof(NameValueCollection).GetProperty("IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
			requestProp.SetValue(HttpContext.Current.Request.QueryString, false, null);

			var stateProp = typeof(HttpApplication).GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic);
			stateProp.SetValue(app, HttpContext.Current);

			return app;
		}


        private string NavigateUrl(int tabId)
        {
            return string.Format("/Default.aspx?tabid={0}", tabId);
        }

		#endregion
	}
}