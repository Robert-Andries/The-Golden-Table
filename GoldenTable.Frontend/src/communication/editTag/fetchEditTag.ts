import { ApplicationError } from "../../common/ApplicationError";
import baseUrl from "../common/baseUrl";
import type { dishTag } from "../getAllTags/response";

type requestBody = {
  value: string;
};

export default async function fetchEditTag(id: string, tag: dishTag) {
  const url = baseUrl + `/dishes/tags/edit/${id}`;
  const requestBody: requestBody = {
    value: tag.value,
  };

  const response = await fetch(url, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(requestBody)
  });

  if (!response.ok) {
    let errorMessage = "unknown error";
    try {
      const responseBody = await response.json();
      errorMessage = responseBody.detail ?? "unknown error";
    } catch (e) {
      // response might be empty or not JSON
    }
    throw new ApplicationError(
      `An error occured while editing tag: ${errorMessage}`,
      response.status,
    );
  }
}
