import { ApplicationError } from "../../../common/ApplicationError";
import { hasMinLenght } from "../../../common/Validators";
import queryClient from "../../../communication/common/queryClient";
import fetchCreateTag from "../../../communication/createTag/fetchCreateTag";

type actionType = {
  errors: string[],
  name: string
};

export default async function createTagAction(_prevState: unknown, formData : FormData, onClose : () => void) : Promise<actionType> {
  const errors : string[] = [];
  const name = formData.get("Name") as string;
  
  if(hasMinLenght(name, 3) === false) errors.push("The name of the tag must have at least 3 characters!");

  const output = {
    errors,
    name
  };

  if(errors.length > 0) return output;

  try {
    await fetchCreateTag(name);
    queryClient.invalidateQueries({queryKey: ['dishes', 'tags'], exact: true})
    onClose();
  } catch(ex : unknown) {
    if(ex instanceof ApplicationError) {
      errors.push(ex.message);
    }
    else if(ex instanceof Error) {
      errors.push(ex.message);
    }
    else {
      errors.push(`An unknown error occured ${ex}`);
    }
  }

  return output;
}