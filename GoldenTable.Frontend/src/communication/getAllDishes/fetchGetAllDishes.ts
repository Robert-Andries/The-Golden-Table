import baseUrl from "../common/baseUrl";
import FetchError from "../common/fetchError";
import type { response as getAllResponse } from "./response";

export async function fetchGetAllDishes(signal?: AbortSignal) {
  const requestUrl = baseUrl + "/dishes/get-all";
  const response = await fetch(requestUrl, { signal });

  const info = await response.json();
  if(!response.ok) {
    throw await FetchError.createAsync(`An error occuredwhile fetching all the dishes`, response);
  }

  const output = info as getAllResponse;
  return output;
}
