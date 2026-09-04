export class ApplicationError extends Error {
    public readonly statusCode: number;
    public readonly innerException?: unknown;

    constructor(message: string, statusCode: number = 500, innerException?: unknown) {
        super(message);
        this.name = "ApplicationError";
        this.statusCode = statusCode;
        this.innerException = innerException;
    }
}
