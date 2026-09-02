import type { ReactNode, InputHTMLAttributes } from "react";
import styles from "./Input.module.css";

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  defaultText: string | number;
}

export default function Input({
  name,
  type,
  defaultText,
  ...rest
}: InputProps): ReactNode {
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
