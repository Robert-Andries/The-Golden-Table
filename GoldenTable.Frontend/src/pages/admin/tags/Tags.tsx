import { useState, type ReactNode } from "react";
import useGetAllTags from "../../../communication/getAllTags/useGetAllTags";
import CreateTagModal from "./CreateTagModal";
import ModifyTagModal from "./ModifyTagModal";
import useModal from "../../../components/useModal";
import styles from "./Tags.module.css";
import type { dishTag } from "../../../types/dishTag";

type props = {
  tags: dishTag[];
};

function DisplayTags({ tags }: props): ReactNode {
  const { isOpen: createModalIsOpen, close: createModalClose, open: createModalOpen } = useModal();
  const { isOpen: modifyModalIsOpen, close: modifyModalClose, open: modifyModalOpen } = useModal();
  const [selectedTag, setSelectedTag] = useState<dishTag | null>(null);

  function handleTagClick(tagId: string) {
    const tag = tags.find((t) => t.id === tagId);
    if (tag) {
      setSelectedTag(tag);
      modifyModalOpen();
    }
  }

  function handleAddTag() {
    createModalOpen();
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1 className={styles.title}>Tags Management</h1>
        <button className={styles.addButton} onClick={handleAddTag}>Add a tag</button>
      </div>

      <div className={styles.tagsGrid}>
        {tags.length === 0 ? (
          <p className={styles.emptyState}>No tags found!</p>
        ) : (
          tags.map((tag) => (
            <div key={tag.id} className={styles.tagCard} onClick={() => handleTagClick(tag.id)}>
              <h3 className={styles.tagValue}>{tag.value}</h3>
            </div>
          ))
        )}
      </div>

      <CreateTagModal isOpen={createModalIsOpen} onClose={createModalClose} />
      {selectedTag && (
        <ModifyTagModal tag={selectedTag} isOpen={modifyModalIsOpen} onClose={modifyModalClose} />
      )}
    </div>
  );
}

function Tags() {
  const { data, isPending, isError, error } = useGetAllTags();
  if (isPending) {
    return <div className={styles.loading}>Loading tags...</div>;
  }
  if (isError) {
    return <div className={styles.error}>{error.message}</div>;
  }

  return <DisplayTags tags={data} />;
}

export default Tags;
