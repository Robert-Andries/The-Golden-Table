import { ApplicationError } from "../../../common/ApplicationError";
import { hasMinLenght } from "../../../common/Validators";
import queryClient from "../../../communication/common/queryClient";
import fetchEditTag from "../../../communication/editTag/fetchEditTag";
import type { dishTag } from "../../../communication/getAllTags/response";

type actionType = {
  errors: string[],
  name: string
}

export default async function modifyTagAction(_formState: unknown, formData : FormData, tagId : string, onClose: () => void) {
  const errors : string[] = [];
  const name = formData.get("Name") as string;

  if(hasMinLenght(name, 3) === false) errors.push("The name should have at least 3 characters!");

  const output : actionType = {
    errors,
    name
  }

  if(errors.length > 0) return output;

  const payload : dishTag = {
    id: tagId,
    value:name
  }
  try {
    await fetchEditTag(tagId, payload);
    queryClient.invalidateQueries({queryKey: ['dishes', 'tags']});
    onClose();
  }
  catch(ex : unknown) {
    if(ex instanceof ApplicationError) errors.push(ex.message);
    else if(ex instanceof Error) errors.push(ex.message);
    else errors.push("An unknown error occured");
  }
  return output;
}