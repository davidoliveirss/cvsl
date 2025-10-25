CREATE TABLE veterinarios (
    id_vet SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    especialidade VARCHAR(100),
    telefone VARCHAR(20) NOT NULL,
    email VARCHAR(100),
    password varchar(255) NOT NULL
);