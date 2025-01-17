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
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Users.Social;

namespace DotNetNuke.Tests.Utilities
{
    public class Constants
    {
        #region Cacheing Constants

        public const string CACHEING_InValidKey = "InValidKey";
        public const string CACHEING_ParamCacheKey = "CacheKey";
        public const string CACHEING_ValidKey = "ValidKey";
        public const string CACHEING_ValidValue = "ValidValue";

        #endregion

        #region User Constants

        public const int USER_Null = -1;
        public const int USER_ValidId = 200;
        public const int USER_InValidId = 42;
        public const int USER_AnonymousUserId = -1;
        public const int USER_TenId = 10;
        public const string USER_TenName = "user10";
        public const int USER_ElevenId = 11;
        public const string USER_ElevenName = "user11";        
        public const int UserID_Host = 1;
        public const int UserID_Admin = 2;
        public const int UserID_User12 = 12;
        public const int UserID_FirstSocialGroupOwner = 13;
        public const string UserName_Admin = "admin";
        public const string UserName_Host = "host";
        public const string UserName_User12 = "user12";
        public const string UserDisplayName_Host = "SuperUser Account";
        public const string UserDisplayName_Admin = "Administrator Account";
        public const string UserDisplayName_User12 = "User 12";
        public const string UserDisplayName_FirstSocialGroupOwner = "First Social Group Owner";
        

        #endregion

        #region Role Constants

        public const int RoleID_Administrators = 0;
        public const int RoleID_RegisteredUsers = 1;
        public const int RoleID_Subscribers = 2;
        public const int RoleID_Translator_EN_US = 3;
        public const int RoleID_FirstSocialGroup = 4;

        public const string RoleName_Administrators = "Administrators";
        public const string RoleName_RegisteredUsers = "RegisteredUsers";
        public const string RoleName_Subscribers = "Subscribers";
        public const string RoleName_Translator_EN_US = "translator_EN_US";
        public const string RoleName_FirstSocialGroup = "First Social Group";

        #endregion

        #region Portal Constants

        public const int PORTAL_Zero = 0;
        public const int PORTAL_One = 1;
        public const int PORTAL_Null = -1;
        public const int PORTAL_ValidPortalId = 1;

        #endregion

        #region PortalSettings Constants

        public const string PORTALSETTING_MessagingAllowAttachments_Name = "MessagingAllowAttachments";
        public const string PORTALSETTING_MessagingAllowAttachments_Value_YES = "YES";
        public const string PORTALSETTING_MessagingAllowAttachments_Value_NO = "NO";

        #endregion

        #region Culture Constants

        public const string CULTURE_EN_US = "en-US";
        #endregion

        #region PortalGroup Constants

        public const int PORTALGROUP_ValidPortalGroupId = 1;
        public const int PORTALGROUP_AddPortalGroupId = 2;
        public const int PORTALGROUP_DeletePortalGroupId = 3;
        public const int PORTALGROUP_InValidPortalGroupId = 999; 
        
        public const string PORTALGROUP_ValidName = "PortalGroupName";
        public const string PORTALGROUP_ValidDescription = "PortalGroupDescription";
        public const int PORTALGROUP_UpdatePortalGroupId = 4;

        public const string PORTALGROUP_UpdateName = "UpdateName";
        public const string PORTALGROUP_UpdateDescription = "UpdateDescription";

        public const int PORTALGROUP_ValidPortalGroupCount = 5;
        public const string PORTALGROUP_ValidNameFormat = "PortalGroupName {0}";
        public const string PORTALGROUP_ValidDescriptionFormat = "PortalGroupDescription {0}";        
        #endregion

        #region ContentItem Constants

        //Valid Content values
        public const int CONTENT_ValidContentItemId = 1;
        public const string CONTENT_ValidContent = "Content";
        public const string CONTENT_ValidContentKey = "ContentKey";
        public const string CONTENT_ValidTitle = "ContentTitle";
        public const string CONTENT_ValidTitle2 = "ContentTitle2";
        public const int CONTENT_ValidModuleId = 30;
        public const int CONTENT_ValidPortalId = 20;
        public const int CONTENT_ValidTabId = 10;

