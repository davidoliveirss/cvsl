CREATE TABLE consultas (
    id_consulta SERIAL PRIMARY KEY,
    data DATE NOT NULL,
    hora TIME NOT NULL,
    motivo TEXT,
    observacoes TEXT,
    id_animal INT REFERENCES animais(id_animal) ON DELETE CASCADE,
    id_vet INT REFERENCES veterinarios(id_vet) ON DELETE SET NULL
);