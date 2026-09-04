import baseUrl from "../common/baseUrl";
import fetchError from "../common/fetchError";

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
    throw await fetchError.createAsync(`Failed to update dish state`, response);
  }
}
