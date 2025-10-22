import { ref, computed, reactive } from 'vue';
import { defineStore } from 'pinia';
import { Aluno } from '@/types/Aluno';

export const useAlunosStore = defineStore('alunos', () => {

    // indicar tipo de array any[] para typescript valores abaixo
    const alunos:Array<Aluno> = reactive([]);
    const aluno:Aluno = reactive(new Aluno(0)); 


    function pesquisa(nome: string) {

        // primeiro limpa valores anteriores
        alunos.length = 0

        // depois pesquisa novos dados e insere no array (mock)
        for (var i = 0; i < 20; i++) {
            const aluno = new Aluno(i);
            aluno.nome = nome + " " + i.toString();
            aluno.numero = i;
            aluno.turma = "Turma X"; 
            alunos.push(aluno)
        }

    }

    function getAluno (id: number): void {

        const i = alunos.findIndex((x) => x.id == id);
        if (i>=0) {
            aluno.nome = alunos[i].nome;
            aluno.numero =  alunos[i].numero;
            aluno.turma =  alunos[i].turma;
            aluno.morada = "Rua tal e tal, "+i.toString();
        }
    }

    return { pesquisa, getAluno, alunos, aluno }
})