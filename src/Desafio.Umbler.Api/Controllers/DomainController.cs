using Desafio.Umbler.Dominio;
using DnsClient;
using Microsoft.AspNetCore.Mvc;
using Whois.NET;

namespace Desafio.Umbler.Api.Controllers
{
    [Route("api")]
    public class DomainController : Controller
    {
        private readonly IDomainService _domainService;

        public DomainController(IDomainService domainService)
        {
            _domainService = domainService;
        }

        [HttpGet, Route("domain/{domainName}")]
        public async Task<IActionResult> Get(string domainName)
        {
            try
            {
                var domain = await _domainService.GetAsync(domainName);

                return Ok(domain);
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
