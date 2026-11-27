import baseUrl from "../common/baseUrl";
import fetchError from "../common/fetchError";

export default async function fetchDeleteImage(id: string) {
  const uri = baseUrl + `/images/delete/${id}`;
  const response = await fetch(uri, {
    method: "DELETE",
  });
  if (!response.ok) {
    throw await fetchError.createAsync(`Failed to delete image`, response);
  }
}
