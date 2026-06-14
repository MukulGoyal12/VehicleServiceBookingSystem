using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceCenterService.Services;
using ServiceCenterService.Models;
using ServiceCenterService.DTO;
using ServiceCenterService.Helpers;
using System.Threading.Tasks;
using System.Linq;

namespace VehicleBookingSystem.Tests
{
    internal class ServiceCenterServiceTest
    {
        private ServiceCenterContext _context;
        private ServiceCenterServiceImpl _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ServiceCenterContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
                .Options;

            _context = new ServiceCenterContext(options);
            var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ServiceCenterServiceImpl>();
            _service = new ServiceCenterServiceImpl(_context, logger);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CreateAsync_ShouldAddServiceCenter_Positive()
        {
            var dto = new ServiceCenterDTO
            {
                CenterName = "Center1",
                City = "Chennai",
                Contact = "1234567890",
                ServiceDescription = "General Service"
            };
            var result = await _service.CreateAsync(dto, "owner1");

            Assert.AreEqual("Success", result.Status);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual("Center1", result.Data.CenterName);
            Assert.AreEqual(1, _context.ServiceCenters.Count());
        }

        [Test]
        public async Task CreateAsync_ShouldReturnFailed_OnDuplicate_Negative()
        {
            var dto = new ServiceCenterDTO
            {
                CenterName = "CenterX",
                City = "Delhi",
                Street = "Street1",
                Pincode = "110001",
                Contact = "9999999999",
                ServiceDescription = "Repair Service"
            };

            await _service.CreateAsync(dto, "ownerX"); // first insert
            var result = await _service.CreateAsync(dto, "ownerX"); // duplicate

            Assert.AreEqual("Failed", result.Status);
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task GetByOwnerIdAsync_ShouldReturnList_Positive()
        {
            var dto = new ServiceCenterDTO
            {
                CenterName = "Center2",
                City = "Mumbai",
                Contact = "1111111111",
                ServiceDescription = "Tyre Service"
            };
            await _service.CreateAsync(dto, "owner2");

            var result = await _service.GetByOwnerIdAsync("owner2");

            Assert.AreEqual("Success", result.Status);
            Assert.IsNotEmpty(result.Data);
            Assert.AreEqual("Center2", result.Data.First().CenterName);
        }

        [Test]
        public async Task GetByOwnerIdAsync_ShouldReturnFailed_Negative()
        {
            var result = await _service.GetByOwnerIdAsync("unknownOwner");

            Assert.AreEqual("Failed", result.Status);
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task GetByIdForOwnerAsync_ShouldReturnServiceCenter_Positive()
        {
            var dto = new ServiceCenterDTO
            {
                CenterName = "Center3",
                City = "Hyd",
                Contact = "2222222222",
                ServiceDescription = "Engine Service"
            };
            var sc = await _service.CreateAsync(dto, "owner3");

            var result = await _service.GetByIdForOwnerAsync(sc.Data.ServiceCenterID, "owner3");

            Assert.AreEqual("Success", result.Status);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual("Center3", result.Data.CenterName);
        }

        [Test]
        public async Task GetByIdForOwnerAsync_ShouldReturnFailed_Negative()
        {
            var result = await _service.GetByIdForOwnerAsync("Muk_999", "ownerX");

            Assert.AreEqual("Failed", result.Status);
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task UpdateForOwnerAsync_ShouldUpdateServiceCenter_Positive()
        {
            var dto = new ServiceCenterDTO
            {
                CenterName = "Center4",
                City = "Pune",
                Contact = "3333333333",
                ServiceDescription = "Brake Service"
            };
            var sc = await _service.CreateAsync(dto, "owner4");

            var updateDto = new ServiceCenterDTO { CenterName = "UpdatedCenter4", ServiceDescription = "Updated Service" };
            var result = await _service.UpdateForOwnerAsync(sc.Data.ServiceCenterID, updateDto, "owner4");

            Assert.AreEqual("Success", result.Status);
            Assert.IsTrue(result.Data);
            Assert.AreEqual("UpdatedCenter4", _context.ServiceCenters.First().CenterName);
        }

        [Test]
        public async Task UpdateForOwnerAsync_ShouldReturnFailed_Negative()
        {
            var dto = new ServiceCenterDTO { CenterName = "DoesNotExist", ServiceDescription = "Dummy" };
            var result = await _service.UpdateForOwnerAsync("Muk_999", dto, "ownerX");

            Assert.AreEqual("Failed", result.Status);
            Assert.IsFalse(result.Data);
        }

        [Test]
        public async Task DeleteAsync_ShouldMarkInactive_Positive()
        {
            var dto = new ServiceCenterDTO
            {
                CenterName = "Center5",
                City = "Delhi",
                Contact = "4444444444",
                ServiceDescription = "Oil Change"
            };
            var sc = await _service.CreateAsync(dto, "owner5");

            var result = await _service.DeleteAsync(sc.Data.ServiceCenterID);

            Assert.AreEqual("Success", result.Status);
            Assert.IsTrue(result.Data);
            Assert.AreEqual(ServiceCenter.CenterStatus.Inactive, _context.ServiceCenters.First().Status);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFailed_Negative()
        {
            var result = await _service.DeleteAsync("Muk_999");

            Assert.AreEqual("Failed", result.Status);
            Assert.IsFalse(result.Data);
        }

        [Test]
        public async Task DeleteForOwnerAsync_ShouldMarkInactive_Positive()
        {
            var dto = new ServiceCenterDTO
            {
                CenterName = "Center6",
                City = "Delhi",
                Contact = "5555555555",
                ServiceDescription = "Battery Service"
            };
            var sc = await _service.CreateAsync(dto, "owner6");

            var result = await _service.DeleteForOwnerAsync(sc.Data.ServiceCenterID, "owner6");

            Assert.AreEqual("Success", result.Status);
            Assert.IsTrue(result.Data);
            Assert.AreEqual(ServiceCenter.CenterStatus.Inactive, _context.ServiceCenters.First().Status);
        }

        [Test]
        public async Task DeleteForOwnerAsync_ShouldReturnFailed_Negative()
        {
            var result = await _service.DeleteForOwnerAsync("Muk_999", "ownerX");

            Assert.AreEqual("Failed", result.Status);
            Assert.IsFalse(result.Data);
        }

        [Test]
        public async Task DeleteByOwnerAsync_ShouldMarkAllInactive_Positive()
        {
            var dto1 = new ServiceCenterDTO
            {
                CenterName = "Center7",
                City = "Delhi",
                Contact = "6666666666",
                ServiceDescription = "Full Service"
            };
            await _service.CreateAsync(dto1, "owner7");

            var dto2 = new ServiceCenterDTO
            {
                CenterName = "Center8",
                City = "Delhi",
                Contact = "7777777777",
                ServiceDescription = "Quick Service"
            };
            await _service.CreateAsync(dto2, "owner7");

            var result = await _service.DeleteByOwnerAsync("owner7");

            Assert.AreEqual("Success", result.Status);
            Assert.IsTrue(result.Data);
            Assert.IsTrue(_context.ServiceCenters.All(sc => sc.Status == ServiceCenter.CenterStatus.Inactive));
        }

        [Test]
        public async Task DeleteByOwnerAsync_ShouldReturnFailed_Negative()
        {
            var result = await _service.DeleteByOwnerAsync("unknownOwner");

            Assert.AreEqual("Failed", result.Status);
            Assert.IsFalse(result.Data);
        }
    }
}
