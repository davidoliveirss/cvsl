# PetGest

PetGest é uma aplicação web de gestão para clínicas veterinárias, desenvolvida para centralizar processos como gestão de animais, consultas, stock, serviços e análise de dados clínicos e operacionais.

## Objetivo

O projeto foi criado com o objetivo de melhorar a organização e eficiência de clínicas veterinárias, reunindo numa única plataforma funcionalidades essenciais do dia a dia.

## Funcionalidades

- Gestão de animais, tutores e registos clínicos
- Marcação e gestão de consultas
- Gestão de produtos, serviços e stock
- Controlo de utilizadores por perfis de acesso
- Registo e consulta de logs para auditoria de operações
- Visualização e interação com dados através de interface web responsiva

## Tecnologias usadas

- **Backend:** C#, ASP.NET Core
- **Frontend:** Vue.js, Quasar, TypeScript
- **Base de dados:** PostgreSQL
- **Acesso a dados:** Dapper / SQL
- **Ferramentas:** Git, GitHub, Postman, Visual Studio, VS Code

## Arquitetura

O PetGest segue uma arquitetura por camadas, separando responsabilidades entre lógica de negócio, acesso a dados e interface, o que facilita manutenção, escalabilidade e organização do código.

## Destaques técnicos

- API REST para comunicação entre frontend e backend
- Estrutura modular para CRUD de múltiplas entidades
- Sistema de autenticação e gestão de permissões por roles
- Logs de operações para maior rastreabilidade
- Interface responsiva focada em usabilidade

## Como executar o projeto

### Pré-requisitos

- .NET SDK
- Node.js e npm
- PostgreSQL

### Passos gerais

1. Clonar o repositório
2. Configurar a base de dados PostgreSQL
3. Atualizar as connection strings e variáveis de ambiente
4. Executar a API ASP.NET Core
5. Instalar dependências do frontend com `npm install`
6. Iniciar a aplicação frontend

## Contexto do projeto

O PetGest foi desenvolvido no âmbito da PAP (Prova de Aptidão Profissional), com foco em aplicar conhecimentos de desenvolvimento full-stack, modelação de base de dados, organização de arquitetura de software e resolução de problemas reais de gestão clínica.
