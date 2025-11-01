CREATE TABLE faturas (
    id_fatura SERIAL PRIMARY KEY,
    data TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    valor_total DECIMAL(10,2) NOT NULL DEFAULT 0,
    transponder VARCHAR(15) REFERENCES animais(transponder) ON DELETE SET NULL,
    id_cliente INT REFERENCES clientes(id) ON DELETE CASCADE,
    id_clinica INT REFERENCES clinicas(id) NOT NULL
);

CREATE TABLE produtos_fatura (
    id_item SERIAL PRIMARY KEY,
    id_fatura INT NOT NULL REFERENCES faturas(id_fatura) ON DELETE CASCADE,
    id_produto INT NOT NULL REFERENCES produtos(id),
    quantidade INT NOT NULL DEFAULT 1,
    preco_unitario DECIMAL(10,2) NOT NULL
);

CREATE TABLE servicos_fatura (
    id_item SERIAL PRIMARY KEY,
    id_fatura INT NOT NULL REFERENCES faturas(id_fatura) ON DELETE CASCADE,
    id_servico INT NOT NULL REFERENCES servicos(id),
    quantidade INT NOT NULL DEFAULT 1,
    preco_unitario DECIMAL(10,2) NOT NULL
);
