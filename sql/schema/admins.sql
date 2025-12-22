CREATE TABLE admins (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    nivel VARCHAR(20) DEFAULT 'admin' CHECK (nivel IN ('super_admin', 'admin')),
    ativo BOOLEAN DEFAULT TRUE
);