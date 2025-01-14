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
using System.Web;

namespace DotNetNuke.Common.Internal
{
    public class GlobalsImpl : IGlobals
    {
        public string ApplicationPath
        {
            get { return Globals.ApplicationPath; }
        }

        public string HostMapPath
        {
            get { return Globals.HostMapPath; }
        }

        public string GetSubFolderPath(string strFileNamePath, int portalId)
        {
            return Globals.GetSubFolderPath(strFileNamePath, portalId);
        }

        public string LinkClick(string link, int tabId, int moduleId)
        {
            return Globals.LinkClick(link, tabId, moduleId);
        }

        public string ResolveUrl(string url)
        {
            return Globals.ResolveUrl(url);
        }

        public string GetDomainName(HttpRequestBase request)
        {
            return Globals.GetDomainName(request);
        }

        public string GetDomainName(HttpRequestBase request, bool parsePortNumber)
        {
            return Globals.GetDomainName(request, parsePortNumber);
        }
    }
}