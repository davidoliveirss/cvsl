CREATE TABLE faturas (
    id_fatura SERIAL PRIMARY KEY,
    data TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    valor_total DECIMAL(10,2) NOT NULL DEFAULT 0,
    transponder VARCHAR(15) REFERENCES animais(transponder) ON DELETE SET NULL,
    tipo_pagamento tipo_pagamento_enum NOT NULL DEFAULT 'dinheiro', 
    id_cliente INT NOT NULL REFERENCES clientes(id) ON DELETE CASCADE,
    id_clinica INT NOT NULL REFERENCES clinicas(id)
);

CREATE TABLE produtos_fatura (
    id_fatura INT NOT NULL REFERENCES faturas(id_fatura) ON DELETE CASCADE,
    id_produto INT NOT NULL REFERENCES produtos(id),
    quantidade INT NOT NULL DEFAULT 1,
    preco_unitario DECIMAL(10,2) NOT NULL,
    PRIMARY KEY(id_fatura,id_produto)
);

CREATE TABLE servicos_fatura (
    id_fatura INT NOT NULL REFERENCES faturas(id_fatura) ON DELETE CASCADE,
    id_servico INT NOT NULL REFERENCES servicos(id),
    quantidade INT NOT NULL DEFAULT 1,
    preco_unitario DECIMAL(10,2) NOT NULL,
    PRIMARY KEY(id_fatura,id_servico)
);

CREATE TABLE tipo_pagamento (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(50) NOT NULL
);

CREATE TABLE iva (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(50) NOT NULL,
    taxa DECIMAL(4,2) NOT NULL
)