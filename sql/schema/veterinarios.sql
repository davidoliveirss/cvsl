CREATE TABLE veterinarios (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    especialidade VARCHAR(100),
    telefone VARCHAR(20) NOT NULL,
    email VARCHAR(100),
    password VARCHAR(255) NOT NULL,
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE
);
