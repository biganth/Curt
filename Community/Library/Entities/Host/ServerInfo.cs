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
using System.Data;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;

#endregion

namespace DotNetNuke.Entities.Host
{
    public class ServerInfo : IHydratable
    {
        public ServerInfo() : this(DateTime.Now, DateTime.Now)
        {
        }

        public ServerInfo(DateTime created, DateTime lastactivity)
        {
            IISAppName = Globals.IISAppName;
            ServerName = Globals.ServerName;
            CreatedDate = created;
            LastActivityDate = lastactivity;
        }

        public int ServerID { get; set; }

        public string IISAppName { get; set; }

        public string ServerName { get; set; }

        public string Url { get; set; }

        public bool Enabled { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastActivityDate { get; set; }

        #region IHydratable Members

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Fills a ServerInfo from a Data Reader
        /// </summary>
        /// <param name="dr">The Data Reader to use</param>
        /// <history>
        /// 	[cnurse]	03/25/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void Fill(IDataReader dr)
        {
            ServerID = Null.SetNullInteger(dr["ServerID"]);
            IISAppName = Null.SetNullString(dr["IISAppName"]);
            ServerName = Null.SetNullString(dr["ServerName"]);
            Url = Null.SetNullString(dr["URL"]);
            Enabled = Null.SetNullBoolean(dr["Enabled"]);
            CreatedDate = Null.SetNullDateTime(dr["CreatedDate"]);
            LastActivityDate = Null.SetNullDateTime(dr["LastActivityDate"]);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Key ID
        /// </summary>
        /// <returns>An Integer</returns>
        /// <history>
        /// 	[cnurse]	03/25/2009   Created
        /// </history>
        /// -----------------------------------------------------------------------------
        public int KeyID
        {
            get
            {
                return ServerID;
            }
            set
            {
                ServerID = value;
            }
        }

        #endregion
    }
}
