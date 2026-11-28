import type {OrderStatus} from "../../../types/orderStatus.ts";
import baseUrl from "../../common/baseUrl.ts";
import apiFetch from "../../common/apiFetch.ts";
import FetchError from "../../common/fetchError.ts";

type requestBody = {
  orderId: string;
  newOrderStatus: OrderStatus;
};

export default async function fetchChangeStatus(orderId: string, status: OrderStatus): Promise<void> {
  const url = baseUrl + "/orders/change-status";
  const requestBody: requestBody = {
    orderId: orderId,
    newOrderStatus: status
  };
  
  const response = await apiFetch(url, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(requestBody)
  });
  
  if(!response.ok) {
    throw await FetchError.createAsync("An error occured while trying to edit status", response);
  }
}