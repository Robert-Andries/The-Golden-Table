import baseUrl from "../../common/baseUrl";
import apiFetch from "../../common/apiFetch";
import FetchError from "../../common/fetchError";
import type { response as getOrdersByCustomerResponse } from "./response";

export default async function fetchGetOrdersByCustomer(signal?: AbortSignal): Promise<getOrdersByCustomerResponse> {
  const url = baseUrl + "/orders/my-orders";

  const response = await apiFetch(url, { signal });

  if (!response.ok) {
    throw await FetchError.createAsync("An error occurred while fetching your orders", response);
  }

  const body = await response.json();
  return body as getOrdersByCustomerResponse;
}
