CREATE TABLE servicos_consulta (
    id_consulta INT NOT NULL,
    id_servico INT NOT NULL,
    PRIMARY KEY (id_consulta, id_servico)
);