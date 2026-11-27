import baseUrl from "../common/baseUrl";
import FetchError from "../common/fetchError";
import type { response as getAllTagsResponse } from "./response";

export async function fetchAllTags(signal?: AbortSignal) {
  const requestUrl = baseUrl + "/dishes/tags/get-all";
  const response = await fetch(requestUrl, { signal });

  const info = await response.json();
  if(!response.ok) {
    throw await FetchError.createAsync(`An error occuredwhile fetching the dish tags`, response);
  }

  const output = info as getAllTagsResponse;
  return output;
}
