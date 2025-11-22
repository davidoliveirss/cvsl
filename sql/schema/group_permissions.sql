CREATE TABLE group_permissions (
    id SERIAL PRIMARY KEY,
    group_name VARCHAR(50) NOT NULL,
    permission VARCHAR(100) NOT NULL
);