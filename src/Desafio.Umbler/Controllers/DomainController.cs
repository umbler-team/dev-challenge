using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Desafio.Umbler.Services;
using Desafio.Umbler.Models.ViewModels;

namespace Desafio.Umbler.Controllers
{
    [Route("api/domain")]
    public class DomainController : Controller
    {
        private readonly DomainService _service;

        public DomainController(DomainService service)
        {
            _service = service;
        }

        [HttpGet("{domainName}")]
        public async Task<IActionResult> Get(string domainName)
        {
            try
            {
                var domain = await _service.GetDomain(domainName);

                var result = new DomainResponse
                {
                    Name = domain.Name,
                    Ip = domain.Ip,
                    HostedAt = domain.HostedAt
                };

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}