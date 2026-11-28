import type { ReactNode } from "react";
import styles from "./LoadingScreen.module.css";

interface LoadingScreenProps {
  message?: string;
}

export function LoadingScreen({ message = "Loading..." }: LoadingScreenProps): ReactNode {
  return (
    <div className={styles.container}>
      <div className={styles.spinner} role="status" aria-label="Loading" />
      <p className={styles.message}>{message}</p>
    </div>
  );
}

export default LoadingScreen;
