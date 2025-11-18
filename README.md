# E-Commerce System

Um sistema de e-commerce completo construído com .NET 9, seguindo a arquitetura limpa (Clean Architecture). O sistema oferece funcionalidades de autenticação, gestão de produtos, criação e acompanhamento de pedidos.

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture**, dividido em camadas:

- **ECommerceSystem.Core**: Entidades de domínio e interfaces
- **ECommerceSystem.Application**: Casos de uso, comandos, queries e DTOs
- **ECommerceSystem.Infrastructure**: Implementações de infraestrutura (EF Core, repositórios)
- **ECommerceSystem.WebApi**: API REST com controllers
- **ECommerceSystem.Tests**: Testes unitários e de integração com NUnit

### Padrões Implementados

- **Repository Pattern**: Abstração do acesso a dados
- **Specification Pattern**: Consultas flexíveis e reutilizáveis
- **Pagination**: Paginação eficiente para grandes volumes de dados
- **CQRS**: Separação entre comandos e queries usando MediatR
- **Unit of Work**: Gerenciamento de transações

## 🚀 Funcionalidades

### 1. Autenticação de Usuários
- Registro de usuários (clientes e administradores)
- Login com JWT
- Controle de permissões baseado em roles

### 2. Gestão de Produtos (Administradores)
- Criar, atualizar, listar e excluir produtos
- Controle de estoque automático
- Produtos ativos/inativos

### 3. Pedidos/Carrinho
- Adicionar produtos ao carrinho
- Atualização automática do estoque
- Cálculo automático do total

### 4. Pagamento
- Simulação de pagamento via PIX ou cartão
- Confirmação de pedido após pagamento

### 5. Histórico e Acompanhamento de Pedidos
- Visualização de pedidos por usuário
- Acompanhamento do status dos pedidos
- Histórico completo de compras

## 🛠️ Tecnologias Utilizadas

- **.NET 9** - Framework principal
- **ASP.NET Core Web API** - API REST
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados
- **Docker** - Containerização
- **Identity** - Autenticação e autorização
- **JWT** - Tokens de autenticação
- **MediatR** - Padrão CQRS
- **AutoMapper** - Mapeamento de objetos
- **Swagger** - Documentação da API

## 📋 Pré-requisitos

- .NET 9 SDK
- Docker e Docker Compose
- Visual Studio 2022 ou VS Code

## 🚀 Como Executar

### Opção 1: Usando Docker (Recomendado)

1. **Clone o repositório**
   ```bash
   git clone <repository-url>
   cd ECommerceSystem
   ```

2. **Execute com Docker Compose**
   ```bash
   docker-compose up --build
   ```

3. **Acesse a documentação da API**
   - Swagger UI: `http://localhost:8080/swagger`
   - API Base: `http://localhost:8080/api`

### Opção 2: Desenvolvimento Local

1. **Clone o repositório**
   ```bash
   git clone <repository-url>
   cd ECommerceSystem
   ```

2. **Restaure os pacotes**
   ```bash
   dotnet restore
   ```

3. **Execute as migrações do banco de dados**
   ```bash
   cd ECommerceSystem.WebApi
   dotnet ef database update
   ```

4. **Execute a aplicação**
   ```bash
   dotnet run
   ```

5. **Acesse a documentação da API**
   - Swagger UI: `http://localhost:5000/swagger`
   - API Base: `http://localhost:5000/api`

## 🔧 Configuração

### Connection String
Configure a string de conexão no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ECommerceDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### JWT Configuration
Configure as chaves JWT no `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyHere12345678901234567890",
    "Issuer": "ECommerceSystem",
    "Audience": "ECommerceUsers"
  }
}
```

## 📚 Endpoints da API

### Autenticação
- `POST /api/auth/register` - Registrar usuário
- `POST /api/auth/login` - Login
- `POST /api/auth/forgot-password` - Solicitar redefinição de senha
- `POST /api/auth/reset-password` - Redefinir a senha com token

### Produtos (Admin)
- `GET /api/products` - Listar produtos (com paginação e filtros)
- `GET /api/products/{id}` - Obter produto por ID
- `POST /api/products` - Criar produto
- `PUT /api/products/{id}` - Atualizar produto
- `DELETE /api/products/{id}` - Excluir produto

