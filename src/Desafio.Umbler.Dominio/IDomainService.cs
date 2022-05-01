using Desafio.Umbler.Dominio.Dto;

namespace Desafio.Umbler.Dominio
{
    public interface IDomainService
    {
        Task<DomainDto> Get(string domainName);
    }
}
