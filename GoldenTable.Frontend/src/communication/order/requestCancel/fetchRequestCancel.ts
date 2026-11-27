import baseUrl from "../../common/baseUrl.ts";
import apiFetch from "../../common/apiFetch.ts";
import fetchError from "../../common/fetchError.ts";

export default async function fetchRequestCancel(orderId: string): Promise<void> {
    const url = baseUrl + `/orders/request-cancel/${orderId}`;

    const response = await apiFetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        }
    });
    if (!response.ok) {
        throw await fetchError.createAsync(`An error occured trying to request the cancellation of specified order`, response);
    }
} 