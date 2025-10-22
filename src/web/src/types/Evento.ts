export class Evento {
    public id: number;
    public titulo!: string;
    public data!: Date;
    public disciplina!: string;
    public modulo!: string;
    
    constructor (id: number) {
        this.id = id;
    }
}