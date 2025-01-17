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

namespace DotNetNuke.Security.Roles.Internal
{
    public interface IRoleController
    {
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Adds a role
        /// </summary>
        /// <param name="role">The Role to Add</param>
        /// <returns>The Id of the new role</returns>
        /// -----------------------------------------------------------------------------
        int AddRole(RoleInfo role);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Deletes a role
        /// </summary>
        /// <param name="role">The Role to delete</param>
        /// -----------------------------------------------------------------------------
        void DeleteRole(RoleInfo role);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fetch a single role based on a predicate
        /// </summary>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="predicate">The predicate (criteria) required</param>
        /// <returns>A RoleInfo object</returns>
        /// -----------------------------------------------------------------------------
        RoleInfo GetRole(int portalId, Func<RoleInfo, bool> predicate);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Obtains a list of roles from the cache (or for the database if the cache has expired)
        /// </summary>
        /// <param name="portalId">The id of the portal</param>
        /// <returns>The list of roles</returns>
        /// -----------------------------------------------------------------------------
        IList<RoleInfo> GetRoles(int portalId);

        /// <summary>
        /// get a list of roles based on progressive search
        /// </summary>
        /// <param name="portalID">the id of the portal</param>
        /// <param name="pageSize">the number of items to return</param>
        /// <param name="filterBy">the text used to trim data</param>
        /// <returns></returns>
        IList<RoleInfo> GetRolesBasicSearch(int portalID, int pageSize, string filterBy);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Get the roles based on a predicate
        /// </summary>
        /// <param name="portalId">Id of the portal</param>
        /// <param name="predicate">The predicate (criteria) required</param>
        /// <returns>A List of RoleInfo objects</returns>
        /// -----------------------------------------------------------------------------
        IList<RoleInfo> GetRoles(int portalId, Func<RoleInfo, bool> predicate);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the settings for a role
        /// </summary>
        /// <param name="roleId">Id of the role</param>
        /// <returns>A Dictionary of settings</returns>
        /// -----------------------------------------------------------------------------
        IDictionary<string, string> GetRoleSettings(int roleId);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Persists a role to the Data Store
        /// </summary>
        /// <param name="role">The role to persist</param>
        /// -----------------------------------------------------------------------------
        void UpdateRole(RoleInfo role);

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Update the role settings
        /// </summary>
        /// <param name="role">The Role</param>
        /// <param name="clearCache">A flag that indicates whether the cache should be cleared</param>
        /// -----------------------------------------------------------------------------
        void UpdateRoleSettings(RoleInfo role, bool clearCache);
    }
}
