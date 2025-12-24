CREATE TYPE tipo_pagamento_enum AS ENUM ('dinheiro', 'multibanco', 'mbway', 'cartao_credito', 'transferencia');

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

CREATE TABLE admins (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    nivel VARCHAR(20) DEFAULT 'admin' CHECK (nivel IN ('super_admin', 'admin')),
    ativo BOOLEAN DEFAULT TRUE
);

CREATE TABLE categorias (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    descricao VARCHAR(255),
    iva DECIMAL(5,2) NOT NULL
);

CREATE TABLE group_permissions (
    id SERIAL PRIMARY KEY,
    group_name VARCHAR(50) NOT NULL,
    permission VARCHAR(100) NOT NULL
);

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

CREATE TABLE servicos (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    descricao TEXT,
    preco DECIMAL(10,2) NOT NULL,
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE
);

CREATE TABLE consultas (
    id SERIAL PRIMARY KEY,
    data DATE NOT NULL,
    hora TIME NOT NULL,
    motivo TEXT,
    observacoes TEXT,
    id_animal INT NOT NULL REFERENCES animais(id) ON DELETE CASCADE,
    id_funcionario INT REFERENCES funcionarios(id) ON DELETE SET NULL,
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE
);

CREATE TABLE faturas (
    id_fatura SERIAL PRIMARY KEY,
    data TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    valor_total DECIMAL(10,2) NOT NULL DEFAULT 0,
    transponder VARCHAR(15) REFERENCES animais(transponder) ON DELETE SET NULL,
    tipo_pagamento tipo_pagamento_enum NOT NULL DEFAULT 'dinheiro', 
    id_cliente INT NOT NULL REFERENCES clientes(id) ON DELETE CASCADE,
    id_clinica INT NOT NULL REFERENCES clinicas(id)
);

CREATE TABLE produtos_fatura (
    id_fatura INT NOT NULL REFERENCES faturas(id_fatura) ON DELETE CASCADE,
    id_produto INT NOT NULL REFERENCES produtos(id),
    quantidade INT NOT NULL DEFAULT 1,
    preco_unitario DECIMAL(10,2) NOT NULL,
    PRIMARY KEY(id_fatura,id_produto)
);

CREATE TABLE servicos_fatura (
    id_fatura INT NOT NULL REFERENCES faturas(id_fatura) ON DELETE CASCADE,
    id_servico INT NOT NULL REFERENCES servicos(id),
    quantidade INT NOT NULL DEFAULT 1,
    preco_unitario DECIMAL(10,2) NOT NULL,
    PRIMARY KEY(id_fatura,id_servico)
);

CREATE TABLE tipo_pagamento (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(50) NOT NULL
);

CREATE TABLE iva (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(50) NOT NULL,
    taxa DECIMAL(4,2) NOT NULL
);

-- Inserts de dados iniciais
-- Password: $2a$11$zyAiBAPwlt2XrqJqDTi9SeLgYNvxzLvg3uqwh7YZP5/Q8eGQosiz.

-- Admin
INSERT INTO admins (nome, email, password, nivel, ativo) 
VALUES ('Admin Principal', 'admin@cvsl.pt', '$2a$11$zyAiBAPwlt2XrqJqDTi9SeLgYNvxzLvg3uqwh7YZP5/Q8eGQosiz.', 'super_admin', TRUE);

-- Clínica
INSERT INTO clinicas (nome, email, password, cp, nif, iban, ativo) 
VALUES ('Clínica Veterinária São Lucas', 'clinica@cvsl.pt', '$2a$11$zyAiBAPwlt2XrqJqDTi9SeLgYNvxzLvg3uqwh7YZP5/Q8eGQosiz.', '1000-001', '123456789', 'PT50000000000000000000001', TRUE);

-- Funcionário (associado à clínica id=1)
INSERT INTO funcionarios (nome, especialidade, telefone, email, password, salario, id_clinica, ativo) 
VALUES ('Dr. João Silva', 'Veterinário', '912345678', 'joao.silva@cvsl.pt', '$2a$11$zyAiBAPwlt2XrqJqDTi9SeLgYNvxzLvg3uqwh7YZP5/Q8eGQosiz.', 1500.00, 1, TRUE);
