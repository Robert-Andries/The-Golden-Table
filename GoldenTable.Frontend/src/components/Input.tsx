import type { ReactNode, InputHTMLAttributes } from "react";
import styles from "./Input.module.css";

interface props extends InputHTMLAttributes<HTMLInputElement> {
  defaultText: string | number;
}

export default function Input({
  name,
  type,
  defaultText,
  ...rest
}: props): ReactNode {
  return (
    <div className={styles.container}>
      <label className={styles.label}>{name}</label>
      <input 
        className={styles.input} 
        name={name} 
        type={type} 
        defaultValue={defaultText} 
        {...rest} 
      />
    </div>
  );
}
