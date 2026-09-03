import { ApplicationError } from "../../common/ApplicationError";
import baseUrl from "../common/baseUrl";

export default async function fetchDeleteTag(tagId: string) {
  const url = baseUrl + `/dishes/tags/delete/${tagId}`;
  const response = await fetch(url, {
    method: "DELETE"
  });

  if(response.ok === false) {
    let errorMessage = "unknown error";
    let status = response.status ?? 500;
    try {
      const responseBody = await response.json();
      errorMessage = responseBody.detail ?? errorMessage;
      status = responseBody.status ?? status;
    } catch (e) {
      // ignore
    }
    throw new ApplicationError(`Could not delete tag: ${errorMessage}`, status);
  }
}