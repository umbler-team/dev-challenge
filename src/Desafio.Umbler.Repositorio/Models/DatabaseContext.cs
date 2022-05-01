using Desafio.Umbler.Dominio.Entities;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Umbler.Repositorio.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
        {

        }

        public DbSet<Domain> Domains { get; set; }
    }
}