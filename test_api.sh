#!/bin/bash

echo "======================================"
echo "🧪 TESTE DA API - CVSL"
echo "======================================"
echo ""

# Cores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

API_URL="http://localhost:5000"

echo "1️⃣  Testando endpoint de login da CLÍNICA..."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "📧 Email: clinica@teste.pt"
echo "🔑 Password: teste123"
echo ""

RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/auth/login/clinica" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "clinica@teste.pt",
    "password": "teste123"
  }')

HTTP_CODE=$(echo "$RESPONSE" | tail -n1)
BODY=$(echo "$RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 200 ]; then
    echo -e "${GREEN}✅ LOGIN CLÍNICA: SUCESSO (HTTP $HTTP_CODE)${NC}"
    echo ""
    echo "📦 Resposta:"
    echo "$BODY" | jq '.' 2>/dev/null || echo "$BODY"
    
    # Extrair token para próximos testes
    TOKEN=$(echo "$BODY" | jq -r '.token' 2>/dev/null)
    echo ""
    echo -e "${YELLOW}🔐 Token JWT gerado:${NC}"
    echo "$TOKEN"
else
    echo -e "${RED}❌ LOGIN CLÍNICA: FALHOU (HTTP $HTTP_CODE)${NC}"
    echo "$BODY"
fi

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

echo "2️⃣  Testando endpoint de login do FUNCIONÁRIO (Veterinário)..."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "📧 Email: vet@teste.pt"
echo "🔑 Password: teste123"
echo ""

RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/auth/login/funcionario" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "vet@teste.pt",
    "password": "teste123"
  }')

HTTP_CODE=$(echo "$RESPONSE" | tail -n1)
BODY=$(echo "$RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 200 ]; then
    echo -e "${GREEN}✅ LOGIN VETERINÁRIO: SUCESSO (HTTP $HTTP_CODE)${NC}"
    echo ""
    echo "📦 Resposta:"
    echo "$BODY" | jq '.' 2>/dev/null || echo "$BODY"
else
    echo -e "${RED}❌ LOGIN VETERINÁRIO: FALHOU (HTTP $HTTP_CODE)${NC}"
    echo "$BODY"
fi

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

echo "3️⃣  Testando endpoint de login do FUNCIONÁRIO (Rececionista)..."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "📧 Email: rececionista@teste.pt"
echo "🔑 Password: teste123"
echo ""

RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/auth/login/funcionario" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "rececionista@teste.pt",
    "password": "teste123"
  }')

HTTP_CODE=$(echo "$RESPONSE" | tail -n1)
BODY=$(echo "$RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 200 ]; then
    echo -e "${GREEN}✅ LOGIN RECECIONISTA: SUCESSO (HTTP $HTTP_CODE)${NC}"
    echo ""
    echo "📦 Resposta:"
    echo "$BODY" | jq '.' 2>/dev/null || echo "$BODY"
else
    echo -e "${RED}❌ LOGIN RECECIONISTA: FALHOU (HTTP $HTTP_CODE)${NC}"
    echo "$BODY"
fi

echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

echo "4️⃣  Testando credenciais INVÁLIDAS..."
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "📧 Email: errado@teste.pt"
echo "🔑 Password: errada"
echo ""

RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/auth/login/clinica" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "errado@teste.pt",
    "password": "errada"
  }')

HTTP_CODE=$(echo "$RESPONSE" | tail -n1)
BODY=$(echo "$RESPONSE" | sed '$d')

if [ "$HTTP_CODE" -eq 401 ]; then
    echo -e "${GREEN}✅ REJEIÇÃO ESPERADA: SUCESSO (HTTP $HTTP_CODE)${NC}"
    echo ""
    echo "📦 Resposta:"
    echo "$BODY" | jq '.' 2>/dev/null || echo "$BODY"
else
    echo -e "${RED}❌ DEVERIA RETORNAR 401 mas retornou (HTTP $HTTP_CODE)${NC}"
    echo "$BODY"
fi

echo ""
echo "======================================"
echo "✨ TESTES CONCLUÍDOS!"
echo "======================================"
