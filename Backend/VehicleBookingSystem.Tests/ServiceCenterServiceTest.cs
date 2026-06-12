using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServiceCenterService.Services;
using ServiceCenterService.Models;
using ServiceCenterService.DTO;
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
                Name = "Center1",
                Location = "Chennai",
                Contact = "1234567890",
                ServiceDescription = "General Service"
            };
            var result = await _service.CreateAsync(dto, "owner1");

            Assert.IsNotNull(result);
            Assert.AreEqual("Center1", result.Name);
            Assert.AreEqual(1, _context.ServiceCenters.Count());
        }

        [Test]
        public async Task CreateAsync_ShouldReturnNull_OnException_Negative()
        {
            _context.Dispose(); // force exception
            var dto = new ServiceCenterDTO
            {
                Name = "CenterX",
                Location = "Delhi",
                Contact = "9999999999",
                ServiceDescription = "Repair Service"
            };
            var result = await _service.CreateAsync(dto, "ownerX");

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetByOwnerIdAsync_ShouldReturnList_Positive()
        {
            var dto = new ServiceCenterDTO
            {
                Name = "Center2",
                Location = "Mumbai",
                Contact = "1111111111",
                ServiceDescription = "Tyre Service"
            };
            await _service.CreateAsync(dto, "owner2");

            var result = await _service.GetByOwnerIdAsync("owner2");

            Assert.IsNotEmpty(result);
            Assert.AreEqual("Center2", result.First().Name);
        }

        [Test]
        public async Task GetByOwnerIdAsync_ShouldReturnEmpty_Negative()
        {
            var result = await _service.GetByOwnerIdAsync("unknownOwner");
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetByIdForOwnerAsync_ShouldReturnServiceCenter_Positive()
        {
            var dto = new ServiceCenterDTO
            {
                Name = "Center3",
                Location = "Hyd",
                Contact = "2222222222",
                ServiceDescription = "Engine Service"
            };
            var sc = await _service.CreateAsync(dto, "owner3");

            var result = await _service.GetByIdForOwnerAsync(sc.ServiceCenterID, "owner3");

            Assert.IsNotNull(result);
            Assert.AreEqual("Center3", result.Name);
        }

        [Test]
        public async Task GetByIdForOwnerAsync_ShouldReturnNull_Negative()
        {
            var result = await _service.GetByIdForOwnerAsync("Muk_999", "ownerX");
            Assert.IsNull(result);
        }

        [Test]
        public async Task UpdateForOwnerAsync_ShouldUpdateServiceCenter_Positive()
        {
            var dto = new ServiceCenterDTO
            {
                Name = "Center4",
                Location = "Pune",
                Contact = "3333333333",
                ServiceDescription = "Brake Service"
            };
            var sc = await _service.CreateAsync(dto, "owner4");

            var updateDto = new ServiceCenterDTO { Name = "UpdatedCenter4", ServiceDescription = "Updated Service" };
            var result = await _service.UpdateForOwnerAsync(sc.ServiceCenterID, updateDto, "owner4");

            Assert.IsTrue(result);
            Assert.AreEqual("UpdatedCenter4", _context.ServiceCenters.First().Name);
        }

        [Test]
        public async Task UpdateForOwnerAsync_ShouldReturnFalse_Negative()
        {
            var dto = new ServiceCenterDTO { Name = "DoesNotExist", ServiceDescription = "Dummy" };
            var result = await _service.UpdateForOwnerAsync("Muk_999", dto, "ownerX");

            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteServiceCenter_Positive()
        {
            var dto = new ServiceCenterDTO
            {
                Name = "Center5",
                Location = "Delhi",
                Contact = "4444444444",
                ServiceDescription = "Oil Change"
            };
            var sc = await _service.CreateAsync(dto, "owner5");

            var result = await _service.DeleteAsync(sc.ServiceCenterID);

            Assert.IsTrue(result);
            Assert.AreEqual(0, _context.ServiceCenters.Count());
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_Negative()
        {
            var result = await _service.DeleteAsync("Muk_999");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteForOwnerAsync_ShouldDeleteServiceCenter_Positive()
        {
            var dto = new ServiceCenterDTO
            {
                Name = "Center6",
                Location = "Delhi",
                Contact = "5555555555",
                ServiceDescription = "Battery Service"
            };
            var sc = await _service.CreateAsync(dto, "owner6");

            var result = await _service.DeleteForOwnerAsync(sc.ServiceCenterID, "owner6");

            Assert.IsTrue(result);
            Assert.AreEqual(0, _context.ServiceCenters.Count());
        }

        [Test]
        public async Task DeleteForOwnerAsync_ShouldReturnFalse_Negative()
        {
            var result = await _service.DeleteForOwnerAsync("Muk_999", "ownerX");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task DeleteByOwnerAsync_ShouldDeleteAllServiceCenters_Positive()
        {
            var dto1 = new ServiceCenterDTO
            {
                Name = "Center7",
                Location = "Delhi",
                Contact = "6666666666",
                ServiceDescription = "Full Service"
            };
            await _service.CreateAsync(dto1, "owner7");

            var dto2 = new ServiceCenterDTO
            {
                Name = "Center8",
                Location = "Delhi",
                Contact = "7777777777",
                ServiceDescription = "Quick Service"
            };
            await _service.CreateAsync(dto2, "owner7");

            var result = await _service.DeleteByOwnerAsync("owner7");

            Assert.IsTrue(result);
            Assert.AreEqual(0, _context.ServiceCenters.Count());
        }

        [Test]
        public async Task DeleteByOwnerAsync_ShouldReturnFalse_Negative()
        {
            var result = await _service.DeleteByOwnerAsync("unknownOwner");
            Assert.IsFalse(result);
        }
    }
}
