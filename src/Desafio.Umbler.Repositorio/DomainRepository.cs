using Desafio.Umbler.Dominio;
using Desafio.Umbler.Dominio.Entities;
using Desafio.Umbler.Repositorio.Models;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Umbler.Repositorio
{
    public class DomainRepository : IDomainRepository
    {
        private readonly DatabaseContext _db;

        public DomainRepository(DatabaseContext db)
        {
            _db = db;
        }

        public async Task AddOrUpdateAsync(Domain domain)
        {
            var domainExist = await GetByNameAsync(domain.Name);

            if (domainExist != null)
            {
                _db.Domains.Update(domainExist);
            }
            else
            {
                await _db.Domains.AddAsync(domain); 
            }

            await _db.SaveChangesAsync();
        }

        public async Task<Domain> GetByNameAsync(string domainName)
        {
            var domain = await _db.Domains.FirstOrDefaultAsync(d => d.Name == domainName);
            return domain;
        }
    }
}
