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

using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Modules.Taxonomy.Presenters;
using DotNetNuke.Modules.Taxonomy.Views;
using DotNetNuke.Modules.Taxonomy.Views.Models;
using DotNetNuke.Services.Cache;
using DotNetNuke.Tests.Utilities.Mocks;

using Moq;

using NUnit.Framework;

namespace DotNetNuke.Tests.Content.Presenters
{
    /// <summary>
    ///   Summary description for VocabulariesListPresenter Tests
    /// </summary>
    [TestFixture]
    public class VocabularyListPresenterTests
    {
         private Mock<CachingProvider> mockCache;

        #region SetUp and TearDown

        [SetUp]
        public void SetUp()
        {
            //Register MockCachingProvider
            mockCache = MockComponentProvider.CreateNew<CachingProvider>();
        }

        [TearDown]
        public void TearDown()
        {
            MockComponentProvider.ResetContainer();
        }

        #endregion       
        
        #region Constructor Tests

        [Test]
        public void VocabularyListPresenter_Constructor_Requires_Non_Null_VocabularyController()
        {
            //Arrange
            var view = new Mock<IVocabularyListView>();

            //Act, Assert
            Assert.Throws<ArgumentNullException>(() => new VocabularyListPresenter(view.Object, null));
        }

        #endregion

        #region Initialization Tests

        //[Test]
        //public void VocabularyListPresenter_OnInit_Calls_Controller_GetVocabularies()
        //{
        //    // Arrange
        //    var mockController = new Mock<IVocabularyController>();
        //    var view = new Mock<IVocabularyListView>();
        //    view.Setup(v => v.Model).Returns(new VocabularyListModel());

        //    var presenter = new VocabularyListPresenter(view.Object, mockController.Object);

        //    // Act (Raise the Initialize Event)
        //    view.Raise(v => v.Initialize += null, EventArgs.Empty);

        //    // Assert
        //    mockController.Verify(c => c.GetVocabularies());
        //}

        //[Test]
        //public void VocabularyListPresenter_OnInit_Sets_Models_NavigateUrlFormatString_Property()
        //{
        //    // Arrange
        //    var mockController = new Mock<IVocabularyController>();
        //    var view = new Mock<IVocabularyListView>();
        //    view.Setup(v => v.Model).Returns(new VocabularyListModel());

        //    var presenter = new VocabularyListPresenter(view.Object, mockController.Object) {ModuleId = Constants.MODULE_ValidId, TabId = Constants.TAB_ValidId};

        //    // Act (Raise the Initialize Event)
        //    view.Raise(v => v.Initialize += null, EventArgs.Empty);

        //    // Assert
        //    //Assert.AreEqual(Globals.NavigateURL(Constants.TAB_ValidId, "EditVocabulary", String.Format("mid={0}", Constants.MODULE_ValidId), "VocabularyId={0}"),
        //    //                view.Object.Model.NavigateUrlFormatString);
        //}

        #endregion

        #region View Load Tests

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void VocabularyListPresenter_Load_Calls_View_ShowAddButton(bool isEditable)
        {
            // Arrange
            var mockController = new Mock<IVocabularyController>();
            var view = new Mock<IVocabularyListView>();
            view.Setup(v => v.Model).Returns(new VocabularyListModel());

            var presenter = new VocabularyListPresenter(view.Object, mockController.Object);
            presenter.IsEditable = isEditable;

            // Act (Raise the Load Event)
            view.Raise(v => v.Load += null, EventArgs.Empty);

            // Assert
            view.Verify(v => v.ShowAddButton(isEditable));
        }

        #endregion

        #region AddVocabulary Tests

        //[Test]
        //public void VocabularyListPresenter_On_Add_Redirects_To_CreateVocabulary()
        //{
        //    // Arrange
        //    var mockController = new Mock<IVocabularyController>();
        //    var view = new Mock<IVocabularyListView>();
        //    view.Setup(v => v.Model).Returns(new VocabularyListModel());

        //    var httpContext = new Mock<HttpContextBase>();
        //    var httpResponse = new Mock<HttpResponseBase>();
        //    httpContext.Setup(h => h.Response).Returns(httpResponse.Object);

        //    var presenter = new VocabularyListPresenter(view.Object, mockController.Object) {HttpContext = httpContext.Object, ModuleId = Constants.MODULE_ValidId, TabId = Constants.TAB_ValidId};

        //    // Act (Raise the AddVocabulary Event)
        //    //view.Raise(v => v.AddVocabulary += null, EventArgs.Empty);

        //    // Assert
        //    httpResponse.Verify(r => r.Redirect(Globals.NavigateURL(Constants.TAB_ValidId, "CreateVocabulary", String.Format("mid={0}", Constants.MODULE_ValidId))));
        //}

        #endregion
    }
}