        public const int CONTENT_ValidContentItemCount = 5;
        public const string CONTENT_ValidContentFormat = "Content {0}";
        public const string CONTENT_ValidContentKeyFormat = "ContentKey {0}";
        public const int CONTENT_ValidStartTabId = 10;
        public const int CONTENT_ValidStartModuleId = 100;

        //InValid Content values
        public const int CONTENT_InValidContentItemId = 999;
        public const string CONTENT_InValidContent = "";
        public const int CONTENT_InValidModuleId = 888;
        public const int CONTENT_InValidPortalId = 777;
        public const int CONTENT_InValidTabId = 99;

        public const int CONTENT_IndexedTrueItemCount = 2;
        public const int CONTENT_TaggedItemCount = 4;
        public const int CONTENT_IndexedFalseItemCount = 3;
        public const bool CONTENT_IndexedFalse = false;
        public const bool CONTENT_IndexedTrue = true;

        public const int CONTENT_AddContentItemId = 2;
        public const int CONTENT_DeleteContentItemId = 3;
        public const int CONTENT_UpdateContentItemId = 4;

        public const string CONTENT_UpdateContent = "Update";
        public const string CONTENT_UpdateContentKey = "UpdateKey";

        public const string CONTENT_ValidMetaDataName = "Creator";
        public const string CONTENT_ValidMetaDataValue = "John Smith";
        public const string CONTENT_NewMetaDataName = "Abstract";
        public const string CONTENT_NewMetaDataValue = "My abstract";
        public const string CONTENT_InValidMetaDataName = "InvalidName";
        public const string CONTENT_InValidMetaDataValue = "InvalidValue";
        public const int CONTENT_MetaDataCount = 4;

        #endregion

        #region ContentType Constants

        public const int CONTENTTYPE_ValidContentTypeId = 1;
        public const string CONTENTTYPE_ValidContentType = "ContentType Name";

        public const int CONTENTTYPE_ValidContentTypeCount = 5;
        public const string CONTENTTYPE_ValidContentTypeFormat = "ContentType Name {0}";

        public const int CONTENTTYPE_InValidContentTypeId = 999;
        public const string CONTENTTYPE_InValidContentType = "Invalid ContentType";

        public const int CONTENTTYPE_AddContentTypeId = 2;
        public const int CONTENTTYPE_DeleteContentTypeId = 3;
        public const int CONTENTTYPE_UpdateContentTypeId = 4;
        public const int CONTENTTYPE_GetByNameContentTypeId = 5;
        public const string CONTENTTYPE_GetByNameContentType = "TestGetByName";
        public const string CONTENTTYPE_OriginalUpdateContentType = "TestUpdate";

        public const string CONTENTTYPE_UpdateContentType = "Update Name";

        #endregion

        #region ScopeType Constants

        public const int SCOPETYPE_ValidScopeTypeId = 1;
        public const string SCOPETYPE_ValidScopeType = "ScopeType Name";

        public const int SCOPETYPE_ValidScopeTypeCount = 5;
        public const string SCOPETYPE_ValidScopeTypeFormat = "ScopeType Name {0}";

        public const int SCOPETYPE_InValidScopeTypeId = 999;
        public const string SCOPETYPE_InValidScopeType = "Invalid ScopeType";

        public const int SCOPETYPE_AddScopeTypeId = 2;
        public const int SCOPETYPE_DeleteScopeTypeId = 3;
        public const int SCOPETYPE_UpdateScopeTypeId = 4;
        public const int SCOPETYPE_GetByNameScopeTypeId = 5;
        public const string SCOPETYPE_GetByNameScopeType = "TestGetByName";
        public const string SCOPETYPE_OriginalUpdateScopeType = "TestUpdate";

