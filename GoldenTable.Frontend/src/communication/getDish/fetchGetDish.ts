import baseUrl from "../common/baseUrl";
import type { dishResponse } from "../common/dishResponse";
import fetchError from "../common/fetchError";

export async function fetchGetDish(id: string, signal?: AbortSignal) {
  const requestUrl = baseUrl + "/dishes/id/" + id;
  const response = await fetch(requestUrl, { signal });

  const info = await response.json();
  if(!response.ok) {
    throw new fetchError('An error occuredwhile fetching the dish!', response.status, info);
  }

  const output = info as dishResponse;
  return output;
}
