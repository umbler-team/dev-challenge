using Desafio.Umbler.Controllers;
using Desafio.Umbler.Services;
using Desafio.Umbler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

[TestClass]
public class DomainControllerTest
{
    [TestMethod]
    public async Task Domain_Returns_Result()
    {
        var serviceMock = new Mock<DomainService>(null);

        serviceMock
            .Setup(s => s.GetDomain("test.com"))
            .ReturnsAsync(new Domain
            {
                Name = "test.com",
                Ip = "127.0.0.1",
                HostedAt = "umbler"
            });

        var controller = new DomainController(serviceMock.Object);

        var response = await controller.Get("test.com");
        var result = response as OkObjectResult;

        Assert.IsNotNull(result);
    }
}