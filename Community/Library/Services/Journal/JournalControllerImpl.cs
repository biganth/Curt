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

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Security.Roles.Internal;

namespace DotNetNuke.Services.Journal
{
    internal class JournalControllerImpl : IJournalController
    {
        private readonly IJournalDataService _dataService;

        #region Constructors

        public JournalControllerImpl()
        {
            _dataService = JournalDataService.Instance;
        }

        #endregion

        #region Private Methods

        private XmlElement CreateElement(XmlDocument xDoc, string name, string value)
        {
            XmlElement xnode = xDoc.CreateElement(name);
            XmlText xtext = xDoc.CreateTextNode(value);
            xnode.AppendChild(xtext);
            return xnode;
        }

        private XmlElement CreateCDataElement(XmlDocument xDoc, string name, string value)
        {
            XmlElement xnode = xDoc.CreateElement(name);
            XmlCDataSection xdata = xDoc.CreateCDataSection(value);
            xnode.AppendChild(xdata);
            return xnode;
        }

        private void UpdateGroupStats(int portalId, int groupId)
        {
            RoleInfo role = TestableRoleController.Instance.GetRole(portalId, r => r.RoleID == groupId);
            if (role == null)
            {
                return;
            }

            for (var i = 0; i < role.Settings.Keys.Count; i++ )
            {
                var key = role.Settings.Keys.ElementAt(i);
                if(key.StartsWith("stat_"))
                {
                    role.Settings[key] = "0";
                }
            }

            using (IDataReader dr = _dataService.Journal_GetStatsForGroup(portalId, groupId))
            {
                while (dr.Read())
                {
                    string settingName = "stat_" + dr["JournalType"];
                    if (role.Settings.ContainsKey(settingName))
                    {
                        role.Settings[settingName] = dr["JournalTypeCount"].ToString();
                    }
                    else
                    {
                        role.Settings.Add(settingName, dr["JournalTypeCount"].ToString());
                    }
                }
                dr.Close();
            }
            TestableRoleController.Instance.UpdateRoleSettings(role, true);
        }

        #endregion

        #region Public Methods

        public void DeleteJournalItem(int portalId, int currentUserId, int journalId)
        {
            JournalItem ji = GetJournalItem(portalId, currentUserId, journalId);
            int groupId = ji.SocialGroupId;
            _dataService.Journal_Delete(journalId);
            if (groupId > 0)
            {
                UpdateGroupStats(portalId, groupId);
            }
        }

        public void DeleteJournalItemByKey(int portalId, string objectKey)
        {
            _dataService.Journal_DeleteByKey(portalId, objectKey);
        }
        public void DeleteJournalItemByGroupId(int portalId, int groupId)
        {
            _dataService.Journal_DeleteByGroupId(portalId, groupId);
        }

        public JournalItem GetJournalItem(int portalId, int currentUserId, int journalId)
        {
            return CBO.FillObject<JournalItem>(_dataService.Journal_Get(portalId, currentUserId, journalId));
        }

        public JournalItem GetJournalItemByKey(int portalId, string objectKey)
        {
            if (string.IsNullOrEmpty(objectKey))
            {
                return null;
            }
            return (JournalItem) CBO.FillObject(_dataService.Journal_GetByKey(portalId, objectKey), typeof (JournalItem));
        }

