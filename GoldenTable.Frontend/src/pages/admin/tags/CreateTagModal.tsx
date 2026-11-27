import { useActionState, type ReactNode } from "react";
import Modal from "../../../components/Modal";
import Input from "../../../components/Input";
import createTagAction from "./createTagAction";
import styles from "./Tags.module.css";

type props = {
  isOpen: boolean;
  onClose: () => void;
};

function CreateTagModal({ isOpen, onClose }: props): ReactNode {
  function boundCreateAction(formState: unknown, formData: FormData) {
    return createTagAction(formState, formData, onClose);
  }

  const [formState, formAction, pending] = useActionState(boundCreateAction, {
    errors: [],
    name: ""
  });

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <form action={formAction} className={styles.form}>
        <h2 style={{margin: 0, color: 'var(--color-primary-400, #60a5fa)'}}>Create Tag</h2>
        {formState.errors.length > 0 && (
          <div className={styles.errorContainer}>
            {formState.errors.map((error, index) => (
              <p key={index} className={styles.errorMessage}>{error}</p>
            ))}
          </div>
        )}
        <Input name="Name" defaultText={formState.name} type="text" />
        <div className={styles.buttonGroup}>
          <button type="button" onClick={onClose} className={styles.cancelBtn} disabled={pending}>Cancel</button>
          <button type="submit" className={styles.submitBtn} disabled={pending}>{pending ? "Submitting..." : "Submit"}</button>
        </div>
      </form>
    </Modal>
  );
}

export default CreateTagModal;
