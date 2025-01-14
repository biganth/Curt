#region Copyright
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
using System.Data;
using System.Data.Common;

using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Tabs;

#endregion

namespace DotNetNuke.Data
{
    public abstract class DataProvider
    {
        #region Shared/Static Methods

        // return the provider
        public static DataProvider Instance()
        {
            DataProvider provider = ComponentFactory.GetComponent<DataProvider>();
            if (provider == null)
                throw new ApplicationException("No data provider exists.");
            return provider;
        }

        #endregion

        public string DefaultProviderName
        {
            get
            {
                return Instance().ProviderName;
            }
        }

        public abstract string ConnectionString { get; }
        public abstract string DatabaseOwner { get; }
        public abstract string ObjectQualifier { get; }
        public abstract string ProviderName { get; }
        public abstract Dictionary<string, string> Settings { get; }

        #region Abstract Methods

        //Generic Methods

        public abstract void ExecuteNonQuery(string ProcedureName, params object[] commandParameters);

        public abstract IDataReader ExecuteReader(string ProcedureName, params object[] commandParameters);

        public abstract object ExecuteScalar(string ProcedureName, params object[] commandParameters);

        public abstract T ExecuteScalar<T>(string ProcedureName, params object[] commandParameters);

        public abstract DataSet ExecuteDataSet(string ProcedureName, params object[] commandParameters);

        public abstract IDataReader ExecuteSQL(string SQL);

        [Obsolete("Temporarily Added in DNN 5.4.2. This will be removed and replaced with named instance support.")]
        public abstract IDataReader ExecuteSQL(string SQL, params IDataParameter[] commandParameters);

        public abstract IDataReader ExecuteSQLTemp(string ConnectionString, string SQL);

        // general
        public abstract DbConnectionStringBuilder GetConnectionStringBuilder();

        public abstract object GetNull(object Field);

        //transaction
        public abstract void CommitTransaction(DbTransaction transaction);

        public abstract string ExecuteScript(string Script, DbTransaction transaction);

        public abstract DbTransaction GetTransaction();

        public abstract void RollbackTransaction(DbTransaction transaction);

        // upgrade
        public abstract string GetProviderPath();

        public abstract string ExecuteScript(string SQL);

        [Obsolete("Temporarily Added in DNN 5.4.2. This will be removed and replaced with named instance support.")]
        public abstract string ExecuteScript(string ConnectionString, string SQL);

        public abstract string ExecuteScript(string SQL, bool UseTransactions);

        public abstract Version GetDatabaseEngineVersion();

        public abstract IDataReader GetDatabaseServer();

        public abstract IDataReader GetDatabaseVersion();

        public abstract Version GetVersion();

        public abstract string TestDatabaseConnection(DbConnectionStringBuilder builder, string Owner, string Qualifier);

        public abstract void UpdateDatabaseVersion(int Major, int Minor, int Build, string Name);

        public abstract IDataReader FindDatabaseVersion(int Major, int Minor, int Build);

        public abstract void UpgradeDatabaseSchema(int Major, int Minor, int Build);

        // host
        public abstract void AddHostSetting(string SettingName, string SettingValue, bool SettingIsSecure, int createdByUserID);

        public abstract IDataReader GetHostSettings();

        public abstract IDataReader GetHostSetting(string SettingName);

        public abstract void UpdateHostSetting(string SettingName, string SettingValue, bool SettingIsSecure, int lastModifiedByUserID);

        public abstract IDataReader GetServers();

        public abstract IDataReader GetServerConfiguration();

        public abstract void UpdateServer(int ServerId, string Url, bool Enabled);

        public abstract void DeleteServer(int ServerId);

        public abstract void UpdateServerActivity(string ServerName, string IISAppName, DateTime CreatedDate, DateTime LastActivityDate);

        // portal
        public abstract int AddPortalInfo(string PortalName, string Currency, string FirstName, string LastName, string Username, string Password, string Email, DateTime ExpiryDate, double HostFee,
                                          double HostSpace, int PageQuota, int UserQuota, int SiteLogHistory, string HomeDirectory, int createdByUserID);

        public abstract int CreatePortal(string PortalName, string Currency, DateTime ExpiryDate, double HostFee, double HostSpace, int PageQuota, int UserQuota, int SiteLogHistory,
                                         string HomeDirectory, int CreatedByUserID);

        public abstract void DeletePortalInfo(int PortalId);

        public abstract void DeletePortalSetting(int PortalId, string SettingName, string CultureCode);

        public abstract void DeletePortalSettings(int PortalId);

        public abstract IDataReader GetExpiredPortals();

        public abstract IDataReader GetPortal(int PortalId, string CultureCode);

        public abstract IDataReader GetPortalByAlias(string PortalAlias);

        public abstract IDataReader GetPortalByTab(int TabId, string PortalAlias);

        public abstract int GetPortalCount();

        public abstract IDataReader GetPortals(string cultureCode);

        public abstract IDataReader GetPortalsByName(string nameToMatch, int pageIndex, int pageSize);

