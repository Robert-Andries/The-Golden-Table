import type { ReactNode } from "react";
import { Link } from "react-router-dom";

import useGetAllDishes from "../../communication/dish/getAllDishes/useGetAllDishes";
import useGetOrdersByCustomer from "../../communication/order/getOrdersByCustomer/useGetOrdersByCustomer";
import OrderList from "./OrderList";
import styles from "./Orders.module.css";

export default function MyOrders(): ReactNode {
  const orders = useGetOrdersByCustomer();
  const dishes = useGetAllDishes();

  let content: ReactNode;
  if (orders.isPending) {
    content = <div className={styles.loading}>Loading your orders...</div>;
  } else if (orders.isError) {
    content = <div className={styles.error}>{`An error occurred while getting your orders, ${orders.error.message}`}</div>;
  } else if (orders.data.length === 0) {
    content = (
      <div className={styles.emptyState}>
        <p>You have not placed any order yet.</p>
        <Link to="/shop" className={styles.primaryBtn}>
          Browse the shop
        </Link>
      </div>
    );
  } else {
    content = <OrderList orders={orders.data} dishes={dishes.data ?? []} />;
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1 className={styles.title}>My orders</h1>
        <Link to="/cart" className={styles.secondaryBtn}>
          Go to cart
        </Link>
      </div>
      {content}
    </div>
  );
}
