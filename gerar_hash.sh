#!/bin/bash

# Script para gerar hash BCrypt de passwords
# Usa a API própria para gerar hashes

if [ -z "$1" ]; then
  echo "╔════════════════════════════════════════════════════════════╗"
  echo "║          🔐 GERADOR DE HASH BCRYPT                        ║"
  echo "╚════════════════════════════════════════════════════════════╝"
  echo ""
  echo "Uso: ./gerar_hash.sh <password>"
  echo ""
  echo "Exemplo:"
  echo "  ./gerar_hash.sh minhasenha123"
  echo ""
  echo "Gerando hashes para passwords padrão..."
  echo ""
  
  # Gerar hashes para passwords comuns
  for pwd in "admin123" "teste123" "clinica123"; do
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo "Password: $pwd"
    
    # Criar temporariamente um pequeno programa C# para gerar o hash
    cat > /tmp/hash_temp.cs << CSHARP
using System;
using BCrypt.Net;

Console.WriteLine(BCrypt.HashPassword("$pwd"));
CSHARP
    
    # Compilar e executar
    cd /home/david/dev/cvsl/src/api
    hash=$(dotnet script eval 'using BCrypt.Net; return BCrypt.HashPassword("'$pwd'");' 2>/dev/null || echo "ERRO")
    
    if [ "$hash" = "ERRO" ]; then
      echo "Hash:     (use o método alternativo abaixo)"
    else
      echo "Hash:     $hash"
    fi
    echo ""
  done
  
  echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
  echo ""
  echo "💡 MÉTODO ALTERNATIVO (mais simples):"
  echo ""
  echo "1. Use a própria API para gerar hash:"
  echo "   - Registre um usuário com a password desejada"
  echo "   - O hash será gerado automaticamente"
  echo ""
  echo "2. Ou crie um endpoint temporário na API:"
  echo "   POST /api/Utils/hash"
  echo "   Body: { \"password\": \"suasenha\" }"
  
else
  password="$1"
  echo "Gerando hash para: $password"
  echo ""
  echo "Use o endpoint de registro da API para gerar o hash automaticamente."
  echo "Exemplo: POST /api/Admin/admins com password: $password"
fi

