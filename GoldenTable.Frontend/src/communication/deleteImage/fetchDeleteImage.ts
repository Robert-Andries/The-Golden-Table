import baseUrl from "../common/baseUrl";
import fetchError from "../common/fetchError";
import apiFetch from "../common/apiFetch";

export default async function fetchDeleteImage(id: string) {
  const uri = baseUrl + `/images/delete/${id}`;
  const response = await apiFetch(uri, {
    method: "DELETE",
  });
  if (!response.ok) {
    throw await fetchError.createAsync(`Failed to delete image`, response);
  }
}