        public abstract IDataReader GetPortalSettings(int PortalId, string CultureCode);

        public abstract IDataReader GetPortalSpaceUsed(int PortalId);

        public abstract void UpdatePortalInfo(int portalId, int portalGroupId, string portalName, string logoFile, string footerText, DateTime expiryDate, int userRegistration, int bannerAdvertising, string currency,
                                              int administratorId, double hostFee, double hostSpace, int pageQuota, int userQuota, string paymentProcessor, string processorUserId,
                                              string processorPassword, string description, string keyWords, string backgroundFile, int siteLogHistory, int splashTabId, int homeTabId, int loginTabId,
                                              int registerTabId, int userTabId, int searchTabId, string defaultLanguage, string homeDirectory, int lastModifiedByUserID, string cultureCode);

        public abstract void UpdatePortalSetting(int PortalId, string SettingName, string SettingValue, int UserID, string CultureCode);

        public abstract void UpdatePortalSetup(int PortalId, int AdministratorId, int AdministratorRoleId, int RegisteredRoleId, int SplashTabId, int HomeTabId, int LoginTabId, int RegisterTabId,
                                               int UserTabId, int SearchTabId, int AdminTabId, string CultureCode);

        public abstract IDataReader VerifyPortalTab(int PortalId, int TabId);

        public abstract IDataReader VerifyPortal(int PortalId);

        public abstract int AddTabAfter(TabInfo tab, int afterTabId, int createdByUserID);

        public abstract int AddTabBefore(TabInfo tab, int beforeTabId, int createdByUserID);

        public abstract int AddTabToEnd(TabInfo tab, int createdByUserID);

        public abstract void DeleteTab(int tabId);

        public abstract void LocalizeTab(int tabId, string cultureCode, int lastModifiedByUserID);

        public abstract void MoveTabAfter(int tabId, int afterTabId, int lastModifiedByUserID);

        public abstract void MoveTabBefore(int tabId, int beforeTabId, int lastModifiedByUserID);

        public abstract void MoveTabToParent(int tabId, int parentId, int lastModifiedByUserID);

        public abstract void UpdateTabVersion(int tabId, Guid versionGuid);

        public abstract void UpdateTab(int tabId, int contentItemId, int portalId, Guid versionGuid, Guid defaultLanguageGuid, Guid localizedVersionGuid, string tabName, bool isVisible,
                                       bool disableLink, int parentId, string iconFile, string iconFileLarge, string title, string description, string keyWords, bool isDeleted, string url,
                                       string skinSrc, string containerSrc, DateTime startDate, DateTime endDate, int refreshInterval, string pageHeadText, bool isSecure,
                                       bool permanentRedirect, float siteMapPriority, int lastModifiedByuserID, string cultureCode);

        public abstract void UpdateTabOrder(int tabId, int tabOrder, int parentId, int lastModifiedByUserID);

        public abstract void UpdateTabTranslationStatus(int tabId, Guid localizedVersionGuid, int lastModifiedByUserID);

        public abstract IDataReader GetTabs(int portalId);

        public abstract IDataReader GetAllTabs();

        public abstract IDataReader GetTabPaths(int portalId, string cultureCode);

        public abstract IDataReader GetTab(int tabId);

        public abstract IDataReader GetTabByUniqueID(Guid uniqueId);

        public abstract IDataReader GetTabByName(string tabName, int portalId);

        public abstract IDataReader GetTabsByParentId(int parentId);

        public abstract IDataReader GetTabsByModuleID(int moduleID);

        public abstract IDataReader GetTabsByPackageID(int portalID, int packageID, bool forHost);

        public abstract int GetTabCount(int portalId);

        public abstract IDataReader GetPortalTabModules(int portalId, int tabId);

        public abstract IDataReader GetTabModule(int tabModuleId);

        public abstract IDataReader GetTabModules(int tabId);

        public abstract IDataReader GetTabPanes(int tabId);

        // module
        public abstract IDataReader GetAllModules();

        public abstract IDataReader GetModules(int PortalId);

        public abstract IDataReader GetAllTabsModules(int PortalId, bool AllTabs);

        public abstract IDataReader GetAllTabsModulesByModuleID(int ModuleId);

        public abstract IDataReader GetModule(int ModuleId, int TabId);

        public abstract IDataReader GetModuleByUniqueID(Guid UniqueID);

        public abstract IDataReader GetModuleByDefinition(int PortalId, string FriendlyName);

        public abstract IDataReader GetSearchModules(int PortalId);

        public abstract int AddModule(int ContentItemID, int PortalID, int ModuleDefID, bool AllTabs, DateTime StartDate, DateTime EndDate, bool InheritViewPermissions, bool IsDeleted,
                                      int createdByUserID);

        public abstract void UpdateModule(int ModuleId, int ContentItemId, bool AllTabs, DateTime StartDate, DateTime EndDate, bool InheritViewPermissions, bool IsDeleted, int lastModifiedByUserID);

        public abstract void DeleteModule(int ModuleId);

