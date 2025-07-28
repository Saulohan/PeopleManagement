# People Management API

## Visão Geral

Este projeto é uma API RESTful desenvolvida em .NET 8.0 para gerenciamento pessoas, oferecendo funcionalidades de cadastro, consulta, atualização e remoção de registros. A API inclui autenticação JWT, validação de dados, e utiliza padrões atuais de desenvolvimento.

O projeto faz parte de um sistema completo de gerenciamento de pessoas, onde o frontend está disponível em: [PeopleManagementApp](https://github.com/Saulohan/PeopleManagementApp)

## Tecnologias Utilizadas

- **.NET 8.0**: Framework para desenvolvimento da API
- **Entity Framework Core 9.0**: ORM para acesso ao banco de dados
- **Swagger**: Documentação interativa da API
- **JWT (JSON Web Tokens)**: Autenticação baseada em tokens
- **FluentValidation**: Validação de requisições
- **AutoMapper**: Mapeamento entre entidades e DTOs
- **ASP.NET Core Identity**: Gerenciamento de senhas e hashing

## Arquitetura do Projeto

O projeto segue uma arquitetura limpa (Clean Architecture) dividida em múltiplas camadas:

```
PeopleManagement/
├── PeopleManagement.API/            # Camada de API/UI
├── PeopleManagement.Application/    # Camada de aplicação
├── PeopleManagement.Domain/         # Camada de domínio
├── PeopleManagement.Infrastructure/ # Camada de infraestrutura
└── PeopleManagement.Test/           # Testes unitários
```

### Detalhamento das Camadas

- **API**: Controllers, validations, middlewares e configurações da API
- **Application**: Serviços, DTOs, interfaces e lógica de negócio
- **Domain**: Entidades de domínio, enums e classes base
- **Infrastructure**: Contexto de banco de dados, implementações de repositórios e serviços de infraestrutura
- **Test**: Testes unitários para os componentes do sistema

## API Endpoints

A API tem disponibilidade de versionamento e está disponível nas versões v1 e v2.

### Autenticação

- `POST /api/v1/Auth`: Autenticação de usuário (login)

### Gerenciamento de Pessoas

#### v1

- `GET /api/v1/People`: Buscar pessoas com filtros opcionais
- `POST /api/v1/People`: Adicionar nova pessoa
- `PUT /api/v1/People/{cpf}`: Atualizar pessoa existente
- `DELETE /api/v1/People/{cpf}`: Excluir pessoa

#### v2

- `POST /api/v2/People`: Adicionar nova pessoa (com validações adicionais)

## Funcionalidades

- **CRUD de Pessoas**: Gerenciamento completo de registros de pessoas
- **Autenticação JWT**: Segurança baseada em tokens
- **Versionamento de API**: Suporte para diferentes versões da API
- **Validação de Dados**: CPF, e-mail e outros campos com FluentValidation
- **Tratamento de Exceções**: Middleware centralizado para tratamento de erros
- **Soft Delete**: Exclusão lógica de registros
- **Filtros Avançados**: Busca com múltiplos critérios

## Requisitos de Sistema

- .NET 8.0 SDK ou superior
- SQL Server (pode ser configurado para usar outros bancos de dados)
- Visual Studio 2022 ou outro IDE compatível com .NET

## Configuração e Execução

### Pré-requisitos

1. Instalar [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Instalar e configurar o SQL Server ou outro banco de dados suportado

### Passos para Execução

1. Clone o repositório:

2. Configurar a conexão com o banco de dados em `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "database": "Server=seu_servidor;Database=PeopleManagement;User Id=seu_usuario;Password=sua_senha;TrustServerCertificate=True"
   }
   ```

3. Executar migrações do Entity Framework para criar o banco de dados:
   ```bash
   dotnet ef database update --project PeopleManagement.Infrastructure --startup-project PeopleManagement.API
   ```

4. Executar a aplicação:
   ```bash
   dotnet run --project PeopleManagement.API
   ```

5. Acessar a documentação Swagger:
   ```
   https://localhost:7001/swagger
   ```

## Testes

Para executar os testes unitários:

```bash
dotnet test PeopleManagement.Test
```

## Frontend da Aplicação

## Estrutura de Dados

### Person (Entidade Principal)

### Person (Entidade Principal)

| Campo         | Tipo        | Descrição                                 |
|---------------|-------------|-------------------------------------------|
| Id            | Guid        | Identificador único da pessoa             |
| Name          | string      | Nome completo                             |
| CPF           | string      | Número de CPF (único)                     |
| Email         | string      | Endereço de e-mail                        |
| Gender        | enum        | Gênero da pessoa                          |
| DateOfBirth   | DateOnly    | Data de nascimento                        |
| Naturality    | string      | Naturalidade                              |
| Nationality   | string      | Nacionalidade                             |
| PasswordHash  | string      | Hash da senha para autenticação           |
| CreatedAt     | DateTime    | Data de criação do registro               |
| UpdatedAt     | DateTime    | Data da última atualização                |
| DeletionAt    | DateTime?   | Data de exclusão (soft delete, opcional)  |
