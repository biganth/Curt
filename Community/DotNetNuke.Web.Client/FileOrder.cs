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
namespace DotNetNuke.Web.Client
{
    /// <summary>
    /// Contains enumerations that define the relative loading order of both JavaScript and CSS files within the framework's registration system.
    /// </summary>
    public class FileOrder
    {
        /// <summary>
        /// Defines load order of key JavaScript files within the framework
        /// </summary>
        public enum Js
        {
            /// <summary>
            /// The default priority (100) indicates that the ordering will be done based on the order in which the registrations are made
            /// </summary>
            DefaultPriority = 100,
            /// <summary>
            /// jQuery (CDN or local file) has the priority of 5
            /// </summary>
            // ReSharper disable InconsistentNaming
            jQuery = 5,
            // ReSharper restore InconsistentNaming
            /// <summary>
            /// jQuery UI (CDN or local file) has the priority of 10
            /// </summary>
// ReSharper disable InconsistentNaming
            jQueryUI = 10,
// ReSharper restore InconsistentNaming
            /// <summary>
            /// /js/dnn.xml.js has the priority of 15
            /// </summary>
            DnnXml = 15,
            /// <summary>
            /// /js/dnn.xml.jsparser.js has the priority of 20
            /// </summary>
            DnnXmlJsParser = 20,
            /// <summary>
            /// /js/dnn.xmlhttp.js has the priority of 25
            /// </summary>
            DnnXmlHttp = 25,
            /// <summary>
            /// /js/dnn.xmlhttp.jsxmlhttprequest.js has the pririty of 30
            /// </summary>
            DnnXmlHttpJsXmlHttpRequest = 30,
            /// <summary>
            /// /js/dnn.dom.positioning.js has the priority of 35
            /// </summary>
            DnnDomPositioning = 35,
            /// <summary>
            /// /js/dnn.controls.js has the priority of 40
            /// </summary>
            DnnControls = 40,
            /// <summary>
            /// /js/dnn.controls.labeledit.js has the priority of 45
            /// </summary>
            DnnControlsLabelEdit = 45,
        }

        /// <summary>
        /// Defines load order of key CSS files within the framework
        /// </summary>
        public enum Css
        {
            /// <summary>
            /// The default priority (100) indicates that the ordering will be done based on the order in which the registrations are made
            /// </summary>
            DefaultPriority = 100,
            /// <summary>
            /// The default.css file has a priority of 5
            /// </summary>
            DefaultCss = 5,
            /// <summary>
            /// Module CSS files have a priority of 10
            /// </summary>
            ModuleCss = 10,
            /// <summary>
            /// Skin CSS files have a priority of 15
            /// </summary>
            SkinCss = 15,
            /// <summary>
            /// Specific skin control's CSS files have a priority of 20
            /// </summary>
            SpecificSkinCss = 20,
            /// <summary>
            /// Container CSS files have a priority of 25
            /// </summary>
            ContainerCss = 25,
            /// <summary>
            /// Specific container control's CSS files have a priority of 30
            /// </summary>
            SpecificContainerCss = 30,
            /// <summary>
            /// The portal.css file has a priority of 35
            /// </summary>
            PortalCss = 35,
        }
    }
}