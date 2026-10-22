import type { dishTag } from "../../types/dishTag";
import baseUrl from "../common/baseUrl";
import fetchError from "../common/fetchError";
import apiFetch from "../common/apiFetch";

type requestBody = {
  value: string;
};

export default async function fetchEditTag(id: string, tag: dishTag) {
  const url = baseUrl + `/dishes/tags/edit/${id}`;
  const requestBody: requestBody = {
    value: tag.value,
  };

  const response = await apiFetch(url, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(requestBody),
  });

  if (!response.ok) {
    throw await fetchError.createAsync(`An error occured while editing tag`, response);
  }
}