        public abstract IDataReader GetTabModuleOrder(int TabId, string PaneName);

        public abstract void UpdateModuleOrder(int TabId, int ModuleId, int ModuleOrder, string PaneName);

        public abstract void AddTabModule(int TabId, int ModuleId, string ModuleTitle, string Header, string Footer, int ModuleOrder, string PaneName, int CacheTime, string CacheMethod,
                                          string Alignment, string Color, string Border, string IconFile, int Visibility, string ContainerSrc, bool DisplayTitle, bool DisplayPrint,
                                          bool DisplaySyndicate, bool IsWebSlice, string WebSliceTitle, DateTime WebSliceExpiryDate, int WebSliceTTL, Guid UniqueId, Guid VersionGuid,
                                          Guid DefaultLanguageGuid, Guid LocalizedVersionGuid, string CultureCode, int createdByUserID);

        public abstract void DeleteTabModule(int TabId, int ModuleId, bool softDelete);

        public abstract void MoveTabModule(int fromTabId, int moduleId, int toTabId, string toPaneName, int lastModifiedByUserID);

        public abstract void RestoreTabModule(int TabId, int ModuleId);

        public abstract void UpdateTabModule(int TabModuleId, int TabId, int ModuleId, string ModuleTitle, string Header, string Footer, int ModuleOrder, string PaneName, int CacheTime,
                                             string CacheMethod, string Alignment, string Color, string Border, string IconFile, int Visibility, string ContainerSrc, bool DisplayTitle,
                                             bool DisplayPrint, bool DisplaySyndicate, bool IsWebSlice, string WebSliceTitle, DateTime WebSliceExpiryDate, int WebSliceTTL, Guid VersionGuid,
                                             Guid DefaultLanguageGuid, Guid LocalizedVersionGuid, string CultureCode, int lastModifiedByUserID);

        public abstract void UpdateTabModuleTranslationStatus(int TabModuleId, Guid LocalizedVersionGuid, int LastModifiedByUserID);

        public abstract void UpdateModuleLastContentModifiedOnDate(int moduleId);

        public abstract void UpdateTabModuleVersion(int TabModuleId, Guid VersionGuid);

        public abstract void UpdateTabModuleVersionByModule(int ModuleId);

        public abstract IDataReader GetModuleSettings(int ModuleId);

        public abstract IDataReader GetModuleSetting(int ModuleId, string SettingName);

        public abstract void AddModuleSetting(int ModuleId, string SettingName, string SettingValue, int createdByUserID);

        public abstract void UpdateModuleSetting(int ModuleId, string SettingName, string SettingValue, int lastModifiedByUserID);

        public abstract void DeleteModuleSetting(int ModuleId, string SettingName);

        public abstract void DeleteModuleSettings(int ModuleId);

        public abstract IDataReader GetTabSettings(int TabId);

        public abstract IDataReader GetTabSetting(int TabId, string SettingName);

        public abstract void AddTabSetting(int TabId, string SettingName, string SettingValue, int createdByUserID);

        public abstract void UpdateTabSetting(int TabId, string SettingName, string SettingValue, int lastModifiedByUserID);

        public abstract void DeleteTabSetting(int TabId, string SettingName);

        public abstract void DeleteTabSettings(int TabId);

        public abstract IDataReader GetTabModuleSettings(int TabModuleId);

        public abstract IDataReader GetTabModuleSetting(int TabModuleId, string SettingName);

        public abstract void AddTabModuleSetting(int TabModuleId, string SettingName, string SettingValue, int createdByUserID);

        public abstract void UpdateTabModuleSetting(int TabModuleId, string SettingName, string SettingValue, int lastModifiedByUserID);

        public abstract void DeleteTabModuleSetting(int TabModuleId, string SettingName);

        public abstract void DeleteTabModuleSettings(int TabModuleId);

        // desktop modules
        public abstract int AddDesktopModule(int packageID, string moduleName, string folderName, string friendlyName,
                                                string description, string version, bool isPremium, bool isAdmin,
                                                string businessControllerClass, int supportedFeatures, string compatibleVersions,
                                                string dependencies, string permissions, int contentItemId, int createdByUserID);
        public abstract void DeleteDesktopModule(int desktopModuleId);
        public abstract IDataReader GetDesktopModules();
        public abstract IDataReader GetDesktopModulesByPortal(int PortalID);
        public abstract void UpdateDesktopModule(int desktopModuleId, int packageID, string moduleName, string folderName, 
                                                    string friendlyName, string description, string version, bool isPremium,
                                                    bool isAdmin, string businessControllerClass, int supportedFeatures, 
                                                    string compatibleVersions, string dependencies, string permissions,
                                                    int contentItemId, int lastModifiedByUserID);

        //portal desktop modules
        public abstract int AddPortalDesktopModule(int portalID, int desktopModuleID, int createdByUserID);
        public abstract void DeletePortalDesktopModules(int portalID, int desktopModuleID);
        public abstract IDataReader GetPortalDesktopModules(int portalID, int desktopModuleID);

