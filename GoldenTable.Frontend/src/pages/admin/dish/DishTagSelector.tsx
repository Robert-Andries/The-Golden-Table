import type { dishTag } from "../../../types/dishTag";
import styles from "./EditDish.module.css";

type Props = {
  tagData: dishTag[];
  activeTags: string[];
  onTagClick: (tagValue: string) => void;
};

export default function DishTagSelector({ tagData, activeTags, onTagClick }: Props) {
  return (
    <div className={styles.tagsSection}>
      <p className={styles.tagsTitle}>Tags</p>
      <div className={styles.tags}>
        {tagData.map((tag) => {
          const isActive = activeTags.includes(tag.value);
          return (
            <h4
              key={tag.id}
              className={`${styles.tag} ${isActive ? styles.active : ""}`}
              onClick={() => onTagClick(tag.value)}
            >
              {tag.value}
            </h4>
          );
        })}
      </div>
    </div>
  );
}
