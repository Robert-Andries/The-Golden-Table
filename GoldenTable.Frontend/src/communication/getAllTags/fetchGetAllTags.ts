import baseUrl from "../common/baseUrl";
import fetchError from "../common/fetchError";
import type { response as getAllTagsResponse } from "./response";

export async function fetchAllTags(signal?: AbortSignal) {
  const requestUrl = baseUrl + "/dishes/tags/get-all";
  const response = await fetch(requestUrl, { signal });

  const info = await response.json();
  if(!response.ok) {
    throw new fetchError('An error occuredwhile fetching the dish tags!', response.status, info);
  }

  const output = info as getAllTagsResponse;
  return output;
}