        public void SaveJournalItem(JournalItem journalItem, int tabId)
        {
            if (journalItem.UserId < 1)
            {
                throw new ArgumentException("journalItem.UserId must be for a real user");
            }
            UserInfo currentUser = UserController.GetUserById(journalItem.PortalId, journalItem.UserId);
            if (currentUser == null)
            {
                throw new Exception("Unable to locate the current user");
            }
            
            string xml = null;
            var portalSecurity = new PortalSecurity();
            if (!String.IsNullOrEmpty(journalItem.Title))
            {
                journalItem.Title = portalSecurity.InputFilter(journalItem.Title, PortalSecurity.FilterFlag.NoMarkup);
            }
            if (!String.IsNullOrEmpty(journalItem.Summary))
            {
                journalItem.Summary = HttpUtility.HtmlDecode(portalSecurity.InputFilter(journalItem.Summary, PortalSecurity.FilterFlag.NoScripting));
            }
            if (!String.IsNullOrEmpty(journalItem.Body))
            {
                journalItem.Body = HttpUtility.HtmlDecode(portalSecurity.InputFilter(journalItem.Body, PortalSecurity.FilterFlag.NoScripting));
            }

            if (!String.IsNullOrEmpty(journalItem.Body))
            {
                var xDoc = new XmlDocument();
                XmlElement xnode = xDoc.CreateElement("items");
                XmlElement xnode2 = xDoc.CreateElement("item");
                xnode2.AppendChild(CreateElement(xDoc, "id", "-1"));
                xnode2.AppendChild(CreateCDataElement(xDoc, "body", journalItem.Body));
                xnode.AppendChild(xnode2);
                xDoc.AppendChild(xnode);
                XmlDeclaration xDec = xDoc.CreateXmlDeclaration("1.0", null, null);
                xDec.Encoding = "UTF-16";
                xDec.Standalone = "yes";
                XmlElement root = xDoc.DocumentElement;
                xDoc.InsertBefore(xDec, root);
                journalItem.JournalXML = xDoc;
                xml = journalItem.JournalXML.OuterXml;
            }
            if (journalItem.ItemData != null)
            {
                if (!String.IsNullOrEmpty(journalItem.ItemData.Title))
                {
                    journalItem.ItemData.Title = portalSecurity.InputFilter(journalItem.ItemData.Title, PortalSecurity.FilterFlag.NoMarkup);
                }
                if (!String.IsNullOrEmpty(journalItem.ItemData.Description))
                {
                    journalItem.ItemData.Description = HttpUtility.HtmlDecode(portalSecurity.InputFilter(journalItem.ItemData.Description, PortalSecurity.FilterFlag.NoScripting));
                }
                if (!String.IsNullOrEmpty(journalItem.ItemData.Url))
                {
                    journalItem.ItemData.Url = portalSecurity.InputFilter(journalItem.ItemData.Url, PortalSecurity.FilterFlag.NoScripting);
                }
                if (!String.IsNullOrEmpty(journalItem.ItemData.ImageUrl))
                {
                    journalItem.ItemData.ImageUrl = portalSecurity.InputFilter(journalItem.ItemData.ImageUrl, PortalSecurity.FilterFlag.NoScripting);
                }
            }
            string journalData = journalItem.ItemData.ToJson();
            if (journalData == "null")
            {
                journalData = null;
            }
            if (String.IsNullOrEmpty(journalItem.SecuritySet))
            {
                journalItem.SecuritySet = "E,";
            }
            else if (!journalItem.SecuritySet.EndsWith(","))
            {
                journalItem.SecuritySet += ",";
            }
            if (journalItem.SecuritySet == "F,")
            {
                journalItem.SecuritySet = "F" + journalItem.UserId.ToString(CultureInfo.InvariantCulture) + ",";
                journalItem.SecuritySet += "P" + journalItem.ProfileId.ToString(CultureInfo.InvariantCulture) + ",";
            }
            if (journalItem.SecuritySet == "U,")
            {
                journalItem.SecuritySet += "U" + journalItem.UserId.ToString(CultureInfo.InvariantCulture) + ",";
            }
            if (journalItem.ProfileId > 0 && journalItem.UserId != journalItem.ProfileId)
            {
                journalItem.SecuritySet += "P" + journalItem.ProfileId.ToString(CultureInfo.InvariantCulture) + ",";
                journalItem.SecuritySet += "U" + journalItem.UserId.ToString(CultureInfo.InvariantCulture) + ",";
            }
            if (!journalItem.SecuritySet.Contains("U" + journalItem.UserId.ToString(CultureInfo.InvariantCulture)))
            {
                journalItem.SecuritySet += "U" + journalItem.UserId.ToString(CultureInfo.InvariantCulture) + ",";
            }
            if (journalItem.SocialGroupId > 0)
            {
                JournalItem item = journalItem;
                RoleInfo role = TestableRoleController.Instance.GetRole(journalItem.PortalId, r => r.SecurityMode != SecurityMode.SecurityRole && r.RoleID == item.SocialGroupId);
                if (role != null)
                {
                    if (currentUser.IsInRole(role.RoleName))
                    {
                        journalItem.SecuritySet += "R" + journalItem.SocialGroupId.ToString(CultureInfo.InvariantCulture) + ",";
                        if (!role.IsPublic)
                        {
                            journalItem.SecuritySet = journalItem.SecuritySet.Replace("E,", String.Empty);
                        }
                    }
                }
            }
            journalItem.JournalId = _dataService.Journal_Save(journalItem.PortalId,
                                                     journalItem.UserId,
                                                     journalItem.ProfileId,
                                                     journalItem.SocialGroupId,
                                                     journalItem.JournalId,
                                                     journalItem.JournalTypeId,
                                                     journalItem.Title,
                                                     journalItem.Summary,
                                                     journalItem.Body,
                                                     journalData,
                                                     xml,
                                                     journalItem.ObjectKey,
                                                     journalItem.AccessKey,
                                                     journalItem.SecuritySet);

            var updatedJournalItem = GetJournalItem(journalItem.PortalId, journalItem.UserId, journalItem.JournalId);
            journalItem.DateCreated = updatedJournalItem.DateCreated;
            journalItem.DateUpdated = updatedJournalItem.DateUpdated;

            var cnt = new Content();

            if (journalItem.ContentItemId > 0)
            {
                cnt.UpdateContentItem(journalItem, tabId);
                _dataService.Journal_UpdateContentItemId(journalItem.JournalId, journalItem.ContentItemId);
            }
            else
            {
                ContentItem ci = cnt.CreateContentItem(journalItem, tabId);
                _dataService.Journal_UpdateContentItemId(journalItem.JournalId, ci.ContentItemId);
                journalItem.ContentItemId = ci.ContentItemId;
            }
            if (journalItem.SocialGroupId > 0)
            {
                try
                {
                    UpdateGroupStats(journalItem.PortalId, journalItem.SocialGroupId);
                }
                catch (Exception exc)
                {
                    Exceptions.Exceptions.LogException(exc);
                }
            }
        }

        #endregion

        #region Journal Types

        public JournalTypeInfo GetJournalType(string journalType)
        {
            return CBO.FillObject<JournalTypeInfo>(_dataService.Journal_Types_Get(journalType));
        }

        public JournalTypeInfo GetJournalTypeById(int journalTypeId)
        {
            return CBO.FillObject<JournalTypeInfo>(_dataService.Journal_Types_GetById(journalTypeId));
        }

        public IEnumerable<JournalTypeInfo> GetJournalTypes(int portalId)
        {
            return CBO.FillCollection<JournalTypeInfo>(_dataService.Journal_Types_List(portalId));
        }

        #endregion
    }
}