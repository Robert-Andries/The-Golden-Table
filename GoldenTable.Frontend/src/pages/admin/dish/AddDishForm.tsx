import { useActionState, useEffect, useState, type ReactNode } from "react";
import { useNavigate } from "react-router-dom";
import { useQueryClient } from "@tanstack/react-query";
import Input from "../../../components/Input";
import EditableNutritionalInformation from "../../../components/EditableNutritionalInformation";
import ImageModal from "../../../components/ImageModal";
import useModal from "../../../components/useModal";
import placeholderImg from "../../../assets/default-placeholder-food.png";
import styles from "./EditDish.module.css";
import type { dishTag } from "../../../communication/getAllTags/response";
import type { ImageInfo } from "../../../communication/getAllImages/response";
import { addAction } from "./addDishAction";
import DishTagSelector from "./DishTagSelector";

type Props = {
  tagData: dishTag[];
};

export default function AddDishForm({ tagData }: Props): ReactNode {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [activeTags, setActiveTags] = useState<string[]>([]);
  const [imageIds, setImageIds] = useState<string[]>([]);
  const [imageUris, setImageUris] = useState<string[]>([]);
  const imageModal = useModal();

  function handleOnTagClick(tagValue: string) {
    setActiveTags((prev) => {
      const isTagActive = prev.includes(tagValue);
      return isTagActive
        ? prev.filter((t) => t !== tagValue)
        : [...prev, tagValue];
    });
  }

  function handleImagesSelected(selectedImages: ImageInfo[]) {
    setImageIds(selectedImages.map((img) => img.id));
    setImageUris(selectedImages.map((img) => img.uri));
  }

  const boundAddAction = async (prevState: unknown, formData: FormData) => {
    return addAction(prevState, formData, activeTags, tagData, imageIds);
  };

  const [formState, formAction, pending] = useActionState(boundAddAction, {
    errors: null,
    success: false,
    enteredValues: {
      name: "",
      description: "",
      basePriceAmount: 0,
      basePriceCurrency: "RON",
      nutritionalInformation: {
        energy: { kcal: 0, kj: 0 },
        gramsOfFat: 0,
        gramsOfCarbohydrates: { total: 0, ofWhichSugar: 0 },
        gramsOfProtein: 0,
        gramsOfSalt: 0,
      }
    }
  });

  useEffect(() => {
    if (formState.success) {
      queryClient.invalidateQueries({ queryKey: ["dishes"] });
      navigate("/admin", { state: { message: "Dish created successfully" } });
    }
  }, [formState.success, navigate, queryClient]);

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
                    alt={`Dish image ${index + 1}`}
                  />
                </div>
              ))
            ) : (
              <div className={styles.imageContainer}>
                <img
                  className={styles.image}
                  src={placeholderImg}
                  alt="Dish image"
                />
              </div>
            )}
          </div>
          <button
            type="button"
            className={styles.editImagesButton}
            onClick={imageModal.open}
          >
            Select Images
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
          <h3 key={error} style={{ color: "var(--color-red-500, #ef4444)" }}>{error}</h3>
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

