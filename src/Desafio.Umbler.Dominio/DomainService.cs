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

        public async Task<Domain> Get(string domainName)
        {
            var pattern = @"^[a-zA-Z0-9-_]+[.\\]+[a-zA-Z0-9-_]+";
            if(!Regex.IsMatch(domainName, pattern))
            {
                throw new Exception("Domínio inválido");
            }

            var domain = await _domainRepository.GetByNameAsync(domainName);

            if (domain == null)
            {
                var response = await WhoisClient.QueryAsync(domainName);

                var result = await _lookupClient.QueryAsync(domainName, QueryType.ANY);
                var record = result.Answers.ARecords().FirstOrDefault();
                var address = record?.Address;
                var ip = address?.ToString();

                var hostResponse = await WhoisClient.QueryAsync(ip);

                domain = new Domain
                {
                    Name = domainName,
                    Ip = ip,
                    UpdatedAt = DateTime.Now,
                    WhoIs = response.Raw,
                    Ttl = record?.TimeToLive ?? 0,
                    HostedAt = hostResponse.OrganizationName
                };

                await _domainRepository.AddAsync(domain);
            }

            if (DateTime.Now.Subtract(domain.UpdatedAt).TotalMinutes > domain.Ttl)
            {
                var response = await WhoisClient.QueryAsync(domainName);

                var result = await _lookupClient.QueryAsync(domainName, QueryType.ANY);
                var record = result.Answers.ARecords().FirstOrDefault();
                var address = record?.Address;
                var ip = address?.ToString();

                var hostResponse = await WhoisClient.QueryAsync(ip);

                domain.Name = domainName;
                domain.Ip = ip;
                domain.UpdatedAt = DateTime.Now;
                domain.WhoIs = response.Raw;
                domain.Ttl = record?.TimeToLive ?? 0;
                domain.HostedAt = hostResponse.OrganizationName;
            }

            return domain;
        }
    }
}
