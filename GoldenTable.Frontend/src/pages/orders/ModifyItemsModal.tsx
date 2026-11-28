import { useState, type FormEvent, type ReactNode } from "react";

import { errorMessage, validateItems } from "../../common/orders";
import useModifyItems from "../../communication/order/modifyItems/useModifyItems";
import Modal from "../../components/Modal";
import FormErrors from "../../components/order/FormErrors";
import OrderItemsEditor from "../../components/order/OrderItemsEditor";
import type { dish } from "../../types/dish";
import type { order, orderItem } from "../../types/order";
import styles from "./Orders.module.css";

type props = {
  order: order;
  dishes: dish[];
  onClose: () => void;
  onSaved: (message: string) => void;
};

/** Mount it only while open, so every opening starts from the order's current items. */
export default function ModifyItemsModal({ order, dishes, onClose, onSaved }: props): ReactNode {
  const [items, setItems] = useState<orderItem[]>(order.items);
  const [errors, setErrors] = useState<string[]>([]);
  const modifyItems = useModifyItems();

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const validationErrors = validateItems(items);
    setErrors(validationErrors);
    if (validationErrors.length > 0) return;

    modifyItems.mutate(
      { orderId: order.id, items },
      {
        onSuccess: () => {
          onSaved("The order items were updated.");
          onClose();
        },
        onError: (error) => setErrors([errorMessage(error)]),
      },
    );
  }

  return (
    <Modal isOpen onClose={onClose}>
      <form className={`${styles.form} ${styles.modalWide}`} onSubmit={handleSubmit}>
        <h2 className={styles.modalTitle}>Modify items</h2>
        <p className={styles.note}>The new list replaces the current one and the total is recalculated.</p>
        <OrderItemsEditor items={items} dishes={dishes} onChange={setItems} disabled={modifyItems.isPending} />
        <FormErrors errors={errors} />
        <div className={styles.buttonGroup}>
          <button type="button" className={styles.secondaryBtn} onClick={onClose} disabled={modifyItems.isPending}>
            Cancel
          </button>
          <button type="submit" className={styles.primaryBtn} disabled={modifyItems.isPending}>
            {modifyItems.isPending ? "Saving..." : "Save items"}
          </button>
        </div>
      </form>
    </Modal>
  );
}
