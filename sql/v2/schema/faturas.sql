CREATE TABLE faturas (
    id_fatura SERIAL PRIMARY KEY,
    data TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    valor_total DECIMAL(10,2),
    transponder varchar(15) REFERENCCES animais(transponder) ON DELETE SET NULL,
    id_cliente INT REFERENCES clientes(id_cliente) ON DELETE CASCADE
);

CREATE TABLE produtos_fatura (
    id_item SERIAL PRIMARY KEY,
    id_fatura INT REFERENCES faturas(id_fatura) ON DELETE CASCADE,
    id_produto INT REFERENCES produtos(id_produto),
    quantidade INT NOT NULL DEFAULT 1,
    preco_unitario DECIMAL(10,2) NOT NULL
);

CREATE TABLE servicos_fatura (
    id_item SERIAL PRIMARY KEY,
    id_fatura INT REFERENCES faturas(id_fatura) ON DELETE CASCADE,
    id_servico INT REFERENCES servicos(id_servico),
    quantidade INT NOT NULL DEFAULT 1,
    preco_unitario DECIMAL(10,2) NOT NULL
);
