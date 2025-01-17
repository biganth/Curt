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
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DotNetNuke.Common;
using DotNetNuke.Common.Internal;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Users.Internal;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Roles.Internal;
using DotNetNuke.Services.Social.Messaging;
using DotNetNuke.Services.Social.Messaging.Internal;
using DotNetNuke.Web.Services;

namespace DotNetNuke.Web.InternalServices
{
    [DnnAuthorize]
    public class MessagingServiceController : DnnController
    {
        public ActionResult WaitTimeForNextMessage()
        {
            object response;
            
            try
            {
                response = new { Result = "success", Value = InternalMessagingController.Instance.WaitTimeForNextMessage(UserInfo) };
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                response = new { Result = "error" };
            }
            
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(string subject, string body, string roleIds, string userIds, string fileIds)
        {
            object response;

            try
            {
                var portalId = PortalController.GetEffectivePortalId(PortalSettings.PortalId);
                var roleIdsList = string.IsNullOrEmpty(roleIds) ? null : roleIds.FromJson<IList<int>>();
                var userIdsList = string.IsNullOrEmpty(userIds) ? null : userIds.FromJson<IList<int>>();
                var fileIdsList = string.IsNullOrEmpty(fileIds) ? null : fileIds.FromJson<IList<int>>();

                var roles = roleIdsList != null && roleIdsList.Count > 0
                    ? roleIdsList.Select(id => TestableRoleController.Instance.GetRole(portalId, r => r.RoleID == id)).Where(role => role != null).ToList()
                    : null;

                List<UserInfo> users = null;
                if (userIdsList != null)
                {
                    var userController = new UserController();
                    users = userIdsList.Select(id => userController.GetUser(portalId, id)).Where(user => user != null).ToList();
                }

                var message = new Message { Subject = HttpUtility.UrlDecode(subject), Body = HttpUtility.UrlDecode(body) };
                MessagingController.Instance.SendMessage(message, roles, users, fileIdsList);
                response = new { Result = "success", Value = message.MessageID };
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                response = new { Result = "error", Message = exc.Message };
            }

            return Json(response);
        }

        public ActionResult Search(string q)
        {
            try
            {
                var portalId = PortalController.GetEffectivePortalId(PortalSettings.PortalId);
                var isAdmin = UserInfo.IsSuperUser || UserInfo.IsInRole("Administrators");
                const int numResults = 10;

                // GetUsersAdvancedSearch doesn't accept a comma or a single quote in the query so we have to remove them for now. See issue 20224.
                q = q.Replace(",", "").Replace("'", "");
                if (q.Length == 0) return Json(null, JsonRequestBehavior.AllowGet);

                var results = TestableUserController.Instance.GetUsersBasicSearch(portalId, 0, numResults, "DisplayName", true, "DisplayName", q)
                    .Select(user => new SearchResult
                    {
                        id = "user-" + user.UserID,
                        name = user.DisplayName,
                        iconfile = string.Format(Globals.UserProfilePicFormattedUrl(), user.UserID, 32, 32),
                    }).ToList();

                //Roles should be visible to Administrators or Group Owner
                if (isAdmin || UserInfo.Social.Roles.Any(role => role.IsOwner))
                {
                    var roles = TestableRoleController.Instance.GetRolesBasicSearch(portalId, numResults, q);
                    results.AddRange(from roleInfo in roles
                                     where
                                         isAdmin ||
                                         UserInfo.Social.Roles.SingleOrDefault(
                                             ur => ur.RoleID == roleInfo.RoleID && ur.IsOwner) != null
                                     select new SearchResult
                                     {
                                         id = "role-" + roleInfo.RoleID,
                                         name = roleInfo.RoleName,
                                         iconfile =
                                             TestableGlobals.Instance.ResolveUrl(
                                                 string.IsNullOrEmpty(roleInfo.IconFile)
                                                     ? "~/images/no_avatar.gif"
                                                     : PortalSettings.HomeDirectory.TrimEnd('/') + "/" +
                                                       roleInfo.IconFile)
                                     });
                }

                return Json(results.OrderBy(sr => sr.name), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This class stores a single search result needed by jQuery Tokeninput
        /// </summary>
        private class SearchResult
        {
            // ReSharper disable InconsistentNaming
            // ReSharper disable NotAccessedField.Local
            public string id;
            public string name;
            public string iconfile;
            // ReSharper restore NotAccessedField.Local
            // ReSharper restore InconsistentNaming
        }
    }
}

