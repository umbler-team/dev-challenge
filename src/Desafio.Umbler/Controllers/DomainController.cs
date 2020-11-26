using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Desafio.Umbler.Models;
using Whois.NET;
using Microsoft.EntityFrameworkCore;
using DnsClient;
using System.IO;

namespace Desafio.Umbler.Controllers
{
    [Route("api")]
    public class DomainController : Controller
    {
        private readonly DatabaseContext _db;

        public DomainController(DatabaseContext db)
        {
            _db = db;
        }

        [HttpGet, Route("domain/{domainName}")]
        public async Task<IActionResult> Get(string domainName)
        {

            if (String.IsNullOrEmpty(domainName) || !Domain.isValid(domainName)) return BadRequest();

            var domain = new Domain();

            try
            {

                domain = await _db.Domains.FirstOrDefaultAsync(d => d.Name == domainName);

                if (domain == null)
                {
                    var response = await WhoisClient.QueryAsync(domainName);

                    var lookup = new LookupClient();
                    var result = await lookup.QueryAsync(domainName, QueryType.A);
                    var resultNS = await lookup.QueryAsync(domainName, QueryType.NS);

                    domain = new Domain(domainName, result, resultNS, response.Raw);

                    var hostResponse = await WhoisClient.QueryAsync(domain.Ip);

                    domain.HostedAt = hostResponse.OrganizationName;

                    _db.Domains.Add(domain);
                }

                if (DateTime.Now.Subtract(domain.UpdatedAt).TotalMinutes > domain.Ttl)
                {
                    var response = await WhoisClient.QueryAsync(domainName);

                    var lookup = new LookupClient();
                    var result = await lookup.QueryAsync(domainName, QueryType.A);
                    var resultNS = await lookup.QueryAsync(domainName, QueryType.NS);

                    domain = new Domain(domainName, result, resultNS, response.Raw);

                    var hostResponse = await WhoisClient.QueryAsync(domain.Ip);

                    domain.HostedAt = hostResponse.OrganizationName;
                    domain.UpdatedAt = DateTime.Now;
                }

                    await _db.SaveChangesAsync();
             }
            catch (Exception e)
            {   
                throw e;
            }
           
            return Ok( new { Name = domain.Name, Ip = domain.Ip, HostedAt = domain.HostedAt, WhoIs = domain.WhoIs, NsRecords = domain.GetNsList()});
        }
    }
}
