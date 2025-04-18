using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PathDistribution.Models.DAL;
using PathDistribution.Models;
using System;
using System.Collections.Generic;

namespace PathDistributionTests.Models.Schedule
{
    [TestClass]
    public class AdminDALTests
    {
        private AdminDAL _mockAdminDAL;

        [TestInitialize]
        public void Setup()
        {
            _mockAdminDAL = new AdminDAL();
        }
        [TestMethod]
        public void GetVacationSchedulesCal_ShouldReturnCorrectData()
        {
            // Arrange
            DateTime? start = new DateTime(2025, 1, 1);
            DateTime? end = new DateTime(2025, 12, 31);
            start = null; // Test with null start date
            end = null; // Test with null end date  

            // Act
            var result = _mockAdminDAL.GetVacationSchedulesCal(start, end);

            // Assert
            Assert.IsNotNull(result);
            
        }
    
       
    }
}
