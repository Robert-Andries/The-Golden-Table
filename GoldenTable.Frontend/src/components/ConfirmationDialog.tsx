import type { ReactNode } from "react";
import Modal from "./Modal";
import styles from "./ConfirmationDialog.module.css";

type Props = {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: () => void;
  title: string,
  message: string
};

export default function ConfirmationDialog({
  isOpen,
  onClose,
  onSubmit,
  title,
  message
}: Props): ReactNode {
  
  return (
    <Modal isOpen={isOpen} onClose={onClose} title={title}>
      <div className={styles.container}>
        <p className={styles.message}>{message}</p>
        <div className={styles.actions}>
          <button
            className={styles.cancelButton}
            type="button"
            onClick={onClose}
          >
            Cancel
          </button>
          <button
            className={styles.submitButton}
            type="button"
            onClick={() => {onSubmit(); onClose();}}
          >
            Submit
          </button>
        </div>
      </div>
    </Modal>
  );
}