        //module definition
        public abstract int AddModuleDefinition(int desktopModuleId, string friendlyName, int DefaultCacheTime, int createdByUserID);
        public abstract void DeleteModuleDefinition(int ModuleDefId);
        public abstract IDataReader GetModuleDefinitions();
        public abstract void UpdateModuleDefinition(int ModuleDefId, string FriendlyName, int DefaultCacheTime, int lastModifiedByUserID);


        public abstract IDataReader GetModuleControls();

        public abstract IDataReader GetModuleControl(int ModuleControlId);

        public abstract IDataReader GetModuleControlsByKey(string ControlKey, int ModuleDefId);

        public abstract IDataReader GetModuleControlByKeyAndSrc(int ModuleDefID, string ControlKey, string ControlSrc);

        public abstract int AddModuleControl(int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, int ControlType, int ViewOrder, string HelpUrl,
                                             bool SupportsPartialRendering, bool SupportsPopUps, int createdByUserID);

        public abstract void UpdateModuleControl(int ModuleControlId, int ModuleDefId, string ControlKey, string ControlTitle, string ControlSrc, string IconFile, int ControlType, int ViewOrder,
                                                 string HelpUrl, bool SupportsPartialRendering, bool SupportsPopUps, int lastModifiedByUserID);

        public abstract void DeleteModuleControl(int ModuleControlId);

        public abstract int AddSkinControl(int packageID, string ControlKey, string ControlSrc, bool SupportsPartialRendering, int CreatedByUserID);

        public abstract void DeleteSkinControl(int skinControlID);

        public abstract IDataReader GetSkinControls();

        public abstract IDataReader GetSkinControl(int skinControlID);

        public abstract IDataReader GetSkinControlByKey(string controlKey);

        public abstract IDataReader GetSkinControlByPackageID(int packageID);

        public abstract void UpdateSkinControl(int skinControlID, int packageID, string ControlKey, string ControlSrc, bool SupportsPartialRendering, int LastModifiedByUserID);

        // files
        public abstract IDataReader GetFiles(int FolderID);

        public abstract IDataReader GetFile(string FileName, int FolderID);

        public abstract IDataReader GetFileById(int FileId);

        public abstract IDataReader GetFileByUniqueID(Guid UniqueID);

        public abstract void DeleteFile(int PortalId, string FileName, int FolderID);

        public abstract void DeleteFiles(int PortalId);

        public abstract int AddFile(int PortalId, Guid UniqueId, Guid VersionGuid, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string Folder, int FolderID,
                                    int createdByUserID, string hash, DateTime LastModificationTime);

        public abstract void UpdateFile(int FileId, Guid VersionGuid, string FileName, string Extension, long Size, int Width, int Height, string ContentType, string Folder, int FolderID,
                                        int lastModifiedByUserID, string hash, DateTime LastModificationTime);

        public abstract DataTable GetAllFiles();

        public abstract IDataReader GetFileContent(int FileId);

        public abstract void UpdateFileContent(int FileId, byte[] StreamFile);

        public abstract void UpdateFileVersion(int FileId, Guid VersionGuid);

        // site log
        public abstract void AddSiteLog(DateTime DateTime, int PortalId, int UserId, string Referrer, string URL, string UserAgent, string UserHostAddress, string UserHostName, int TabId,
                                        int AffiliateId);

        public abstract IDataReader GetSiteLogReports();

        public abstract IDataReader GetSiteLog(int PortalId, string PortalAlias, string ReportName, DateTime StartDate, DateTime EndDate);

        public abstract void DeleteSiteLog(DateTime DateTime, int PortalId);

        // database 
        public abstract IDataReader GetTables();

        public abstract IDataReader GetFields(string TableName);

        // vendors
        public abstract IDataReader GetVendors(int PortalId, bool UnAuthorized, int PageIndex, int PageSize);

        public abstract IDataReader GetVendorsByEmail(string Filter, int PortalId, int PageIndex, int PageSize);

        public abstract IDataReader GetVendorsByName(string Filter, int PortalId, int PageIndex, int PageSize);

        public abstract IDataReader GetVendor(int VendorID, int PortalID);

        public abstract void DeleteVendor(int VendorID);

        public abstract int AddVendor(int PortalID, string VendorName, string Unit, string Street, string City, string Region, string Country, string PostalCode, string Telephone, string Fax,
                                      string Cell, string Email, string Website, string FirstName, string LastName, string UserName, string LogoFile, string KeyWords, string Authorized);

        public abstract void UpdateVendor(int VendorID, string VendorName, string Unit, string Street, string City, string Region, string Country, string PostalCode, string Telephone, string Fax,
                                          string Cell, string Email, string Website, string FirstName, string LastName, string UserName, string LogoFile, string KeyWords, string Authorized);

        [Obsolete("Obsoleted in 6.0.0, the Vendor Classifications feature was never fully implemented and will be removed from the API")]
        public abstract IDataReader GetVendorClassifications(int VendorId);

