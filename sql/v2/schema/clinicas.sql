CREATE TABLE animais (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    cp VARCHAR(8) NOT NULL,
    data_nascimento DATE,
    sexo CHAR(1),
    id_clinica INT REFERENCES clinicas(id) NOT NULL ON DELETE CASCADE 
);