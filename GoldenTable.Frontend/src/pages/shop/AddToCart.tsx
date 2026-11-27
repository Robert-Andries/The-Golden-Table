import { useEffect, useState, type ReactNode } from "react";
import { Link } from "react-router-dom";

import { useAuth } from "../../auth/useAuth";
import { useCart } from "../../cart/useCart";
import { formatMoney, unitPrice } from "../../common/orders";
import type { dish } from "../../types/dish";
import styles from "./AddToCart.module.css";

type props = {
  dish: dish;
};

const STANDARD_SIZE = "";

export default function AddToCart({ dish }: props): ReactNode {
  const { authenticated, hasRole } = useAuth();
  const { addItem } = useCart();
  const [sizeName, setSizeName] = useState<string>(STANDARD_SIZE);
  const [quantity, setQuantity] = useState<number>(1);
  const [added, setAdded] = useState<boolean>(false);

  const selectedSize = sizeName === STANDARD_SIZE ? null : sizeName;
  const validQuantity = Number.isInteger(quantity) && quantity > 0;

  useEffect(() => {
    if (!added) return;
    const timer = setTimeout(() => setAdded(false), 3000);
    return () => clearTimeout(timer);
  }, [added]);

  // Guests may fill a cart and connect at checkout; signed-in accounts need the permission
  if (authenticated && !hasRole("orders:create")) return null;

  function handleAdd() {
    if (!validQuantity) return;
    addItem({ dishId: dish.id, sizeName: selectedSize, quantity });
    setAdded(true);
  }

  return (
    <div className={styles.container}>
      <div className={styles.controls}>
        {dish.sizes.length > 0 && (
          <label className={styles.field}>
            <span className={styles.label}>Size</span>
            <select className={styles.control} value={sizeName} onChange={(e) => setSizeName(e.target.value)}>
              <option value={STANDARD_SIZE}>Standard</option>
              {dish.sizes.map((size) => (
                <option key={size.name} value={size.name}>
                  {size.name} (+{size.priceAdded})
                </option>
              ))}
            </select>
          </label>
        )}
        <label className={styles.field}>
          <span className={styles.label}>Quantity</span>
          <input
            className={styles.control}
            type="number"
            min={1}
            step={1}
            value={quantity}
            onChange={(e) => setQuantity(Number.parseInt(e.target.value, 10) || 0)}
          />
        </label>
      </div>

      <div className={styles.footer}>
        <span className={styles.price}>
          {formatMoney(unitPrice(dish, selectedSize) * (validQuantity ? quantity : 0), dish.basePriceCurrency)}
        </span>
        <button type="button" className={styles.addButton} onClick={handleAdd} disabled={!validQuantity}>
          Add to cart
        </button>
      </div>

      {added && (
        <p className={styles.added} role="status">
          Added to your cart. <Link to="/cart">View cart</Link>
        </p>
      )}
    </div>
  );
}
