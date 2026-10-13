import type { dish } from "../../types/dish";
import baseUrl from "../common/baseUrl";
import fetchError from "../common/fetchError";
import apiFetch from "../common/apiFetch";

export async function fetchGetDish(id: string, signal?: AbortSignal) {
  const requestUrl = baseUrl + "/dishes/id/" + id;
  const response = await apiFetch(requestUrl, { signal });

  if (!response.ok) {
    throw await fetchError.createAsync(`An error occurred while fetching the dish`, response);
  }

  const body = await response.json();
  const output = body as dish;
  return output;
}
