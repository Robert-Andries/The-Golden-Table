import baseUrl from "../common/baseUrl";
import fetchError from "../common/fetchError";

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
    throw await fetchError.createAsync('Failed to create dish',response);
  }
}