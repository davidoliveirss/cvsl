CREATE TABLE faturas (
    id_fatura SERIAL PRIMARY KEY,
    data TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    valor_total DECIMAL(10,2),
    
    id_cliente INT REFERENCES clientes(id_cliente) ON DELETE CASCADE
);

CREATE TABLE itens_fatura (
    id_item SERIAL PRIMARY KEY,
    id_fatura INT REFERENCES faturas(id_fatura) ON DELETE CASCADE,
    id_produto INT REFERENCES produtos(id_produto),
    id_servico INT REFERENCES servicos(id_servico),
    quantidade INT NOT NULL DEFAULT 1,
    preco_unitario DECIMAL(10,2) NOT NULL
);
