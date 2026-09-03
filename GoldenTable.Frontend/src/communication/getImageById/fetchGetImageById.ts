import { ApplicationError } from "../../common/ApplicationError";
import baseUrl from "../common/baseUrl";
import type { ImageInfo } from "../getAllImages/response";

export default async function fetchGetImageById(id: string) : Promise<ImageInfo> {
  const uri = baseUrl + `/images/get/${id}`;

  const response = await fetch(uri);

  const body = await response.json();
  if(!response.ok) {
    throw new ApplicationError(`Could not fetch image with id ${id}: ${body.detail ?? "unknown error"}`, body.status ?? 500)
  }

  const image = body as ImageInfo;
  return image;
}