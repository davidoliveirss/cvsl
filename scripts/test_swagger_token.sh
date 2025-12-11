#!/bin/bash

echo "╔════════════════════════════════════════════════════════════╗"
echo "║     🧪 TESTE RÁPIDO: TOKEN DO SWAGGER                      ║"
echo "╚════════════════════════════════════════════════════════════╝"
echo ""

# Obter token
echo "1️⃣  Obtendo token de admin..."
RESPONSE=$(curl -s -X POST http://localhost:5000/api/Auth/login/admin \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@cvsl.pt", "password": "admin123"}')

TOKEN=$(echo "$RESPONSE" | jq -r '.token')

if [ "$TOKEN" = "null" ] || [ -z "$TOKEN" ]; then
  echo "❌ Erro: Não foi possível obter o token"
  echo "   Verifique se a API está rodando em http://localhost:5000"
  exit 1
fi

echo "✅ Token obtido com sucesso!"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📋 COPIE ESTE TOKEN PARA O SWAGGER:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "$TOKEN"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "2️⃣  Testando token..."

# Testar token
RESPONSE_CODE=$(curl -s -w "%{http_code}" -o /tmp/test_response.json \
  -X GET "http://localhost:5000/api/Admin/clinicas" \
  -H "Authorization: Bearer $TOKEN")

if [ "$RESPONSE_CODE" = "200" ]; then
  echo "✅ Token válido! Resposta:"
  cat /tmp/test_response.json | jq .
  echo ""
  echo "🎉 SUCESSO! Use este token no Swagger:"
  echo "   1. Clique em 'Authorize' 🔓"
  echo "   2. Cole o token acima (sem 'Bearer')"
  echo "   3. Clique em 'Authorize' e depois 'Close'"
else
  echo "❌ Erro: Token inválido (HTTP $RESPONSE_CODE)"
  echo "   Resposta:"
  cat /tmp/test_response.json
fi

echo ""

