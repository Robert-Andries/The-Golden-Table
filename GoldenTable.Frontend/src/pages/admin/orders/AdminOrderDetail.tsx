import { useState, type ReactNode } from "react";
import { Link, useParams } from "react-router-dom";

import { Can } from "../../../auth/Can";
import { ApplicationError } from "../../../common/ApplicationError";
import { allowedStatusTransitions, errorMessage, shortId } from "../../../common/orders";
import useGetAllDishes from "../../../communication/dish/getAllDishes/useGetAllDishes";
import useChangeStatus from "../../../communication/order/changeStatus/useChangeStatus";
import useGetOrderById from "../../../communication/order/getOrderById/useGetOrderById";
import ConfirmationDialog from "../../../components/ConfirmationDialog";
import FormErrors from "../../../components/order/FormErrors";
import OrderSummary from "../../../components/order/OrderSummary";
import useNotification from "../../../components/order/useNotification";
import type { dish } from "../../../types/dish";
import type { order } from "../../../types/order";
import { OrderStatus } from "../../../types/orderStatus";
import styles from "../../orders/Orders.module.css";

type props = {
  order: order;
  dishes: dish[];
};

const actionLabels: Record<OrderStatus, string> = {
  [OrderStatus.Placed]: "Mark as placed",
  [OrderStatus.Confirmed]: "Confirm",
  [OrderStatus.Rejected]: "Reject",
  [OrderStatus.Cancelled]: "Cancel order",
  [OrderStatus.Preparing]: "Start preparing",
  [OrderStatus.Sent]: "Mark as sent",
};

/** Final statuses: ask before applying them. */
const needsConfirmation: OrderStatus[] = [OrderStatus.Rejected, OrderStatus.Cancelled];

function DisplayAdminOrder({ order, dishes }: props): ReactNode {
  const { message, notify } = useNotification();
  const changeStatus = useChangeStatus();
  const [errors, setErrors] = useState<string[]>([]);
  const [pendingConfirmation, setPendingConfirmation] = useState<OrderStatus | null>(null);

  const transitions = allowedStatusTransitions[order.status];

  function applyStatus(status: OrderStatus) {
    setErrors([]);
    changeStatus.mutate(
      { orderId: order.id, status },
      {
        onSuccess: () => notify(`The order is now ${status.toLowerCase()}.`),
        onError: (error) => setErrors([errorMessage(error)]),
      },
    );
  }

  function handleClick(status: OrderStatus) {
    if (needsConfirmation.includes(status)) {
      setPendingConfirmation(status);
    } else {
      applyStatus(status);
    }
  }

  return (
    <>
      {message && <div className={styles.notification}>{message}</div>}

      <section className={styles.section}>
        {order.cancelRequested && transitions.includes(OrderStatus.Cancelled) && (
          <p className={styles.note}>The customer asked for this order to be cancelled.</p>
        )}
        {transitions.length === 0 ? (
          <p className={styles.note}>This order is {order.status.toLowerCase()}; its status can no longer change.</p>
        ) : (
          <Can permission="orders:change-status">
            <div className={styles.actions}>
              {transitions.map((status) => (
                <button
                  key={status}
                  type="button"
                  className={needsConfirmation.includes(status) ? styles.dangerBtn : styles.primaryBtn}
                  onClick={() => handleClick(status)}
                  disabled={changeStatus.isPending}
                >
                  {actionLabels[status]}
                </button>
              ))}
            </div>
          </Can>
        )}
        <FormErrors errors={errors} />
      </section>

      <OrderSummary order={order} dishes={dishes} showCustomer />

      <ConfirmationDialog
        isOpen={pendingConfirmation !== null}
        onClose={() => setPendingConfirmation(null)}
        onSubmit={() => pendingConfirmation && applyStatus(pendingConfirmation)}
        title={`${pendingConfirmation ? actionLabels[pendingConfirmation] : ""}?`}
        message="This is a final status: the order cannot be changed afterwards."
      />
    </>
  );
}

export default function AdminOrderDetail(): ReactNode {
  const { id } = useParams<{ id: string }>();
  const orderQuery = useGetOrderById(id);
  const dishes = useGetAllDishes();

  if (!id) {
    throw new ApplicationError("Order ID is missing from the URL.", 400);
  }

  let content: ReactNode;
  if (orderQuery.isPending) {
    content = <div className={styles.loading}>Loading the order...</div>;
  } else if (orderQuery.isError) {
    content = <div className={styles.error}>Error: {orderQuery.error.message}</div>;
  } else {
    content = <DisplayAdminOrder order={orderQuery.data} dishes={dishes.data ?? []} />;
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1 className={styles.title}>Order #{shortId(id)}</h1>
        <Link to="/admin/orders" className={styles.backLink}>
          ← All orders
        </Link>
      </div>
      {content}
    </div>
  );
}
