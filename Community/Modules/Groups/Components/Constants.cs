﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNuke.Modules.Groups.Components {
    public class Constants {
        internal const string DefaultRoleGroupSetting = "DefaultRoleGroup_Setting";
        internal const string DefautlGroupViewMode = "DefaultGroupViewMode_Setting";
        internal const string GroupViewPage = "GroupViewPage_Setting";
        internal const string GroupListPage = "GroupListPage_Setting";
        internal const string GroupLoadView = "GroupLoadView_Setting";

 
        internal const string GroupViewTemplate = "GroupViewTemplate_Setting";
        internal const string GroupListTemplate = "GroupListTemplate_Setting";
        internal const string GroupModerationEnabled = "GroupModerationEnabled_Setting";

        internal const string GroupPendingNotification = "GroupPendingNotification";  //Sent to Moderators when a group is created and moderation is enabled.
        internal const string GroupApprovedNotification = "GroupApprovedNotification";  //Sent to group creator when group is approved.
        internal const string GroupCreatedNotification = "GroupCreatedNotification"; //Sent to Admins/Moderators when a new group is created and moderation is disabled.
        internal const string GroupRejectedNotification = "GroupRejectedNotification"; //Sent to group creator when a group is rejected.

        internal const string MemberPendingNotification = "GroupMemberPendingNotification";  //Sent to Group Owners when a new member has requested access to a private group.
        internal const string MemberApprovedNotification = "GroupMemberApprovedNotification"; //Sent to Member when membership is approved.
        internal const string MemberJoinedNotification = "MemberJoinedNotification"; //Sent to Group Owners when a new member has joined a public group.
        internal const string MemberRejectedNotification = "GroupMemberRejectedNotification"; //Sent to requesting member when membership is rejected.


        internal const string SharedResourcesPath = "~/DesktopModules/SocialGroups/App_LocalResources/SharedResources.resx";
        internal const string ModulePath = "~/DesktopModules/SocialGroups/";
       
        
    }
}