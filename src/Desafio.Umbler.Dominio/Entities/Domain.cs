using System.ComponentModel.DataAnnotations;

namespace Desafio.Umbler.Dominio.Entities
{
    public class Domain
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ip { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string WhoIs { get; set; }
        public int Ttl { get; set; }
        public string HostedAt { get; set; }
    }
}
