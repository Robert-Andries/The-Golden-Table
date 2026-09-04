import { useActionState, useEffect, useState, type ReactNode } from "react";
import { useNavigate } from "react-router-dom";
import { useQueryClient } from "@tanstack/react-query";
import Input from "../../../components/Input";
import EditableNutritionalInformation from "../../../components/EditableNutritionalInformation";
import ImageModal from "../../../components/ImageModal";
import useModal from "../../../components/useModal";
import placeholderImg from "../../../assets/default-placeholder-food.png";
import styles from "./EditDish.module.css";
import { editAction } from "./editDishAction";
import DishTagSelector from "./DishTagSelector";
import type { image } from "../../../types/image";
import type { dish } from "../../../types/dish";
import type { dishTag } from "../../../types/dishTag";

type Props = {
  data: dish;
  tagData: dishTag[];
};

export default function EditDishForm({ data, tagData }: Props): ReactNode {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [activeTags, setActiveTags] = useState<string[]>(data.tags ? [...data.tags] : []);
  const [imageIds, setImageIds] = useState<string[]>(data.imageIds || []);
  const [imageUris, setImageUris] = useState<string[]>(data.imagesUris || []);
  const imageModal = useModal();

  function handleOnTagClick(tagValue: string) {
    setActiveTags((prev) => {
      const isTagActive = prev.includes(tagValue);
      return isTagActive
        ? prev.filter((t) => t !== tagValue)
        : [...prev, tagValue];
    });
  }

  function handleImagesSelected(selectedImages: image[]) {
    setImageIds(selectedImages.map((img) => img.id));
    setImageUris(selectedImages.map((img) => img.uri));
  }

  const boundEditAction = async (prevState: unknown, formData: FormData) => {
    return editAction(prevState, formData, data, activeTags, tagData, imageIds);
  };

  const [formState, formAction, pending] = useActionState(boundEditAction, {
    errors: null,
    success: false,
    enteredValues: {
      name: data.name,
      description: data.description,
      basePriceAmount: data.basePriceAmount,
      basePriceCurrency: data.basePriceCurrency,
      nutritionalInformation: data.nutritionalInformation
    }
  });

  useEffect(() => {
    if (formState.success) {
      queryClient.removeQueries({ queryKey: ['dish', data.id] });
      queryClient.invalidateQueries({ queryKey: ['dishes'] });
      navigate("/admin", { state: { message: "Dish saved successfully" } });
    }
  }, [formState.success, navigate, queryClient, data.id]);

  return (
    <form action={formAction}>
      <div className={styles.container}>
        <div className={styles.imageSection}>
          <div className={styles.imageGallery}>
            {imageUris.length > 0 ? (
              imageUris.map((uri, index) => (
                <div key={uri} className={styles.imageContainer}>
                  <img
                    className={styles.image}
                    src={uri}
                    alt={`${data.name || "Dish"} image ${index + 1}`}
                  />
                </div>
              ))
            ) : (
              <div className={styles.imageContainer}>
                <img
                  className={styles.image}
                  src={placeholderImg}
                  alt={data.name || "Dish image"}
                />
              </div>
            )}
          </div>
          <button
            type="button"
            className={styles.editImagesButton}
            onClick={imageModal.open}
          >
            Edit Images
          </button>
        </div>
        <div className={styles.formSection}>
          <div className={styles.detailsForm}>
            <Input name="Name" type="text" defaultText={formState.enteredValues.name} />
            <Input
              name="Description"
              type="text"
              defaultText={formState.enteredValues.description}
            />
            <DishTagSelector 
              tagData={tagData} 
              activeTags={activeTags} 
              onTagClick={handleOnTagClick} 
            />
            <div className={styles.priceGroup}>
              <Input
                name="Base price amount"
                type="number"
                defaultText={formState.enteredValues.basePriceAmount}
              />
              <Input
                name="Base price currency"
                type="text"
                defaultText={formState.enteredValues.basePriceCurrency}
              />
            </div>
          </div>
          <EditableNutritionalInformation
            nutritional={formState.enteredValues.nutritionalInformation}
          />
        </div>
      </div>
      <div>
        {formState.errors?.map((error: string) => (
          <h3 key={error}>{error}</h3>
        ))}
      </div>
      <input 
        type="submit" 
        value={pending ? "Submitting..." : "Submit"} 
        disabled={pending} 
        className={styles.submitButton}
      />

      <ImageModal
        isOpen={imageModal.isOpen}
        onClose={imageModal.close}
        preSelectedImageIds={imageIds}
        extractedImages={handleImagesSelected}
      />
    </form>
  );
}

