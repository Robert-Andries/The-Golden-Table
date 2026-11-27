import { type ReactNode } from "react";
import styles from "./EditDish.module.css";
import AddDishForm from "./AddDishForm";
import useGetAllTags from "../../../communication/getAllTags/useGetAllTags";

function AddDish(): ReactNode {
  const {
    data: tagData,
    isPending: tagIsPending,
    isError: tagIsError,
    error: tagError,
  } = useGetAllTags();

  if (tagIsPending) {
    return <div className={styles.loading}>Loading form...</div>;
  }

  if (tagIsError) {
    return (
      <div className={styles.error}>
        Error: {tagError?.message}
      </div>
    );
  }

  if (!tagData) {
    return null;
  }

  return <AddDishForm tagData={tagData} />;
}

export default AddDish;