        public const string SCOPETYPE_UpdateScopeType = "Update Name";

        #endregion

        #region Tag Constants

        public const int TAG_DuplicateContentItemId = 1;
        public const int TAG_DuplicateTermId = 6;
        public const int TAG_NoContentContentId = 99;
        public const int TAG_ValidContentId = 1;
        public const int TAG_ValidContentCount = 2;

        #endregion

        #region Term Constants

        public const string TERM_CacheKey = "DNN_Terms_{0}";

        public const int TERM_ValidTermId = 1;
        public const int TERM_InValidTermId = 999;
        public const int TERM_AddTermId = 2;
        public const int TERM_DeleteTermId = 3;
        public const int TERM_UpdateTermId = 4;
        public const int TERM_ValidParentTermId = 2;
        public const int TERM_InValidParentTermId = 888;

        public const string TERM_ValidName = "Term Name";
        public const string TERM_InValidName = "";
        public const string TERM_UnusedName = "Unused";
        public const string TERM_UpdateName = "Update Name";
        public const string TERM_OriginalUpdateName = "LCD";
        public const int TERM_ValidVocabularyId = 2;
        public const int TERM_ValidGetTermsByVocabularyCount = 9;

        public const int TERM_InsertChildBeforeParentId = 2;

        public const int TERM_ValidTermStartId = 1;
        public const int TERM_ValidCount = 5;
        public const string TERM_ValidNameFormat = "Term Name {0}";
        public const int TERM_ValidCountForVocabulary1 = 2;
        public const int TERM_ValidVocabulary1 = 1;
        public const int TERM_ValidVocabulary2 = 2;
        public const int TERM_ValidWeight = 0;
        public const int TERM_UpdateWeight = 5;

        public const int TERM_ValidCountForContent1 = 2;
        public const int TERM_ValidContent1 = 1;
        public const int TERM_ValidContent2 = 2;

        #endregion

        #region Vocabulary Constants

        public const string VOCABULARY_CacheKey = "DNN_Vocabularies";

        public const int VOCABULARY_ValidVocabularyId = 1;
        public const int VOCABULARY_HierarchyVocabularyId = 2;
        public const int VOCABULARY_InValidVocabularyId = 999;
        public const int VOCABULARY_AddVocabularyId = 2;
        public const int VOCABULARY_DeleteVocabularyId = 3;
        public const int VOCABULARY_UpdateVocabularyId = 4;

        public const string VOCABULARY_ValidName = "Vocabulary Name";
        public const string VOCABULARY_InValidName = "";
        public const string VOCABULARY_UpdateName = "Update Name";

        public const VocabularyType VOCABULARY_ValidType = VocabularyType.Simple;
        public const int VOCABULARY_SimpleTypeId = 1;
        public const int VOCABULARY_HierarchyTypeId = 2;

        public const int VOCABULARY_ValidScopeTypeId = 2;
        public const int VOCABULARY_InValidScopeTypeId = 888;
        public const int VOCABULARY_UpdateScopeTypeId = 3;

        public const int VOCABULARY_ValidScopeId = 1;
        public const int VOCABULARY_InValidScopeId = 3;
        public const int VOCABULARY_UpdateScopeId = 2;
        public const string VOCABULARY_OriginalUpdateName = "TestUpdate";

        public const int VOCABULARY_ValidWeight = 0;
        public const int VOCABULARY_UpdateWeight = 5;

        public const int VOCABULARY_ValidCount = 5;
        public const string VOCABULARY_ValidNameFormat = "Vocabulary Name {0}";
        public const int VOCABULARY_ValidCountForScope1 = 2;
        public const int VOCABULARY_ValidScope1 = 1;
        public const int VOCABULARY_ValidScope2 = 2;

        #endregion

        #region Folder Constants

