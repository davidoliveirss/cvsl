CREATE TABLE servicos (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    descricao TEXT,
    preco DECIMAL(10,2) NOT NULL,
    id_clinica INT REFERENCES clinicas(id) NOT NULL ON DELETE CASCADE
);