CREATE TABLE clinicas (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    cp VARCHAR(8) NOT NULL,
    nif VARCHAR(9),
    id_clinica INT REFERENCES clinicas(id) NOT NULL ON DELETE CASCADE 
);