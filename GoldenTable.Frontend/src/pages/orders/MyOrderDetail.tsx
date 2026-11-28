import { useState, type ReactNode } from "react";
import { Link, useParams } from "react-router-dom";

import { ApplicationError } from "../../common/ApplicationError";
import { errorMessage, isOrderFinished, shortId } from "../../common/orders";
import useGetAllDishes from "../../communication/dish/getAllDishes/useGetAllDishes";
import useGetMyOrderById from "../../communication/order/getMyOrderById/useGetMyOrderById";
import useRequestCancel from "../../communication/order/requestCancel/useRequestCancel";
import ConfirmationDialog from "../../components/ConfirmationDialog";
import FormErrors from "../../components/order/FormErrors";
import OrderSummary from "../../components/order/OrderSummary";
import useNotification from "../../components/order/useNotification";
import useModal from "../../components/useModal";
import type { dish } from "../../types/dish";
import type { order } from "../../types/order";
import ModifyItemsModal from "./ModifyItemsModal";
import UpdateAddressModal from "./UpdateAddressModal";
import styles from "./Orders.module.css";

type props = {
  order: order;
  dishes: dish[];
};

function DisplayMyOrder({ order, dishes }: props): ReactNode {
  const { message, notify } = useNotification();
  const itemsModal = useModal();
  const addressModal = useModal();
  const cancelDialog = useModal();
  const requestCancel = useRequestCancel();
  const [errors, setErrors] = useState<string[]>([]);

  const finished = isOrderFinished(order.status);

  function handleRequestCancel() {
    setErrors([]);
    requestCancel.mutate(order.id, {
      onSuccess: () => notify("Your cancellation request was sent."),
      onError: (error) => setErrors([errorMessage(error)]),
    });
  }

  return (
    <>
      {message && <div className={styles.notification}>{message}</div>}

      <section className={styles.section}>
        {finished ? (
          <p className={styles.note}>This order is {order.status.toLowerCase()} and can no longer be changed.</p>
        ) : (
          <div className={styles.actions}>
            <button type="button" className={styles.secondaryBtn} onClick={itemsModal.open}>
              Modify items
            </button>
            <button type="button" className={styles.secondaryBtn} onClick={addressModal.open}>
              Change address
            </button>
            <button
              type="button"
              className={styles.dangerBtn}
              onClick={cancelDialog.open}
              disabled={order.cancelRequested || requestCancel.isPending}
            >
              {order.cancelRequested ? "Cancellation requested" : "Request cancellation"}
            </button>
          </div>
        )}
        <FormErrors errors={errors} />
      </section>

      <OrderSummary order={order} dishes={dishes} />

      {itemsModal.isOpen && (
        <ModifyItemsModal order={order} dishes={dishes} onClose={itemsModal.close} onSaved={notify} />
      )}
      {addressModal.isOpen && (
        <UpdateAddressModal order={order} onClose={addressModal.close} onSaved={notify} />
      )}
      <ConfirmationDialog
        isOpen={cancelDialog.isOpen}
        onClose={cancelDialog.close}
        onSubmit={handleRequestCancel}
        title="Request the cancellation of this order?"
        message="The restaurant staff will review your request and cancel the order if it is still possible."
      />
    </>
  );
}

export default function MyOrderDetail(): ReactNode {
  const { id } = useParams<{ id: string }>();
  const orderQuery = useGetMyOrderById(id);
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
    content = <DisplayMyOrder order={orderQuery.data} dishes={dishes.data ?? []} />;
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1 className={styles.title}>Order #{shortId(id)}</h1>
        <Link to="/orders" className={styles.backLink}>
          ← All my orders
        </Link>
      </div>
      {content}
    </div>
  );
}
