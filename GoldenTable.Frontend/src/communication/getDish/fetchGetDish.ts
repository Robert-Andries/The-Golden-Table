import type { dish } from "../../types/dish";
import baseUrl from "../common/baseUrl";
import fetchError from "../common/fetchError";

export async function fetchGetDish(id: string, signal?: AbortSignal) {
  const requestUrl = baseUrl + "/dishes/id/" + id;
  const response = await fetch(requestUrl, { signal });

  const body = await response.json();
  if(!response.ok) {
    throw await fetchError.createAsync(`An error occured while fetching the dish`, response);
  }

  const output = body as dish;
  return output;
}
