CREATE TABLE clinicas (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL,
    password VARCHAR(255) NOT NULL, 
    cp VARCHAR(8) NOT NULL,
    nif VARCHAR(9),
    iban varchar(25) NOT NULL
);
