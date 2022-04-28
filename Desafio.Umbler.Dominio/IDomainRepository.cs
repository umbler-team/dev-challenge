using Desafio.Umbler.Dominio.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desafio.Umbler.Dominio
{
    public interface IDomainRepository
    {
        Task<Domain> GetByNameAsync(string domainName);

        Task AddAsync(Domain domain);
    }
}
