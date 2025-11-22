CREATE TABLE funcionarios(
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    especialidade VARCHAR(100),
    telefone VARCHAR(20) NOT NULL,
    email VARCHAR(100) NOT NULL,
    password VARCHAR(255) NOT NULL,
    salario DECIMAL(10,2) NOT NULL,
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE
);