        [Obsolete("Obsoleted in 6.0.0, the Vendor Classifications feature was never fully implemented and will be removed from the API")]
        public abstract void DeleteVendorClassifications(int VendorId);

        [Obsolete("Obsoleted in 6.0.0, the Vendor Classifications feature was never fully implemented and will be removed from the API")]
        public abstract int AddVendorClassification(int VendorId, int ClassificationId);

        // banners
        public abstract IDataReader GetBanners(int VendorId);

        public abstract IDataReader GetBanner(int BannerId);

        public abstract DataTable GetBannerGroups(int PortalId);

        public abstract void DeleteBanner(int BannerId);

        public abstract int AddBanner(string BannerName, int VendorId, string ImageFile, string URL, int Impressions, double CPM, DateTime StartDate, DateTime EndDate, string UserName,
                                      int BannerTypeId, string Description, string GroupName, int Criteria, int Width, int Height);

        public abstract void UpdateBanner(int BannerId, string BannerName, string ImageFile, string URL, int Impressions, double CPM, DateTime StartDate, DateTime EndDate, string UserName,
                                          int BannerTypeId, string Description, string GroupName, int Criteria, int Width, int Height);

        public abstract IDataReader FindBanners(int PortalId, int BannerTypeId, string GroupName);

        public abstract void UpdateBannerViews(int BannerId, DateTime StartDate, DateTime EndDate);

        public abstract void UpdateBannerClickThrough(int BannerId, int VendorId);

        // affiliates
        public abstract IDataReader GetAffiliates(int VendorId);

        public abstract IDataReader GetAffiliate(int AffiliateId, int VendorId, int PortalID);

        public abstract void DeleteAffiliate(int AffiliateId);

        public abstract int AddAffiliate(int VendorId, DateTime StartDate, DateTime EndDate, double CPC, double CPA);

        public abstract void UpdateAffiliate(int AffiliateId, DateTime StartDate, DateTime EndDate, double CPC, double CPA);

        public abstract void UpdateAffiliateStats(int AffiliateId, int Clicks, int Acquisitions);

        // skins/containers
        //Public MustOverride Function GetSkin(ByVal SkinRoot As String, ByVal PortalId As Integer, ByVal SkinType As Integer) As IDataReader
        //Public MustOverride Function GetSkins(ByVal PortalId As Integer) As IDataReader
        public abstract bool CanDeleteSkin(string SkinType, string SkinFoldername);

        public abstract int AddSkin(int skinPackageID, string skinSrc);

        public abstract int AddSkinPackage(int packageID, int portalID, string skinName, string skinType, int CreatedByUserID);

        public abstract void DeleteSkin(int skinID);

        public abstract void DeleteSkinPackage(int skinPackageID);

        public abstract IDataReader GetSkinByPackageID(int packageID);

        public abstract IDataReader GetSkinPackage(int portalID, string skinName, string skinType);

        public abstract void UpdateSkin(int skinID, string skinSrc);

        public abstract void UpdateSkinPackage(int skinPackageID, int packageID, int portalID, string skinName, string skinType, int LastModifiedByUserID);

        // personalization
        public abstract IDataReader GetAllProfiles();

        public abstract IDataReader GetProfile(int UserId, int PortalId);

        public abstract void AddProfile(int UserId, int PortalId);

        public abstract void UpdateProfile(int UserId, int PortalId, string ProfileData);

        //profile property definitions
        public abstract int AddPropertyDefinition(int PortalId, int ModuleDefId, int DataType, string DefaultValue, string PropertyCategory, 
                                                    string PropertyName, bool ReadOnly, bool Required, string ValidationExpression, int ViewOrder, 
                                                    bool Visible, int Length, int DefaultVisibility, int CreatedByUserID);

        public abstract void DeletePropertyDefinition(int definitionId);

        public abstract IDataReader GetPropertyDefinition(int definitionId);

        public abstract IDataReader GetPropertyDefinitionByName(int portalId, string name);

        public abstract IDataReader GetPropertyDefinitionsByPortal(int portalId);

        public abstract void UpdatePropertyDefinition(int PropertyDefinitionId, int DataType, string DefaultValue, string PropertyCategory,
                                                        string PropertyName, bool ReadOnly, bool Required, string ValidationExpression, int ViewOrder, 
                                                        bool Visible, int Length, int DefaultVisibility, int LastModifiedByUserID);

        // urls
        public abstract IDataReader GetUrls(int PortalID);

        public abstract IDataReader GetUrl(int PortalID, string Url);

        public abstract void AddUrl(int PortalID, string Url);

        public abstract void DeleteUrl(int PortalID, string Url);

        public abstract IDataReader GetUrlTracking(int PortalID, string Url, int ModuleId);

        public abstract void AddUrlTracking(int PortalID, string Url, string UrlType, bool LogActivity, bool TrackClicks, int ModuleId, bool NewWindow);

        public abstract void UpdateUrlTracking(int PortalID, string Url, bool LogActivity, bool TrackClicks, int ModuleId, bool NewWindow);

