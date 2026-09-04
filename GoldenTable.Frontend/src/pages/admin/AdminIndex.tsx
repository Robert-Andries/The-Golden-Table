import { useEffect, useState, type ReactNode } from "react";
import { NavLink, useLocation } from "react-router-dom";

import placeholderImg from "../../assets/default-placeholder-food.png"
import useGetAllDishes from "../../communication/getAllDishes/useGetAllDishes";
import styles from "./AdminIndex.module.css";

export default function AdminIndex() : ReactNode {
  const { data: dishes, isPending, isError, error } = useGetAllDishes();
  const location = useLocation();
  const locationState = location.state as { message?: string };
  const [notification, setNotification] = useState<string | undefined>(locationState?.message);

  useEffect(() => {
    if (notification) {
      window.history.replaceState({}, '');
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
    return <div className={styles.error}>{`An error occured while getting dishes, ${error.message}`}</div>;
  }
  if(dishes.length === 0){ 
    return <div className={styles.emptyState}>No dishes found!</div>
  }

  return (
    <div className={styles.container}>
      {notification && (
        <div style={{ backgroundColor: '#4CAF50', color: 'white', padding: '1rem', textAlign: 'center', position: 'fixed', top: 0, left: 0, right: 0, zIndex: 1000, transition: 'opacity 0.3s ease-in-out' }}>
          {notification}
        </div>
      )}
      <div className={styles.header}>
        <h1 className={styles.title}>Admin Dashboard</h1>
        <NavLink to={`images`} className={styles.addButton}>
          Manage Images
        </NavLink>
        <NavLink to={`tags`} className={styles.addButton}>
          Manage Tags
        </NavLink>
        <NavLink to={`add`} className={styles.addButton}>
          Add a new dish
        </NavLink>
      </div>
      <div className={styles.list}>
        {dishes.map((dish) => (
          <div key={dish.id} className={styles.row}>
            <img className={styles.image} src={dish.imagesUris?.[0] || placeholderImg} alt={dish.name} />
            <div className={styles.details}>
              <h2 className={styles.name}>{dish.name}</h2>
              <div className={styles.tags}>
                {dish.tags?.map((tag) => (
                  <h4 key={tag} className={styles.tag}>{tag}</h4>
                ))}
              </div>
            </div>
            <NavLink to={`edit/${dish.id}`} className={styles.editButton}>
              Edit
            </NavLink>
          </div>
        ))}
      </div>
    </div>
  );
}