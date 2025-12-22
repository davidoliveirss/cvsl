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
