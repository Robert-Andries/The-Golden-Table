import type { ReactNode } from "react";
import { NavLink } from "react-router-dom";
import styles from "./NotFound.module.css";

export default function NotFound() : ReactNode  {
  return (
    <div className={styles.container}>
      <h1 className={styles.title}>404 - Not Found</h1>
      <p className={styles.message}>
        Hmm... you find yourself in a weird place. Maybe you're searching for something that does not exist. 
        Better to go back to the start page.
      </p>
      <NavLink to="/" className={styles.link}>
        Back to Home
      </NavLink>
    </div>
  );
}