CREATE TABLE animais (
    id SERIAL PRIMARY KEY,
    transponder varchar(15) UNIQUE,
    nome VARCHAR(50) NOT NULL,
    especie VARCHAR(50),
    raca VARCHAR(50),
    data_nascimento DATE,
    sexo CHAR(1),
    id_cliente INT REFERENCES clientes(id) ON DELETE CASCADE,
    id_clinica INT REFERENCES clinicas(id) NOT NULL ON DELETE CASCADE
);