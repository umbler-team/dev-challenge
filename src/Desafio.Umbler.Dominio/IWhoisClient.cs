using Whois.NET;

namespace Desafio.Umbler.Dominio
{
    public interface IWhoisClient
    {
        Task<WhoisResponse> QueryAsync(string query);
    }
}
