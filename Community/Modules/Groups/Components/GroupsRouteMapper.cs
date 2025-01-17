﻿using System;
using DotNetNuke.Web.Services;

namespace DotNetNuke.Modules.Groups 
{
    public class ServiceRouteMapper : IServiceRouteMapper 
    {
        public void RegisterRoutes(IMapRoute mapRouteManager) 
        {
            mapRouteManager.MapRoute("SocialGroups", "{controller}.ashx/{action}", new[] { "DotNetNuke.Modules.Groups" });
        }
    }
}