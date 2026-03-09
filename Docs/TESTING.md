# Estratégia de Testes

O sistema foi estruturado desde o início com testabilidade em mente. A Clean Architecture facilita a criação de testes isolados para cada camada.

## Projetos de teste

A solução inclui três projetos específicos:

- **CGD.Domain.Tests**
  - Contém testes de unidade das entidades do domínio.
  - Validações de `DataAnnotations`, invariantes e enums.

- **CGD.APP.Tests**
  - Reúne testes dos serviços de aplicação (casos de uso).
  - Utiliza **Moq** para simular repositórios, permitindo verificar comportamentos e lançamento de exceções sem depender da infraestrutura.

- **CGD.API.Tests**
  - Abriga testes de unidade de controllers.
  - Cada rota está coberta com pelo menos um caso "feliz" e exames de branches (modelo inválido, recurso não encontrado, ausência de claim, etc.).
  - Helpers reutilizáveis (`ControllerTestHelpers.CreateWithUser`, `ControllerTestHelpers.AddModelError`) reduzem boilerplate ao construir `ControllerContext` e manipular `ModelState`.
  - Métodos dos controllers são chamados diretamente com objetos `ControllerContext` configurados; o serviço subjacente é mocado.
  - Pode evoluir para testes de integração utilizando `WebApplicationFactory` e um banco em memória.

## Ferramentas e convenções

- **Framework de testes:** [xUnit](https://xunit.net/)
- **Mocking:** [Moq](https://github.com/moq/moq4)
- **Asserts legíveis:** [FluentAssertions](https://fluentassertions.com/)
- **Nomenclatura:** `Metodo_Should_Resultado_When_Condicao` ou `Should_Resultado_When_Condicao`.

Exemplo:

```csharp
[Fact]
public async Task CreateAsync_ThrowsUserNotFound_WhenDebtorMissing()
{
    // arrange
    ...
}
```

## Executando testes

No diretório raiz da solução basta rodar:

```bash
dotnet test
```

O comando compila (se necessário) e executa todos os testes dos três projetos.
