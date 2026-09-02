import type { ReactNode } from "react";
import { useParams } from "react-router-dom";

import { ApplicationError } from "../../common/ApplicationError";

import useGetDish from "../../communication/getDish/useGetDish";
import placeholderImg from "../../assets/default-placeholder-food.png";
import NutritionalInformation from "../../components/NutritionalInformation";
import styles from "./DishItem.module.css";

export default function DishItem(): ReactNode {
  const { id } = useParams<{ id: string }>();
  const { data, isPending, isError, error } = useGetDish(id);

  if (!id) {
    throw new ApplicationError("Dish ID is missing from the URL.", 400);
  }

  if (isPending) {
    return <div className={styles.loading}>Loading dish details...</div>;
  }

  if (isError) {
    return <div className={styles.error}>Error: {error.message}</div>;
  }

  return (
    <div className={styles.container}>
      <div className={styles.imageContainer}>
        <img className={styles.image} src={data.imagesUris?.[0] || placeholderImg} alt={data.name || "Dish image"} />
      </div>
      <div className={styles.infoSection}>
        <div className={styles.details}>
          <h1 className={styles.title}>{data.name}</h1>
          <p className={styles.description}>{data.description}</p>
          <div className={styles.tags}>
            {data.tags?.map((tag) => (
              <h4 key={tag} className={styles.tag}>{tag}</h4>
            ))}
          </div>
          <h3 className={styles.price}>
            {data.basePriceAmount} {data.basePriceCurrency}
          </h3>
        </div>
        <NutritionalInformation nutritional={data.nutritionalInformation}/>
      </div>
    </div>
  );
}
