import baseUrl from "../../common/baseUrl";
import apiFetch from "../../common/apiFetch";
import FetchError from "../../common/fetchError";
import type { response as getOrdersResponse } from "./response";

export default async function fetchGetOrders(signal?: AbortSignal): Promise<getOrdersResponse> {
  const url = baseUrl + "/orders/get-all";

  const response = await apiFetch(url, { signal });

  if (!response.ok) {
    throw await FetchError.createAsync("An error occurred while fetching all the orders", response);
  }

  const body = await response.json();
  return body as getOrdersResponse;
}
