CREATE TABLE clientes (
    id_cliente SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    nif VARCHAR(9),
    morada VARCHAR(200),
    telefone VARCHAR(9) NOT NULL,
    email VARCHAR(100)
);