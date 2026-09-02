import { useState } from "react";

export default function useModal(initialState: boolean = false) {
  const [isOpen, setIsOpen] = useState(initialState);

  function open() {
    setIsOpen(true);
  }

  function close() {
    setIsOpen(false);
  }

  function toggle() {
    setIsOpen((prev) => !prev);
  }

  return { isOpen, open, close, toggle };
}