### Pedidos
- `GET /api/orders` - Listar pedidos do usuário (com paginação e filtros)
- `GET /api/orders/{id}` - Obter pedido por ID
- `POST /api/orders` - Criar pedido
- `PUT /api/orders/{id}/status` - Atualizar status (Admin)

## 🗃️ Estrutura do Banco de Dados

### Tabelas Principais
- **Users**: Usuários do sistema
- **Products**: Catálogo de produtos
- **Orders**: Pedidos realizados
- **OrderItems**: Itens dos pedidos

### Status dos Pedidos
- Pending
- Confirmed
- Shipped
- Delivered
- Cancelled

## 🔐 Autorização

### Roles
- **Admin**: Acesso completo ao sistema
- **Customer**: Acesso aos próprios pedidos e produtos

### Role seeding (criação automática)

Ao iniciar a aplicação, o sistema garante que as roles necessárias existam no banco de dados. Isso é feito de forma idempotente durante o startup da API — as roles `Admin` e `Customer` são criadas automaticamente se estiverem ausentes.

Como criar um usuário administrador inicial (exemplo):

1. Faça um POST para o endpoint de registro:

```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "P@ssw0rd",
  "firstName": "Admin",
  "lastName": "User",
  "isAdmin": true
}
```

2. A API irá atribuir automaticamente a role `Admin` ao usuário quando `isAdmin` for `true`.

Observação: Em ambientes de produção, considere criar um processo seguro para provisionar o primeiro administrador (ex.: script seguro, variável de ambiente, ou procedimento de criação manual com acesso controlado).

### Políticas
- `AdminOnly`: Apenas administradores
- `CustomerOnly`: Apenas clientes

### Initial admin (criação automática opcional)

Você pode configurar a criação automática de um usuário administrador na inicialização da API (útil em desenvolvimento). Para ativar, defina as seguintes chaves em `appsettings.json` ou variáveis de ambiente:

- `InitialAdmin:Enabled` (boolean) — habilita a criação automática
- `InitialAdmin:Email` (string) — email do administrador inicial
- `InitialAdmin:Password` (string) — senha do administrador inicial

Exemplo `appsettings.Development.json`:

```json
{
  "InitialAdmin": {
    "Enabled": true,
    "Email": "admin@example.com",
    "Password": "P@ssw0rd"
  }
}
```

O comportamento é idempotente: se já existir um usuário com a role `Admin`, nenhum usuário adicional será criado.

Observação: não habilite a criação automática em produção sem um processo seguro para armazenar credenciais.

## 🧪 Testes

### Testes Unitários
- **ProductHandlersTests**: Testa os handlers de produtos
  - Criação, atualização, exclusão e consulta de produtos
  - Validação de regras de negócio
- **OrderHandlersTests**: Testa os handlers de pedidos
  - Criação de pedidos com validação de estoque
  - Atualização de status de pedidos
  - Tratamento de erros (produto não encontrado, estoque insuficiente)

### Testes de Integração
- **ECommerceDbContextTests**: Testa operações do Entity Framework
  - CRUD de produtos e pedidos
  - Relacionamentos entre entidades
  - Consultas complexas
- **RepositoryTests**: Testa os repositórios
  - Operações de banco de dados
  - Consultas específicas (produtos ativos, pedidos por usuário)

### Tecnologias de Teste
- **NUnit**: Framework de testes
- **Moq**: Para mocking de dependências
- **FluentAssertions**: Para assertions mais legíveis
- **InMemory Database**: Para testes de integração sem banco real

Para executar os testes:
```bash
dotnet test
```

### Testes Unitários
```bash
dotnet test --filter "UnitTests"
```

### Testes de Integração
```bash
dotnet test --filter "IntegrationTests"
```

### Todos os Testes
```bash
dotnet test
```

### Cobertura de Testes
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info
```

## 📦 Implantação

1. Configure as variáveis de ambiente para produção
2. Execute as migrações no servidor de produção
3. Configure o IIS ou container Docker
4. Atualize as configurações de CORS se necessário

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 📞 Suporte

Para suporte, entre em contato com a equipe de desenvolvimento ou abra uma issue no repositório.

---

**Nota**: Este é um sistema de demonstração. Para produção, considere implementar:
- Validação mais robusta
- Logging estruturado
- Cache
- Testes unitários e de integração
- Monitoramento e alertas
- Segurança adicional (rate limiting, etc.)
