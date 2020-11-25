using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Umbler.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
        {

        }

        public DbSet<Domain> Domains { get; set; }
    }

    public class Domain
    {
        public Domain()
        {

        }

        public Domain(string domainName, IDnsQueryResponse result, IDnsQueryResponse resultNs, string whois)
        {
            var NSRecords = resultNs.Answers.NsRecords().ToList();
            var record = result.Answers.ARecords().FirstOrDefault();
            var address = record?.Address;
            var ip = address?.ToString();

            Name = domainName;
            Ip = ip;
            UpdatedAt = DateTime.Now;
            WhoIs = whois;
            Ttl = record?.TimeToLive ?? 0;
            NsList = this.SetNsList(NSRecords);
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string WhoIs { get; set; }
        public int Ttl { get; set; }
        public string HostedAt { get; set; }
        public string NsList{ get; set; }

        public List<string> GetNsList()
        {
            return this.NsList.Split(';').ToList();
        }

        private string SetNsList(List<NsRecord> nsRecords)
        {
            var nsRecordUmounted = string.Empty;
            var lastItem = nsRecords.Last();

            nsRecords.ForEach(nr => {
                nsRecordUmounted += nr.NSDName == lastItem.NSDName ? nr.NSDName : nr.NSDName + ";";
            });

            return nsRecordUmounted;
        }
    }

}