import { type ReactNode } from "react";
import { useParams } from "react-router-dom";
import { ApplicationError } from "../../common/ApplicationError";
import useGetDish from "../../communication/getDish/useGetDish";
import useGetAllTags from "../../communication/getAllTags/useGetAllTags";
import styles from "./EditDish.module.css";
import EditDishForm from "./EditDishForm";

function EditDish(): ReactNode {
  const { id } = useParams<{ id: string }>();
  const { data, isPending, isError, error } = useGetDish(id);
  const {
    data: tagData,
    isPending: tagIsPending,
    isError: tagIsError,
    error: tagError,
  } = useGetAllTags();

  if (!id) {
    throw new ApplicationError("Dish ID is missing from the URL.", 400);
  }

  if (isPending || tagIsPending) {
    return <div className={styles.loading}>Loading dish details...</div>;
  }

  if (isError || tagIsError) {
    return (
      <div className={styles.error}>
        Error: {error?.message ?? tagError?.message}
      </div>
    );
  }

  if (!data || !tagData) {
    return null;
  }

  return <EditDishForm data={data} tagData={tagData} />;
}

export default EditDish;
