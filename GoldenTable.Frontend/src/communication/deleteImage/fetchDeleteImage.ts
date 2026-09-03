import { ApplicationError } from "../../common/ApplicationError";
import baseUrl from "../common/baseUrl";

export default async function fetchDeleteImage(id: string) {
  const uri = baseUrl + `/images/delete/${id}`;
  const response = await fetch(uri, {
    method: "DELETE",
  });
  if (!response.ok) {
    const body = await response.json().catch(() => ({}));
    throw new ApplicationError(
      `Failed to delete image. ${body.detail ?? ""}`,
      response.status,
    );
  }
}
