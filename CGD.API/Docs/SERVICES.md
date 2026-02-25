# Documentação dos Serviços do CGD.APP

Serviços da camada de aplicação fornecidos pelo projeto `CGD.APP`. Cada serviço encapsula regras de negócio, validações e interação com repositórios ou outros serviços. As interfaces estão em `CGD.APP.Services` e as implementações em suas respectivas subpastas.

---

##  Serviço de Autenticação (`IAuthServices` / `AuthServices`)

**Finalidade:** lidar com fluxos de cadastro e autenticação de usuários.

### Métodos

| Método                                     | Entrada         | Retorna         | Descrição                                                                                                                  |
| ------------------------------------------ | --------------- | --------------- | -------------------------------------------------------------------------------------------------------------------------- |
| `SignupAsync(AuthSignupDto authSignupDto)` | DTO de cadastro | `Task`          | Cria uma nova conta de usuário. Executa hash de senha e verifica duplicatas. Lança exceções de validação em caso de falha. |
| `LoginAsync(AuthLoginDto authLoginDto)`    | DTO de login    | `Task<UserDto>` | Verifica credenciais e retorna o perfil do usuário autenticado. Lança erro em credenciais inválidas.                       |

### DTOs usados

- `AuthSignupDto` – email, nome, senha, etc.
- `AuthLoginDto` – email e senha.
- `UserDto` – perfil básico retornado no login.

---

## Serviço de Usuário (`IUserService` / `UserService`)

**Finalidade:** gerenciar usuários da aplicação (CRUD, paginação, totais, dados enriquecidos).

### Responsabilidades principais

- Criar e atualizar usuários calculando corretamente a idade.
- Suportar criação simples (para administradores de grupo) e criação completa (auto‑cadastro).
- Paginar resultados e limitar `pageSize` a [1..100].
- Recuperar usuários que compartilham grupos com um determinado usuário.
- Calcular totais por usuário e saldos gerais.
- Fornecer modelos de usuário enriquecidos (com grupo e despesas) para visualizações administrativas.

### Métodos públicos

| Método                                                             | Descrição                                                                  | Observações importantes                     |
| ------------------------------------------------------------------ | -------------------------------------------------------------------------- | ------------------------------------------- |
| `CreateSimpleAsync(UserSimpleCreateDto dto, Guid adminGroupId)`    | Adiciona usuário e associa automaticamente a um grupo.                     | Calcula idade; requer id do admin do grupo. |
| `CreateAsync(UserCreateDto dto)`                                   | Criação completa com hash de senha.                                        | Valida email único.                         |
| `GetByIdAsync(Guid id)`                                            | Busca usuário ou lança `UserNotFoundException`.                            |
| `GetPagedAsync(int page, int pageSize)`                            | Retorna uma página de usuários. Valida parâmetros.                         |
| `GetPagedByCommonGroupsAsync(Guid userId, int page, int pageSize)` | Usuários que compartilham qualquer grupo com `userId`.                     |
| `UpdateAsync(Guid id, UserUpdateDto dto)`                          | Modifica nome e/ou data de nascimento (recalcula idade).                   |
| `DeleteAsync(Guid id)`                                             | Remove usuário ou lança `UserNotFoundException`.                           |
| `GetUserTotalsAsync()`                                             | Agrega receita/despesa por usuário e total geral.                          |
| `GetAllEnrichedUsers(Guid adminGroupId)`                           | Retorna lista de `EnrichedUserDto` com dados de usuário, grupo e despesas. |

### DTOs & exceções

- Utiliza `UserDto`, `UserSimpleCreateDto`, `UserCreateDto`, `UserUpdateDto`, `UserTotalsResponseDto`, `EnrichedUserDto` etc.
- Lança `UserNotFoundException` e diversas exceções de argumento/validação.

---

## Serviço de Grupo (`IGroupService` / `GroupService`)

**Finalidade:** gerenciar grupos e seus membros.

### Funcionalidades

- Operações CRUD para grupos (nome, criador).
- Listar grupos visíveis para um usuário específico.
- Adicionar ou remover usuários de um grupo; garante existência de ambos.
- Consultar grupos aos quais um usuário pertence.

### Resumo dos métodos

