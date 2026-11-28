import type { order } from "../../../types/order";
import baseUrl from "../../common/baseUrl";
import apiFetch from "../../common/apiFetch";
import FetchError from "../../common/fetchError";
import type { createOrderPayload } from "./createOrderPayload";

export default async function fetchCreateOrder(payload: createOrderPayload): Promise<order> {
  const url = baseUrl + "/orders/create";

  const response = await apiFetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    throw await FetchError.createAsync("An error occurred while trying to create the order", response);
  }

  const body = await response.json();
  return body as order;
}
