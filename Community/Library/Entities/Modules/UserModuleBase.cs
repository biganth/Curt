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
using System.IO;
using System.Web.Caching;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Vendors;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Entities.Modules
{
    public enum DisplayMode
    {
        All = 0,
        FirstLetter = 1,
        None = 2
    }

    public enum UsersControl
    {
        Combo = 0,
        TextBox = 1
    }

    /// -----------------------------------------------------------------------------
    /// Project	 :  DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Modules
    /// Class	 :  UserModuleBase
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The UserModuleBase class defines a custom base class inherited by all
    /// desktop portal modules within the Portal that manage Users.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///		[cnurse]	03/20/2006
    /// </history>
    /// -----------------------------------------------------------------------------
    public class UserModuleBase : PortalModuleBase
    {
        private UserInfo _User;

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether we are in Add User mode
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/06/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected bool AddUser
        {
            get
            {
                return (UserId == Null.NullInteger);
            }
        }

        ///// <summary>
        ///// Gets the effective portalId for User (returns the current PortalId unless Portal
        ///// is in a PortalGroup, when it will return the PortalId of the Master Portal).
        ///// </summary>
        //protected int EffectivePortalId
        //{
        //    get { return PortalController.GetEffectivePortalId(PortalId); }
        //}

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether the current user is an Administrator (or SuperUser)
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/03/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected bool IsAdmin
        {
            get
            {
                return IsEditable;
            }
        }

        /// <summary>
        /// gets whether this is the current user or admin
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        protected bool IsUserOrAdmin
        {
            get
            {
                if (!IsUser)
                {
                    if (Request.IsAuthenticated)
                    {
                        if (!PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether this control is in the Host menu
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/13/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected bool IsHostTab
        {
            get
            {
                return base.IsHostMenu;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether the control is being called form the User Accounts module
        /// </summary>
        /// <history>
        /// 	[cnurse]	07/07/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected bool IsEdit
        {
            get
            {
                bool _IsEdit = false;
                if (Request.QueryString["ctl"] != null)
                {
                    string ctl = Request.QueryString["ctl"];
                    if (ctl == "Edit")
                    {
                        _IsEdit = true;
                    }
                }
                return _IsEdit;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether the current user is modifying their profile
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected bool IsProfile
        {
            get
            {
                bool _IsProfile = false;
                if (IsUser)
                {
                    if (PortalSettings.UserTabId != -1)
                    {
						//user defined tab
                        if (PortalSettings.ActiveTab.TabID == PortalSettings.UserTabId)
                        {
                            _IsProfile = true;
                        }
                    }
                    else
                    {
						//admin tab
                        if (Request.QueryString["ctl"] != null)
                        {
                            string ctl = Request.QueryString["ctl"];
                            if (ctl == "Profile")
                            {
                                _IsProfile = true;
                            }
                        }
                    }
                }
                return _IsProfile;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether an anonymous user is trying to register
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/21/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected bool IsRegister
        {
            get
            {
                return !IsAdmin && !IsUser;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether the User is editing their own information
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/03/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected bool IsUser
        {
            get
            {
                return Request.IsAuthenticated && (User.UserID == UserInfo.UserID);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the PortalId to use for this control
        /// </summary>
        /// <history>
        /// 	[cnurse]	02/21/2007  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected int UserPortalID
        {
            get
            {
                return IsHostTab ? Null.NullInteger : PortalId;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the User associated with this control
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/02/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public UserInfo User
        {
            get
            {
                return _User ?? (_User = AddUser ? InitialiseUser() : UserController.GetUserById(UserPortalID, UserId));
            }
            set
            {
                _User = value;
                if (_User != null)
                {
                    UserId = _User.UserID;
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the UserId associated with this control
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/01/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public new int UserId
        {
            get
            {
                int _UserId = Null.NullInteger;
                if (ViewState["UserId"] == null)
                {
                    if (Request.QueryString["userid"] != null)
                    {
                        _UserId = Int32.Parse(Request.QueryString["userid"]);
                        ViewState["UserId"] = _UserId;
                    }
                }
                else
                {
                    _UserId = Convert.ToInt32(ViewState["UserId"]);
                }
                return _UserId;
            }
            set
            {
                ViewState["UserId"] = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets a Setting for the Module
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	05/01/2006  Created
        ///     [cnurse]    02/07/2008  DNN-7003 Fixed GetSetting() in UserModuleBase so it handles situation where one or more settings are missing.
        /// </history>
        /// -----------------------------------------------------------------------------
        public static object GetSetting(int portalId, string settingKey)
        {
            Hashtable settings = UserController.GetUserSettings(portalId);
            if (settings[settingKey] == null)
            {
                settings = UserController.GetUserSettings(portalId, settings);
            }
            return settings[settingKey];
        }

        public static void UpdateSetting(int portalId, string key, string setting)
        {
            if (portalId == Null.NullInteger)
            {
                HostController.Instance.Update(new ConfigurationSetting {Value = setting, Key = key});
            }
            else
            {
                PortalController.UpdatePortalSetting(portalId, key, setting);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Updates the Settings for the Module
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	06/27/2006  Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void UpdateSettings(int portalId, Hashtable settings)
        {
            string key;
            string setting;
            IDictionaryEnumerator settingsEnumerator = settings.GetEnumerator();
            while (settingsEnumerator.MoveNext())
            {
                key = Convert.ToString(settingsEnumerator.Key);
                setting = Convert.ToString(settingsEnumerator.Value);
                UpdateSetting(portalId, key, setting);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// InitialiseUser initialises a "new" user
        /// </summary>
        /// <history>
        /// 	[cnurse]	03/13/2006
        /// </history>
        /// -----------------------------------------------------------------------------
        private UserInfo InitialiseUser()
        {
            var newUser = new UserInfo();
            if (IsHostMenu && !IsRegister)
            {
                newUser.IsSuperUser = true;
            }
            else
            {
                newUser.PortalID = PortalId;
            }

            //Initialise the ProfileProperties Collection
            string lc = new Localization().CurrentUICulture;

            newUser.Profile.InitialiseProfile(PortalId);
            newUser.Profile.PreferredTimeZone = PortalSettings.TimeZone;

            newUser.Profile.PreferredLocale = lc;

            //Set default countr
            string country = Null.NullString;
            country = LookupCountry();
            if (!String.IsNullOrEmpty(country))
            {
                newUser.Profile.Country = country;
            }
            //Set AffiliateId
            int AffiliateId = Null.NullInteger;
            if (Request.Cookies["AffiliateId"] != null)
            {
                AffiliateId = int.Parse(Request.Cookies["AffiliateId"].Value);
            }
            newUser.AffiliateID = AffiliateId;
            return newUser;
        }

        private string LookupCountry()
        {
            string IP;
            bool IsLocal = false;
            bool _CacheGeoIPData = true;
            string _GeoIPFile;
            _GeoIPFile = "controls/CountryListBox/Data/GeoIP.dat";
            if (Page.Request.UserHostAddress == "127.0.0.1")
            {
				//'The country cannot be detected because the user is local.
                IsLocal = true;
                //Set the IP address in case they didn't specify LocalhostCountryCode
                IP = Page.Request.UserHostAddress;
            }
            else
            {
				//Set the IP address so we can find the country
                IP = Page.Request.UserHostAddress;
            }
            //Check to see if we need to generate the Cache for the GeoIPData file
            if (Context.Cache.Get("GeoIPData") == null && _CacheGeoIPData)
            {
				//Store it as	well as	setting	a dependency on	the	file
                Context.Cache.Insert("GeoIPData", CountryLookup.FileToMemory(Context.Server.MapPath(_GeoIPFile)), new CacheDependency(Context.Server.MapPath(_GeoIPFile)));
            }
			
            //Check to see if the request is a localhost request
            //and see if the LocalhostCountryCode is specified
            if (IsLocal)
            {
                return Null.NullString;
            }
			
            //Either this is a remote request or it is a local
            //request with no LocalhostCountryCode specified
            CountryLookup _CountryLookup;

            //Check to see if we are using the Cached
            //version of the GeoIPData file
            if (_CacheGeoIPData)
            {
				//Yes, get it from cache
                _CountryLookup = new CountryLookup((MemoryStream) Context.Cache.Get("GeoIPData"));
            }
            else
            {
				//No, get it from file
                _CountryLookup = new CountryLookup(Context.Server.MapPath(_GeoIPFile));
            }
            //Get the country code based on the IP address
            string country = Null.NullString;
            try
            {
                country = _CountryLookup.LookupCountryName(IP);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
            return country;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddLocalizedModuleMessage adds a localized module message
        /// </summary>
        /// <param name="message">The localized message</param>
        /// <param name="type">The type of message</param>
        /// <param name="display">A flag that determines whether the message should be displayed</param>
        /// <history>
        /// 	[cnurse]	03/14/2006
        /// 	[cnurse]	07/03/2007  Moved to Base Class and changed to Protected
        /// </history>
        /// -----------------------------------------------------------------------------
        protected void AddLocalizedModuleMessage(string message, ModuleMessage.ModuleMessageType type, bool display)
        {
            if (display)
            {
                UI.Skins.Skin.AddModuleMessage(this, message, type);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// AddModuleMessage adds a module message
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="type">The type of message</param>
        /// <param name="display">A flag that determines whether the message should be displayed</param>
        /// <history>
        /// 	[cnurse]	03/14/2006
        /// 	[cnurse]	07/03/2007  Moved to Base Class and changed to Protected
        /// </history>
        /// -----------------------------------------------------------------------------
        protected void AddModuleMessage(string message, ModuleMessage.ModuleMessageType type, bool display)
        {
            AddLocalizedModuleMessage(Localization.GetString(message, LocalResourceFile), type, display);
        }

        protected string CompleteUserCreation(UserCreateStatus createStatus, UserInfo newUser, bool notify, bool register)
        {
            string strMessage = "";
            ModuleMessage.ModuleMessageType message = ModuleMessage.ModuleMessageType.RedError;
            if (register)
            {
				//send notification to portal administrator of new user registration
                strMessage += Mail.SendMail(newUser, MessageType.UserRegistrationAdmin, PortalSettings);

                var loginStatus = UserLoginStatus.LOGIN_FAILURE;

                //complete registration
                switch (PortalSettings.UserRegistration)
                {
                    case (int) Globals.PortalRegistrationType.PrivateRegistration:
                        strMessage += Mail.SendMail(newUser, MessageType.UserRegistrationPrivate, PortalSettings);

                        //show a message that a portal administrator has to verify the user credentials
                        if (string.IsNullOrEmpty(strMessage))
                        {
                            strMessage += string.Format(Localization.GetString("PrivateConfirmationMessage", Localization.SharedResourceFile), newUser.Email);
                            message = ModuleMessage.ModuleMessageType.GreenSuccess;
                        }
                        break;
                    case (int) Globals.PortalRegistrationType.PublicRegistration:
                        Mail.SendMail(newUser, MessageType.UserRegistrationPublic, PortalSettings);
                        UserController.UserLogin(PortalSettings.PortalId, newUser.Username, newUser.Membership.Password, "", PortalSettings.PortalName, "", ref loginStatus, false);
                        break;
                    case (int) Globals.PortalRegistrationType.VerifiedRegistration:
                        Mail.SendMail(newUser, MessageType.UserRegistrationVerified, PortalSettings);
                        UserController.UserLogin(PortalSettings.PortalId, newUser.Username, newUser.Membership.Password, "", PortalSettings.PortalName, "", ref loginStatus, false);
                        break;
                }
                //affiliate
                if (!Null.IsNull(User.AffiliateID))
                {
                    var objAffiliates = new AffiliateController();
                    objAffiliates.UpdateAffiliateStats(newUser.AffiliateID, 0, 1);
                }
                //store preferredlocale in cookie
                Localization.SetLanguage(newUser.Profile.PreferredLocale);
                if (IsRegister && message == ModuleMessage.ModuleMessageType.RedError)
                {
                    AddLocalizedModuleMessage(string.Format(Localization.GetString("SendMail.Error", Localization.SharedResourceFile), strMessage), message, (!String.IsNullOrEmpty(strMessage)));
                }
                else
                {
                    AddLocalizedModuleMessage(strMessage, message, (!String.IsNullOrEmpty(strMessage)));
                }
            }
            else
            {
                if (notify)
                {
					//Send Notification to User
                    if (PortalSettings.UserRegistration == (int) Globals.PortalRegistrationType.VerifiedRegistration)
                    {
                        strMessage += Mail.SendMail(newUser, MessageType.UserRegistrationVerified, PortalSettings);
                    }
                    else
                    {
                        strMessage += Mail.SendMail(newUser, MessageType.UserRegistrationPublic, PortalSettings);
                    }
                }
            }
            //Log Event to Event Log
            var objEventLog = new EventLogController();
            objEventLog.AddLog(newUser, PortalSettings, UserId, newUser.Username, EventLogController.EventLogType.USER_CREATED);
            return strMessage;
        }

        [Obsolete("In DotNetNuke 5.0 there is no longer the concept of an Admin Page.  All pages are controlled by Permissions")]
        protected bool IsAdminTab
        {
            get
            {
                return false;
            }
        }

        [Obsolete("In DotNetNuke 5.2 replaced by UserController.GetDefaultUserSettings().")]
        public static Hashtable GetDefaultSettings()
        {
            return UserController.GetDefaultUserSettings();
        }

        [Obsolete("In DotNetNuke 5.2 replaced by UserController.GetUserSettings(settings).")]
        public static Hashtable GetSettings(Hashtable settings)
        {
            return UserController.GetUserSettings(PortalController.GetCurrentPortalSettings().PortalId, settings);
        }
    }
}
