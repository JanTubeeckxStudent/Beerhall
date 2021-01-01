using System;
using System.Collections.Generic;
using System.Linq;
using Beerhall.Controllers;
using Beerhall.Models.Domain;
using Beerhall.Models.ViewModels;
using Beerhall.Tests.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace Beerhall.Tests.Controllers
{
    public class BrewerControllerTest
    {
        private BrewerController _controller;
        private DummyApplicationDbContext _dummyContext;
        private Mock<IBrewerRepository> _brewerRepository;
        private Mock<ILocationRepository> _locationRepository;

        public BrewerControllerTest()
        {
            _dummyContext = new DummyApplicationDbContext();
            _brewerRepository = new Mock<IBrewerRepository>();
            _locationRepository = new Mock<ILocationRepository>();
            _controller = new BrewerController(_brewerRepository.Object, _locationRepository.Object)
            {
                TempData = new Mock<ITempDataDictionary>().Object
            };
        }

        #region Index
        [Fact]
        public void Index_PassesOrderedListOfBrewersInViewResultModelAndStoresTotalTurnOverInViewData()
        {
            _brewerRepository.Setup(m => m.getAll()).Returns(_dummyContext.Brewers);
            var result = Assert.IsType<ViewResult>(_controller.Index());
            var brewersInModel = Assert.IsType<List<Brewer>>(result.Model);
            Assert.Equal(3, brewersInModel.Count);
            Assert.Equal("Bavik", brewersInModel[0].Name);
            Assert.Equal("De Leeuw", brewersInModel[1].Name);
            Assert.Equal("Duvel Moortgat", brewersInModel[2].Name);
            Assert.Equal(20050000, result.ViewData["TotalTurnover"]);
        }
        #endregion

        #region Index GET
        [Fact]
        public void Edit_PassesBrewerInEditViewModelAndReturnsSelectListOflocations()
        {
            _brewerRepository.Setup(m => m.GetBy(1)).Returns(_dummyContext.Bavik);
            _locationRepository.Setup(m => m.GetAll()).Returns(_dummyContext.Locations);
            var result = Assert.IsType<ViewResult>(_controller.Edit(1));
            var brewerEvm = Assert.IsType<BrewerEditViewModel>(result.Model);
            var locationsInViewData = Assert.IsType<SelectList>(result.ViewData["Locations"]);
            Assert.Equal("Bavik", brewerEvm.Name);
            Assert.Equal("8531", brewerEvm.PostalCode);
            Assert.Equal(3, locationsInViewData.Count());
        }
        #endregion

        #region Edit POST
        [Fact]
        public void Edit_ValidEdit_UpdatesAndPersistsBrewerAndRedirectsToActionIndex()
        {
            _brewerRepository.Setup(m => m.GetBy(1)).Returns(_dummyContext.Bavik);
            var brewerEvm = new BrewerEditViewModel(_dummyContext.Bavik)
            {
                Street = "nieuwe straat 1"
            };
            var result = Assert.IsType<RedirectToActionResult>(_controller.Edit(brewerEvm, 1));
            var bavik = _dummyContext.Bavik;
            Assert.Equal("Index", result?.ActionName);
            Assert.Equal("Bavik", bavik.Name);
            Assert.Equal("nieuwe straat 1", bavik.Street);
            _brewerRepository.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void Edit_InvalidEdit_DoesNotChangeNorPersistsBrewerAndRedirectsToActionIndex()
        {
            _brewerRepository.Setup(m => m.GetBy(1)).Returns(_dummyContext.Bavik);
            var brewerEvm = new BrewerEditViewModel(_dummyContext.Bavik) { Turnover = -1 };
            var result = Assert.IsType<RedirectToActionResult>(_controller.Edit(brewerEvm, 1));
            var bavik = _dummyContext.Bavik;
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Bavik", bavik.Name);
            Assert.Equal("Rijksweg 33", bavik.Street);
            Assert.Equal(20000000, bavik.Turnover);
            _brewerRepository.Verify(m => m.SaveChanges(), Times.Never());
        }
        #endregion

        #region Create GET
        [Fact]
        public void Create_PassesNewBrewerInEditViewModelAndReturnsSelectListOfGemeentenWithNoSelectedValue()
        {
            _locationRepository.Setup(m => m.GetAll()).Returns(_dummyContext.Locations);
            var result = Assert.IsType<ViewResult>(_controller.Create());
            var locationsInViewData = Assert.IsType<SelectList>(result.ViewData["Locations"]);
            var brewerEvm = Assert.IsType<BrewerEditViewModel>(result.Model);
            Assert.Null(brewerEvm.Name);
            Assert.Equal(3, locationsInViewData.Count());
            Assert.Null(locationsInViewData?.SelectedValue);
        }
        #endregion

        #region Create POST
        [Fact]
        public void Create_ValidBrewer_CreatesAndPersistsBrewerAndRedirectsToActionIndex()
        {
            _brewerRepository.Setup(m => m.Add(It.IsAny<Brewer>()));
            var brewerEvm = new BrewerEditViewModel(new Brewer("Chimay")
            {
                Location = _dummyContext.Locations.Last(),
                Street = "TestStraat 10 ",
                Turnover = 8000000
            });
            var result = Assert.IsType<RedirectToActionResult>(_controller.Create(brewerEvm));
            Assert.Equal("Index", result?.ActionName);
            _brewerRepository.Verify(m => m.Add(It.IsAny<Brewer>()), Times.Once());
            _brewerRepository.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void Create_InvalidBrewer_DoesNotCreateNorPersistsBrewerAndRedirectsToActionIndex()
        {
            _brewerRepository.Setup(m => m.Add(It.IsAny<Brewer>()));
            var brewerEvm = new BrewerEditViewModel(new Brewer("Chimay")) { Turnover = -1 };
            var result = Assert.IsType<RedirectToActionResult>(_controller.Create(brewerEvm)); ;
            Assert.Equal("Index", result.ActionName);
            _brewerRepository.Verify(m => m.Add(It.IsAny<Brewer>()), Times.Never());
            _brewerRepository.Verify(m => m.SaveChanges(), Times.Never());
        }
        #endregion

        #region Delete GET
        [Fact]
        public void Delete_PassesNameOfBrewerInViewData()
        {
            _brewerRepository.Setup(m => m.GetBy(1)).Returns(_dummyContext.Bavik);
            _brewerRepository.Setup(m => m.Delete(It.IsAny<Brewer>()));
            var result = Assert.IsType<ViewResult>(_controller.Delete(1));
            Assert.Equal("Bavik", result.ViewData["name"]);
        }
        #endregion

        #region Delete POST
        [Fact]
        public void Delete_ExistingBrewer_DeletesBrewerAndPersistsChangesAndRedirectsToActionIndex()
        {
            _brewerRepository.Setup(m => m.GetBy(1)).Returns(_dummyContext.Bavik);
            _brewerRepository.Setup(m => m.Delete(It.IsAny<Brewer>()));
            var result = Assert.IsType<RedirectToActionResult>(_controller.DeleteConfirmed(1));
            Assert.Equal("Index", result.ActionName);
            _brewerRepository.Verify(m => m.GetBy(1), Times.Once());
            _brewerRepository.Verify(m => m.Delete(It.IsAny<Brewer>()), Times.Once());
            _brewerRepository.Verify(m => m.SaveChanges(), Times.Once());
        }
        #endregion
    }
}
