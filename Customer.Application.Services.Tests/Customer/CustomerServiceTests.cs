using System.Collections.Generic;
using System.Threading.Tasks;
using Customer.Application.Abstractions;
using Customer.Application.Abstractions.Configuration;
using Customer.Domain.Models;
using Customer.Domain.Repositories;
using Customer.WebApi.Services.Customer;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Customer.Application.Services.Tests.Customer
{
    public class CustomerServiceTests
    {
        private CustomerService _customerService;
        private Mock<IOptionsSnapshot<PostLtOptions>> _postLtOptions;
        private Mock<ICustomerRepository> _customerRepository;
        private Mock<IDataReader> _dataReader;
        private Mock<IPostLtClient> _postLtClient;

        [SetUp]
        public void Setup()
        {
            _postLtOptions = new Mock<IOptionsSnapshot<PostLtOptions>>();
            _postLtOptions.Setup(x => x.Value).Returns(new PostLtOptions());
            _dataReader = new Mock<IDataReader>();
            _postLtClient = new Mock<IPostLtClient>();
            _customerRepository = new Mock<ICustomerRepository>();
            var getCustomersResult = new List<CustomerEntity>
            {
                new CustomerEntity
                {
                    Id = 1,
                    Name = "Test name",
                    Address = "Test address"
                }
            };

            _customerRepository.Setup(x => x.GetCustomersAsync()).ReturnsAsync(getCustomersResult);
            _customerService = new CustomerService(
                _postLtOptions.Object,
                _dataReader.Object,
                _postLtClient.Object,
                _customerRepository.Object);
        }

        [Test]
        public async Task GetCustomersTests()
        {
            //Act
            var getCustomersResult = await _customerService.GetCustomersAsync();

            //Assert
            Assert.That(getCustomersResult, Is.Not.Null);
            Assert.That(getCustomersResult.Count == 1, Is.True);
        }
    }
}
