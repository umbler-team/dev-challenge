
# Desafio Umbler

Esta é uma aplicação web que recebe um domínio e mostra suas informações de DNS.
Ex: umbler.com

- Name servers (ns254.umbler.com)
- IP do registro A (177.55.66.99)
- Dados de Whois
- Empresa que está hospedado (Locaweb)

Essas informações são descobertas através de consultas nos servidores DNS e de Whois.
Os dados obtidos são salvos em um banco de dados, evitando uma segunda consulta desnecessariamente, caso seu Ttl ainda não tenha expirado.

Tecnologias Backend utilizadas:

- Asp.Net Core
- C#
- MySql
- Entity Framework

Tecnologias Frontend utilizadas:

- Webpack
- Babel
- ES7

Para rodar o projeto você vai precisar instalar:

- .NET Core SDK (https://www.microsoft.com/net/download/windows  .Net Core 2.0.3 SDK)
- Um editor de código, acoselhado Visual Studio ou VisualStudio Code. (https://code.visualstudio.com/)
- NodeJs para "buildar" o FrontEnd (https://nodejs.org/en/)
- Um banco de dados MySql (crie um gratuitamente no app da Umbler https://app.umbler.com/)

Com as ferramentas devidamente instaladas, basta executar os seguintes comandos:

Para "buildar" o javascript

`npm install`
`npm run build`

Para Rodar o projeto:

`dotnet run` (ou clique em "play" no editor)

# Objetivos:

Se você rodar o projeto e testar um domínio, verá que ele já está funcionando. Porém, queremos melhorar varios pontos deste projeto:

# FrontEnd

 - Os dados retornados não estão formatados, e devem ser apresentados de uma forma legível.
 - Não há validação no frontend permitindo que seja submetido uma requsição inválida para o servidor.

# BackEnd

 - O projeto está rodando em .Net Core 2.0.3, porém é necessário fazer o upgrade para o .Net Core 3.1.0.
 - Não há validação no backend permitindo que um requisição inválida prossiga, o que ocasiona exceptions (erro 500).
 - A complexidade ciclomática do controller está muito alta, o ideal seria utilizar uma arquitetura em camadas.
 - O DomainController está retornando a própria entidade de domínio por JSON, o que faz com que propriedades como Id, Ttl e UpdatedAt sejam mandadas para o cliente web desnecessariamente. Retornar uma ViewModel neste caso seria mais aconselhado.

# Testes

 - A cobertura de testes unitários está muito baixa, e o DomainController está impossível de ser testado pois não há como "mockar" a infraestrutura.
 - O Banco de dados já está sendo "mockado" graças ao InMemoryDataBase do EntityFramework, mas as consultas ao Whois e Dns não. 

