using Desafio.Umbler.Dominio.Entities;

namespace Desafio.Umbler.Dominio
{
    public interface IDomainRepository
    {
        Task<Domain> GetByNameAsync(string domainName);

        Task AddOrUpdateAsync(Domain domain);
    }
}
