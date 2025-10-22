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

--v2 

CREATE TABLE faturas (
    id_fatura SERIAL PRIMARY KEY,
    data_emissao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    valor_total DECIMAL(10,2) NOT NULL,
    id_cliente INT NOT NULL REFERENCES clientes(id_cliente) ON DELETE CASCADE,
    nif VARCHAR(9) REFERENCES clientes(nif) ON DELETE CASCADE,
    transponder  VARCHAR(15) REFERENCES animais(transponder) ON DELETE CASCADE
);

CREATE TABLE itens_fatura (
    id_item SERIAL PRIMARY KEY,
    id_fatura INT NOT NULL REFERENCES faturas(id_fatura) ON DELETE CASCADE,
    id_produto INT NULL REFERENCES produtos(id_produto),
    id_servico INT NULL REFERENCES servicos(id_servico),
    quantidade INT NOT NULL DEFAULT 1,
    preco_unitario DECIMAL(10,2) NOT NULL,
    CONSTRAINT chk_produto_ou_servico CHECK (
        (id_produto IS NOT NULL AND id_servico IS NULL)
        OR
        (id_produto IS NULL AND id_servico IS NOT NULL)
    )
);
