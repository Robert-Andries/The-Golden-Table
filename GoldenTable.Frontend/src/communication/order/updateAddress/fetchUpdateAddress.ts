import baseUrl from "../../common/baseUrl";
import apiFetch from "../../common/apiFetch";
import FetchError from "../../common/fetchError";
import type { updateAddressPayload } from "./updateAddressPayload";

export default async function fetchUpdateAddress(payload: updateAddressPayload): Promise<void> {
  const url = baseUrl + "/orders/update-address";

  const response = await apiFetch(url, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    throw await FetchError.createAsync("An error occurred while trying to update the order address", response);
  }
}
