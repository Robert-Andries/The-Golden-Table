import baseUrl from "../common/baseUrl";
import fetchError from "../common/fetchError";
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
    throw await fetchError.createAsync(`Failed to update dish state`, response);
  }
}
