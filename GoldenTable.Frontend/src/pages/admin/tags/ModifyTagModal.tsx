import { useActionState, type ReactNode } from "react";
import type { dishTag } from "../../../communication/getAllTags/response";
import Modal from "../../../components/Modal";
import Input from "../../../components/Input";
import useModal from "../../../components/useModal";
import ConfirmationDialog from "../../../components/ConfirmationDialog";
import fetchDeleteTag from "../../../communication/deleteTag/fetchDeleteTag";
import modifyTagAction from "./modifyTagAction";
import styles from "./Tags.module.css";
import queryClient from "../../../communication/common/queryClient";

type props = {
  tag: dishTag;
  isOpen: boolean;
  onClose: () => void;
};

export default function ModifyTagModal({
  tag,
  isOpen,
  onClose,
}: props): ReactNode {
  const {
    isOpen: confirmationIsOpen,
    open: confirmationOpen,
    close: confirmationClose,
  } = useModal();

  function handleDelete(e: React.MouseEvent) {
    e.preventDefault();
    confirmationOpen();
  }

  function deleteTag() {
    async function deleteAsync(): Promise<void> {
      await fetchDeleteTag(tag.id);
      queryClient.invalidateQueries({queryKey: ['dishes', 'tags']});
      onClose();
    }
    deleteAsync();
  }

  function boundModifyAction(formState: unknown, formData: FormData) {
    return modifyTagAction(formState, formData, tag.id, onClose);
  }

  const [formState, formAction, pending] = useActionState(boundModifyAction, {
    errors: [],
    name: tag.value,
  });

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <form action={formAction} className={styles.form}>
        <h2 style={{margin: 0, color: 'var(--color-primary-400, #60a5fa)'}}>Modify Tag</h2>
        {formState.errors.length > 0 && (
          <div className={styles.errorContainer}>
            {formState.errors.map((error, index) => (
              <p key={index} className={styles.errorMessage}>{error}</p>
            ))}
          </div>
        )}
        <Input type="text" name="Name" defaultText={formState.name} />
        
        <div className={styles.buttonGroup}>
          <button type="button" onClick={handleDelete} className={styles.deleteBtn} disabled={pending}>
            Delete
          </button>
          <button type="button" onClick={onClose} className={styles.cancelBtn} disabled={pending}>
            Cancel
          </button>
          <button type="submit" className={styles.submitBtn} disabled={pending}>
            {pending ? "Submitting..." : `Submit`}
          </button>
        </div>
      </form>

      <ConfirmationDialog
        isOpen={confirmationIsOpen}
        onClose={confirmationClose}
        onSubmit={deleteTag}
        title="Are you sure you want to delete this tag?"
        message="This action is irreversible and permanent!"
      />
    </Modal>
  );
}
