import type { image } from "../../types/image";
import baseUrl from "../common/baseUrl";
import FetchError from "../common/fetchError";

export default async function fetchGetImageById(id: string) : Promise<image> {
  const uri = baseUrl + `/images/get/${id}`;
  const response = await fetch(uri);

  if(!response.ok) {
    throw await FetchError.createAsync(`Could not fetch image with id ${id}`, response)
  }

  const body = await response.json();
  const image = body as image;
  return image;
}