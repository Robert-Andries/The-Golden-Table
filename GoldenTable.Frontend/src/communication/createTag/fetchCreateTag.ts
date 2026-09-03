import { ApplicationError } from "../../common/ApplicationError";
import baseUrl from "../common/baseUrl";

type requestBody = {
  value: string
}

export default async function fetchCreateTag(name : string) : Promise<void> {
  const url = baseUrl + "/dishes/tags/create";
  const body : requestBody = {
    value: name
  }
  const response = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(body)
  })

  if(!response.ok) {
    let errorMessage = "unknown error";
    try {
      const responseBody = await response.json();
      errorMessage = responseBody.detail ?? "unknown error";
    } catch (e) {
      // ignore
    }
    throw new ApplicationError(`An error occured while trying to create a tag. ${errorMessage}`, response.status);
  }
}