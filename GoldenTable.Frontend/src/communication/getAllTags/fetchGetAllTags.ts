import baseUrl from "../common/baseUrl";
import FetchError from "../common/fetchError";
import apiFetch from "../common/apiFetch";
import type { response as getAllTagsResponse } from "./response";

export async function fetchAllTags(signal?: AbortSignal) {
  const requestUrl = baseUrl + "/dishes/tags/get-all";
  const response = await apiFetch(requestUrl, { signal });

  if (!response.ok) {
    throw await FetchError.createAsync(`An error occurred while fetching the dish tags`, response);
  }

  const info = await response.json();
  const output = info as getAllTagsResponse;
  return output;
}
