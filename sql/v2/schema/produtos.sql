CREATE TABLE produtos (
    id_produto SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    categoria VARCHAR(50),
    preco DECIMAL(10,2) NOT NULL,
    unidades_por_caixa INT NOT NULL,
    quantidade_stock INT NOT NULL DEFAULT 0,
    id_clinica INT REFERENCES clinicas(id) NOT NULL ON DELETE CASCADE
);