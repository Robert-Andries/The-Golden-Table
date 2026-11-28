import type { order } from "../../../types/order";
import baseUrl from "../../common/baseUrl";
import apiFetch from "../../common/apiFetch";
import FetchError from "../../common/fetchError";

export default async function fetchGetMyOrderById(id: string, signal?: AbortSignal): Promise<order> {
  const url = baseUrl + `/orders/my-orders/${id}`;

  const response = await apiFetch(url, { signal });

  if (!response.ok) {
    throw await FetchError.createAsync("An error occurred while fetching the order", response);
  }

  const body = await response.json();
  return body as order;
}
