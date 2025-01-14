#region Copyright
// 
// DotNetNukeŽ - http://www.dotnetnuke.com
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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Web.Caching;

using DotNetNuke.Collections;
using DotNetNuke.Collections.Internal;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Cache;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.OutputCache;

#endregion

namespace DotNetNuke.Common.Utilities
{
    public enum CoreCacheType
    {
        Host = 1,
        Portal = 2,
        Tab = 3
    }

    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Common.Utilities
    /// Class:      DataCache
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The DataCache class is a facade class for the CachingProvider Instance's
    /// </summary>
    /// <history>
    ///     [cnurse]	12/01/2007	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class DataCache
    {
        //Host keys
        public const string SecureHostSettingsCacheKey = "SecureHostSettings";
        public const string UnSecureHostSettingsCacheKey = "UnsecureHostSettings";
        public const string HostSettingsCacheKey = "HostSettings";
        public const CacheItemPriority HostSettingsCachePriority = CacheItemPriority.NotRemovable;
        public const int HostSettingsCacheTimeOut = 20;

        //Portal keys
        public const string PortalAliasCacheKey = "PortalAlias";
        public const CacheItemPriority PortalAliasCachePriority = CacheItemPriority.NotRemovable;
        public const int PortalAliasCacheTimeOut = 200;

        public const string PortalSettingsCacheKey = "PortalSettings{0}";
        public const CacheItemPriority PortalSettingsCachePriority = CacheItemPriority.NotRemovable;
        public const int PortalSettingsCacheTimeOut = 20;

        public const string PortalDictionaryCacheKey = "PortalDictionary";
        public const CacheItemPriority PortalDictionaryCachePriority = CacheItemPriority.High;
        public const int PortalDictionaryTimeOut = 20;

        public const string PortalCacheKey = "Portal{0}_{1}";
        public const CacheItemPriority PortalCachePriority = CacheItemPriority.High;
        public const int PortalCacheTimeOut = 20;

        public const string PortalUserCountCacheKey = "PortalUserCount{0}";
        public const CacheItemPriority PortalUserCountCachePriority = CacheItemPriority.High;
        public const int PortalUserCountCacheTimeOut = 20;

        public const string PortalGroupsCacheKey = "PortalGroups";
        public const CacheItemPriority PortalGroupsCachePriority = CacheItemPriority.High;
        public const int PortalGroupsCacheTimeOut = 20;

        //Tab cache keys
        public const string TabCacheKey = "Tab_Tabs{0}";
        public const CacheItemPriority TabCachePriority = CacheItemPriority.High;
        public const int TabCacheTimeOut = 20;
        public const string TabPathCacheKey = "Tab_TabPathDictionary{0}_{1}";
        public const CacheItemPriority TabPathCachePriority = CacheItemPriority.High;
        public const int TabPathCacheTimeOut = 20;
        public const string TabPermissionCacheKey = "Tab_TabPermissions{0}";
        public const CacheItemPriority TabPermissionCachePriority = CacheItemPriority.High;
        public const int TabPermissionCacheTimeOut = 20;

        public const string AuthenticationServicesCacheKey = "AuthenticationServices";
        public const CacheItemPriority AuthenticationServicesCachePriority = CacheItemPriority.NotRemovable;
        public const int AuthenticationServicesCacheTimeOut = 20;

        public const string DesktopModulePermissionCacheKey = "DesktopModulePermissions";
        public const CacheItemPriority DesktopModulePermissionCachePriority = CacheItemPriority.High;
        public const int DesktopModulePermissionCacheTimeOut = 20;

        public const string DesktopModuleCacheKey = "DesktopModulesByPortal{0}";
        public const CacheItemPriority DesktopModuleCachePriority = CacheItemPriority.High;
        public const int DesktopModuleCacheTimeOut = 20;

        public const string PortalDesktopModuleCacheKey = "PortalDesktopModules{0}";
        public const CacheItemPriority PortalDesktopModuleCachePriority = CacheItemPriority.AboveNormal;
        public const int PortalDesktopModuleCacheTimeOut = 20;

        public const string ModuleDefinitionCacheKey = "ModuleDefinitions";
        public const CacheItemPriority ModuleDefinitionCachePriority = CacheItemPriority.High;
        public const int ModuleDefinitionCacheTimeOut = 20;

        public const string ModuleControlsCacheKey = "ModuleControls";
        public const CacheItemPriority ModuleControlsCachePriority = CacheItemPriority.High;
        public const int ModuleControlsCacheTimeOut = 20;

        public const string TabModuleCacheKey = "TabModules{0}";
        public const CacheItemPriority TabModuleCachePriority = CacheItemPriority.AboveNormal;
        public const int TabModuleCacheTimeOut = 20;

        public const string ModulePermissionCacheKey = "ModulePermissions{0}";
        public const CacheItemPriority ModulePermissionCachePriority = CacheItemPriority.AboveNormal;
        public const int ModulePermissionCacheTimeOut = 20;

        public const string ModuleCacheKey = "Modules{0}";
        public const int ModuleCacheTimeOut = 20;

        public const string FolderCacheKey = "Folders{0}";
        public const int FolderCacheTimeOut = 20;
        public const CacheItemPriority FolderCachePriority = CacheItemPriority.Normal;

        public const string FolderUserCacheKey = "Folders|{0}|{1}|{2}";
        public const int FolderUserCacheTimeOut = 20;
        public const CacheItemPriority FolderUserCachePriority = CacheItemPriority.Normal;

        public const string FolderMappingCacheKey = "FolderMapping|{0}";
        public const int FolderMappingCacheTimeOut = 20;
        public const CacheItemPriority FolderMappingCachePriority = CacheItemPriority.High;

        public const string FolderPermissionCacheKey = "FolderPermissions{0}";
        public const CacheItemPriority FolderPermissionCachePriority = CacheItemPriority.Normal;
        public const int FolderPermissionCacheTimeOut = 20;

        public const string ListsCacheKey = "Lists{0}";
        public const CacheItemPriority ListsCachePriority = CacheItemPriority.Normal;
        public const int ListsCacheTimeOut = 20;

        public const string ProfileDefinitionsCacheKey = "ProfileDefinitions{0}";
        public const int ProfileDefinitionsCacheTimeOut = 20;

        public const string UserCacheKey = "UserInfo|{0}|{1}";
        public const int UserCacheTimeOut = 1;
        public const CacheItemPriority UserCachePriority = CacheItemPriority.Normal;

        public const string UserLookupCacheKey = "UserLookup|{0}";
        public const int UserLookupCacheTimeOut = 20;
        public const CacheItemPriority UserLookupCachePriority = CacheItemPriority.High;

        public const string LocalesCacheKey = "Locales{0}";
        public const CacheItemPriority LocalesCachePriority = CacheItemPriority.Normal;
        public const int LocalesCacheTimeOut = 20;

        public const string SkinDefaultsCacheKey = "SkinDefaults_{0}";
        public const CacheItemPriority SkinDefaultsCachePriority = CacheItemPriority.Normal;
        public const int SkinDefaultsCacheTimeOut = 20;

        public const CacheItemPriority ResourceFilesCachePriority = CacheItemPriority.Normal;
        public const int ResourceFilesCacheTimeOut = 20;

        public const string ResourceFileLookupDictionaryCacheKey = "ResourceFileLookupDictionary";
        public const CacheItemPriority ResourceFileLookupDictionaryCachePriority = CacheItemPriority.NotRemovable;
        public const int ResourceFileLookupDictionaryTimeOut = 200;

        public const string SkinsCacheKey = "GetSkins{0}";

        public const string BannersCacheKey = "Banners:{0}:{1}:{2}";
        public const CacheItemPriority BannersCachePriority = CacheItemPriority.Normal;
        public const int BannersCacheTimeOut = 20;

		public const string RedirectionsCacheKey = "Redirections:{0}";
		public const CacheItemPriority RedirectionsCachePriority = CacheItemPriority.Default;
		public const int RedirectionsCacheTimeOut = 20;

		public const string PreviewProfilesCacheKey = "PreviewProfiles:{0}";
		public const CacheItemPriority PreviewProfilesCachePriority = CacheItemPriority.Default;
		public const int PreviewProfilesCacheTimeOut = 20;

        public const string RelationshipTypesCacheKey = "RelationshipTypes";
        public const CacheItemPriority RelationshipTypesCachePriority = CacheItemPriority.Default;
        public const int RelationshipTypesCacheTimeOut = 20;

        public const string RelationshipByPortalIDCacheKey = "RelationshipByPortalID:{0}";
        public const CacheItemPriority RelationshipByPortalIDCachePriority = CacheItemPriority.Default;
        public const int RelationshipByPortalIDCacheTimeOut = 20;

        public const string RolesCacheKey = "Roles:{0}";
        public const CacheItemPriority RolesCachePriority = CacheItemPriority.Default;
        public const int RolesCacheTimeOut = 20;

        public const string NotificationTypesCacheKey = "NotificationTypes:{0}";
        public const CacheItemPriority NotificationTypesCachePriority = CacheItemPriority.Default;
        public const int NotificationTypesTimeOut = 20;

        public const string NotificationTypeActionsCacheKey = "NotificationTypeActions:{0}";
        public const string NotificationTypeActionsByNameCacheKey = "NotificationTypeActions:{0}|{1}";
        public const CacheItemPriority NotificationTypeActionsPriority = CacheItemPriority.Default;
        public const int NotificationTypeActionsTimeOut = 20;

        private static string _CachePersistenceEnabled = "";

        private static readonly ReaderWriterLock dictionaryLock = new ReaderWriterLock();
        private static readonly Dictionary<string, object> lockDictionary = new Dictionary<string, object>();

        private static readonly SharedDictionary<string, Object> dictionaryCache = new SharedDictionary<string, Object>();

        public static bool CachePersistenceEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(_CachePersistenceEnabled))
                {
                    if (Config.GetSetting("EnableCachePersistence") == null)
                    {
                        _CachePersistenceEnabled = "false";
                    }
                    else
                    {
                        _CachePersistenceEnabled = Config.GetSetting("EnableCachePersistence");
                    }
                }
                return bool.Parse(_CachePersistenceEnabled);
            }
        }

        private static string GetDnnCacheKey(string CacheKey)
        {
            return CachingProvider.GetCacheKey(CacheKey);
        }

        private static string CleanCacheKey(string CacheKey)
        {
            return CachingProvider.CleanCacheKey(CacheKey);
        }

        internal static void ItemRemovedCallback(string key, object value, CacheItemRemovedReason removedReason)
        {
            //if the item was removed from the cache, log the key and reason to the event log
            try
            {
                if (Globals.Status == Globals.UpgradeStatus.None)
                {
                    var objEventLogInfo = new LogInfo();
                    switch (removedReason)
                    {
                        case CacheItemRemovedReason.Removed:
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_REMOVED.ToString();
                            break;
                        case CacheItemRemovedReason.Expired:
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_EXPIRED.ToString();
                            break;
                        case CacheItemRemovedReason.Underused:
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_UNDERUSED.ToString();
                            break;
                        case CacheItemRemovedReason.DependencyChanged:
                            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_DEPENDENCYCHANGED.ToString();
                            break;
                    }
                    objEventLogInfo.LogProperties.Add(new LogDetailInfo(key, removedReason.ToString()));
                    var objEventLog = new EventLogController();
                    objEventLog.AddLog(objEventLogInfo);
                }
            }
            catch (Exception exc)
            {
                //Swallow exception            
                DnnLog.Error(exc);
            }
        }

        public static void ClearCache()
        {
            CachingProvider.Instance().Clear("Prefix", "DNN_");
            using (ISharedCollectionLock writeLock = dictionaryCache.GetWriteLock())
            {
                dictionaryCache.Clear();
            }

            //log the cache clear event
            var objEventLogInfo = new LogInfo();
            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_REFRESH.ToString();
            objEventLogInfo.LogProperties.Add(new LogDetailInfo("*", "Refresh"));
            var objEventLog = new EventLogController();
            objEventLog.AddLog(objEventLogInfo);
        }

        public static void ClearCache(string cachePrefix)
        {
            CachingProvider.Instance().Clear("Prefix", GetDnnCacheKey(cachePrefix));
        }

        public static void ClearFolderCache(int PortalId)
        {
            CachingProvider.Instance().Clear("Folder", PortalId.ToString());
        }

        public static void ClearHostCache(bool Cascade)
        {
            if (Cascade)
            {
                ClearCache();
            }
            else
            {
                CachingProvider.Instance().Clear("Host", "");
            }
        }

        public static void ClearModuleCache(int TabId)
        {
            CachingProvider.Instance().Clear("Module", TabId.ToString());
            Dictionary<int, int> portals = PortalController.GetPortalDictionary();
            if (portals.ContainsKey(TabId))
            {
                var tabController = new TabController();
                Hashtable tabSettings = null;

                tabSettings = tabController.GetTabSettings(TabId);
                if (tabSettings["CacheProvider"] != null && tabSettings["CacheProvider"].ToString().Length > 0)
                {
                    OutputCachingProvider outputProvider = OutputCachingProvider.Instance(tabSettings["CacheProvider"].ToString());
                    if (outputProvider != null)
                    {
                        outputProvider.Remove(TabId);
                    }
                }
            }
        }

        public static void ClearModulePermissionsCachesByPortal(int PortalId)
        {
            CachingProvider.Instance().Clear("ModulePermissionsByPortal", PortalId.ToString());
        }

        public static void ClearPortalCache(int PortalId, bool Cascade)
        {
            if (Cascade)
            {
                CachingProvider.Instance().Clear("PortalCascade", PortalId.ToString());
            }
            else
            {
                CachingProvider.Instance().Clear("Portal", PortalId.ToString());
            }
        }

        public static void ClearTabsCache(int PortalId)
        {
            CachingProvider.Instance().Clear("Tab", PortalId.ToString());
        }

        public static void ClearDefinitionsCache(int PortalId)
        {
            RemoveCache(string.Format(ProfileDefinitionsCacheKey, PortalId));
        }

        public static void ClearDesktopModulePermissionsCache()
        {
            RemoveCache(DesktopModulePermissionCacheKey);
        }

        public static void ClearFolderPermissionsCache(int PortalId)
        {
            RemoveCache(string.Format(FolderPermissionCacheKey, PortalId));
        }

        public static void ClearListsCache(int PortalId)
        {
            RemoveCache(string.Format(ListsCacheKey, PortalId));
        }

        public static void ClearModulePermissionsCache(int TabId)
        {
            RemoveCache(string.Format(ModulePermissionCacheKey, TabId));
        }

        public static void ClearTabPermissionsCache(int PortalId)
        {
            RemoveCache(string.Format(TabPermissionCacheKey, PortalId));
        }

        public static void ClearUserCache(int PortalId, string username)
        {
            RemoveCache(string.Format(UserCacheKey, PortalId, username));
        }

        private static object GetCachedDataFromRuntimeCache(CacheItemArgs cacheItemArgs, CacheItemExpiredCallback cacheItemExpired)
        {
            object objObject = GetCache(cacheItemArgs.CacheKey);

            // if item is not cached
            if (objObject == null)
            {
                //Get Unique Lock for cacheKey
                object @lock = GetUniqueLockObject(cacheItemArgs.CacheKey);

                // prevent other threads from entering this block while we regenerate the cache
                lock (@lock)
                {
                    // try to retrieve object from the cache again (in case another thread loaded the object since we first checked)
                    objObject = GetCache(cacheItemArgs.CacheKey);

                    // if object was still not retrieved

                    if (objObject == null)
                    {
                        // get object from data source using delegate
                        try
                        {
                            objObject = cacheItemExpired(cacheItemArgs);
                        }
                        catch (Exception ex)
                        {
                            objObject = null;
                            Exceptions.LogException(ex);
                        }

                        // set cache timeout
                        int timeOut = cacheItemArgs.CacheTimeOut * Convert.ToInt32(Host.PerformanceSetting);

                        // if we retrieved a valid object and we are using caching
                        if (objObject != null && timeOut > 0)
                        {
                            // save the object in the cache
                            SetCache(cacheItemArgs.CacheKey,
                                     objObject,
                                     cacheItemArgs.CacheDependency,
                                     Cache.NoAbsoluteExpiration,
                                     TimeSpan.FromMinutes(timeOut),
                                     cacheItemArgs.CachePriority,
                                     cacheItemArgs.CacheCallback);

                            // check if the item was actually saved in the cache

                            if (GetCache(cacheItemArgs.CacheKey) == null)
                            {
                                // log the event if the item was not saved in the cache ( likely because we are out of memory )
                                var objEventLogInfo = new LogInfo();
                                objEventLogInfo.LogTypeKey = EventLogController.EventLogType.CACHE_OVERFLOW.ToString();
                                objEventLogInfo.LogProperties.Add(new LogDetailInfo(cacheItemArgs.CacheKey, "Overflow - Item Not Cached"));
                                var objEventLog = new EventLogController();
                                objEventLog.AddLog(objEventLogInfo);
                            }
                        }

                        //This thread won so remove unique Lock from collection
                        RemoveUniqueLockObject(cacheItemArgs.CacheKey);
                    }
                }
            }

            return objObject;
        }

        private static object GetCachedDataFromDictionary(CacheItemArgs cacheItemArgs, CacheItemExpiredCallback cacheItemExpired)
        {
            object cachedObject = null;

            bool isFound;
            using (ISharedCollectionLock readLock = dictionaryCache.GetReadLock())
            {
                isFound = dictionaryCache.TryGetValue(cacheItemArgs.CacheKey, out cachedObject);
            }

            if (!isFound)
            {
                // get object from data source using delegate
                try
                {
                    if (cacheItemExpired != null)
                        cachedObject = cacheItemExpired(cacheItemArgs);
                    else
                        cachedObject = null;
                }
                catch (Exception ex)
                {
                    cachedObject = null;
                    Exceptions.LogException(ex);
                }

                using (ISharedCollectionLock writeLock = dictionaryCache.GetWriteLock())
                {
                    if (!dictionaryCache.ContainsKey(cacheItemArgs.CacheKey))
                    {
                        if (cachedObject != null)
                        {
                            dictionaryCache[cacheItemArgs.CacheKey] = cachedObject;
                        }
                    }
                }
            }

            return cachedObject;
        }

        public static TObject GetCachedData<TObject>(CacheItemArgs cacheItemArgs, CacheItemExpiredCallback cacheItemExpired)
        {
            // declare local object and try and retrieve item from the cache
            return GetCachedData<TObject>(cacheItemArgs, cacheItemExpired, false);
        }

        internal static TObject GetCachedData<TObject>(CacheItemArgs cacheItemArgs, CacheItemExpiredCallback cacheItemExpired, bool storeInDictionary)
        {
            object objObject = null;

            if (storeInDictionary)
            {
                //Use Thread Safe SharedDictionary
                objObject = GetCachedDataFromDictionary(cacheItemArgs, cacheItemExpired);
            }
            else
            {
                //Use Cache
                objObject = GetCachedDataFromRuntimeCache(cacheItemArgs, cacheItemExpired);
            }

            // return the object
            if (objObject == null)
            {
                return default(TObject);
            }
            else
            {
                return (TObject)objObject;
            }
        }

        private static object GetUniqueLockObject(string key)
        {
            object @lock = null;
            dictionaryLock.AcquireReaderLock(new TimeSpan(0, 0, 5));
            try
            {
                //Try to get lock Object (for key) from Dictionary
                if (lockDictionary.ContainsKey(key))
                {
                    @lock = lockDictionary[key];
                }
            }
            finally
            {
                dictionaryLock.ReleaseReaderLock();
            }
            if (@lock == null)
            {
                dictionaryLock.AcquireWriterLock(new TimeSpan(0, 0, 5));
                try
                {
                    //Double check dictionary
                    if (!lockDictionary.ContainsKey(key))
                    {
                        //Create new lock
                        lockDictionary[key] = new object();
                    }
                    //Retrieve lock
                    @lock = lockDictionary[key];
                }
                finally
                {
                    dictionaryLock.ReleaseWriterLock();
                }
            }
            return @lock;
        }

        private static void RemoveUniqueLockObject(string key)
        {
            dictionaryLock.AcquireWriterLock(new TimeSpan(0, 0, 5));
            try
            {
                //check dictionary
                if (lockDictionary.ContainsKey(key))
                {
                    //Remove lock
                    lockDictionary.Remove(key);
                }
            }
            finally
            {
                dictionaryLock.ReleaseWriterLock();
            }
        }

        public static TObject GetCache<TObject>(string CacheKey)
        {
            object objObject = GetCache(CacheKey);
            if (objObject == null)
            {
                return default(TObject);
            }
            return (TObject)objObject;
        }

        public static object GetCache(string CacheKey)
        {
            return CachingProvider.Instance().GetItem(GetDnnCacheKey(CacheKey));
        }

        public static void RemoveCache(string CacheKey)
        {
            CachingProvider.Instance().Remove(GetDnnCacheKey(CacheKey));
        }

        public static void RemoveFromPrivateDictionary(string DnnCacheKey)
        {
            using (ISharedCollectionLock writeLock = dictionaryCache.GetWriteLock())
            {
                dictionaryCache.Remove(CleanCacheKey(DnnCacheKey));
            }
        }

        public static void SetCache(string CacheKey, object objObject)
        {
            DNNCacheDependency objDependency = null;
            SetCache(CacheKey, objObject, objDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        public static void SetCache(string CacheKey, object objObject, DNNCacheDependency objDependency)
        {
            SetCache(CacheKey, objObject, objDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        public static void SetCache(string CacheKey, object objObject, DateTime AbsoluteExpiration)
        {
            DNNCacheDependency objDependency = null;
            SetCache(CacheKey, objObject, objDependency, AbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        public static void SetCache(string CacheKey, object objObject, TimeSpan SlidingExpiration)
        {
            DNNCacheDependency objDependency = null;
            SetCache(CacheKey, objObject, objDependency, Cache.NoAbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, null);
        }

        public static void SetCache(string CacheKey, object objObject, DNNCacheDependency objDependency, DateTime AbsoluteExpiration, TimeSpan SlidingExpiration)
        {
            SetCache(CacheKey, objObject, objDependency, AbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, null);
        }

        public static void SetCache(string CacheKey, object objObject, DNNCacheDependency objDependency, DateTime AbsoluteExpiration, TimeSpan SlidingExpiration, CacheItemPriority Priority,
                                    CacheItemRemovedCallback OnRemoveCallback)
        {
            if (objObject != null)
            {
                //if no OnRemoveCallback value is specified, use the default method
                if (OnRemoveCallback == null)
                {
                    OnRemoveCallback = ItemRemovedCallback;
                }
                CachingProvider.Instance().Insert(GetDnnCacheKey(CacheKey), objObject, objDependency, AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback);
            }
        }

        #region "Obsolete Methods"

        [Obsolete("Deprecated in DNN 5.0 - Replace by ClearHostCache(True)")]
        public static void ClearModuleCache()
        {
            ClearHostCache(true);
        }

        [Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")]
        public static object GetPersistentCacheItem(string CacheKey, Type objType)
        {
            return CachingProvider.Instance().GetItem(GetDnnCacheKey(CacheKey));
        }

        [Obsolete("Deprecated in DNN 5.1.1 - Should have been declared Friend")]
        public static void ClearDesktopModuleCache(int PortalId)
        {
            RemoveCache(string.Format(DesktopModuleCacheKey, PortalId));
            RemoveCache(ModuleDefinitionCacheKey);
            RemoveCache(ModuleControlsCacheKey);
        }

        [Obsolete("Deprecated in DNN 5.1.1 - Should have been declared Friend")]
        public static void ClearHostSettingsCache()
        {
            RemoveCache(HostSettingsCacheKey);
            RemoveCache(SecureHostSettingsCacheKey);
        }

        [Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")]
        public static void RemovePersistentCacheItem(string CacheKey)
        {
            CachingProvider.Instance().Remove(GetDnnCacheKey(CacheKey));
        }

        [Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")]
        public static void SetCache(string CacheKey, object objObject, bool PersistAppRestart)
        {
            DNNCacheDependency objDependency = null;
            SetCache(CacheKey, objObject, objDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        [Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")]
        public static void SetCache(string CacheKey, object objObject, CacheDependency objDependency, bool PersistAppRestart)
        {
            SetCache(CacheKey, objObject, new DNNCacheDependency(objDependency), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        [Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")]
        public static void SetCache(string CacheKey, object objObject, DateTime AbsoluteExpiration, bool PersistAppRestart)
        {
            DNNCacheDependency objDependency = null;
            SetCache(CacheKey, objObject, objDependency, AbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        [Obsolete("Deprecated in DNN 5.1 - Cache Persistence is not supported")]
        public static void SetCache(string CacheKey, object objObject, TimeSpan SlidingExpiration, bool PersistAppRestart)
        {
            DNNCacheDependency objDependency = null;
            SetCache(CacheKey, objObject, objDependency, Cache.NoAbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, null);
        }

        [Obsolete(
            "Deprecated in DNN 5.1 - SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As DotNetNuke.Services.Cache.DNNCacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan)"
            )]
        public static void SetCache(string CacheKey, object objObject, CacheDependency objDependency, DateTime AbsoluteExpiration, TimeSpan SlidingExpiration, bool PersistAppRestart)
        {
            SetCache(CacheKey, objObject, new DNNCacheDependency(objDependency), AbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, null);
        }

        [Obsolete(
            "Deprecated in DNN 5.1 - SetCache(ByVal CacheKey As String, ByVal objObject As Object, ByVal objDependency As DotNetNuke.Services.Cache.DNNCacheDependency, ByVal AbsoluteExpiration As Date, ByVal SlidingExpiration As System.TimeSpan, ByVal Priority As CacheItemPriority, ByVal OnRemoveCallback As CacheItemRemovedCallback)"
            )]
        public static void SetCache(string CacheKey, object objObject, CacheDependency objDependency, DateTime AbsoluteExpiration, TimeSpan SlidingExpiration, CacheItemPriority Priority,
                                    CacheItemRemovedCallback OnRemoveCallback, bool PersistAppRestart)
        {
            SetCache(CacheKey, objObject, new DNNCacheDependency(objDependency), AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback);
        }

        [Obsolete("Deprecated in DNN 5.1 - Use new overload that uses a DNNCacheDependency")]
        public static void SetCache(string CacheKey, object objObject, CacheDependency objDependency)
        {
            SetCache(CacheKey, objObject, new DNNCacheDependency(objDependency), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        }

        [Obsolete("Deprecated in DNN 5.1 - Use new overload that uses a DNNCacheDependency")]
        public static void SetCache(string CacheKey, object objObject, CacheDependency objDependency, DateTime AbsoluteExpiration, TimeSpan SlidingExpiration)
        {
            SetCache(CacheKey, objObject, new DNNCacheDependency(objDependency), AbsoluteExpiration, SlidingExpiration, CacheItemPriority.Normal, null);
        }

        [Obsolete("Deprecated in DNN 5.1 - Use new overload that uses a DNNCacheDependency")]
        public static void SetCache(string CacheKey, object objObject, CacheDependency objDependency, DateTime AbsoluteExpiration, TimeSpan SlidingExpiration, CacheItemPriority Priority,
                                    CacheItemRemovedCallback OnRemoveCallback)
        {
            SetCache(CacheKey, objObject, new DNNCacheDependency(objDependency), AbsoluteExpiration, SlidingExpiration, Priority, OnRemoveCallback);
        }

        #endregion
    }
}
