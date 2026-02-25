# Controle de Gastos

Aplicação para gestão financeira colaborativa: registre despesas por grupos, acompanhe saldos e débitos entre membros.

## Sobre o Projeto

Este projeto foi desenvolvido para demonstrar minhas habilidades como desenvolvedor C# e dotnet, apresentando uma API RESTful que permite o controle de gastos em grupos de usuários com gerenciamento de saldos, débitos e autenticação via JWT.

## Funcionalidades

- Cadastro e autenticação de usuários
- Criação e gerenciamento de grupos de despesas
- Registro de categorias e despesas vinculadas a grupos
- Distribuição automática de valores entre membros
- Consultas de saldos e débitos por usuário
- Documentação integrada e testes automatizados

## Tecnologias

- .NET 8 / C#
- .NET
- Entity Framework com PostgreSQL
- JWT para segurança
- xUnit + Moq para testes unitários

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download)
- PostgreSQL
- Docker

## Instalação e Uso

1. Clone o repositório:
   ```bash
   git clone https://github.com/EricksonLOOP/ControleDeGastos
   cd .\ControleDeGastos
   ```
2. Restaure dependências e compile:
   ```bash
   docker compose build
   docker compose up
   ```

6. Acesse `https://localhost:8080` para testar endpoints.

## Configuração

As principais configurações ficam em `appsettings.json` / `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=cgd_db;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Issuer": "ControleDeGastosAPI",
    "Audience": "ControleDeGastosFrontend",
    "SecretKey": "dev-secret-key-12345678901234567890123456789012",
    "ExpirationMinutes": 60
  }
}
```

Altere conforme necessário e mantenha a chave secreta segura.

## Executando Testes

O projeto inclui suítes de testes para API, domínio e serviços.

No diretório do solution:

```bash
dotnet test
```

Ele irá rodar todos os projetos de teste (`CGD.API.Tests`, `CGD.APP.Tests`, `CGD.Domain.Tests`).

## Documentação

A pasta `Docs` contém arquivos detalhados:

| Nome          | Descrição                                   | Acesso Rápido                  |
| ------------- | ------------------------------------------- | ------------------------------ |
| Arquitetura   | Informações sobre as decisões arquiteturais | [Ler agora](./ARCHITECTURE.md) |
| Controladores | Descrição dos endpoints                     | [Ler agora](./Controllers.md)  |
| Serviços      | Regras de negócio e injeção                 | [Ler agora](./SERVICES.md)     |
| Testes        | Estratégia de testes e exemplos             | [Ler agora](./TESTING.md)      |

## Autor

- Dev: [Erickson Dias](https://github.com/EricksonLOOP)
- LinkedIn: [Erickson Augusto](https://linkedin.com/in/erickson-augusto)
- Portfólio: [Oppo](https://oppodev.site)
