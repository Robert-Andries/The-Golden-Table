import type { ReactNode } from "react";
import { NavLink } from "react-router-dom";

import useGetAllDishes from "../../communication/getAllDishes/useGetAllDishes";
import placeholderImg from "../../assets/default-placeholder-food.png";
import styles from "./Shop.module.css";

function Shop(): ReactNode {
  const { data: dishes, isPending, isError, error } = useGetAllDishes();

  if (isPending) {
    return <div className={styles.loading}>Loading dishes, please wait...</div>;
  }
  if (isError) {
    return <div className={styles.error}>{`An error occured while getting dishes, ${error.message}`}</div>;
  }
  if(dishes.length === 0){ 
    return <div className={styles.emptyState}>No dishes found!</div>
  }

  return (
    <div className={styles.container}>
      {dishes.map((dish) => (
        <div key={dish.id} className={styles.card}>
          <img className={styles.image} src={dish.imagesUris?.[0] || placeholderImg} alt={dish.name} />
          <div className={styles.content}>
            <h2 className={styles.title}>{dish.name}</h2>
            <div className={styles.tags}>
              {dish.tags?.map((tag) => (
                <h4 key={tag} className={styles.tag}>{tag}</h4>
              ))}
            </div>
            <NavLink to={`${dish.id}`} className={styles.link}>
              View Details
            </NavLink>
          </div>
        </div>
      ))}
    </div>
  );
}

export default Shop;
