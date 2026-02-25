# Arquitetura

Este documento descreve as escolhas de arquitetura adotadas no projeto **ControleDeGastos**. O objetivo é fornecer uma visão clara de como o sistema está organizado, quais padrões foram aplicados e por que tais decisões foram tomadas.

---

## Visão Geral

O sistema foi desenhado para ser modular, testável e fácil de evoluir. Para atingir esses objetivos escolhemos a **Clean Architecture** como estilo arquitetural e aplicamos os princípios de **Domain-Driven Design (DDD)** no design das entidades e serviços.

Essa combinação permite separar em camadas bem definidas as regras de negócio, a lógica de aplicação e a infraestrutura, mantendo o domínio livre de dependências externas e facilitando o desenvolvimento orientado a testes e a troca de tecnologias.

---

## Principais Camadas da Clean Architecture

1. **Domain (CGD.Domain):**
   - Contém as entidades, objetos de valor, agregados e interfaces de repositório.
   - Não depende de nenhum outro projeto; representa o coração da aplicação.
   - As regras de negócio mais importantes residem aqui.

2. **Application / Services (CGD.APP):**
   - Implementa casos de uso e orquestra as regras de negócio do domínio.
   - Define os DTOs que atravessam as fronteiras entre camadas.
   - Inclui mapeadores para converter entre entidades do domínio e DTOs.
   - Depende apenas do projeto de domínio e de abstrações para repositórios/serviços externos.

3. **Infrastructure (CGD.Infra):**
   - Contém implementações concretas de repositórios, contexto de dados e integrações com bancos, serviços externos, etc.
   - É onde reside a implementação de Entity Framework Core, migrações e acesso a dados.
   - Depende de projetos de domínio e, se necessário, de outros pacotes de infraestrutura.

4. **Cross-cuting (CGD.CrossCutting):**
   - Recursos compartilhados como exceções personalizadas, configurações de segurança e injeção de dependências.
   - Permite evitar duplicação entre as camadas superiores.

5. **API / Presentation (CGD.API):**
   - Exposição HTTP dos casos de uso utilizando ASP.NET Core.
   - Configuração de middlewares (exceção, autenticação JWT) e definição de controllers.
   - Mapeia as chamadas HTTP para os serviços da camada de aplicação.

---

## Domain-Driven Design (DDD)

Adotamos DDD como guia para modelar o domínio financeiro do sistema. Algumas decisões importantes:

- **Entidades de primeiro nível:** `User`, `Category`, `Expense`, `Group` e `GroupMember` representam agregados com invariantes próprias.
- **Value Objects e Filtros:** Filtros de pesquisa e objetos de valor encapsulam comportamentos que não devem ser modificados diretamente.
- **Exceções específicas:** Exceções como `UserNotFoundException` e `InvalidTransactionForMinorException` são definidas no projeto `CGD.CrossCutting` para apoiar a validação de regras de negócio.

O foco do DDD no código é manter as regras de negócio perto das entidades e evitar que a lógica se espalhe por outras camadas.

---

## Racional das Decisões

- **Separação de Responsabilidades:** ao isolar o domínio, mudançãs na infraestrutura (p.ex. troca de banco de dados) não afetam a lógica principal.
- **Testabilidade:** cada camada pode ser testada isoladamente. A camada de domínio e de aplicação são totalmente desacopladas.
- **Evolução e Manutenibilidade:** novas funcionalidades podem ser adicionadas implementando novos casos de uso sem impactar o core.
- **Organização do Código:** o workspace reflete essa estrutura, facilitando a navegação e a colaboração entre equipes.

> Para detalhes sobre a estratégia de testes e instruções de execução, confira o arquivo [`Docs/TESTING.md`](TESTING.md).

---
