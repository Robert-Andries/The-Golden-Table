import type { dishTag } from "../../types/dishTag";
import baseUrl from "../common/baseUrl";
import fetchError from "../common/fetchError";

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
    body: JSON.stringify(requestBody),
  });

  if (!response.ok) {
    throw await fetchError.createAsync(`An error occured while editing tag`, response);
  }
}
