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
namespace DotNetNuke.Entities.Modules.Actions
{
    ///-----------------------------------------------------------------------------
    /// Project		: DotNetNuke
    /// Class		: ModuleActionEventListener
    ///
    ///-----------------------------------------------------------------------------
    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    /// <history>
    /// 	[jbrinkman] 	12/27/2003	Created
    /// </history>
    ///-----------------------------------------------------------------------------
    public class ModuleActionEventListener
    {
        private readonly ActionEventHandler _actionEvent;
        private readonly int _moduleID;

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ModID"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        /// <history>
        /// 	[jbrinkman] 	12/27/2003	Created
        /// </history>
        ///-----------------------------------------------------------------------------
        public ModuleActionEventListener(int ModID, ActionEventHandler e)
        {
            _moduleID = ModID;
            _actionEvent = e;
        }

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        /// <history>
        /// 	[jbrinkman] 	12/27/2003	Created
        /// </history>
        ///-----------------------------------------------------------------------------
        public int ModuleID
        {
            get
            {
                return _moduleID;
            }
        }

        ///-----------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        /// <history>
        /// 	[jbrinkman] 	12/27/2003	Created
        /// </history>
        ///-----------------------------------------------------------------------------
        public ActionEventHandler ActionEvent
        {
            get
            {
                return _actionEvent;
            }
        }
    }
}
