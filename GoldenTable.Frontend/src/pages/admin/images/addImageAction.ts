import { hasMinLenght, isValidUri } from "../../../common/validators";
import queryClient from "../../../communication/common/queryClient";
import type { createImagePayload } from "../../../communication/createImage/createImagePayload";
import { fetchCreateImage } from "../../../communication/createImage/fetchCreateImage";

type actionType = {
  errors: string[],
  name: string,
  description: string,
  uri: string
}

export default async function actionFunction(_prevState: unknown, formData : FormData, onClose : () => void) : Promise<actionType> {
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

    const payload : createImagePayload = {
      name,
      description,
      uri
    }

    try {
      await fetchCreateImage(payload);
      queryClient.invalidateQueries({queryKey: ['images']});
      onClose();
    }
    catch(error : unknown) {
      const errorMessage = error instanceof Error ? error.message : "Failed to create image";
      output.errors.push(errorMessage);
    }

    return output;
  }
