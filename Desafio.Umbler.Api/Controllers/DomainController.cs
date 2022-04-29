using Desafio.Umbler.Dominio;
using Microsoft.AspNetCore.Mvc;

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
            var domain = await _domainService.Get(domainName);

            return Ok(domain);
        }
    }
}
