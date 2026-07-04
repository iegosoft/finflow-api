# FinFlow API

![CI](https://github.com/iegosoft/finflow-api/actions/workflows/ci.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-green)

API RESTful de controle financeiro pessoal desenvolvida em C# com ASP.NET Core 8.

---

## Funcionalidades

- Autenticação com JWT Bearer Token e Refresh Token
- Cadastro e gerenciamento de categorias financeiras
- Registro e consulta de transações (receitas e saídas)
- Relatório mensal por categoria
- Documentação interativa via Swagger

---

## Stack

| Tecnologia | Versão |
|---|---|
| C# / .NET | 8.0 |
| ASP.NET Core Web API | 8.0 |
| Entity Framework Core | 8.0 |
| PostgreSQL | 16 |
| xUnit + Moq | - |
| Docker + Docker Compose | - |

---

## Estrutura do Projeto

```
finflow-api/
├── src/
│   ├── FinFlow.API/              ← Controllers, Program.cs, Middlewares
│   ├── FinFlow.Application/      ← Services, DTOs, Interfaces de Service
│   ├── FinFlow.Domain/           ← Entidades, Interfaces de Repository, Enums
│   └── FinFlow.Infra/            ← DbContext, Repositories, Migrations
├── tests/
│   └── FinFlow.Tests/            ← xUnit, Moq, FluentAssertions
├── .github/
│   └── workflows/
│       └── ci.yml
├── docker-compose.yml
├── .gitignore
├── LICENSE
└── README.md
```

---

## Como rodar localmente

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### 1. Clone o repositório

```bash
git clone https://github.com/iegosoft/finflow-api.git
cd finflow-api
```

### 2. Suba o banco de dados

```bash
docker compose up postgres -d
```

### 3. Aplique as migrations

```bash
dotnet ef database update --project src/FinFlow.Infra --startup-project src/FinFlow.API
```

### 4. Execute a API

```bash
dotnet run --project src/FinFlow.API
```

### 5. Acesse o Swagger

Abra o navegador em: `http://localhost:5000`

---

## Endpoints

### Autenticação

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/auth/registrar` | Cria uma nova conta |
| POST | `/api/auth/login` | Retorna JWT + RefreshToken |
| POST | `/api/auth/refresh-token` | Renova o JWT |

### Categorias *(requer JWT)*

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/categorias` | Lista categorias do usuário |
| POST | `/api/categorias` | Cria uma categoria |
| PUT | `/api/categorias/{id}` | Atualiza uma categoria |
| DELETE | `/api/categorias/{id}` | Remove uma categoria |

### Transações *(requer JWT)*

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/transacoes` | Lista com filtros |
| GET | `/api/transacoes/{id}` | Busca por ID |
| POST | `/api/transacoes` | Cria uma transação |
| PUT | `/api/transacoes/{id}` | Atualiza uma transação |
| DELETE | `/api/transacoes/{id}` | Remove uma transação |
| GET | `/api/transacoes/relatorio-mensal` | Relatório mensal por categoria |

---

## Licença

MIT © 2026 [Iego Costa](https://github.com/iegosoft)
