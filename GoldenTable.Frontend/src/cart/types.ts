import type { orderItem } from "../types/order";

export interface CartContextValue {
  items: orderItem[];
  /** Total number of units in the cart */
  count: number;
  /** Adds an item, merging it with an existing line for the same dish and size */
  addItem: (item: orderItem) => void;
  setItems: (items: orderItem[]) => void;
  clear: () => void;
}
