export default class fetchError extends Error {
    code: number;
    info: string;

    constructor(errorMessage: string, code: number, info: string) {
        super(errorMessage);
        this.code = code;
        this.info = info;
    }
}