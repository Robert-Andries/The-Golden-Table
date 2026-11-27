import { useState, type ReactNode } from "react";
import ImageItem from "./ImageItem";
import ModifyImageModal from "./ModifyImageModal";
import styles from "./ManageImages.module.css";
import fetchDeleteImage from "../../../communication/deleteImage/fetchDeleteImage";
import queryClient from "../../../communication/common/queryClient";
import useGetAllImages from "../../../communication/getAllImages/useGetAllImages";
import ConfirmationDialog from "../../../components/ConfirmationDialog";
import { ApplicationError } from "../../../common/ApplicationError";
import useModal from "../../../components/useModal";
import AddImageModal from "./AddImageModal";

function ManageImages(): ReactNode {
  const { data: images, isPending, isError, error } = useGetAllImages();
  const {
    isOpen: addImageModalOpen,
    open: openAddImageModal,
    close: closeAddImageModal,
  } = useModal();
  const {
    isOpen: confirmationIsOpen,
    open: openConfirmation,
    close: closeConfirmation,
  } = useModal();
  const {
    isOpen: modifyImageModal,
    open: openImageModal,
    close: closeImageModal,
  } = useModal();
  const [focusedImageId, setFocusedImageId] = useState<string>("");

  function onDeleteImage(id: string) {
    setFocusedImageId(id);
    openConfirmation();
  }

  function onModifyImage(id: string) {
    setFocusedImageId(id);
    openImageModal();
  }

  function deleteImage(id: string) {
    async function deleteAsyncFunction() {
      try {
        await fetchDeleteImage(id);
        queryClient.invalidateQueries({ queryKey: ["images"] });
      } catch (error: unknown) {
        if (error instanceof ApplicationError) {
          // Notification
          return;
        }
        if (error instanceof Error) {
          // Notification
          return;
        }
        // Notification
        console.error(
          "An unknown error occured while trying to delete an image",
          error,
        );
      }
    }

    deleteAsyncFunction();
  }

  if (isPending) {
    return <div className={styles.loading}>Fetching images...</div>;
  }

  if (isError) {
    return <div className={styles.error}>{`There was an error getting the images: ${error.message}`}</div>;
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1 className={styles.title}>Manage Images</h1>
        <button className={styles.addButton} onClick={openAddImageModal}>
          Add image
        </button>
      </div>
      <div className={styles.list}>
        {images?.map((image) => (
          <ImageItem
            image={image}
            onDelete={onDeleteImage}
            onModify={onModifyImage}
            key={image.id}
          />
        ))}
      </div>
      <div>
        
      </div>

      <AddImageModal
        isOpen={addImageModalOpen}
        onClose={closeAddImageModal}
      />

      <ModifyImageModal
        isOpen={modifyImageModal}
        onClose={closeImageModal}
        imageId={focusedImageId}
      />

      <ConfirmationDialog
        isOpen={confirmationIsOpen}
        onClose={() => {
          closeConfirmation();
        }}
        onSubmit={() => deleteImage(focusedImageId)}
        title="Are you sure?"
        message="This action is ireversable and the image will be deleted forever (a very long time). Think carefully before pressing submit!"
      />
    </div>
  );
}

export default ManageImages;
