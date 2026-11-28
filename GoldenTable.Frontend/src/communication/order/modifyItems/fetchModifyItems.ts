import baseUrl from "../../common/baseUrl";
import apiFetch from "../../common/apiFetch";
import FetchError from "../../common/fetchError";
import type { modifyItemsPayload } from "./modifyItemsPayload";

export default async function fetchModifyItems(payload: modifyItemsPayload): Promise<void> {
  const url = baseUrl + "/orders/modify-items";

  const response = await apiFetch(url, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    throw await FetchError.createAsync("An error occurred while trying to modify the order items", response);
  }
}
