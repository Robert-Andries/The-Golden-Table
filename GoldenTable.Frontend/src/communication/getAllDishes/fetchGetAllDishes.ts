import baseUrl from "../common/baseUrl";
import FetchError from "../common/fetchError";
import apiFetch from "../common/apiFetch";
import type { response as getAllResponse } from "./response";

export async function fetchGetAllDishes(signal?: AbortSignal) {
  const requestUrl = baseUrl + "/dishes/get-all";
  const response = await apiFetch(requestUrl, { signal });

  if (!response.ok) {
    throw await FetchError.createAsync(`An error occurred while fetching all the dishes`, response);
  }

  const info = await response.json();
  const output = info as getAllResponse;
  return output;
}
