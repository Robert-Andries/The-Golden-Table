export const OrderStatus = {
  Placed: "Placed",
  Rejected: "Rejected",
  Confirmed: "Confirmed",
  Cancelled: "Cancelled",
  Preparing: "Preparing",
  Sent: "Sent",
} as const;

export type OrderStatus = (typeof OrderStatus)[keyof typeof OrderStatus];