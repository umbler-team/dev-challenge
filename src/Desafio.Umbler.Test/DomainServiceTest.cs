using Desafio.Umbler.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Desafio.Umbler.Models;


[TestClass]
public class DomainServiceTest
{
    [TestMethod]
    public async Task Should_Return_Error_When_Domain_Invalid()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: "DomainService_Invalid_Domain")
            .Options;

        using var db = new DatabaseContext(options);
        var service = new DomainService(db);

        await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
            Task.Run(() => service.GetDomain("google")));
    }
}