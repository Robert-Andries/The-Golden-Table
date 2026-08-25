import type { ReactNode } from "react";
import { NavLink } from "react-router-dom";

import style from "./MainHeader.module.css";

function MainHeader(): ReactNode {
  return (
    <header className={style.header}>
      <nav>
        <ul className={style.list}>
          <li>
            <NavLink
              to="/"
              className={({ isActive }) =>
                isActive ? style.active : undefined
              }
              end
            >
              Home
            </NavLink>
          </li>
          <li>
            <NavLink
              to="/shop"
              className={({ isActive }) =>
                isActive ? style.active : undefined
              }
              end
            >
              Shop
            </NavLink>
          </li>
        </ul>
      </nav>
    </header>
  );
}

export default MainHeader;
