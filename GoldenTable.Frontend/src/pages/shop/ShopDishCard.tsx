import { useState, type ReactNode } from "react";
import type { dishResponse } from "../../communication/common/dishResponse";
import placeholderImg from "../../assets/default-placeholder-food.png";
import { NavLink } from "react-router-dom";
import styles from "./ShopDishCard.module.css";

type props = {
  dish: dishResponse;
};

function ShopDishCard({ dish }: props): ReactNode {
  const [displayedImage, setDisplayedImage] = useState<string>(
    dish.imagesUris.at(0) ?? "",
  );

  function handleSelectImage(image: string) {
    if (image !== displayedImage) {
      setDisplayedImage(image);
    }
  }

  return (
    <div key={dish.id} className={styles.card}>
      {dish.imagesUris.length === 0 ? (
        <img src={placeholderImg} alt={dish.name} className={styles.image} />
      ) : (
        <div>
          <img src={displayedImage} alt={dish.name} className={styles.image} />
          <div className={styles.thumbnails}>
            {dish.imagesUris.map((image) => (
              <img
                key={image}
                src={image}
                alt={`Dish thumbnail`}
                onClick={() => handleSelectImage(image)}
                className={`${styles.thumbnail} ${image === displayedImage ? styles.thumbnailActive : ""}`}
              />
            ))}
          </div>
        </div>
      )}

      <div className={styles.content}>
        <h2 className={styles.title}>{dish.name}</h2>
        <div className={styles.tags}>
          {dish.tags?.map((tag) => (
            <h4 key={tag} className={styles.tag}>{tag}</h4>
          ))}
        </div>
        <NavLink to={`${dish.id}`} className={styles.link}>View Details</NavLink>
      </div>
    </div>
  );
}

export default ShopDishCard;
