using Desafio.Umbler.Api.Controllers;
using Desafio.Umbler.Api.Views;
using Desafio.Umbler.Dominio;
using Desafio.Umbler.Dominio.Dto;
using Desafio.Umbler.Dominio.Entities;
using Desafio.Umbler.Repositorio;
using Desafio.Umbler.Repositorio.Models;
using DnsClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using Whois.NET;

namespace Desafio.Umbler.Test
{
    [TestClass]
    public class ControllersTest
    {
        [TestMethod]
        public void Home_Index_returns_View()
        {
            //arrange 
            var controller = new HomeController();

            //act
            var response = controller.Index();
            var result = response as ViewResult;

            //assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Home_Error_returns_View_With_Model()
        {
            //arrange 
            var controller = new HomeController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            //act
            var response = controller.Error();
            var result = response as ViewResult;
            var model = result.Model as ErrorViewModel;

            //assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void Domain_In_Database()
        {
            //arrange 
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Find_searches_url")
                .Options;

            var domain = new Domain { Id = 1, Ip = "192.168.0.1", Name = "test.com", UpdatedAt = DateTime.Now, HostedAt = "umbler.corp", Ttl = 60, WhoIs = "Ns.umbler.com" };

            // Insert seed data into the database using one instance of the context
            using (var db = new DatabaseContext(options))
            {
                db.Domains.Add(domain);
                db.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            var domainServiceMock = new Mock<IDomainService>();
            domainServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).Returns(Task.FromResult(new DomainDto { Ip = domain.Ip, Name = domain.Name, HostedAt = domain.HostedAt, WhoIs = domain.WhoIs }));

            var controller = new DomainController(domainServiceMock.Object);

            //act
            var response = controller.Get("test.com");
            var result = response.Result as OkObjectResult;
            var obj = result.Value as Domain;
            Assert.AreEqual(obj.Id, domain.Id);
            Assert.AreEqual(obj.Ip, domain.Ip);
            Assert.AreEqual(obj.Name, domain.Name);
        }

        [TestMethod]
        public void Domain_Not_In_Database()
        {
            //arrange 
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Find_searches_url")
                .Options;

            // Use a clean instance of the context to run the test
            var domainServiceMock = new Mock<IDomainService>();
            domainServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).Returns(Task.FromResult(new DomainDto()));

            var controller = new DomainController(domainServiceMock.Object);

            //act
            var response = controller.Get("test.com");
            var result = response.Result as OkObjectResult;
            var obj = result.Value as Domain;
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void Domain_Moking_LookupClient()
        {
            //arrange 
            var lookupClient = new Mock<ILookupClient>();
            var domainName = "test.com";

            var dnsResponse = new Mock<IDnsQueryResponse>();
            lookupClient.Setup(l => l.QueryAsync(domainName, QueryType.ANY, QueryClass.IN, System.Threading.CancellationToken.None)).ReturnsAsync(dnsResponse.Object);

            //arrange 
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Find_searches_url")
                .Options;

            // Use a clean instance of the context to run the test
            var domainServiceMock = new Mock<IDomainService>();
            domainServiceMock.Setup(x => x.GetAsync(It.IsAny<string>())).Returns(Task.FromResult(new DomainDto()));

            var controller = new DomainController(domainServiceMock.Object);
            //inject lookupClient in controller constructor
            //var controller = new DomainController(db/*,IWhoisClient, lookupClient*/ );

            //act
            var response = controller.Get("test.com");
            var result = response.Result as OkObjectResult;
            var obj = result.Value as Domain;
            Assert.IsNotNull(obj);
        }

        //[TestMethod]
        //public void Domain_Moking_WhoisClient()
        //{
        //    //arrange
        //    //whois is a static class, we need to create a class to "wrapper" in a mockable version of WhoisClient
        //    //var whoisClient = new Mock<IWhoisClient>();
        //    var domainName = "test.com";

        //    //whoisClient.Setup(l => l.QueryAsync(domainName)).Returns(Task.FromResult(new WhoisResponse()));

        //    //arrange 
        //    var options = new DbContextOptionsBuilder<DatabaseContext>()
        //        .UseInMemoryDatabase(databaseName: "Find_searches_url")
        //        .Options;

        //    var db = new DatabaseContext(options);

        //    var DomainService = new DomainService(new DomainRepository(db), new LookupClient());

        //    // Use a clean instance of the context to run the test
        //    //using (var db = new DatabaseContext(options))
        //    //{
        //    //inject IWhoisClient in controller's constructor
        //    var controller = new DomainController(DomainService/*db,IWhoisClient, ILookupClient*/);

        //    //act
        //    var response = controller.Get("test.com");
        //    var result = response.Result as OkObjectResult;
        //    var obj = result.Value as Domain;
        //    Assert.IsNotNull(obj);
        //    //}
        //}
    }
}