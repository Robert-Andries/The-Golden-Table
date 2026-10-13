import baseUrl from "../common/baseUrl";
import FetchError from "../common/fetchError";
import apiFetch from "../common/apiFetch";
import type { response as getAllImagesResponse } from "./response";

export async function fetchGetAllImages(signal?: AbortSignal) {
  const requestUrl = baseUrl + "/images/get-all";
  const response = await apiFetch(requestUrl, { signal });

  if (!response.ok) {
    throw await FetchError.createAsync('An error occurred while fetching all the images', response);
  }

  const info = await response.json();
  const output = info as getAllImagesResponse;
  return output;
}
