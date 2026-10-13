import type { ReactNode } from "react";
import placeholderImage from "../../../assets/default-placeholder-food.png";
import styles from "./ImageItem.module.css";
import { isValidUri } from "../../../common/validators";
import type { image } from "../../../types/image";
import { Can } from "../../../auth/Can";

type props = {
  image: image;
  onDelete: (id: string) => void;
  onModify: (id: string) => void;
};

function ImageItem({ image, onDelete, onModify }: props): ReactNode {
  return (
    <div className={styles.card}>
      <img
        className={styles.image}
        src={isValidUri(image.uri) ? image.uri : placeholderImage}
        alt={image.name || "Dish image"}
      />
      <div className={styles.info}>
        <h1 className={styles.name}>{image.name}</h1>
        <p className={styles.description}>{image.description}</p>
      </div>
      <div className={styles.actions}>
        <Can permission="dishes:edit-image">
          <button
            className={styles.modifyButton}
            onClick={() => onModify(image.id)}
          >
            Modify
          </button>
        </Can>
        <Can permission="dishes:delete-image">
          <button
            className={styles.deleteButton}
            onClick={() => onDelete(image.id)}
          >
            Delete
          </button>
        </Can>
      </div>
    </div>
  );
}

export default ImageItem;