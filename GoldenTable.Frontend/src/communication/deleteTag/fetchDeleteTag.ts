import baseUrl from "../common/baseUrl";
import fetchError from "../common/fetchError";

export default async function fetchDeleteTag(tagId: string) {
  const url = baseUrl + `/dishes/tags/delete/${tagId}`;
  const response = await fetch(url, {
    method: "DELETE"
  });

  if(response.ok === false) {
    throw await fetchError.createAsync(`Could not delete tag`, response);
  }
}