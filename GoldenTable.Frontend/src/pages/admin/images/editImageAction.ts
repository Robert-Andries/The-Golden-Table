import { hasMinLenght, isValidUri } from "../../../common/Validators";
import queryClient from "../../../communication/common/queryClient";
import type { editImagePayload } from "../../../communication/editImage/editImagePayload";
import { fetchEditImage } from "../../../communication/editImage/fetchEditImage";


type actionType = {
  errors: string[],
  name: string,
  description: string,
  uri: string
}

export default async function actionFunction(_prevState: unknown, formData : FormData, imageId : string, onClose : () => void) : Promise<actionType> {
    const errors : string[] = [];
    const name = formData.get("Name") as string;
    const description = formData.get("Description") as string;
    const uri = formData.get("Uri") as string;

    if(hasMinLenght(name, 3) === false) errors.push("The name should have at least 3 characters.");
    if(hasMinLenght(description, 10) === false) errors.push("The description should have at least 10 characters.");
    if(isValidUri(uri) === false) errors.push("The uri is invalid.");

    const output : actionType = {
      errors,
      name,
      description,
      uri
    };

    if(errors.length > 0)
      return output;

    const payload : editImagePayload = {
      name,
      description,
      uri
    }

    try {
      await fetchEditImage(imageId, payload);
      queryClient.invalidateQueries({queryKey: ['images']});
      onClose();
    }
    catch(error : unknown) {
      const errorMessage = error instanceof Error ? error.message : "Failed to update dish";
      output.errors.push(errorMessage);
    }

    return output;
  }