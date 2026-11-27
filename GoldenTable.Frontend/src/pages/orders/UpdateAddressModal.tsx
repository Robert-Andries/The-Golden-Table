import { useState, type FormEvent, type ReactNode } from "react";

import { errorMessage, readAddress, sameAddress, validateAddress } from "../../common/orders";
import useUpdateAddress from "../../communication/order/updateAddress/useUpdateAddress";
import Modal from "../../components/Modal";
import AddressFields from "../../components/order/AddressFields";
import FormErrors from "../../components/order/FormErrors";
import type { order } from "../../types/order";
import styles from "./Orders.module.css";

type props = {
  order: order;
  onClose: () => void;
  onSaved: (message: string) => void;
};

/** Mount it only while open, so every opening starts from the order's current address. */
export default function UpdateAddressModal({ order, onClose, onSaved }: props): ReactNode {
  const [errors, setErrors] = useState<string[]>([]);
  const updateAddress = useUpdateAddress();

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const address = readAddress(new FormData(event.currentTarget));
    const validationErrors = validateAddress(address);
    if (validationErrors.length === 0 && sameAddress(address, order.address)) {
      validationErrors.push("The new address is the same as the current one.");
    }
    setErrors(validationErrors);
    if (validationErrors.length > 0) return;

    updateAddress.mutate(
      { orderId: order.id, address },
      {
        onSuccess: () => {
          onSaved("The delivery address was updated.");
          onClose();
        },
        onError: (error) => setErrors([errorMessage(error)]),
      },
    );
  }

  return (
    <Modal isOpen onClose={onClose}>
      <form className={`${styles.form} ${styles.modalWide}`} onSubmit={handleSubmit}>
        <h2 className={styles.modalTitle}>Change delivery address</h2>
        <AddressFields defaultAddress={order.address} />
        <FormErrors errors={errors} />
        <div className={styles.buttonGroup}>
          <button type="button" className={styles.secondaryBtn} onClick={onClose} disabled={updateAddress.isPending}>
            Cancel
          </button>
          <button type="submit" className={styles.primaryBtn} disabled={updateAddress.isPending}>
            {updateAddress.isPending ? "Saving..." : "Save address"}
          </button>
        </div>
      </form>
    </Modal>
  );
}