        public const int FOLDER_ValidFileId = 1;
        public const int FOLDER_ValidFileSize = 16;
        public const int FOLDER_ValidFolderId = 3;
        public const int FOLDER_OtherValidFolderId = 7;
        public const int FOLDER_ValidFolderMappingID = 5;
        public const string FOLDER_ValidFileName = "file.txt";
        public const string FOLDER_ValidFilePath = "C:\\folder\\file.txt";
        public const string FOLDER_ValidFolderName = "folder";
        public const string FOLDER_ValidFolderPath = "C:\\folder";
        public const string FOLDER_ValidFolderRelativePath = "folder/";
        public const string FOLDER_ValidFolderProviderType = "ValidFolderProvider";
        public const string FOLDER_ValidRootFolderMapPath = "C:\\inetpub\\wwwroot\\dotnetnuke\\portals\\20";
        public const string FOLDER_ValidSecureFilePath = "C:\\folder\\file.txt.resources";
        public const string FOLDER_ValidSubFolderName = "subfolder";
        public const string FOLDER_ValidSubFolderPath = "C:\\folder\\subfolder";
        public const string FOLDER_ValidSubFolderRelativePath = "folder/subfolder/";
        public const string FOLDER_ValidUNCFolderPath = @"\\SERVER\folder";
        public const string FOLDER_ValidUNCSubFolderPath = @"\\SERVER\folder\subfolder";
        public const string FOLDER_ValidZipFileName = "file.zip";
        public const string FOLDER_ValidZipFilePath = "C:\\folder\\file.zip";
        public const string FOLDER_OtherValidFileName = "otherfile.txt";
        public const string FOLDER_OtherInvalidFileNameExtension = "otherfile.asp";
        public const string FOLDER_OtherValidFilePath = "C:\\folder\\otherfile.txt";
        public const string FOLDER_OtherValidFolderName = "otherfolder";
        public const string FOLDER_OtherValidFolderPath = "C:\\otherfolder";
        public const string FOLDER_OtherValidFolderRelativePath = "otherfolder/";
        public const string FOLDER_OtherValidSecureFilePath = "C:\\folder\\otherfile.txt.resources";
        public const string FOLDER_OtherValidSubFolderPath = "C:\\folder\\othersubfolder";
        public const string FOLDER_OtherValidSubFolderRelativePath = "folder/othersubfolder/";
        public const string FOLDER_ModifiedFileHash = "0123456789X";
        public const string FOLDER_UnmodifiedFileHash = "0123456789";

        #endregion        

        #region Social constants

        public const int SOCIAL_InValidRelationshipType = 999;
        public const int SOCIAL_InValidRelationship = 999;
        public const int SOCIAL_InValidUserRelationship = 999;
        public const int SOCIAL_FriendRelationshipTypeID = 1;
        public const int SOCIAL_FollowerRelationshipTypeID = 2;
        public const int SOCIAL_FriendRelationshipID = 1;
        public const int SOCIAL_FollowerRelationshipID = 2;
        public const int SOCIAL_UserRelationshipIDUser10User11 = 3;
        public const int SOCIAL_UserRelationshipIDUser12User13 = 4;
        public const int SOCIAL_PrefereceIDForUser11 = 1;

        public const string SOCIAL_RelationshipTypeName = "TestType";
        public const string SOCIAL_RelationshipName = "TestName";

        public const string LOCALIZATION_RelationshipType_Deleted_Key = "RelationshipType_Deleted";
        public const string LOCALIZATION_RelationshipType_Deleted = "Deleted RelationshipType {0} : ID {1}";
        public const string LOCALIZATION_RelationshipType_Added_Key = "RelationshipType_Added";
        public const string LOCALIZATION_RelationshipType_Added = "Added RelationshipType {0}";
        public const string LOCALIZATION_RelationshipType_Updated_Key = "RelationshipType_Updated";
        public const string LOCALIZATION_RelationshipType_Updated = "Updated RelationshipType {0}";

