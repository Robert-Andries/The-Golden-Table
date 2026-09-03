import { useActionState, type ReactNode } from "react";
import editImageAction from "./editImageAction";
import styles from "./ModifyImageModal.module.css";
import useGetImageById from "../../../communication/getImageById/useGetImageById";
import Input from "../../../components/Input";
import Modal from "../../../components/Modal";
import type { ImageInfo } from "../../../communication/getAllImages/response";

type Props = {
  isOpen: boolean;
  onClose: () => void;
  imageId: string;
};

function ModifyImageFormContent({
  image,
  onClose,
}: {
  image: ImageInfo;
  onClose: () => void;
}): ReactNode {
  const boundEditAction = async (prevState: unknown, formData: FormData) => {
    return editImageAction(prevState, formData, image.id, onClose);
  };

  const [formState, formAction, pending] = useActionState(boundEditAction, {
    errors: [],
    name: image.name,
    description: image.description,
    uri: image.uri,
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
            <h3 key={idx} className={styles.errorText}>
              {error}
            </h3>
          ))}
        </div>
      )}

      <div className={styles.actions}>
        <button
          className={styles.cancelButton}
          type="button"
          onClick={onClose}
          disabled={pending}
        >
          Cancel
        </button>
        <button
          className={styles.submitButton}
          type="submit"
          disabled={pending}
        >
          {pending ? "Submitting..." : "Submit"}
        </button>
      </div>
    </form>
  );
}

function ModifyImageForm({
  imageId,
  onClose,
}: {
  imageId: string;
  onClose: () => void;
}): ReactNode {
  const { data: image, isPending, isError, error } = useGetImageById(imageId);

  if (isPending) {
    return <p className={styles.loading}>Loading image...</p>;
  }

  if (isError) {
    return <p className={styles.error}>{error.message}</p>;
  }

  if (!image) {
    return <p className={styles.error}>Image not found</p>;
  }

  return <ModifyImageFormContent image={image} onClose={onClose} />;
}

export default function ModifyImageModal({
  isOpen,
  onClose,
  imageId,
}: Props): ReactNode {
  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Modify Image">
      <ModifyImageForm imageId={imageId} onClose={onClose} />
    </Modal>
  );
}
