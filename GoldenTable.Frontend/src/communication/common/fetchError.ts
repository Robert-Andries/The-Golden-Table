import { ApplicationError } from "../../common/ApplicationError";

export default class FetchError extends ApplicationError {
  private constructor(message: string, statusCode: number) {
    super(message, statusCode);
  }

  public static async createAsync(message: string, response: Response): Promise<FetchError> {
    let detailMessage = "unknown error";

    try {
      const body: unknown = await response.json();
      if (body !== null && typeof body === "object" && "detail" in body) {
          detailMessage = String((body as Record<string, unknown>).detail);
      }
    } catch {
      detailMessage = "failed to parse response body";
    }

    const finalMessage = `${message}  ${detailMessage}`;
    
    return new FetchError(finalMessage, response.status);
  }
}