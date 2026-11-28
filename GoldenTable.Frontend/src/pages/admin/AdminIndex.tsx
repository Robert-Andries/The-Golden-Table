import { useEffect, useState, type ReactNode } from "react";
import { NavLink, useLocation } from "react-router-dom";

import placeholderImg from "../../assets/default-placeholder-food.png";
import useGetAllDishes from "../../communication/getAllDishes/useGetAllDishes";
import { Can } from "../../auth/Can";
import styles from "./AdminIndex.module.css";

export default function AdminIndex(): ReactNode {
  const { data: dishes, isPending, isError, error } = useGetAllDishes();
  const location = useLocation();
  const locationState = location.state as { message?: string };
  const [notification, setNotification] = useState<string | undefined>(
    locationState?.message
  );

  useEffect(() => {
    if (notification) {
      window.history.replaceState({}, "");
      const timer = setTimeout(() => {
        setNotification(undefined);
      }, 3000);
      return () => clearTimeout(timer);
    }
  }, [notification]);

  if (isPending) {
    return <div className={styles.loading}>Loading dishes, please wait...</div>;
  }
  if (isError) {
    return (
      <div className={styles.error}>{`An error occurred while getting dishes, ${error.message}`}</div>
    );
  }
  if (!dishes || dishes.length === 0) {
    return <div className={styles.emptyState}>No dishes found!</div>;
  }

  return (
    <div className={styles.container}>
      {notification && (
        <div className={styles.notification}>
          {notification}
        </div>
      )}
      <div className={styles.header}>
        <h1 className={styles.title}>Admin Dashboard</h1>
        <Can permission="dishes:create-image">
          <NavLink to="images" className={styles.addButton}>
            Manage Images
          </NavLink>
        </Can>
        <Can permission="dishes:create-tag">
          <NavLink to="tags" className={styles.addButton}>
            Manage Tags
          </NavLink>
        </Can>
        <Can permission="dishes:create">
          <NavLink to="add" className={styles.addButton}>
            Add a new dish
          </NavLink>
        </Can>
      </div>
      <div className={styles.list}>
        {dishes.map((dish) => (
          <div key={dish.id} className={styles.row}>
            <img
              className={styles.image}
              src={dish.imagesUris?.[0] || placeholderImg}
              alt={dish.name}
            />
            <div className={styles.details}>
              <h2 className={styles.name}>{dish.name}</h2>
              <div className={styles.tags}>
                {dish.tags?.map((tag) => (
                  <h4 key={tag} className={styles.tag}>
                    {tag}
                  </h4>
                ))}
              </div>
            </div>
            <Can permission="dishes:edit">
              <NavLink to={`edit/${dish.id}`} className={styles.editButton}>
                Edit
              </NavLink>
            </Can>
          </div>
        ))}
      </div>
    </div>
  );
}