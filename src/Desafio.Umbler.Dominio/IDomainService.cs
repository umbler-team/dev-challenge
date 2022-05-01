using Desafio.Umbler.Dominio.Entities;

namespace Desafio.Umbler.Dominio
{
    public interface IDomainService
    {
        Task<Domain> Get(string domainName);
    }
}
