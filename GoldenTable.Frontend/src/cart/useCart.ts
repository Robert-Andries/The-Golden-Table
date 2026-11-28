import { useContext } from "react";
import { CartContext } from "./CartContext";
import type { CartContextValue } from "./types";

export function useCart(): CartContextValue {
  const ctx = useContext(CartContext);
  if (!ctx) {
    throw new Error("useCart must be used inside <CartProvider>");
  }
  return ctx;
}

export default useCart;
