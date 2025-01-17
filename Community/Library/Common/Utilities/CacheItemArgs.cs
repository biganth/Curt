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

using System.Collections;
using System.Web.Caching;

using DotNetNuke.Services.Cache;

#endregion

namespace DotNetNuke.Common.Utilities
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Common.Utilities
    /// Class:      CacheItemArgs
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The CacheItemArgs class provides an EventArgs implementation for the
    /// CacheItemExpiredCallback delegate
    /// </summary>
    /// <history>
    ///     [cnurse]	01/12/2008	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class CacheItemArgs
    {
        private ArrayList _paramList;

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Constructs a new CacheItemArgs Object
        /// </summary>
        /// <param name="key"></param>
        /// <history>
        ///     [cnurse]	01/12/2008	created
        /// </history>
        ///-----------------------------------------------------------------------------
        public CacheItemArgs(string key)
        {
            CacheKey = key;
            CacheTimeOut = 20;
            CachePriority = CacheItemPriority.Default;
            //_ParamList = new ArrayList();
        }

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Constructs a new CacheItemArgs Object
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeout"></param>
        /// <history>
        ///     [cnurse]	01/12/2008	created
        /// </history>
        ///-----------------------------------------------------------------------------
        public CacheItemArgs(string key, int timeout)
            : this(key)
        {
            CacheTimeOut = timeout;
            CachePriority = CacheItemPriority.Default;
            //_ParamList = new ArrayList();
        }

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Constructs a new CacheItemArgs Object
        /// </summary>
        /// <param name="key"></param>
        /// <param name="priority"></param>
        /// <history>
        ///     [cnurse]	01/12/2008	created
        /// </history>
        ///-----------------------------------------------------------------------------
        public CacheItemArgs(string key, CacheItemPriority priority)
            : this(key)
        {
            CachePriority = priority;
            //_ParamList = new ArrayList();
        }

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Constructs a new CacheItemArgs Object
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeout"></param>
        /// <param name="priority"></param>
        /// <history>
        ///     [cnurse]	07/15/2008	created
        /// </history>
        ///-----------------------------------------------------------------------------
        public CacheItemArgs(string key, int timeout, CacheItemPriority priority)
            : this(key)
        {
            CacheTimeOut = timeout;
            CachePriority = priority;
        }

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Constructs a new CacheItemArgs Object
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeout"></param>
        /// <param name="priority"></param>
        /// <param name="parameters"></param>
        /// <history>
        ///     [cnurse]	07/14/2008	created
        /// </history>
        ///-----------------------------------------------------------------------------
        public CacheItemArgs(string key, int timeout, CacheItemPriority priority, params object[] parameters)
            : this(key)
        {
            CacheTimeOut = timeout;
            CachePriority = priority;
            Params = parameters;
        }

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Cache Item's CacheItemRemovedCallback delegate
        /// </summary>
        /// <history>
        ///     [cnurse]	01/13/2008	created
        /// </history>
        ///-----------------------------------------------------------------------------
        public CacheItemRemovedCallback CacheCallback { get; set; }

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Cache Item's CacheDependency
        /// </summary>
        /// <history>
        ///     [cnurse]	01/12/2008	created
        /// </history>
        ///-----------------------------------------------------------------------------
        public DNNCacheDependency CacheDependency { get; set; }

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Cache Item's Key
        /// </summary>
        /// <history>
        ///     [cnurse]	01/12/2008	created
        /// </history>
        ///-----------------------------------------------------------------------------
        public string CacheKey { get; private set; }

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Cache Item's priority (defaults to Default)
        /// </summary>
        /// <remarks>Note: DotNetNuke currently doesn't support the ASP.NET Cache's
        /// ItemPriority, but this is included for possible future use. </remarks>
        /// <history>
        ///     [cnurse]	01/12/2008	created
        /// </history>
        ///-----------------------------------------------------------------------------
        public CacheItemPriority CachePriority { get; set; }

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Cache Item's Timeout
        /// </summary>
        /// <history>
        ///     [cnurse]	01/12/2008	created
        /// </history>
        ///-----------------------------------------------------------------------------
        public int CacheTimeOut { get; set; }

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Cache Item's Parameter List
        /// </summary>
        /// <history>
        ///     [cnurse]	01/12/2008	created
        /// </history>
        ///-----------------------------------------------------------------------------
        public ArrayList ParamList
        {
            get
            {
                if (_paramList == null)
                {
                    _paramList = new ArrayList();
					//add additional params to this list if its not null
					if (Params != null)
					{
						foreach (object param in Params)
						{
							_paramList.Add(param);
						}
					}
                }

                return _paramList;
            }
        }

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Gets the Cache Item's Parameter Array
        /// </summary>
        /// <history>
        ///     [cnurse]	01/12/2008	created
        /// </history>
        ///-----------------------------------------------------------------------------
        public object[] Params { get; private set; }

        public string ProcedureName { get; set; }
    }
}
