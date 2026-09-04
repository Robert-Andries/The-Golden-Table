import { useActionState, type ReactNode } from "react";
import addImageAction from "./addImageAction";
import styles from "./ModifyImageModal.module.css";
import Input from "../../../components/Input";
import Modal from "../../../components/Modal";

type Props = {
  isOpen: boolean;
  onClose: () => void;
};

function AddImageForm({
  onClose,
}: {
  onClose: () => void;
}): ReactNode {
  const boundAddAction = async (prevState: unknown, formData: FormData) => {
    return addImageAction(prevState, formData, onClose);
  };

  const [formState, formAction, pending] = useActionState(boundAddAction, {
    errors: [],
    name: "",
    description: "",
    uri: "",
  });

  return (
    <form action={formAction} className={styles.form}>
      <Input name="Name" type="text" defaultText={formState.name} />
      <Input
        name="Description"
        type="text"
        defaultText={formState.description}
      />
      <Input name="Uri" type="text" defaultText={formState.uri} />

      {formState.errors.length > 0 && (
        <div className={styles.errors}>
          {formState.errors.map((error, idx) => (
            <h3 key={idx} className={styles.errorText}>{error}</h3>
          ))}
        </div>
      )}

      <div className={styles.actions}>
        <button className={styles.cancelButton} type="button" onClick={onClose} disabled={pending}>
          Cancel
        </button>
        <button className={styles.submitButton} type="submit" disabled={pending}>
          {pending ? "Submitting..." : "Submit"}
        </button>
      </div>
    </form>
  );
}

export default function AddImageModal({
  isOpen,
  onClose,
}: Props): ReactNode {
  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Add Image">
      {isOpen && <AddImageForm onClose={onClose} />}
    </Modal>
  );
}
