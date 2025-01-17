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
using System.Linq;
using System.Web.Mvc;
using DotNetNuke.HttpModules.Services.Internal;

namespace DotNetNuke.Web.Services
{
    public class DnnControllerActionInvoker : ControllerActionInvoker
    {
        protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            FilterInfo filters = base.GetFilters(controllerContext, actionDescriptor);

            bool overrideDefaultAuthLevel = filters.AuthorizationFilters.Any(x => x is IOverrideDefaultAuthLevel);

            if(!overrideDefaultAuthLevel)
            {
                filters.AuthorizationFilters.Add(new DnnAuthorizeAttribute {RequiresHost = true});
            }

            return filters;
        }

        protected override AuthorizationContext InvokeAuthorizationFilters(ControllerContext controllerContext, IList<IAuthorizationFilter> filters, ActionDescriptor actionDescriptor)
        {
            var context = base.InvokeAuthorizationFilters(controllerContext, filters, actionDescriptor);

            if(context.Result != null && context.Result is HttpUnauthorizedResult)
            {
                var sac = new ServicesContextWrapper(controllerContext.HttpContext);
                sac.DoA401 = true;
            }

            return context;
        }
    }
}