import { ApplicationError } from "../../common/ApplicationError";
import baseUrl from "../common/baseUrl";

export async function fetchCreateDish(payload: unknown) {
  const url = baseUrl + `/dishes/create`;

  const response = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    const body = await response.json().catch(() => ({}));
    throw new ApplicationError(`Failed to create dish. ${body.detail ?? ""}`, response.status);
  }
}