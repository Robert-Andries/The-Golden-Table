import type { ReactNode } from "react";
import { formatDate, formatMoney, unitPrice } from "../../common/orders";
import type { dish } from "../../types/dish";
import type { order } from "../../types/order";
import OrderStatusBadge from "./OrderStatusBadge";
import styles from "./Order.module.css";

type props = {
  order: order;
  dishes: dish[];
  showCustomer?: boolean;
};

export default function OrderSummary({ order, dishes, showCustomer = false }: props): ReactNode {
  const { address } = order;

  return (
    <div className={styles.summary}>
      <section className={styles.card}>
        <h2 className={styles.cardTitle}>Overview</h2>
        <dl className={styles.meta}>
          <dt>Status</dt>
          <dd>
            <OrderStatusBadge status={order.status} />{" "}
            {order.cancelRequested && <span className={styles.cancelFlag}>Cancellation requested</span>}
          </dd>
          <dt>Order id</dt>
          <dd>{order.id}</dd>
          {showCustomer && (
            <>
              <dt>Customer id</dt>
              <dd>{order.customerId}</dd>
            </>
          )}
          <dt>Placed on</dt>
          <dd>{formatDate(order.createdOnUtc)}</dd>
          <dt>Last change</dt>
          <dd>{formatDate(order.modifiedOnUtc)}</dd>
        </dl>
      </section>

      <section className={styles.card}>
        <h2 className={styles.cardTitle}>Items</h2>
        <table className={styles.itemsTable}>
          <thead>
            <tr>
              <th>Dish</th>
              <th>Size</th>
              <th className={styles.numeric}>Quantity</th>
              <th className={styles.numeric}>Current price</th>
            </tr>
          </thead>
          <tbody>
            {order.items.map((item, index) => {
              const itemDish = dishes.find((d) => d.id === item.dishId);
              return (
                <tr key={`${item.dishId}-${index}`}>
                  <td>{itemDish?.name ?? "Unavailable dish"}</td>
                  <td>{item.sizeName ?? "Standard"}</td>
                  <td className={styles.numeric}>{item.quantity}</td>
                  <td className={styles.numeric}>
                    {itemDish
                      ? formatMoney(unitPrice(itemDish, item.sizeName) * item.quantity, itemDish.basePriceCurrency)
                      : "—"}
                  </td>
                </tr>
              );
            })}
          </tbody>
          <tfoot>
            <tr>
              <td colSpan={3}>Total charged</td>
              <td className={styles.numeric}>{formatMoney(order.totalPriceAmount, order.totalPriceCurrency)}</td>
            </tr>
          </tfoot>
        </table>
      </section>

      <section className={styles.card}>
        <h2 className={styles.cardTitle}>Delivery address</h2>
        <dl className={styles.meta}>
          <dt>County</dt>
          <dd>{address.county}</dd>
          <dt>City</dt>
          <dd>{address.city}</dd>
          <dt>Street</dt>
          <dd>
            {address.street} {address.streetNumber}
          </dd>
          <dt>Zip code</dt>
          <dd>{address.zipCode}</dd>
        </dl>
      </section>
    </div>
  );
}
