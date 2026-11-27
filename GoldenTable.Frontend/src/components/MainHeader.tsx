import type { ReactNode } from "react";
import { NavLink, Link } from "react-router-dom";
import { useAuth } from "../auth/useAuth";
import { useCart } from "../cart/useCart";
import style from "./MainHeader.module.css";

function MainHeader(): ReactNode {
  const { authenticated, user, hasRole, login, logout } = useAuth();
  const isAdmin = hasRole("admin");
  const { count: cartCount } = useCart();
  // Guests can fill a cart and connect at checkout
  const canOrder = !authenticated || hasRole("orders:create");
  const canViewOwnOrders = authenticated && hasRole("orders:view-own-orders");

  const displayName =
    user?.fullName || user?.username || user?.email || "Utilizator";
  const initial = (user?.firstName?.[0] || user?.username?.[0] || "U").toUpperCase();

  return (
    <header className={style.header}>
      <div className={style.container}>
        <Link to="/" className={style.brand}>
          The Golden Table
        </Link>
        <div className={style.navSection}>
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
              {canViewOwnOrders && (
                <li>
                  <NavLink
                    to="/orders"
                    className={({ isActive }) =>
                      isActive ? style.active : undefined
                    }
                  >
                    My orders
                  </NavLink>
                </li>
              )}
              {canOrder && (
                <li>
                  <NavLink
                    to="/cart"
                    className={({ isActive }) =>
                      isActive ? style.active : undefined
                    }
                  >
                    {cartCount > 0 ? `Cart (${cartCount})` : "Cart"}
                  </NavLink>
                </li>
              )}
              {isAdmin && (
                <li>
                  <NavLink
                    to="/admin"
                    className={({ isActive }) =>
                      isActive ? style.active : undefined
                    }
                  >
                    Admin
                  </NavLink>
                </li>
              )}
            </ul>
          </nav>

          <div className={style.authSection}>
            {authenticated ? (
              <>
                <div className={style.userBadge} title={user?.email || displayName}>
                  <div className={style.avatar}>{initial}</div>
                  <span className={style.userName}>{displayName}</span>
                  {isAdmin && <span className={style.roleTag}>Admin</span>}
                </div>
                <button
                  type="button"
                  onClick={() => logout()}
                  className={style.logoutBtn}
                >
                  Disconnect
                </button>
              </>
            ) : (
              <button
                type="button"
                onClick={() => login()}
                className={style.loginBtn}
              >
                Connect
              </button>
            )}
          </div>
        </div>
      </div>
    </header>
  );
}

export default MainHeader;
