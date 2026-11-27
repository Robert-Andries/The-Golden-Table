import type { ReactNode } from "react";
import type { OrderStatus } from "../../types/orderStatus";
import styles from "./Order.module.css";

type props = {
  status: OrderStatus;
};

export default function OrderStatusBadge({ status }: props): ReactNode {
  return <span className={`${styles.badge} ${styles[status]}`}>{status}</span>;
}
