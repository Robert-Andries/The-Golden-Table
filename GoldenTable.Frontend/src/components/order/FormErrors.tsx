import type { ReactNode } from "react";
import styles from "./Order.module.css";

type props = {
  errors: string[];
};

export default function FormErrors({ errors }: props): ReactNode {
  if (errors.length === 0) return null;

  return (
    <div className={styles.errorContainer} role="alert">
      {errors.map((error) => (
        <p key={error} className={styles.errorMessage}>
          {error}
        </p>
      ))}
    </div>
  );
}
