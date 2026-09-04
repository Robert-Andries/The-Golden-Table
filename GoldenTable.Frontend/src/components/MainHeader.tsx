import type { ReactNode } from "react";
import { NavLink, Link } from "react-router-dom";

import style from "./MainHeader.module.css";

function MainHeader(): ReactNode {
  return (
    <header className={style.header}>
      <div className={style.container}>
        <Link to="/" className={style.brand}>
          The Golden Table
        </Link>
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
            <li>
              <NavLink
                to="/admin"
                className={({ isActive }) =>
                  isActive ? style.active : undefined
                }
                end
              >
                Admin
              </NavLink>
            </li>
          </ul>
        </nav>
      </div>
    </header>
  );
}

export default MainHeader;
