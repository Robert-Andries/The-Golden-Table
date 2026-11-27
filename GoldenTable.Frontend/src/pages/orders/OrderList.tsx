import type { ReactNode } from "react";
import { Link } from "react-router-dom";

import { describeItem, formatDate, formatMoney, shortId } from "../../common/orders";
import OrderStatusBadge from "../../components/order/OrderStatusBadge";
import orderStyles from "../../components/order/Order.module.css";
import type { dish } from "../../types/dish";
import type { order } from "../../types/order";
import styles from "./Orders.module.css";

type props = {
  orders: order[];
  dishes: dish[];
  /** Shows the customer id on each row, for staff views */
  showCustomer?: boolean;
};

/** Orders, newest first, each linking to its detail page (relative to the current route). */
export default function OrderList({ orders, dishes, showCustomer = false }: props): ReactNode {
  const sorted = [...orders].sort((a, b) => b.createdOnUtc.localeCompare(a.createdOnUtc));

  return (
    <div className={styles.list}>
      {sorted.map((order) => (
        <Link key={order.id} to={order.id} className={styles.row}>
          <div className={styles.rowMain}>
            <h2 className={styles.rowTitle}>Order #{shortId(order.id)}</h2>
            <div className={styles.rowMeta}>
              <OrderStatusBadge status={order.status} />
              {order.cancelRequested && <span className={orderStyles.cancelFlag}>Cancellation requested</span>}
              <span>{formatDate(order.createdOnUtc)}</span>
              {showCustomer && <span>Customer {shortId(order.customerId)}</span>}
            </div>
            <span className={styles.rowItems}>
              {order.items.map((item) => `${item.quantity} × ${describeItem(item, dishes)}`).join(", ")}
            </span>
          </div>
          <span className={styles.rowTotal}>{formatMoney(order.totalPriceAmount, order.totalPriceCurrency)}</span>
        </Link>
      ))}
    </div>
  );
}
