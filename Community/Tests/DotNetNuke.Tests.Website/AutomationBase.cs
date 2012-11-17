using System;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Security;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.HTMLEditorProvider;
using DotNetNuke.Modules.NavigationProvider;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Profile;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Cache;
using DotNetNuke.Services.ClientCapability;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.ModuleCache;
using DotNetNuke.Services.OutputCache;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.Services.Search;
using DotNetNuke.Services.Sitemap;
using DotNetNuke.Services.Url.FriendlyUrl;
using NUnit.Framework;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using MembershipProvider = DotNetNuke.Security.Membership.MembershipProvider;
using RoleProvider = DotNetNuke.Security.Roles.RoleProvider;


namespace DotNetNuke.Tests.Website
{
    public class AutomationBase
    {
        private readonly FirefoxProfile _ffp;
        public readonly string SiteUrl = ConfigurationManager.AppSettings["SiteURL"];
        private readonly string _browser = ConfigurationManager.AppSettings["Browser"];
        private readonly int _portalId = int.Parse(ConfigurationManager.AppSettings["PortalId"]);
        public static bool CloseBrowser = bool.Parse(ConfigurationManager.AppSettings["CloseWindow"]);
        protected SqlDataProvider SqlProvider;
        public int PortalId { get; private set; }
        public static TabInfo Page { get; set; }
        public static ModuleInfo Module { get; set; }
        public static UserInfo User { get; set; }
        public static IWebDriver Driver { get; set; }
        private static bool _alreadyLoaded;

        public AutomationBase()
        {
            if (Driver == null)
            {
                var simulator = new HttpSimulator.HttpSimulator();
                simulator.SimulateRequest();

                InstallComponents();

                LoadDnnProviders("data;logging;caching;authentication;members;roles;profiles;permissions;folder");

                //fix Globals.ApplicationMapPath
                var appPath = ConfigurationManager.AppSettings["DefaultPhysicalAppPath"];
                if (!string.IsNullOrEmpty(appPath))
                {
                    var mappath = typeof(Globals).GetField("_applicationMapPath", BindingFlags.Static | BindingFlags.NonPublic);
                    mappath.SetValue(null, appPath);
                }

                //fix membership
                var providerProp = typeof(Membership).GetField("s_Provider", BindingFlags.Static | BindingFlags.NonPublic);
                providerProp.SetValue(null, Membership.Providers["AspNetSqlMembershipProvider"]);

                var objPortalAliasInfo = new DotNetNuke.Entities.Portals.PortalAliasInfo { PortalID = _portalId };
                var ps = new DotNetNuke.Entities.Portals.PortalSettings(59, objPortalAliasInfo);
                System.Web.HttpContext.Current.Items.Add("PortalSettings", ps);
                SqlProvider = new SqlDataProvider();
                PortalId = _portalId;

                switch (_browser)
                {
                    case Common.BrowserType.firefox:
                        _ffp = new FirefoxProfile { AcceptUntrustedCertificates = true };
                        Driver = new FirefoxDriver(_ffp);
                        Driver.Navigate().GoToUrl(SiteUrl);
                        break;
                    case Common.BrowserType.ie:
                        Driver = new InternetExplorerDriver();
                        Driver.Navigate().GoToUrl(SiteUrl);
                        break;
                    case Common.BrowserType.chrome:
                        Driver = new ChromeDriver();
                        Driver.Navigate().GoToUrl(SiteUrl);
                        break;
                }
            }
        }

