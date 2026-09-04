import baseUrl from "../common/baseUrl";
import FetchError from "../common/fetchError";

type requestBody = {
  value: string;
};

export default async function fetchCreateTag(name: string): Promise<void> {
  const url = baseUrl + "/dishes/tags/create";
  const body: requestBody = {
    value: name,
  };
  const response = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    throw await FetchError.createAsync(`An error occured while trying to create a tag`, response);
  }
}