        public abstract void DeleteUrlTracking(int PortalID, string Url, int ModuleId);

        public abstract void UpdateUrlTrackingStats(int PortalID, string Url, int ModuleId);

        public abstract IDataReader GetUrlLog(int UrlTrackingID, DateTime StartDate, DateTime EndDate);

        public abstract void AddUrlLog(int UrlTrackingID, int UserID);

        //Folders
        public abstract IDataReader GetFoldersByPortal(int PortalID);

        public abstract IDataReader GetFoldersByPortalAndPermissions(int PortalID, string Permissions, int UserID);

        public abstract IDataReader GetFolder(int FolderID);

        public abstract IDataReader GetFolder(int PortalID, string FolderPath);

        public abstract IDataReader GetFolderByUniqueID(Guid UniqueID);

        public abstract int AddFolder(int PortalID, Guid UniqueId, Guid VersionGuid, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached, DateTime LastUpdated, int createdByUserID, int folderMappingID);

        public abstract void UpdateFolder(int PortalID, Guid VersionGuid, int FolderID, string FolderPath, int StorageLocation, bool IsProtected, bool IsCached, DateTime LastUpdated, int lastModifiedByUserID, int folderMappingID);

        public abstract void DeleteFolder(int PortalID, string FolderPath);

        public abstract void UpdateFolderVersion(int FolderID, Guid VersionGuid);

        //Permission
        public abstract IDataReader GetPermission(int permissionID);

        public abstract IDataReader GetPermissionsByModuleDefID(int ModuleDefID);

        public abstract IDataReader GetPermissionsByModuleID(int ModuleID);

        public abstract IDataReader GetPermissionsByPortalDesktopModule();

        public abstract IDataReader GetPermissionsByFolder();

        public abstract IDataReader GetPermissionByCodeAndKey(string PermissionCode, string PermissionKey);

        public abstract IDataReader GetPermissionsByTab();

        public abstract void DeletePermission(int permissionID);

        public abstract int AddPermission(string permissionCode, int moduleDefID, string permissionKey, string permissionName, int createdByUserID);

        public abstract void UpdatePermission(int permissionID, string permissionCode, int moduleDefID, string permissionKey, string permissionName, int lastModifiedByUserID);

        //ModulePermission
        public abstract IDataReader GetModulePermission(int modulePermissionID);

        public abstract IDataReader GetModulePermissionsByModuleID(int moduleID, int PermissionID);

        public abstract IDataReader GetModulePermissionsByPortal(int PortalID);

        public abstract IDataReader GetModulePermissionsByTabID(int TabID);

        public abstract void DeleteModulePermissionsByModuleID(int ModuleID);

        public abstract void DeleteModulePermissionsByUserID(int PortalID, int UserID);

        public abstract void DeleteModulePermission(int modulePermissionID);

        public abstract int AddModulePermission(int moduleID, int PermissionID, int roleID, bool AllowAccess, int UserID, int createdByUserID);

        public abstract void UpdateModulePermission(int modulePermissionID, int moduleID, int PermissionID, int roleID, bool AllowAccess, int UserID, int lastModifiedByUserID);

        //TabPermission
        public abstract IDataReader GetTabPermissionsByPortal(int PortalID);

        public abstract IDataReader GetTabPermissionsByTabID(int TabID, int PermissionID);

        public abstract void DeleteTabPermissionsByTabID(int TabID);

        public abstract void DeleteTabPermissionsByUserID(int PortalID, int UserID);

        public abstract void DeleteTabPermission(int TabPermissionID);

        public abstract int AddTabPermission(int TabID, int PermissionID, int roleID, bool AllowAccess, int UserID, int createdByUserID);

        public abstract void UpdateTabPermission(int TabPermissionID, int TabID, int PermissionID, int roleID, bool AllowAccess, int UserID, int lastModifiedByUserID);

        //FolderPermission
        public abstract IDataReader GetFolderPermission(int FolderPermissionID);

        public abstract IDataReader GetFolderPermissionsByPortal(int PortalID);

        public abstract IDataReader GetFolderPermissionsByFolderPath(int PortalID, string FolderPath, int PermissionID);

        public abstract void DeleteFolderPermissionsByFolderPath(int PortalID, string FolderPath);

        public abstract void DeleteFolderPermissionsByUserID(int PortalID, int UserID);

        public abstract void DeleteFolderPermission(int FolderPermissionID);

        public abstract int AddFolderPermission(int FolderID, int PermissionID, int roleID, bool AllowAccess, int UserID, int createdByUserID);

        public abstract void UpdateFolderPermission(int FolderPermissionID, int FolderID, int PermissionID, int roleID, bool AllowAccess, int UserID, int lastModifiedByUserID);

        //DesktopModulePermission
        public abstract IDataReader GetDesktopModulePermission(int desktopModulePermissionID);

        public abstract IDataReader GetDesktopModulePermissionsByPortalDesktopModuleID(int portalDesktopModuleID);

        public abstract IDataReader GetDesktopModulePermissions();

