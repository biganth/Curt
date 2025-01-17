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
using System.Text;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;

#endregion

namespace DotNetNuke.Security.Permissions.Controls
{
    public class TabPermissionsGrid : PermissionsGrid
    {
        #region "Private Members"

        private List<PermissionInfoBase> _PermissionsList;
        private int _TabID = -1;
        private TabPermissionCollection _TabPermissions;

        #endregion

        #region "Protected Properties"

        protected override List<PermissionInfoBase> PermissionsList
        {
            get
            {
                if (_PermissionsList == null && _TabPermissions != null)
                {
                    _PermissionsList = _TabPermissions.ToList();
                }
                return _PermissionsList;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Permissions Collection
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public TabPermissionCollection Permissions
        {
            get
            {
                //First Update Permissions in case they have been changed
                UpdatePermissions();

                //Return the TabPermissions
                return _TabPermissions;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and Sets the Id of the Tab
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public int TabID
        {
            get
            {
                return _TabID;
            }
            set
            {
                _TabID = value;
                if (!Page.IsPostBack)
                {
                    GetTabPermissions();
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the TabPermissions from the Data Store
        /// </summary>
        /// <history>
        ///     [cnurse]    01/12/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void GetTabPermissions()
        {
            _TabPermissions = new TabPermissionCollection(TabPermissionController.GetTabPermissions(TabID, PortalId));
            _PermissionsList = null;
        }

        public override void DataBind()
        {
            GetTabPermissions();
            base.DataBind();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Parse the Permission Keys used to persist the Permissions in the ViewState
        /// </summary>
        /// <param name="Settings">A string array of settings</param>
        /// <history>
        ///     [cnurse]    01/09/2006  Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        private TabPermissionInfo ParseKeys(string[] Settings)
        {
            var objTabPermission = new TabPermissionInfo();

            //Call base class to load base properties
            base.ParsePermissionKeys(objTabPermission, Settings);
            if (String.IsNullOrEmpty(Settings[2]))
            {
                objTabPermission.TabPermissionID = -1;
            }
            else
            {
                objTabPermission.TabPermissionID = Convert.ToInt32(Settings[2]);
            }
            objTabPermission.TabID = TabID;

            return objTabPermission;
        }

        protected override void AddPermission(PermissionInfo permission, int roleId, string roleName, int userId, string displayName, bool allowAccess)
        {
            var objPermission = new TabPermissionInfo(permission);
            objPermission.TabID = TabID;
            objPermission.RoleID = roleId;
            objPermission.RoleName = roleName;
            objPermission.AllowAccess = allowAccess;
            objPermission.UserID = userId;
            objPermission.DisplayName = displayName;
            _TabPermissions.Add(objPermission, true);

            //Clear Permission List
            _PermissionsList = null;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates a Permission
        /// </summary>
        /// <param name="permissions">The permissions collection</param>
        /// <param name="user">The user to add</param>
        /// <history>
        ///     [cnurse]    01/12/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void AddPermission(ArrayList permissions, UserInfo user)
        {
            //Search TabPermission Collection for the user 
            bool isMatch = false;
            foreach (TabPermissionInfo objTabPermission in _TabPermissions)
            {
                if (objTabPermission.UserID == user.UserID)
                {
                    isMatch = true;
                    break;
                }
            }

            //user not found so add new
            if (!isMatch)
            {
                foreach (PermissionInfo objPermission in permissions)
                {
                    if (objPermission.PermissionKey == "VIEW")
                    {
                        AddPermission(objPermission, int.Parse(Globals.glbRoleNothing), Null.NullString, user.UserID, user.DisplayName, true);
                    }
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Enabled status of the permission
        /// </summary>
        /// <param name="objPerm">The permission being loaded</param>
        /// <param name="role">The role</param>
        /// <param name="column">The column of the Grid</param>
        /// <history>
        ///     [cnurse]    01/13/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override bool GetEnabled(PermissionInfo objPerm, RoleInfo role, int column)
        {
            return role.RoleID != AdministratorRoleId;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Value of the permission
        /// </summary>
        /// <param name="objPerm">The permission being loaded</param>
        /// <param name="role">The role</param>
        /// <param name="column">The column of the Grid</param>
        /// <param name="defaultState">Default State.</param>
        /// <returns>A Boolean (True or False)</returns>
        /// <history>
        ///     [cnurse]    01/09/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override string GetPermission(PermissionInfo objPerm, RoleInfo role, int column, string defaultState)
        {
            string permission;

            if (role.RoleID == AdministratorRoleId)
            {
                permission = PermissionTypeGrant;
            }
            else
            {
                //Call base class method to handle standard permissions
                permission = base.GetPermission(objPerm, role, column, PermissionTypeNull);
            }
            return permission;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the permissions from the Database
        /// </summary>
        /// <history>
        ///     [cnurse]    01/12/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override ArrayList GetPermissions()
        {
            return PermissionController.GetPermissionsByTab();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Load the ViewState
        /// </summary>
        /// <param name="savedState">The saved state</param>
        /// <history>
        ///     [cnurse]    01/12/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                //Load State from the array of objects that was saved with SaveViewState.
                var myState = (object[])savedState;

                //Load Base Controls ViewState
                if (myState[0] != null)
                {
                    base.LoadViewState(myState[0]);
                }

                //Load TabId
                if (myState[1] != null)
                {
                    TabID = Convert.ToInt32(myState[1]);
                }

                //Load TabPermissions
                if (myState[2] != null)
                {
                    _TabPermissions = new TabPermissionCollection();
                    string state = Convert.ToString(myState[2]);
                    if (!String.IsNullOrEmpty(state))
                    {
                        //First Break the String into individual Keys
                        string[] permissionKeys = state.Split(new[] { "##" }, StringSplitOptions.None);
                        foreach (string key in permissionKeys)
                        {
                            string[] Settings = key.Split('|');
                            _TabPermissions.Add(ParseKeys(Settings));
                        }
                    }
                }
            }
        }

        protected override void RemovePermission(int permissionID, int roleID, int userID)
        {
            _TabPermissions.Remove(permissionID, roleID, userID);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Saves the ViewState
        /// </summary>
        /// <history>
        ///     [cnurse]    01/12/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override object SaveViewState()
        {
            var allStates = new object[3];

            //Save the Base Controls ViewState
            allStates[0] = base.SaveViewState();

            //Save the Tab Id
            allStates[1] = TabID;

            //Persist the TabPermisisons
            var sb = new StringBuilder();
            if (_TabPermissions != null)
            {
                bool addDelimiter = false;
                foreach (TabPermissionInfo objTabPermission in _TabPermissions)
                {
                    if (addDelimiter)
                    {
                        sb.Append("##");
                    }
                    else
                    {
                        addDelimiter = true;
                    }
                    sb.Append(BuildKey(objTabPermission.AllowAccess,
                                       objTabPermission.PermissionID,
                                       objTabPermission.TabPermissionID,
                                       objTabPermission.RoleID,
                                       objTabPermission.RoleName,
                                       objTabPermission.UserID,
                                       objTabPermission.DisplayName));
                }
            }
            allStates[2] = sb.ToString();
            return allStates;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// returns whether or not the derived grid supports Deny permissions
        /// </summary>
        /// <history>
        ///     [cnurse]    01/09/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override bool SupportsDenyPermissions(PermissionInfo permissionInfo)
        {
            return true;
        }

        #endregion

        #region "Public Methods"

        public override void GenerateDataGrid()
        {
        }

        #endregion
    }
}
