CREATE TABLE categorias (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    descricao VARCHAR(255),
    iva DECIMAL(5,2) NOT NULL,  -- por exemplo 23.00, 06.00, etc.
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE
);
