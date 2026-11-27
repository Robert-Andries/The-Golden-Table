import { useState, type ReactNode } from "react";
import { estimateTotal, formatMoney, unitPrice } from "../../common/orders";
import type { dish } from "../../types/dish";
import type { orderItem } from "../../types/order";
import styles from "./Order.module.css";

type props = {
  items: orderItem[];
  dishes: dish[];
  onChange: (items: orderItem[]) => void;
  disabled?: boolean;
};

const STANDARD_SIZE = "";

export default function OrderItemsEditor({ items, dishes, onChange, disabled = false }: props): ReactNode {
  const [dishToAdd, setDishToAdd] = useState<string>("");
  const total = estimateTotal(items, dishes);

  function updateItem(index: number, change: Partial<orderItem>) {
    onChange(items.map((item, i) => (i === index ? { ...item, ...change } : item)));
  }

  function removeItem(index: number) {
    onChange(items.filter((_, i) => i !== index));
  }

  function addDish() {
    if (!dishToAdd) return;
    const existing = items.find((i) => i.dishId === dishToAdd && i.sizeName === null);
    if (existing) {
      onChange(items.map((i) => (i === existing ? { ...i, quantity: i.quantity + 1 } : i)));
    } else {
      onChange([...items, { dishId: dishToAdd, sizeName: null, quantity: 1 }]);
    }
    setDishToAdd("");
  }

  return (
    <div className={styles.editor}>
      {items.length === 0 && <p className={styles.muted}>No dishes selected yet.</p>}

      {items.map((item, index) => {
        const itemDish = dishes.find((d) => d.id === item.dishId);
        return (
          <div key={`${item.dishId}-${index}`} className={styles.editorRow}>
            <span className={styles.dishName} title={itemDish?.name}>
              {itemDish?.name ?? "Unavailable dish"}
            </span>

            <select
              className={styles.control}
              aria-label="Size"
              value={item.sizeName ?? STANDARD_SIZE}
              disabled={disabled || !itemDish || itemDish.sizes.length === 0}
              onChange={(e) =>
                updateItem(index, { sizeName: e.target.value === STANDARD_SIZE ? null : e.target.value })
              }
            >
              <option value={STANDARD_SIZE}>Standard</option>
              {itemDish?.sizes.map((size) => (
                <option key={size.name} value={size.name}>
                  {size.name} (+{size.priceAdded})
                </option>
              ))}
            </select>

            <input
              className={styles.control}
              aria-label="Quantity"
              type="number"
              min={1}
              step={1}
              value={item.quantity}
              disabled={disabled}
              onChange={(e) => updateItem(index, { quantity: Number.parseInt(e.target.value, 10) || 0 })}
            />

            <span className={styles.linePrice}>
              {itemDish
                ? formatMoney(unitPrice(itemDish, item.sizeName) * item.quantity, itemDish.basePriceCurrency)
                : "—"}
            </span>

            <button
              type="button"
              className={styles.removeBtn}
              onClick={() => removeItem(index)}
              disabled={disabled}
              aria-label={`Remove ${itemDish?.name ?? "dish"}`}
            >
              Remove
            </button>
          </div>
        );
      })}

      <div className={styles.addRow}>
        <select
          className={styles.control}
          aria-label="Dish to add"
          value={dishToAdd}
          disabled={disabled}
          onChange={(e) => setDishToAdd(e.target.value)}
        >
          <option value="">Choose a dish to add...</option>
          {dishes.map((d) => (
            <option key={d.id} value={d.id}>
              {d.name} — {formatMoney(d.basePriceAmount, d.basePriceCurrency)}
            </option>
          ))}
        </select>
        <button type="button" className={styles.addBtn} onClick={addDish} disabled={disabled || !dishToAdd}>
          Add dish
        </button>
      </div>

      {items.length > 0 && (
        <div className={styles.total}>
          <span>Estimated total:</span>
          <span className={styles.totalValue}>
            {total ? formatMoney(total.amount, total.currency) : "cannot be estimated"}
          </span>
        </div>
      )}
    </div>
  );
}
