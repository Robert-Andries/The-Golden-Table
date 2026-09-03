import { ApplicationError } from "../../common/ApplicationError";
import baseUrl from "../common/baseUrl";
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
    const body = await response.json().catch(() => ({}));
    throw new ApplicationError(`Failed to create image. ${body.detail ?? ''}`, response.status);
  }
}
