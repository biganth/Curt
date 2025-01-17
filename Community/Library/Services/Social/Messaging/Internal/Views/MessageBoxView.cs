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
using System.Collections.Generic;
using System.Xml.Serialization;

#endregion

namespace DotNetNuke.Services.Social.Messaging.Internal.Views
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Messaging.Views
    /// Class:      MessageBoxView
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The MessageBoxView contains the View of Inbox, Sent or Archived messages
    /// </summary>
    /// -----------------------------------------------------------------------------
    [Serializable]
    public class MessageBoxView 
    {
        /// <summary>
        /// Total Number of Coversations
        /// </summary>
        [XmlAttribute]
        public int TotalConversations { get; set; }

        /// <summary>
        /// Total Number of New (Unread) Threads
        /// </summary>
        [XmlAttribute]
        public int TotalNewThreads { get; set; }

        /// <summary>
        /// List of Conversations
        /// </summary>
        [XmlAttribute]
        public List<MessageConversationView> Conversations { get; set; }

    }
}