        public const string LOCALIZATION_Relationship_Deleted_Key = "Relationship_Deleted";
        public const string LOCALIZATION_Relationship_Deleted = "Deleted Relationship {0} : ID {1}";
        public const string LOCALIZATION_Relationship_Added_Key = "Relationship_Added";
        public const string LOCALIZATION_Relationship_Added = "Added Relationship {0}";
        public const string LOCALIZATION_Relationship_Updated_Key = "Relationship_Updated";
        public const string LOCALIZATION_Relationship_Updated = "Updated Relationship {0}";

        public const string LOCALIZATION_UserRelationship_Deleted_Key = "UserRelationship_Deleted";
        public const string LOCALIZATION_UserRelationship_Deleted = "Deleted UserRelationship ID {0}, UserID {1}, RelatedUserID {2}";
        public const string LOCALIZATION_UserRelationship_Added_Key = "UserRelationship_Added";
        public const string LOCALIZATION_UserRelationship_Added = "Added UserRelationship ID {0}, UserID {1}, RelatedUserID {2}";
        public const string LOCALIZATION_UserRelationship_Updated_Key = "UserRelationship_Updated";
        public const string LOCALIZATION_UserRelationship_Updated = "Updated UserRelationship ID {0}, UserID {1}, RelatedUserID {2}";


        public const string LOCALIZATION_UserRelationshipPreference_Deleted_Key = "UserRelationshipPreference_Deleted";
        public const string LOCALIZATION_UserRelationshipPreference_Deleted = "Deleted UserRelationshipPreference ID {0} for User ID {1} and Relationship ID {2}";
        public const string LOCALIZATION_UserRelationshipPreference_Added_Key = "UserRelationshipPreference_Added";
        public const string LOCALIZATION_UserRelationshipPreference_Added = "Added UserRelationshipPreference ID {0} for User ID {1} and Relationship ID {2}";
        public const string LOCALIZATION_UserRelationshipPreference_Updated_Key = "UserRelationshipPreference_Updated";
        public const string LOCALIZATION_UserRelationshipPreference_Updated = "UpdatedUserRelationshipPreference ID {0} for User ID {1} and Relationship ID {2}";

        #endregion

        #region SocialMessaging constants

        public const bool Messaging_ReadMessage = true;
        public const bool Messaging_UnReadMessage = false;
        public const bool Messaging_ArchivedMessage = true;
        public const bool Messaging_UnArchivedMessage = false;
        public const int Messaging_RecipientId_1 = 1;
        public const int Messaging_RecipientId_2 = 2;
        public const int Messaging_MessageId_1 = 1;
        public const int Messaging_NotificationTypeId = 1;
        public const string Messaging_NotificationTypeName = "AcceptFriend";
        public const string Messaging_NotificationTypeDescription = "Accept Friend Notification";
        public const int Messaging_NotificationTypeTTL = 1440; // This is one day in minutes
        public const int Messaging_NotificationTypeDesktopModuleId = 3;
        public const int Messaging_NotificationTypeActionId = 6;
        public const string Messaging_NotificationTypeActionNameResourceKey = "Accept";
        public const string Messaging_NotificationTypeActionDescriptionResourceKey = "Accept a friend request";
        public const string Messaging_NotificationTypeActionConfirmResourceKey = "Are you sure you want to accept this friend?";
        public const string Messaging_NotificationTypeActionAPICall = "~/DesktopModules/ModuleName/API/ModuleService.ashx/Accept";
        public const int Messaging_NotificationActionId = 4;
        public const string Messaging_NotificationActionKey = "{F:1}{U:2}";
        public const string Messaging_NotificationSubject = "Friend Request Received";
        public const string Messaging_NotificationBody = "You've received a new friend request from {0}";
        public const bool Messaging_IncludeDismissAction = true;
        public const string Messaging_NotificationContext = "context";

        #endregion

        public const int TAB_ValidId = 10;
        public const int MODULE_ValidId = 100;
    }
}