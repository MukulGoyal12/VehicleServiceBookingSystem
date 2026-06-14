using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Moq;
using OwnerService.Services;
using OwnerService.Models;
using OwnerService.DTO;
using OwnerService.Helpers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceCenterService.DTO;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace VehicleBookingSystem.Tests
{
    public class OwnerServiceTest
    {
        private OwnerContext _context;
        private OwnerServiceImpl _service;
        private Mock<IHttpClientFactory> _httpClientFactory;
        private Mock<ILogger<OwnerServiceImpl>> _logger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<OwnerContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new OwnerContext(options);
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _logger = new Mock<ILogger<OwnerServiceImpl>>();

            _service = new OwnerServiceImpl(_context, _httpClientFactory.Object, _logger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void RegisterOwner_ShouldAddNewOwner_Positive()
        {
            var dto = new OwnerDTO { Name = "Test", Email = "test@test.com", Phone = "123", Password = "pass" };
            var result = _service.RegisterOwner(dto);

            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual("Owner Register Successfully", result.Message);
            Assert.IsNotNull(result.Data); // OwnerId returned
            Assert.IsTrue(_context.Owners.Any(o => o.Email == "test@test.com"));
        }

        [Test]
        public void RegisterOwner_ShouldRejectDuplicate_Negative()
        {
            var dto = new OwnerDTO { Name = "Test", Email = "dup@test.com", Phone = "123", Password = "pass" };
            _service.RegisterOwner(dto);

            var result = _service.RegisterOwner(dto);

            Assert.AreEqual("Failed", result.Status);
            Assert.AreEqual("Owner Already Exists", result.Message);
        }

        [Test]
        public void Login_ShouldFailWithWrongPassword_Negative()
        {
            var dto = new OwnerDTO { Name = "Test", Email = "wrong@test.com", Phone = "123", Password = "pass" };
            _service.RegisterOwner(dto);

            var login = new LoginModel { Email = "wrong@test.com", Password = "bad" };
            var result = _service.Login(login);

            Assert.AreEqual("Failed", result.Status);
            Assert.AreEqual("Invalid credentials", result.Message);
            Assert.IsNull(result.Data); // token not returned
        }

        [Test]
        public void GetOwnerById_ShouldReturnOwner_Positive()
        {
            var dto = new OwnerDTO { Name = "Test", Email = "get@test.com", Phone = "123", Password = "pass" };
            _service.RegisterOwner(dto);
            var owner = _context.Owners.First();

            var result = _service.GetOwnerById(owner.OwnerId);

            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual("Owner fetched successfully", result.Message);
            Assert.AreEqual("Test", result.Data.Name);
        }

        [Test]
        public void GetOwnerById_ShouldReturnFailed_Negative()
        {
            var result = _service.GetOwnerById("Muk_999");

            Assert.AreEqual("Failed", result.Status);
            Assert.AreEqual("Owner not found or inactive", result.Message);
            Assert.IsNull(result.Data);
        }

        [Test]
        public void UpdateOwner_ShouldUpdateFields_Positive()
        {
            var dto = new OwnerDTO { Name = "Test", Email = "update@test.com", Phone = "123", Password = "pass" };
            _service.RegisterOwner(dto);
            var owner = _context.Owners.First();

            var updated = new OwnerDTO { Name = "UpdatedName" };
            var result = _service.UpdateOwner(owner.OwnerId, updated);

            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual("Owner updated successfully", result.Message);
            Assert.AreEqual("UpdatedName", _context.Owners.First().Name);
        }

        [Test]
        public void UpdateOwner_ShouldReturnNotFound_Negative()
        {
            var updated = new OwnerDTO { Name = "UpdatedName" };
            var result = _service.UpdateOwner("Muk_999", updated);

            Assert.AreEqual("Failed", result.Status);
            Assert.AreEqual("Owner not found", result.Message);
        }

        [Test]
        public async Task DeleteOwner_ShouldDeleteOwner_Positive()
        {
            var dto = new OwnerDTO { Name = "Test", Email = "delete@test.com", Phone = "123", Password = "pass" };
            _service.RegisterOwner(dto);
            var owner = _context.Owners.First();

            // Mock HttpClient response
            var client = new HttpClient(new FakeHttpMessageHandler(System.Net.HttpStatusCode.OK));
            _httpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

            var result = await _service.DeleteOwner(owner.OwnerId);

            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual("Owner and linked ServiceCenters deleted successfully", result.Message);
            Assert.AreEqual(owner.OwnerId, result.Data);
        }

        [Test]
        public async Task DeleteOwner_ShouldReturnNotFound_Negative()
        {
            var result = await _service.DeleteOwner("Muk_999");

            Assert.AreEqual("Failed", result.Status);
            Assert.AreEqual("Owner not found", result.Message);
        }

        [Test]
        public void AddServiceCenter_ShouldLinkServiceCenter_Positive()
        {
            var dto = new OwnerDTO { Name = "Test", Email = "sc@test.com", Phone = "123", Password = "pass" };
            _service.RegisterOwner(dto);
            var owner = _context.Owners.First();

            var payload = new AddServiceCenterDTO { OwnerId = owner.OwnerId, ServiceCenterId = "SC_101" };
            var result = _service.AddServiceCenter(payload);

            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual("ServiceCenter linked to Owner successfully", result.Message);
            Assert.AreEqual("SC_101", result.Data);
            Assert.Contains("SC_101", (System.Collections.ICollection?)owner.ServiceCenterIds);
        }

        [Test]
        public void RemoveServiceCenter_ShouldUnlinkServiceCenter_Positive()
        {
            var dto = new OwnerDTO { Name = "Test", Email = "sc2@test.com", Phone = "123", Password = "pass" };
            _service.RegisterOwner(dto);
            var owner = _context.Owners.First();

            owner.ServiceCenterIds.Add("SC_202");
            _context.SaveChanges();

            var payload = new AddServiceCenterDTO { OwnerId = owner.OwnerId, ServiceCenterId = "SC_202" };
            var result = _service.RemoveServiceCenter(payload);

            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual("ServiceCenter removed from Owner successfully", result.Message);
            Assert.AreEqual("SC_202", result.Data);
            Assert.IsFalse(owner.ServiceCenterIds.Contains("SC_202"));
        }
    }

    // Fake HttpMessageHandler for mocking HttpClient
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly System.Net.HttpStatusCode _statusCode;

        public FakeHttpMessageHandler(System.Net.HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(_statusCode));
        }
    }
}