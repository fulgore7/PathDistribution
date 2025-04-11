using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PathDistribution.Controllers;
using PathDistribution.Models.DAL;
using PathDistribution.Models;
using System;
using System.Web.Mvc;
using System.Collections.Generic;

namespace PathDistribution.Controllers.Tests
{
    [TestClass()]
    public class AdminControllerTests
    {
        private AdminDAL _mockAdminDAL;
        private AdminController _controller;

        [TestInitialize]
        public void Setup()
        {

            _mockAdminDAL = new AdminDAL();
            _controller = new AdminController();
        }

        [TestMethod()]
        public void VacationSchedule_ReturnsViewResult_WithVacationSchedules()
        {
            // Arrange
           // var expectedSchedules = new VacationSchedules();
           // _mockAdminDAL.Setup(dal => dal.GetVacationSchedules(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                        // .Returns(expectedSchedules);

            // Act
            var result = _controller.VacationSchedule(true) as ViewResult;
            var model = result.Model as VacationSchedules;
            var pathScheduleData = model.PathScheduleData;
            var ptoRequests = model.PTORequests;
            var assignments = model.Assignments;
            
            // Assert
            Assert.IsNotNull(result);
           // Assert.AreEqual(expectedSchedules, result.Model);
        }

        [TestMethod()]
        public void RefreshVacationScheduleCal_ReturnsPartialViewResult_WithPathScheduleData()
        {
            // Arrange
            var dteStart = new DateTime(2023, 1, 1);
            var dteEnd = new DateTime(2023, 12, 31);
            var expectedSchedules = new VacationSchedules
            {
                PathScheduleData = new List<PathScheduleData>()
            };
            //_mockAdminDAL.Setup(dal => dal.GetVacationSchedules(dteStart, dteEnd))
                         //.Returns(expectedSchedules);

            // Act
            var result = _controller.RefreshVacationScheduleCal(dteStart, dteEnd) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("VacationScheduleCal2", result.ViewName);
           // Assert.AreEqual(expectedSchedules.PathScheduleData, result.Model);
        }
    


            [TestMethod()]
            public void RefreshVacationScheduleCal2_ReturnsPartialViewResult_WithPathScheduleData()
            {
                // Arrange
                var dteStart = new DateTime(2025, 1, 1);
                var dteEnd = new DateTime(2025, 1, 31);
                var expectedSchedules = new List<PathScheduleDatesCal>();
                // Act
                var result = _controller.RefreshVacationScheduleCal2(dteStart, dteEnd) as PartialViewResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual("VacationScheduleCal2", result.ViewName);
                Assert.AreEqual(expectedSchedules, result.Model);
            }
        }
    }



