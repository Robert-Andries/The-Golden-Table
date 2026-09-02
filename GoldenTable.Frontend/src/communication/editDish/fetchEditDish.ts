import { ApplicationError } from "../../common/ApplicationError";
import baseUrl from "../common/baseUrl";

export async function fetchEditDish(id: string, payload: unknown) {
  const url = baseUrl + `/dishes/edit/${id}`;

  const response = await fetch(url, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    const body = await response.json().catch(() => ({}));
    throw new ApplicationError(`Failed to update dish state. ${body.detail ?? ''}`, response.status);
  }
}
