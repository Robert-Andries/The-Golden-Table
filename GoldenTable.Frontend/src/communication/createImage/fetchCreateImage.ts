import baseUrl from "../common/baseUrl";
import FetchError from "../common/fetchError";
import type { createImagePayload } from "./createImagePayload";

export async function fetchCreateImage(payload: createImagePayload) {
  const url = baseUrl + `/images/create`;

  const response = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    throw await FetchError.createAsync("Failed to create image", response);
  }
}
