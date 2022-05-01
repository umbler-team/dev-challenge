using Desafio.Umbler.Dominio.Entities;

namespace Desafio.Umbler.Dominio
{
    public interface IDomainService
    {
        Task<Domain> GetAsync(string domainName);
    }
}
