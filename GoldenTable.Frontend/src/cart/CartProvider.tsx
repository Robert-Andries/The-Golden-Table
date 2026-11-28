import { useEffect, useState, type ReactNode } from "react";
import type { orderItem } from "../types/order";
import { CartContext } from "./CartContext";
import type { CartContextValue } from "./types";

const STORAGE_KEY = "goldentable-cart";

function isOrderItem(value: unknown): value is orderItem {
  if (value === null || typeof value !== "object") return false;
  const item = value as Record<string, unknown>;
  return (
    typeof item.dishId === "string" &&
    (item.sizeName === null || typeof item.sizeName === "string") &&
    typeof item.quantity === "number"
  );
}

function loadCart(): orderItem[] {
  try {
    const stored: unknown = JSON.parse(localStorage.getItem(STORAGE_KEY) ?? "[]");
    return Array.isArray(stored) ? stored.filter(isOrderItem) : [];
  } catch {
    return [];
  }
}

export function CartProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<orderItem[]>(loadCart);

  useEffect(() => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
  }, [items]);

  function addItem(item: orderItem) {
    setItems((current) => {
      const existing = current.find(
        (i) => i.dishId === item.dishId && i.sizeName === item.sizeName,
      );
      if (!existing) return [...current, item];
      return current.map((i) =>
        i === existing ? { ...i, quantity: i.quantity + item.quantity } : i,
      );
    });
  }

  const value: CartContextValue = {
    items,
    count: items.reduce((sum, i) => sum + i.quantity, 0),
    addItem,
    setItems,
    clear: () => setItems([]),
  };

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
}

export default CartProvider;
