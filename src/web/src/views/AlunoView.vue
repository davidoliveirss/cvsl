<script setup lang="ts">
import router from '@/router';
import { useAlunosStore } from '@/stores/alunos';
import { QInput } from 'quasar';
import { onMounted, reactive, ref } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();
const alunosStore = useAlunosStore();

onMounted(() => {
    const id:number = parseInt(route.params.id as string);
    console.log("aluno:", id)
    
    alunosStore.getAluno(id);   
})

function voltarPesquisa() {
    router.back()
}

</script>
<template>
<div>
    <h4>Ficha de aluno</h4>
    <q-input v-model="alunosStore.aluno.nome" label="Nome do aluno" />
    <q-input v-model="alunosStore.aluno.numero" label="Numero" />
    <q-input v-model="alunosStore.aluno.turma" label="Turma" />
    <q-input v-model="alunosStore.aluno.morada" label="Morada" />
    <div class="q-pa-md float-right">
        <q-btn icon="arrow_back" color="negative" @click="voltarPesquisa()">Voltar</q-btn>
        <q-btn icon="done" color="positive">Gravar</q-btn>
    </div>
</div>
</template>