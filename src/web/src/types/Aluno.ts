export class Aluno {
    public id: number;
    public nome!: string;
    public dataNasc!: Date;
    public turma!: string;
    public numero!: number;
    public morada!: string;

    constructor (id: number) {
        this.id = id;
    }
}