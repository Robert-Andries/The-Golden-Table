import { ApplicationError } from "../../common/ApplicationError";
import baseUrl from "../common/baseUrl";
import type { editImagePayload } from "./editImagePayload";

export async function fetchEditImage(id: string, payload: editImagePayload) {
  const url = baseUrl + `/images/edit/${id}`;

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
