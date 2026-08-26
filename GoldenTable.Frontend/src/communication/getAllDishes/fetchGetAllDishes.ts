import baseUrl from "../common/baseUrl";
import fetchError from "../common/fetchError";
import type { response as getAllResponse } from "./response";

export async function fetchGetAllDishes(signal?: AbortSignal) {
  const requestUrl = baseUrl + "/dishes/get-all";
  const response = await fetch(requestUrl, { signal });

  const info = await response.json();
  if(!response.ok) {
    throw new fetchError('An error occuredwhile fetching all the dishes!', response.status, info);
  }

  const output = info as getAllResponse;
  return output;
}
