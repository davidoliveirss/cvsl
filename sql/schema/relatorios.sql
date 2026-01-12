CREATE TABLE relatorios (
    id SERIAL PRIMARY KEY,
    titulo VARCHAR(15) UNIQUE,
    motivo TEXT NOT NULL,
    diagnostico_presuntivo TEXT NOT NULL,
    diagnostico_definitivo TEXT,
    observacoes TEXT NOT NULL,
    data_consulta date NOT NULL,
    id_animal INT NOT NULL REFERENCES animais(id) ON DELETE CASCADE,
    id_clinica INT NOT NULL REFERENCES clinicas(id) ON DELETE CASCADE,
);
