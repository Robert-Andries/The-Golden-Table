import type { ReactNode } from "react";
import { useState, useEffect } from "react";
import Modal from "./Modal";
import useGetAllImages from "../communication/getAllImages/useGetAllImages";
import type { ImageInfo } from "../communication/getAllImages/response";
import styles from "./ImageModal.module.css";

type Props = {
  isOpen: boolean;
  onClose: () => void;
  preSelectedImageIds?: string[];
  extractedImages: (selectedImages: ImageInfo[]) => void;
};

export default function ImageModal({
  isOpen,
  onClose,
  preSelectedImageIds = [],
  extractedImages,
}: Props): ReactNode {
  const { data: images, isLoading, isError } = useGetAllImages();
  const [selectedIds, setSelectedIds] = useState<Set<string>>(
    new Set(preSelectedImageIds)
  );

  // Sync pre-selected IDs when the modal opens or the prop changes
  useEffect(() => {
    if (isOpen) {
      setSelectedIds(new Set(preSelectedImageIds));
    }
  }, [isOpen, preSelectedImageIds]);

  function handleToggleImage(id: string) {
    setSelectedIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) {
        next.delete(id);
      } else {
        next.add(id);
      }
      return next;
    });
  }

  function handleCancel() {
    onClose();
  }

  function handleSubmit() {
    if (!images) return;

    const selected = images.filter((img) => selectedIds.has(img.id));
    extractedImages(selected);
    onClose();
  }

  return (
    <Modal isOpen={isOpen} onClose={handleCancel} title="Select Images">
      {isLoading && <p className={styles.loading}>Loading images…</p>}

      {isError && (
        <p className={styles.error}>Failed to load images. Please try again.</p>
      )}

      {images && images.length === 0 && (
        <p className={styles.empty}>No images available.</p>
      )}

      {images && images.length > 0 && (
        <div className={styles.grid}>
          {images.map((img) => {
            const isSelected = selectedIds.has(img.id);
            return (
              <div
                key={img.id}
                className={`${styles.imageCard} ${isSelected ? styles.selected : ""}`}
                onClick={() => handleToggleImage(img.id)}
                role="checkbox"
                aria-checked={isSelected}
                aria-label={img.name}
                tabIndex={0}
                onKeyDown={(e) => {
                  if (e.key === "Enter" || e.key === " ") {
                    e.preventDefault();
                    handleToggleImage(img.id);
                  }
                }}
              >
                <img
                  src={img.uri}
                  alt={img.name}
                  className={styles.image}
                />
                {isSelected && <span className={styles.checkmark}>✓</span>}
                <div className={styles.info}>
                  <span className={styles.name}>{img.name}</span>
                  <span className={styles.description}>
                    {img.description.length > 30
                      ? img.description.slice(0, 30) + "…"
                      : img.description}
                  </span>
                </div>
              </div>
            );
          })}
        </div>
      )}

      <div className={styles.actions}>
        <button
          type="button"
          className={styles.cancelButton}
          onClick={handleCancel}
        >
          Cancel
        </button>
        <button
          type="button"
          className={styles.submitButton}
          onClick={handleSubmit}
        >
          Submit
        </button>
      </div>
    </Modal>
  );
}