        private static void InstallComponents()
        {
            //creates a simple container for the component model
            ComponentFactory.Container = new SimpleContainer();
            ComponentFactory.InstallComponents(new ProviderInstaller("data", typeof(DataProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("caching", typeof(DotNetNuke.Services.Cache.CachingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("logging", typeof(DotNetNuke.Services.Log.EventLog.LoggingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("scheduling", typeof(DotNetNuke.Services.Scheduling.SchedulingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("searchIndex", typeof(DotNetNuke.Services.Search.IndexingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("searchDataStore", typeof(DotNetNuke.Services.Search.SearchDataStoreProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("friendlyUrl", typeof(DotNetNuke.Services.Url.FriendlyUrl.FriendlyUrlProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("members", typeof(DotNetNuke.Security.Membership.MembershipProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("roles", typeof(DotNetNuke.Security.Roles.RoleProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("profiles", typeof(DotNetNuke.Security.Profile.ProfileProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("permissions", typeof(DotNetNuke.Security.Permissions.PermissionProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("outputCaching", typeof(DotNetNuke.Services.OutputCache.OutputCachingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("moduleCaching", typeof(DotNetNuke.Services.ModuleCache.ModuleCachingProvider)));
            ComponentFactory.InstallComponents(new ProviderInstaller("folder", typeof(DotNetNuke.Services.FileSystem.FolderProvider)));

            var provider = ComponentFactory.GetComponent<DotNetNuke.Security.Permissions.PermissionProvider>();
            if (provider == null)
            {
                ComponentFactory.RegisterComponentInstance<DotNetNuke.Security.Permissions.PermissionProvider>(new DotNetNuke.Security.Permissions.PermissionProvider());
            }
        }

        private static void RegisterIfNotAlreadyRegistered<TConcrete>() where TConcrete : class, new()
        {
            RegisterIfNotAlreadyRegistered<TConcrete, TConcrete>("");
        }

        private static void RegisterIfNotAlreadyRegistered<TAbstract, TConcrete>(string name)
            where TAbstract : class
            where TConcrete : class, new()
        {
            var provider = ComponentFactory.GetComponent<TAbstract>();
            if (provider == null)
            {
                if (String.IsNullOrEmpty(name))
                {
                    ComponentFactory.RegisterComponentInstance<TAbstract>(new TConcrete());
                }
                else
                {
                    ComponentFactory.RegisterComponentInstance<TAbstract>(name, new TConcrete());
                }
            }
        }

        /// <summary>
        /// This proc loads up specified DNN providers, because the BuildManager doesn't get the context right
        /// The providers are cahced so that the DNN base buildManager calls don't have to load up hte providers
        /// </summary>
        private static void LoadDnnProviders(string providerList)
        {
            if (_alreadyLoaded)
            {
                return;
            }
            _alreadyLoaded = true;
            if (providerList != null)
            {
                var providers = providerList.Split(';');
                foreach (var provider in providers)
                {
                    if (provider.Length > 0)
                    {
                        var config =
                            DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(provider);
                        if (config != null)
                        {
                            foreach (string providerName in config.Providers.Keys)
                            {
                                var providerValue =
                                    (DotNetNuke.Framework.Providers.Provider)config.Providers[providerName];
                                var type = providerValue.Type;
                                var assembly = providerValue.Name;

                                if (type.Contains(", ")) //get the straight typename, no assembly, for the cache key
                                {
                                    assembly = type.Substring(type.IndexOf(", ") + 1);
                                    type = type.Substring(0, type.IndexOf(", "));
                                }

                                var cacheKey = type;

                                DotNetNuke.Framework.Reflection.CreateType(providerValue.Type, cacheKey, true, false);
                            }
                        }
                    }
                }
            }
        }
    }

    public class Wait
    {
        public const int DefaultTimeout = 5000;

        public static IWebElement ForElement(ISearchContext searchContext, By by,
                                             int millisecondsToWait = DefaultTimeout)
        {
            var endTime = DateTime.Now.AddMilliseconds(millisecondsToWait);
            while (DateTime.Now < endTime)
            {
                try
                {
                    var element = searchContext.FindElement(by);
                    return element;
                }
                catch (NoSuchElementException)
                {
                }
            }
            throw new NoSuchElementException(by + " not found in " +
                                             millisecondsToWait + "ms");
        }
    }

    public static class FindElementExtension
    {
        private static readonly int TimeOut = int.Parse(ConfigurationManager.AppSettings["TimeOut"]);

        public static IWebElement FindDnnElementById(this IWebDriver driver, params object[] id)
        {
            driver.SwitchTo().DefaultContent();
            try
            {
                var popUp = driver.FindElement(By.Id("iPopUp"));
                driver.SwitchTo().Frame(popUp);
            }
            catch
            {
                Console.WriteLine("");
            }
            return Wait.ForElement(driver, By.Id(id[1].ToString()), TimeOut);
        }

        public static IWebElement FindDnnElementByXpath(this IWebDriver driver, params object[] xpath)
        {
            driver.SwitchTo().DefaultContent();
            try
            {
                var popUp = driver.FindElement(By.Id("iPopUp"));
                driver.SwitchTo().Frame(popUp);
            }
            catch
            {
                Console.WriteLine("");
            }
            return Wait.ForElement(driver, By.XPath(xpath[1].ToString()), TimeOut);
        }
    }
}