| Método                                                                 | Descrição                                 |
| ---------------------------------------------------------------------- | ----------------------------------------- |
| `GetByIdAsync(Guid id, Guid userId)`                                   | Retorna grupo se `userId` tiver acesso.   |
| `GetAllAsync(Guid userId)`                                             | Todos os grupos visíveis ao usuário.      |
| `CreateAsync(GroupCreateDto dto, Guid userId)`                         | Novo grupo criado por `userId`.           |
| `UpdateAsync(Guid id, string name, Guid userId)`                       | Renomeia grupo; necessita de acesso.      |
| `DeleteAsync(Guid id)`                                                 | Remove grupo por id.                      |
| `AddUserToGroupAsync(Guid groupId, Guid groupAdmin, Guid userToBeAdd)` | Adiciona membro; valida ambos os membros. |
| `RemoveUserFromGroupAsync(Guid groupId, Guid userId)`                  | Remove membro do grupo.                   |
| `GetGroupsByUserIdAsync(Guid userId)`                                  | Grupos onde `userId` é membro.            |

### DTOs

- `GroupDto`, `GroupCreateDto`, `GroupMemberDto`, etc.

---

##  Serviço de Categoria (`IExpenseCategoryService` / `ExpenseCategoryService`)

**Finalidade:** tratar das categorias de despesa/receita pertencentes aos usuários.

### Responsabilidades

- CRUD com paginação e cálculo de totais.
- Garantir limites de página e tamanho da página.
- Calcular totais por categoria e saldo geral usando despesas associadas.

### Métodos

| Método                                                       | Descrição                                                 |
| ------------------------------------------------------------ | --------------------------------------------------------- |
| `CreateAsync(Guid userId, CategoryCreateDto dto)`            | Adiciona categoria para o usuário.                        |
| `GetByIdAsync(Guid id)`                                      | Busca categoria ou lança erro.                            |
| `GetByUserIdAsync(Guid userId)`                              | Todas as categorias de um usuário.                        |
| `GetPagedByUserIdAsync(Guid userId, int page, int pageSize)` | Categoria paginada; valida parâmetros.                    |
| `UpdateAsync(Guid id, CategoryCreateDto dto)`                | Modifica categoria existente.                             |
| `DeleteAsync(Guid id)`                                       | Remove categoria.                                         |
| `GetCategoryTotalsAsync(Guid userId)`                        | Totais agrupados por categoria com saldo receita/despesa. |

### Exceções

- Lança `ArgumentException` ou exceções customizadas para parâmetros inválidos ou entidades não encontradas.

### DTOs de saída

- `CategoryDto`, `CategoryTotalsDto`, `CategoryTotalsResponseDto`.

---

##  Serviço de Despesa (`IExpenseService` / `ExpenseService`)

**Finalidade:** gerenciar transações financeiras, validando regras de negócio.

### Comportamentos principais

- Criação valida:
  - Devedor existe e regras de idade/tipo (menores).
  - Categoria existe, pertence ao usuário e corresponde ao propósito.
- Atualização com validações semelhantes.
- Exclusão e consulta com tratamento de erros.
- Consulta por usuário, filtrada por data/valor/descrição.
- Recupera todas as despesas de membros do grupo de um usuário via `IGroupService`.

### Métodos disponíveis

| Método                                                            | Descrição                              |
| ----------------------------------------------------------------- | -------------------------------------- |
| `CreateAsync(ExpenseCreateDto dto, Guid adminUserId)`             | Adiciona nova despesa/receita.         |
| `GetByIdAsync(Guid id)`                                           | Recupera transação única.              |
| `GetByUserIdAsync(Guid userId)`                                   | Todas as transações de um usuário.     |
| `GetByUserIdAsync(Guid userId, ExpenseFilterDto filter)`          | Lista filtrada.                        |
| `UpdateAsync(Guid expenseId, Guid adminId, ExpenseUpdateDto dto)` | Modifica registro.                     |
| `DeleteAsync(Guid id)`                                            | Remove transação.                      |
| `GetAll(Guid userId)`                                             | Despesas de todos os membros do grupo. |

### Exceções tratadas

- `ExpenseNotFoundException`, `UserNotFoundException`,
  `CategoryNotFoundException`, `InvalidTransactionForMinorException`,
  `InvalidCategoryPurposeException`, etc.

### DTOs

- `ExpenseDto`, `ExpenseCreateDto`, `ExpenseUpdateDto`,
  `ExpenseFilterDto`.

---
