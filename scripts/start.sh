#!/bin/bash

echo "════════════════════════════════════════════════════════"
echo "🚀 INICIANDO PROJETO CVSL - Full Stack"
echo "════════════════════════════════════════════════════════"
echo ""

# Cores
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Verificar se API está rodando
if lsof -Pi :5000 -sTCP:LISTEN -t >/dev/null ; then
    echo -e "${YELLOW}⚠️  Porta 5000 já está em uso${NC}"
    echo "Matando processo..."
    kill -9 $(lsof -t -i:5000)
    sleep 2
fi

# Verificar se Frontend está rodando
if lsof -Pi :5173 -sTCP:LISTEN -t >/dev/null ; then
    echo -e "${YELLOW}⚠️  Porta 5173 já está em uso${NC}"
    echo "Matando processo..."
    kill -9 $(lsof -t -i:5173)
    sleep 2
fi

echo ""
echo "════════════════════════════════════════════════════════"
echo -e "${BLUE}🔧 Iniciando API Backend (C#)...${NC}"
echo "════════════════════════════════════════════════════════"
echo ""

cd /home/david/dev/cvsl/src/api

# Iniciar API em background
dotnet run --urls="http://localhost:5000" > /tmp/cvsl_api.log 2>&1 &
API_PID=$!

echo "API PID: $API_PID"
echo "Aguardando API iniciar..."
sleep 8

# Verificar se API iniciou
if lsof -Pi :5000 -sTCP:LISTEN -t >/dev/null ; then
    echo -e "${GREEN}✅ API rodando em http://localhost:5000${NC}"
    echo -e "   📄 Swagger: http://localhost:5000/swagger"
else
    echo -e "${YELLOW}❌ Erro ao iniciar API${NC}"
    cat /tmp/cvsl_api.log
    exit 1
fi

echo ""
echo "════════════════════════════════════════════════════════"
echo -e "${BLUE}🌐 Iniciando Frontend (Vue.js)...${NC}"
echo "════════════════════════════════════════════════════════"
echo ""

cd /home/david/dev/cvsl/src/web

# Iniciar frontend em background
npm run dev > /tmp/cvsl_frontend.log 2>&1 &
FRONTEND_PID=$!

echo "Frontend PID: $FRONTEND_PID"
echo "Aguardando frontend iniciar..."
sleep 5

# Verificar se frontend iniciou
if lsof -Pi :5173 -sTCP:LISTEN -t >/dev/null ; then
    echo -e "${GREEN}✅ Frontend rodando em http://localhost:5173${NC}"
else
    echo -e "${YELLOW}❌ Erro ao iniciar frontend${NC}"
    cat /tmp/cvsl_frontend.log
    kill $API_PID
    exit 1
fi

echo ""
echo "════════════════════════════════════════════════════════"
echo -e "${GREEN}🎉 TUDO PRONTO!${NC}"
echo "════════════════════════════════════════════════════════"
echo ""
echo "📍 URLs:"
echo "   • Frontend:  http://localhost:5173"
echo "   • API:       http://localhost:5000"
echo "   • Swagger:   http://localhost:5000/swagger"
echo ""
echo "🔑 Credenciais de Teste:"
echo ""
echo "   Clínica:"
echo "   📧 clinica@teste.pt"
echo "   🔒 teste123"
echo ""
echo "   Veterinário:"
echo "   📧 vet@teste.pt"
echo "   🔒 teste123"
echo ""
echo "   Rececionista:"
echo "   📧 rececionista@teste.pt"
echo "   🔒 teste123"
echo ""
echo "════════════════════════════════════════════════════════"
echo ""
echo "💡 Dicas:"
echo "   • Logs da API: tail -f /tmp/cvsl_api.log"
echo "   • Logs Frontend: tail -f /tmp/cvsl_frontend.log"
echo ""
echo "🛑 Para parar tudo:"
echo "   • kill $API_PID $FRONTEND_PID"
echo "   • ou pressiona Ctrl+C e depois executa:"
echo "     pkill -f 'dotnet.*api' && pkill -f 'vite'"
echo ""
echo "════════════════════════════════════════════════════════"
echo ""
echo "Pressione Ctrl+C para parar os servidores..."
echo ""

# Guardar PIDs em arquivo
echo "$API_PID $FRONTEND_PID" > /tmp/cvsl_pids.txt

# Esperar por Ctrl+C
trap ctrl_c INT

function ctrl_c() {
    echo ""
    echo "════════════════════════════════════════════════════════"
    echo "🛑 Parando servidores..."
    echo "════════════════════════════════════════════════════════"
    kill $API_PID 2>/dev/null
    kill $FRONTEND_PID 2>/dev/null
    pkill -f 'dotnet.*api' 2>/dev/null
    pkill -f 'vite' 2>/dev/null
    echo "✅ Servidores parados"
    echo ""
    exit 0
}

# Manter script rodando
wait
