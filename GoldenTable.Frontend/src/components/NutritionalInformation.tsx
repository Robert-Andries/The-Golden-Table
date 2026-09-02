import type { ReactNode } from "react";
import type { nutritionalInformation } from "../communication/common/dishResponse";
import styles from "./NutritionalInformation.module.css";

type NutritionalProps = {
    nutritional: nutritionalInformation;
};

export default function NutritionalInformation({ nutritional }: NutritionalProps) : ReactNode {
  return (
    <div className={styles.container}>
      <h2 className={styles.title}>Nutritional Information per 100g</h2>
      
      <div className={styles.list}>
        <div className={styles.item}>
          <h3 className={styles.label}>Energy</h3>
          <p className={styles.value}>{nutritional.energy.kcal} Kcal / {nutritional.energy.kj} KJ</p>
        </div>

        <div className={styles.item}>
          <h3 className={styles.label}>Carbohydrates</h3>
          <p className={styles.value}>{nutritional.gramsOfCarbohydrates.total}g</p>
        </div>
        <div className={styles.subItem}>
          <h3 className={styles.label}>Of which sugars</h3>
          <p className={styles.value}>{nutritional.gramsOfCarbohydrates.ofWhichSugar}g</p>
        </div>

        <div className={styles.item}>
          <h3 className={styles.label}>Fats</h3>
          <p className={styles.value}>{nutritional.gramsOfFat}g</p>
        </div>

        <div className={styles.item}>
          <h3 className={styles.label}>Protein</h3>
          <p className={styles.value}>{nutritional.gramsOfProtein}g</p>
        </div>

        <div className={styles.item}>
          <h3 className={styles.label}>Salt</h3>
          <p className={styles.value}>{nutritional.gramsOfSalt}g</p>
        </div>
      </div>
    </div>
  );
}