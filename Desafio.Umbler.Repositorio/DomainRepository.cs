using Desafio.Umbler.Dominio;
using Desafio.Umbler.Dominio.Entities;
using Desafio.Umbler.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desafio.Umbler.Repositorio
{
    public class DomainRepository : IDomainRepository
    {
        private readonly DatabaseContext _db;

        public DomainRepository(DatabaseContext db)
        {
            _db = db;
        }

        public Task AddAsync(Domain domain)
        {
            _db.Domains.Add(domain); 

            return Task.CompletedTask;
        }

        public async Task<Domain> GetByNameAsync(string domainName)
        {
            var domain = await _db.Domains.FirstOrDefaultAsync(d => d.Name == domainName);
            return domain;
        }
    }
}
