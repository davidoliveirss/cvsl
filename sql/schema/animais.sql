CREATE TABLE animais (
    id SERIAL PRIMARY KEY,
    transponder VARCHAR(15) UNIQUE,
    nome VARCHAR(50) NOT NULL,
    especie VARCHAR(50),
    raca VARCHAR(50),
    data_nascimento DATE,
    sexo CHAR(1),
    id_cliente INT NOT NULL REFERENCES clientes(id) ON DELETE CASCADE,
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE,
    ativo BOOLEAN DEFAULT TRUE
);
