CREATE TABLE produtos (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    categoria VARCHAR(50),
    preco DECIMAL(10,2) NOT NULL,
    unidades_por_caixa INT NOT NULL,
    quantidade_stock INT NOT NULL DEFAULT 0,
    id_iva INT NOT NULL REFERENCES iva(id),
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE
);
