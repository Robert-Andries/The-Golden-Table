import { fetchEditDish } from "../../../communication/editDish/fetchEditDish";
import {
  hasMinLenght,
  isGreaterThen0,
  isValidCurrency,
} from "../../../common/validators";
import { getInputNutritionsFromFormData } from "../../../components/EditableNutritionalInformation";
import type { dish, nutritionalInformation } from "../../../types/dish";
import type { dishTag } from "../../../types/dishTag";

export type actionObject = {
  errors: string[] | null;
  success?: boolean;
  enteredValues: {
    name: string;
    description: string;
    basePriceAmount: number;
    basePriceCurrency: string;
    nutritionalInformation: nutritionalInformation;
  };
};

export async function editAction(
  prevFormState: unknown,
  formData: FormData,
  originalData: dish,
  activeTags: string[],
  tagData: dishTag[],
  imageIds: string[]
): Promise<actionObject> {
  if(prevFormState !== FormData) {
    // TODO return if that is the case
  }

  const errors: string[] = [];
  const nameValue = formData.get("Name") as string;
  const descriptionValue = formData.get("Description") as string;
  const basePriceValue = Number(formData.get("Base price amount"));
  const basePriceCurrencyValue = formData.get("Base price currency") as string;
  const nutritionalInfoValue = getInputNutritionsFromFormData(formData);

  if (hasMinLenght(nameValue, 3) === false)
    errors.push("Name has to be at least 3 characters long!");
  if (hasMinLenght(descriptionValue, 5) === false)
    errors.push("Description has to be at least 5 characters long!");
  if (isGreaterThen0(basePriceValue) === false)
    errors.push("Base price has to be at least 0!");
  if (isValidCurrency(basePriceCurrencyValue) === false)
    errors.push("Currency is not valid!");
  if (nutritionalInfoValue.errors.length > 0)
    errors.push(...nutritionalInfoValue.errors);

  const output: actionObject = {
    errors,
    success: false,
    enteredValues: {
      name: nameValue,
      description: descriptionValue,
      basePriceAmount: basePriceValue,
      basePriceCurrency: basePriceCurrencyValue,
      nutritionalInformation: nutritionalInfoValue.enteredValues
    },
  };

  if (errors.length > 0) {
    return output;
  }
  
  const tagIds = activeTags
    .map(tagValue => tagData.find(t => t.value === tagValue)?.id)
    .filter(id => id !== undefined);

  const payload = {
    name: nameValue,
    description: descriptionValue,
    basePriceAmount: basePriceValue,
    basePriceCurrency: basePriceCurrencyValue,
    dishSizes: originalData.sizes || [],
    nutritionalInformation: {
      kcal: nutritionalInfoValue.enteredValues.energy.kcal,
      gramsOfFat: nutritionalInfoValue.enteredValues.gramsOfFat,
      gramsOfCarbohydrates: nutritionalInfoValue.enteredValues.gramsOfCarbohydrates.total,
      gramsOfSugar: nutritionalInfoValue.enteredValues.gramsOfCarbohydrates.ofWhichSugar,
      gramsOfProtein: nutritionalInfoValue.enteredValues.gramsOfProtein,
      gramsOfSalt: nutritionalInfoValue.enteredValues.gramsOfSalt
    },
    imageIds: imageIds,
    dishCategory: originalData.category,
    tagIds: tagIds
  };

  try {
    await fetchEditDish(originalData.id, payload);
    output.success = true;
  } catch (error: unknown) {
    const errorMessage = error instanceof Error ? error.message : "Failed to update dish";
    output.errors!.push(errorMessage);
  }

  return output;
}
