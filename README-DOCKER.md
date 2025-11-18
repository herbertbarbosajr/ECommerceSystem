# Docker Setup - ECommerce System

Este projeto está configurado para rodar completamente dentro do Docker, incluindo:
- PostgreSQL (banco de dados)
- API .NET (backend)
- Frontend React (Vite)

## Pré-requisitos

- Docker Desktop instalado e rodando
- Docker Compose (geralmente incluído no Docker Desktop)

## Como executar

### 1. Subir todos os serviços

Na raiz do projeto, execute:

```bash
docker compose up --build
```

Este comando irá:
- Construir as imagens da API e do Frontend
- Subir o PostgreSQL
- Subir a API na porta 8080
- Subir o Frontend na porta 3000

### 2. Acessar as aplicações

- **Frontend**: http://localhost:3000
- **API**: http://localhost:8080
- **Swagger (API Docs)**: http://localhost:8080/swagger
- **PostgreSQL**: localhost:5432

### 3. Credenciais padrão (desenvolvimento)

O sistema cria automaticamente um usuário admin quando iniciado:

- **Email**: admin@example.com
- **Senha**: P@ssw0rd

## Estrutura dos serviços

### PostgreSQL
- **Porta**: 5432
- **Database**: ECommerceDb
- **User**: postgres
- **Password**: postgres

### API (.NET)
- **Porta interna**: 80
- **Porta externa**: 8080
- **Rede**: ecommerce_network
- **Nome do serviço**: ecommerce_api

### Frontend (React)
- **Porta**: 3000
- **Rede**: ecommerce_network
- **Nome do serviço**: ecommerce_web
- **Variável de ambiente**: `VITE_API_BASE_URL=http://localhost:8080`
- **Nota**: O frontend usa `localhost:8080` porque o navegador não consegue resolver nomes de serviços Docker internos

## Comandos úteis

### Parar todos os serviços
```bash
docker compose down
```

### Parar e remover volumes (limpar banco de dados)
```bash
docker compose down -v
```

### Ver logs de um serviço específico
```bash
docker compose logs -f ecommerce_api
docker compose logs -f ecommerce_web
docker compose logs -f postgres
```

### Reconstruir apenas um serviço
```bash
docker compose up --build ecommerce_web
```

### Executar comandos dentro de um container
```bash
# Acessar o container do frontend
docker compose exec ecommerce_web sh

# Acessar o container da API
docker compose exec ecommerce_api sh
```

## Desenvolvimento

### Hot Reload

O frontend está configurado com hot reload através de volumes:
- O código local é sincronizado com o container
- Mudanças no código são refletidas automaticamente

### Variáveis de Ambiente

#### Frontend
A variável `VITE_API_BASE_URL` é definida no `docker-compose.yml`:
- **Docker**: `http://ecommerce_api/api` (comunicação interna)
- **Local**: `http://localhost:8080/api` (se rodar `npm run dev` fora do Docker)

#### API
As variáveis da API são definidas no `docker-compose.yml`:
- `ASPNETCORE_ENVIRONMENT=Development`
- `ASPNETCORE_URLS=http://+:80`
- Configurações de admin inicial

## Troubleshooting

### Porta já em uso
Se a porta 3000 ou 8080 já estiver em uso, você pode alterar no `docker-compose.yml`:
```yaml
ports:
  - "3001:3000"  # Mude 3000 para 3001
```

### Frontend não conecta com a API
Verifique se:
1. A API está rodando: `docker compose ps`
2. A variável `VITE_API_BASE_URL` está correta
3. O CORS está configurado na API (já está configurado no `Program.cs`)

### Banco de dados não conecta
Verifique os logs:
```bash
docker compose logs postgres
docker compose logs ecommerce_api
```

## Produção

Para produção, você deve:
1. Alterar `ASPNETCORE_ENVIRONMENT` para `Production`
2. Desabilitar a criação de admin inicial
3. Usar variáveis de ambiente seguras
4. Configurar HTTPS
5. Usar um build de produção do frontend (não o dev server)

