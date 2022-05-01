using Desafio.Umbler.Dominio.Entities;
using DnsClient;
using System.Text.RegularExpressions;
using Whois.NET;

namespace Desafio.Umbler.Dominio
{
    public class DomainService : IDomainService
    {
        private readonly IDomainRepository _domainRepository;
        private readonly ILookupClient _lookupClient;

        public DomainService(IDomainRepository domainRepository, ILookupClient lookupClient)
        {
            _domainRepository = domainRepository;
            _lookupClient = lookupClient;
        }

        public async Task<Domain> GetAsync(string domainName)
        {
            ValidarDominio(domainName);

            var domain = await _domainRepository.GetByNameAsync(domainName);

            if (domain == null || DeveAtualizarDominio(domain))
            {
                domain = await BuscarDominio(domainName);
                await _domainRepository.AddOrUpdateAsync(domain);
            }

            return domain;
        }

        private static void ValidarDominio(string domainName)
        {
            var pattern = @"^[a-zA-Z0-9-_]+[.\\]+[a-zA-Z0-9-_]+";
            if (!Regex.IsMatch(domainName, pattern))
            {
                throw new ArgumentException("Domínio inválido");
            }
        }

        private static bool DeveAtualizarDominio(Domain domain)
        {
            return DateTime.Now.Subtract(domain.UpdatedAt).TotalMinutes > domain.Ttl;
        }

        private async Task<Domain> BuscarDominio(string domainName)
        {
            var response = await WhoisClient.QueryAsync(domainName);

            var result = await _lookupClient.QueryAsync(domainName, QueryType.ANY);
            var record = result.Answers.ARecords().FirstOrDefault();
            var address = record?.Address;
            var ip = address?.ToString();

            var hostResponse = await WhoisClient.QueryAsync(ip);

            var domain = new Domain
            {
                Name = domainName,
                Ip = ip,
                UpdatedAt = DateTime.Now,
                WhoIs = response.Raw,
                Ttl = record?.TimeToLive ?? 0,
                HostedAt = hostResponse.OrganizationName
            };
            
            return domain;
        }
    }
}
