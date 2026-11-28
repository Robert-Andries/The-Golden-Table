import { useState, type ReactNode } from "react";
import { Link } from "react-router-dom";

import { allOrderStatuses } from "../../../common/orders";
import useGetAllDishes from "../../../communication/dish/getAllDishes/useGetAllDishes";
import useGetOrders from "../../../communication/order/getOrders/useGetOrders";
import type { dish } from "../../../types/dish";
import type { order } from "../../../types/order";
import type { OrderStatus } from "../../../types/orderStatus";
import OrderList from "../../orders/OrderList";
import styles from "../../orders/Orders.module.css";

const CANCEL_REQUESTED = "CancelRequested";
type filter = "All" | OrderStatus | typeof CANCEL_REQUESTED;

type props = {
  orders: order[];
  dishes: dish[];
};

function matchesFilter(order: order, selected: filter): boolean {
  if (selected === "All") return true;
  if (selected === CANCEL_REQUESTED) return order.cancelRequested;
  return order.status === selected;
}

function DisplayOrders({ orders, dishes }: props): ReactNode {
  const [selected, setSelected] = useState<filter>("All");
  const filters: filter[] = ["All", ...allOrderStatuses, CANCEL_REQUESTED];
  const displayed = orders.filter((o) => matchesFilter(o, selected));

  function label(f: filter): string {
    const count = orders.filter((o) => matchesFilter(o, f)).length;
    const name = f === CANCEL_REQUESTED ? "Cancel requested" : f;
    return `${name} (${count})`;
  }

  return (
    <>
      <div className={styles.filters} role="group" aria-label="Filter orders">
        {filters.map((f) => (
          <button
            key={f}
            type="button"
            className={`${styles.filterChip} ${f === selected ? styles.filterChipActive : ""}`}
            aria-pressed={f === selected}
            onClick={() => setSelected(f)}
          >
            {label(f)}
          </button>
        ))}
      </div>
      {displayed.length === 0 ? (
        <div className={styles.emptyState}>No orders match this filter.</div>
      ) : (
        <OrderList orders={displayed} dishes={dishes} showCustomer />
      )}
    </>
  );
}

export default function AdminOrders(): ReactNode {
  const orders = useGetOrders();
  const dishes = useGetAllDishes();

  let content: ReactNode;
  if (orders.isPending) {
    content = <div className={styles.loading}>Loading orders, please wait...</div>;
  } else if (orders.isError) {
    content = <div className={styles.error}>{`An error occurred while getting orders, ${orders.error.message}`}</div>;
  } else if (orders.data.length === 0) {
    content = <div className={styles.emptyState}>No orders yet.</div>;
  } else {
    content = <DisplayOrders orders={orders.data} dishes={dishes.data ?? []} />;
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1 className={styles.title}>Orders</h1>
        <Link to="/admin" className={styles.backLink}>
          ← Admin dashboard
        </Link>
      </div>
      {content}
    </div>
  );
}