        public abstract void DeleteDesktopModulePermissionsByPortalDesktopModuleID(int portalDesktopModuleID);

        public abstract void DeleteDesktopModulePermissionsByUserID(int userID);

        public abstract void DeleteDesktopModulePermission(int desktopModulePermissionID);

        public abstract int AddDesktopModulePermission(int portalDesktopModuleID, int permissionID, int roleID, bool allowAccess, int userID, int createdByUserID);

        public abstract void UpdateDesktopModulePermission(int desktopModulePermissionID, int portalDesktopModuleID, int permissionID, int roleID, bool allowAccess, int userID,
                                                           int lastModifiedByUserID);

        // search engine
        public abstract IDataReader GetSearchIndexers();

        public abstract IDataReader GetSearchResultModules(int PortalID);

        // content search datastore
        public abstract void DeleteSearchItems(int ModuleID);

        public abstract void DeleteSearchItem(int SearchItemId);

        public abstract void DeleteSearchItemWords(int SearchItemId);

        public abstract int AddSearchItem(string Title, string Description, int Author, DateTime PubDate, int ModuleId, string Key, string Guid, int ImageFileId);

        public abstract IDataReader GetSearchCommonWordsByLocale(string Locale);

        public abstract IDataReader GetDefaultLanguageByModule(string ModuleList);

        public abstract IDataReader GetSearchSettings(int ModuleId);

        public abstract IDataReader GetSearchWords();

        public abstract int AddSearchWord(string Word);

        public abstract int AddSearchItemWord(int SearchItemId, int SearchWordsID, int Occurrences);

        public abstract void AddSearchItemWordPosition(int SearchItemWordID, string ContentPositions);

        public abstract IDataReader GetSearchResults(int PortalID, string Word);

        public abstract IDataReader GetSearchItems(int PortalID, int TabID, int ModuleID);

        public abstract IDataReader GetSearchResults(int PortalID, int TabID, int ModuleID);

        public abstract IDataReader GetSearchItem(int ModuleID, string SearchKey);

        public abstract void UpdateSearchItem(int SearchItemId, string Title, string Description, int Author, DateTime PubDate, int ModuleId, string Key, string Guid, int HitCount, int ImageFileId);

        //Lists
        public abstract IDataReader GetLists(int PortalID);

        public abstract IDataReader GetList(string ListName, string ParentKey, int PortalID);

        public abstract IDataReader GetListEntry(int EntryID);

        public abstract IDataReader GetListEntry(string ListName, string Value);

        public abstract IDataReader GetListEntriesByListName(string ListName, string ParentKey, int PortalID);

        public abstract int AddListEntry(string ListName, string Value, string Text, int ParentID, int Level, bool EnableSortOrder, int DefinitionID, string Description, int PortalID, bool SystemList,
                                         int CreatedByUserID);

        public abstract void UpdateListEntry(int EntryID, string Value, string Text, string Description, int LastModifiedByUserID);

        public abstract void DeleteListEntryByID(int EntryID, bool DeleteChild);

        public abstract void DeleteList(string ListName, string ParentKey);

        public abstract void DeleteListEntryByListName(string ListName, string Value, bool DeleteChild);

        public abstract void UpdateListSortOrder(int EntryID, bool MoveUp);

        //portal alias
        public abstract IDataReader GetPortalAlias(string PortalAlias, int PortalID);

        public abstract IDataReader GetPortalAliasByPortalID(int PortalID);

        public abstract IDataReader GetPortalAliasByPortalAliasID(int PortalAliasID);

        public abstract IDataReader GetPortalByPortalAliasID(int PortalAliasId);

        public abstract void UpdatePortalAlias(string PortalAlias, int lastModifiedByUserID);

        public abstract void UpdatePortalAliasInfo(int PortalAliasID, int PortalID, string HTTPAlias, int lastModifiedByUserID);

        public abstract int AddPortalAlias(int PortalID, string HTTPAlias, int createdByUserID);

        public abstract void DeletePortalAlias(int PortalAliasID);

        //event Queue
        public abstract int AddEventMessage(string eventName, int priority, string processorType, string processorCommand, string body, string sender, string subscriberId, string authorizedRoles,
                                            string exceptionMessage, DateTime sentDate, DateTime expirationDate, string attributes);

        public abstract IDataReader GetEventMessages(string eventName);

        public abstract IDataReader GetEventMessagesBySubscriber(string eventName, string subscriberId);

        public abstract void SetEventMessageComplete(int eventMessageId);

        //Authentication
        public abstract int AddAuthentication(int packageID, string authenticationType, bool isEnabled, string settingsControlSrc, string loginControlSrc, string logoffControlSrc, int CreatedByUserID);

        public abstract int AddUserAuthentication(int userID, string authenticationType, string authenticationToken, int CreatedByUserID);

        public abstract void DeleteAuthentication(int authenticationID);

        public abstract IDataReader GetAuthenticationService(int authenticationID);

        public abstract IDataReader GetAuthenticationServiceByPackageID(int packageID);

        public abstract IDataReader GetAuthenticationServiceByType(string authenticationType);

