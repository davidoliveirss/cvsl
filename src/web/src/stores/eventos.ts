import { ref, computed, reactive } from 'vue';
import { defineStore } from 'pinia';
import { Evento } from '@/types/Evento';

export const useEventosStore = defineStore('eventos', () => {

    // indicar tipo de array any[] para typescript valores abaixo
    const eventos:Array<Evento> = reactive([]);
    const evento:Evento = reactive(new Evento(0)); 


    function pesquisa(disciplina: string) {

        // primeiro limpa valores anteriores
        eventos.length = 0

        // depois pesquisa novos dados e insere no array (mock)
        for (var i = 0; i < 20; i++) {
            const evento = new Evento(i);
            evento.id = i;
            evento.titulo = i.toString()+" evento";
            evento.data = new Date('2025-04-16');
            evento.disciplina = "Disciplina X"; 
            evento.modulo = "Modulo Y";
            eventos.push(evento)
        }

    }

    function getAluno (id: number): void {

        const i = eventos.findIndex((x) => x.id == id);
        if (i>=0) {
            evento.titulo = eventos[i].titulo;
            evento.disciplina =  eventos[i].disciplina;
            evento.modulo =  eventos[i].modulo;
        }
    }

    return { pesquisa, getAluno, eventos, evento }
})