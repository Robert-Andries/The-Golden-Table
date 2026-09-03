import { useState, type ReactNode } from "react";
import { useParams } from "react-router-dom";

import { ApplicationError } from "../../common/ApplicationError";

import useGetDish from "../../communication/getDish/useGetDish";
import placeholderImg from "../../assets/default-placeholder-food.png";
import NutritionalInformation from "../../components/NutritionalInformation";
import styles from "./DishItem.module.css";
import type { dishResponse } from "../../communication/common/dishResponse";

type props = {
  dish: dishResponse;
};

function DisplayDishItem({ dish }: props): ReactNode {
  const [selectedDishImage, setSelectedDishImage] = useState<string>(
    dish.imagesUris.at(0) || "",
  );

  function handleSelectImage(image: string) {
    if (image !== selectedDishImage) {
      setSelectedDishImage(image);
    }
  }

  return (
    <div className={styles.container}>
      <div className={styles.imageContainer}>
        {dish.imagesUris.length === 0 ? (
          <img className={styles.image} src={placeholderImg} alt={dish.name} />
        ) : (
          <>
            <img
              className={styles.image}
              src={selectedDishImage}
              alt={dish.name}
            />
            <div className={styles.thumbnails}>
              {dish.imagesUris.map((image) => (
                <img
                  key={image}
                  src={image}
                  alt={`Dish thumbnail`}
                  onClick={() => handleSelectImage(image)}
                  className={`${styles.thumbnail} ${image === selectedDishImage ? styles.thumbnailActive : ""}`}
                />
              ))}
            </div>
          </>
        )}
      </div>
      <div className={styles.infoSection}>
        <div className={styles.details}>
          <h1 className={styles.title}>{dish.name}</h1>
          <p className={styles.description}>{dish.description}</p>
          <div className={styles.tags}>
            {dish.tags?.map((tag) => (
              <h4 key={tag} className={styles.tag}>
                {tag}
              </h4>
            ))}
          </div>
          <h3 className={styles.price}>
            {dish.basePriceAmount} {dish.basePriceCurrency}
          </h3>
        </div>
        <NutritionalInformation nutritional={dish.nutritionalInformation} />
      </div>
    </div>
  );
}

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
  return <DisplayDishItem dish={data} />;
}
