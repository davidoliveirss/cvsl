<script setup lang="ts">
import SearchBox from '@/components/BarraPesquisas.vue';
import router from '@/router';
import { useAlunosStore } from '@/stores/alunos';
import type { Aluno } from '@/types/Aluno';
import { QTable, type QTableColumn } from 'quasar';

const alunosStore = useAlunosStore();

// QTableColumn é necessário para evitar problema de typescript
const columns: QTableColumn<any>[] = [
  { name: 'nome', field: "nome", label: 'Nome do aluno', align: 'left', sortable: true },
  { name: 'numero', field: 'numero', label: 'Numero',  align: 'center', sortable: true },
  { name: 'turma', field: 'turma', label: 'Turma',  align: 'left', sortable: true },
]

function search(s:string) {
    alunosStore.pesquisa(s);
}

function abrirAluno(evt:Event, row:Object, index:number) {
  //console.log(evt, row, index )

  const id = (row as Aluno).id;

  router.push("aluno/"+id.toString())

}

</script>
<template>

    <title>Teste</title>

    <SearchBox texto="" @onsearch="search"/>

    <div class="q-pa-md">
    <q-table
      title="Alunos"
      :rows="alunosStore.alunos"
      :columns="columns"
      row-key="id"
      @row-dblclick="abrirAluno"
    />
</div>
  
</template>