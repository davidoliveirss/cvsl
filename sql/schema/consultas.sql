CREATE TABLE consultas (
    id SERIAL PRIMARY KEY,
    data DATE NOT NULL,
    hora TIME NOT NULL,
    motivo TEXT,
    observacoes TEXT,
    id_animal INT NOT NULL REFERENCES animais(id) ON DELETE CASCADE,
    id_vet INT REFERENCES veterinarios(id) ON DELETE SET NULL,
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE
);
