using System.Threading.Tasks;
using Desafio.Umbler.Models;

public interface IDomainService
{
    Task<Domain> GetDomain(string domainName);
}