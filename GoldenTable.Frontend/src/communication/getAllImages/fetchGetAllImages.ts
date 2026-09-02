import baseUrl from "../common/baseUrl";
import fetchError from "../common/fetchError";
import type { response as getAllImagesResponse } from "./response";

export async function fetchGetAllImages(signal?: AbortSignal) {
  const requestUrl = baseUrl + "/images/get-all";
  const response = await fetch(requestUrl, { signal });

  const info = await response.json();
  if (!response.ok) {
    throw new fetchError(
      "An error occurred while fetching all the images!",
      response.status,
      info
    );
  }

  const output = info as getAllImagesResponse;
  return output;
}
