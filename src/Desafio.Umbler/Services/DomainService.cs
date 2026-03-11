using System;
using System.Linq;
using System.Threading.Tasks;
using Desafio.Umbler.Models;
using Microsoft.EntityFrameworkCore;
using Whois.NET;
using DnsClient;

namespace Desafio.Umbler.Services
{
    public class DomainService
    {
        private readonly DatabaseContext _db;

        public DomainService(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<Domain> GetDomain(string domainName)
        {
            if (string.IsNullOrWhiteSpace(domainName) || !domainName.Contains("."))
                throw new ArgumentException("Domínio inválido");

            var domain = await _db.Domains.FirstOrDefaultAsync(d => d.Name == domainName);

            if (domain != null)
                return domain;

            var whois = await WhoisClient.QueryAsync(domainName);

            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(domainName, QueryType.A);
            var record = result.Answers.ARecords().FirstOrDefault();

            var ip = record?.Address?.ToString();

            var hostResponse = await WhoisClient.QueryAsync(ip);

            domain = new Domain
            {
                Name = domainName,
                Ip = ip,
                WhoIs = whois.Raw,
                HostedAt = hostResponse.OrganizationName,
                Ttl = record?.TimeToLive ?? 0,
                UpdatedAt = DateTime.Now
            };

            _db.Domains.Add(domain);
            await _db.SaveChangesAsync();

            return domain;
        }
    }
}