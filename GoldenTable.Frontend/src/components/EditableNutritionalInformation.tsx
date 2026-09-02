import type { ReactNode } from "react";
import type { nutritionalInformation } from "../communication/common/dishResponse";
import styles from "./EditableNutritionalInformation.module.css";
import Input from "./Input";
import { KCalToKJ } from "../common/Convertors";
import { isGreaterOrEqualTo0 } from "../common/Validators";

type NutritionalProps = {
  nutritional: nutritionalInformation;
};

type nutritionalFormData = {
  errors: string[],
  enteredValues : nutritionalInformation
}

export default function EditableNutritionalInformation({
  nutritional,
}: NutritionalProps): ReactNode {
  return (
    <div className={styles.container}>
      <h2 className={styles.title}>Nutritional Information per 100g</h2>

      <div className={styles.grid}>
        <Input
          name="Energy (Kcal)"
          type="number"
          defaultText={nutritional.energy.kcal}
        />
        <Input
          name="Carbohydrates (g)"
          type="number"
          defaultText={nutritional.gramsOfCarbohydrates.total}
        />
        <Input
          name="Of which sugars (g)"
          type="number"
          defaultText={nutritional.gramsOfCarbohydrates.ofWhichSugar}
        />
        <Input
          name="Fats (g)"
          type="number"
          defaultText={nutritional.gramsOfFat}
        />
        <Input
          name="Protein (g)"
          type="number"
          defaultText={nutritional.gramsOfProtein}
        />
        <Input
          name="Salt (g)"
          type="number"
          defaultText={nutritional.gramsOfSalt}
        />
      </div>
    </div>
  );
}

export function getInputNutritionsFromFormData(
  formdata: FormData,
): nutritionalFormData {

  const errors : string[] = [];

  const energyValue = Number(formdata.get("Energy (Kcal)"));
  const carbohydratesValue = Number(formdata.get("Carbohydrates (g)"));
  const sugarValue = Number(formdata.get("Of which sugars (g)"));
  const fatsValue = Number(formdata.get("Fats (g)"));
  const proteinValue = Number(formdata.get("Protein (g)"));
  const saltValue = Number(formdata.get("Salt (g)"));

  if(isGreaterOrEqualTo0(energyValue) === false) errors.push("Energy field is invalid!");
  if(isGreaterOrEqualTo0(carbohydratesValue) === false) errors.push("Total carbohydrates field is invalid!");
  if(isGreaterOrEqualTo0(sugarValue) === false) errors.push("Sugar field is invalid!");
  if(isGreaterOrEqualTo0(fatsValue) === false) errors.push("Fats field is invalid!");
  if(isGreaterOrEqualTo0(proteinValue) === false) errors.push("Protein field is invalid!");
  if(isGreaterOrEqualTo0(saltValue) === false) errors.push("Salt field is invalid!");

  const output: nutritionalFormData = {
    errors,
    enteredValues: {energy: {
      kcal: energyValue!,
      kj: KCalToKJ(energyValue!),
    },
    gramsOfFat: fatsValue!,
    gramsOfCarbohydrates: {
      total: carbohydratesValue!,
      ofWhichSugar: sugarValue!,
    },
    gramsOfProtein: proteinValue!,
    gramsOfSalt: saltValue!,
  }};

  return output;
}
