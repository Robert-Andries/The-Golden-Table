import type { ReactNode } from "react";
import { NavLink } from "react-router-dom";
import styles from "./Index.module.css";

function Index() : ReactNode {
    return (
      <div className={styles.hero}>
        <h1 className={styles.title}>Welcome to The Golden Table</h1>
        <NavLink to="/shop" className={styles.link}>
          Explore Our Menu
        </NavLink>
      </div>
    );
}

export default Index;