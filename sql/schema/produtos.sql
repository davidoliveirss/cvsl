CREATE TABLE produtos (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    id_categoria INT NOT NULL REFERENCES categorias(id),
    preco DECIMAL(10,2) NOT NULL,
    unidades_por_caixa INT NOT NULL,
    quantidade_stock INT NOT NULL DEFAULT 0,
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE
);