        public abstract IDataReader GetAuthenticationServices();

        public abstract IDataReader GetEnabledAuthenticationServices();

        public abstract void UpdateAuthentication(int authenticationID, int packageID, string authenticationType, bool isEnabled, string settingsControlSrc, string loginControlSrc,
                                                  string logoffControlSrc, int LastModifiedByUserID);

        //Packages
        public abstract int AddPackage(int portalID, string name, string friendlyName, string description, string type, string version, string license, string manifest, string owner,
                                       string organization, string url, string email, string releaseNotes, bool isSystemPackage, int createdByUserID, string folderName, string iconFile);

        public abstract void DeletePackage(int packageID);

        public abstract IDataReader GetPackage(int packageID);

        public abstract IDataReader GetPackageByName(int portalID, string name);

        public abstract IDataReader GetPackages(int portalID);

        public abstract IDataReader GetPackagesByType(int portalID, string type);

        public abstract IDataReader GetPackageType(string type);

        public abstract IDataReader GetPackageTypes();

        public abstract IDataReader GetModulePackagesInUse(int portalID, bool forHost);

        public abstract int RegisterAssembly(int packageID, string assemblyName, string version);

        public abstract bool UnRegisterAssembly(int packageID, string assemblyName);

        public abstract void UpdatePackage(int portalID, string name, string friendlyName, string description, string type, string version, string license, string manifest, string owner,
                                           string organization, string url, string email, string releaseNotes, bool isSystemPackage, int lastModifiedByUserID, string folderName, string iconFile);

        //languages
        public abstract int AddLanguage(string cultureCode, string cultureName, string fallbackCulture, int CreatedByUserID);

        public abstract void DeleteLanguage(int languageID);

        public abstract IDataReader GetLanguages();

        public abstract void UpdateLanguage(int languageID, string cultureCode, string cultureName, string fallbackCulture, int LastModifiedByUserID);

        public abstract int AddPortalLanguage(int portalID, int languageID, bool IsPublished, int CreatedByUserID);

        public abstract void DeletePortalLanguages(int portalID, int languageID);

        public abstract IDataReader GetLanguagesByPortal(int portalID);

        public abstract void UpdatePortalLanguage(int portalID, int languageID, bool IsPublished, int ByValUpdatedByUserID);

        public abstract int AddLanguagePack(int packageID, int languageID, int dependentPackageID, int CreatedByUserID);

        public abstract void DeleteLanguagePack(int languagePackID);

        public abstract IDataReader GetLanguagePackByPackage(int packageID);

        public abstract int UpdateLanguagePack(int languagePackID, int packageID, int languageID, int dependentPackageID, int LastModifiedByUserID);

        //localisation
        public abstract string GetPortalDefaultLanguage(int portalID);

        public abstract void UpdatePortalDefaultLanguage(int portalID, string CultureCode);

        public abstract void EnsureLocalizationExists(int portalID, string CultureCode);

        //folder mappings
        public abstract int AddFolderMapping(int portalID, string mappingName, string folderProviderType, int createdByUserID);

        public abstract void UpdateFolderMapping(int folderMappingID, string mappingName, int priority, int lastModifiedByUserID);

        public abstract void DeleteFolderMapping(int folderMappingID);

        public abstract IDataReader GetFolderMapping(int folderMappingID);

        public abstract IDataReader GetFolderMappingByMappingName(int portalID, string mappingName);

        public abstract IDataReader GetFolderMappings(int portalID);

        public abstract void AddFolderMappingSetting(int folderMappingID, string settingName, string settingValue, int createdByUserID);

        public abstract void UpdateFolderMappingSetting(int folderMappingID, string settingName, string settingValue, int lastModifiedByUserID);

        public abstract IDataReader GetFolderMappingSettings(int folderMappingID);

        public abstract IDataReader GetFolderMappingSetting(int folderMappingID, string settingName);

        public abstract void AddDefaultFolderTypes(int portalID);

        //SystemDateTime Utility
        public abstract DateTime GetDatabaseTimeUtc();

        public abstract DateTime GetDatabaseTime();

		#region Mobile Stuff

		public abstract void DeletePreviewProfile(int id);

    	public abstract void DeleteRedirection(int id);

    	public abstract void DeleteRedirectionRule(int id);

    	public abstract IDataReader GetPreviewProfiles(int portalId);

        public abstract IDataReader GetAllRedirections();

    	public abstract IDataReader GetRedirections(int portalId);

    	public abstract IDataReader GetRedirectionRules(int redirectionId);

    	public abstract int SaveRedirection(int id, int portalId, string name, int type, int sortOrder, int sourceTabId, bool includeChildTabs, int targetType, object targetValue, bool enabled, int userId);

    	public abstract void SaveRedirectionRule(int id, int redirectionId, string capbility, string expression);

    	public abstract int SavePreviewProfile(int id, int portalId, string name, int width, int height, string userAgent, int sortOrder, int userId);

    	#endregion
     
    	#endregion
    }
}