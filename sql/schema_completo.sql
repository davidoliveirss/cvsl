-- ==========================================
-- CVSL - Schema Completo para PostgreSQL
-- ==========================================
-- Este ficheiro contém todos os schemas ordenados corretamente
-- para evitar erros de chaves estrangeiras.
-- ==========================================

-- Desativar verificação de chaves estrangeiras temporariamente (opcional)
-- SET session_replication_role = 'replica';

-- ==========================================
-- 1. TIPOS ENUMERADOS (devem ser criados primeiro)
-- ==========================================

CREATE TYPE tipo_pagamento_enum AS ENUM ('dinheiro', 'multibanco', 'mbway', 'cartao_credito', 'transferencia');

-- ==========================================
-- 2. TABELAS SEM DEPENDÊNCIAS
-- ==========================================

-- Clínicas (tabela base - muitas outras dependem dela)
CREATE TABLE clinicas (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL,
    password VARCHAR(255) NOT NULL, 
    cp VARCHAR(8) NOT NULL,
    nif VARCHAR(9),
    iban varchar(25) NOT NULL,
    ativo BOOLEAN DEFAULT TRUE
);

-- Admins (sem dependências)
CREATE TABLE admins (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    nivel VARCHAR(20) DEFAULT 'admin' CHECK (nivel IN ('super_admin', 'admin')),
    ativo BOOLEAN DEFAULT TRUE
);

CREATE INDEX idx_admins_email ON admins(email);

-- Categorias (sem dependências)
CREATE TABLE categorias (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    descricao VARCHAR(255),
    iva DECIMAL(5,2) NOT NULL
);

-- Group Permissions (sem dependências)
CREATE TABLE group_permissions (
    id SERIAL PRIMARY KEY,
    group_name VARCHAR(50) NOT NULL,
    permission VARCHAR(100) NOT NULL
);

-- Sessions (sem dependências de FK)
CREATE TABLE sessions (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    session_token VARCHAR(255) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_accessed TIMESTAMP NOT NULL
);

-- Tipo de Pagamento (lookup table)
CREATE TABLE tipo_pagamento (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(50) NOT NULL
);

-- IVA (lookup table)
CREATE TABLE iva (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(50) NOT NULL,
    taxa DECIMAL(4,2) NOT NULL
);

-- ==========================================
-- 3. TABELAS DEPENDENTES DE CLÍNICAS
-- ==========================================

-- Clientes (depende de clinicas)
CREATE TABLE clientes (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    nif VARCHAR(9),
    morada VARCHAR(200),
    telefone VARCHAR(9) NOT NULL,
    email VARCHAR(100),
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE,
    ativo BOOLEAN DEFAULT TRUE
);

-- Funcionários/Veterinários (depende de clinicas)
CREATE TABLE funcionarios(
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    especialidade VARCHAR(100),
    telefone VARCHAR(20) NOT NULL,
    email VARCHAR(100) NOT NULL,
    password VARCHAR(255) NOT NULL,
    salario DECIMAL(10,2) NOT NULL,
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE,
    ativo BOOLEAN DEFAULT TRUE
);

-- Nota: veterinarios pode ser uma view ou tabela separada
CREATE TABLE veterinarios (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    especialidade VARCHAR(100),
    telefone VARCHAR(20) NOT NULL,
    email VARCHAR(100) NOT NULL,
    password VARCHAR(255) NOT NULL,
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE,
    ativo BOOLEAN DEFAULT TRUE
);

-- Produtos (depende de clinicas e categorias)
CREATE TABLE produtos (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    id_categoria INT NOT NULL REFERENCES categorias(id),
    preco DECIMAL(10,2) NOT NULL,
    unidades_por_caixa INT NOT NULL,
    quantidade_stock INT NOT NULL DEFAULT 0,
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE,
    ativo BOOLEAN DEFAULT TRUE
);

-- Serviços (depende de clinicas)
CREATE TABLE servicos (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    descricao TEXT,
    preco DECIMAL(10,2) NOT NULL,
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE
);

-- ==========================================
-- 4. TABELAS DEPENDENTES DE CLIENTES E CLÍNICAS
-- ==========================================

-- Animais (depende de clientes e clinicas)
CREATE TABLE animais (
    id SERIAL PRIMARY KEY,
    transponder VARCHAR(15) UNIQUE,
    nome VARCHAR(50) NOT NULL,
    especie VARCHAR(50),
    raca VARCHAR(50),
    data_nascimento DATE,
    sexo CHAR(1),
    id_cliente INT NOT NULL REFERENCES clientes(id) ON DELETE CASCADE,
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE
);

-- ==========================================
-- 5. TABELAS DEPENDENTES DE ANIMAIS
-- ==========================================

-- Consultas (depende de animais, veterinarios e clinicas)
CREATE TABLE consultas (
    id SERIAL PRIMARY KEY,
    data DATE NOT NULL,
    hora TIME NOT NULL,
    motivo TEXT,
    observacoes TEXT,
    id_animal INT NOT NULL REFERENCES animais(id) ON DELETE CASCADE,
    id_vet INT REFERENCES veterinarios(id) ON DELETE SET NULL,
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE
);

-- Faturas (depende de animais, clientes e clinicas)
CREATE TABLE faturas (
    id_fatura SERIAL PRIMARY KEY,
    data TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    valor_total DECIMAL(10,2) NOT NULL DEFAULT 0,
    transponder VARCHAR(15) REFERENCES animais(transponder) ON DELETE SET NULL,
    tipo_pagamento tipo_pagamento_enum NOT NULL DEFAULT 'dinheiro', 
    id_cliente INT NOT NULL REFERENCES clientes(id) ON DELETE CASCADE,
    id_clinica INT NOT NULL REFERENCES clinicas(id)
);

-- ==========================================
-- 6. TABELAS DE JUNÇÃO/ASSOCIAÇÃO
-- ==========================================

-- Produtos da Fatura (tabela de junção)
CREATE TABLE produtos_fatura (
    id_fatura INT NOT NULL REFERENCES faturas(id_fatura) ON DELETE CASCADE,
    id_produto INT NOT NULL REFERENCES produtos(id),
    quantidade INT NOT NULL DEFAULT 1,
    preco_unitario DECIMAL(10,2) NOT NULL,
    PRIMARY KEY(id_fatura, id_produto)
);

-- Serviços da Fatura (tabela de junção)
CREATE TABLE servicos_fatura (
    id_fatura INT NOT NULL REFERENCES faturas(id_fatura) ON DELETE CASCADE,
    id_servico INT NOT NULL REFERENCES servicos(id),
    quantidade INT NOT NULL DEFAULT 1,
    preco_unitario DECIMAL(10,2) NOT NULL,
    PRIMARY KEY(id_fatura, id_servico)
);

-- ==========================================
-- REATIVAR VERIFICAÇÃO DE CHAVES (se desativada)
-- ==========================================
-- SET session_replication_role = 'origin';

-- ==========================================
-- FIM DO SCHEMA
-- ==========================================
