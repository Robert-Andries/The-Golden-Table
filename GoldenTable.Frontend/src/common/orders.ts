import type { dish } from "../types/dish";
import type { address, orderItem } from "../types/order";
import { OrderStatus } from "../types/orderStatus";

/** Mirrors Order.AllowedTransitions in the backend domain. */
export const allowedStatusTransitions: Record<OrderStatus, OrderStatus[]> = {
  [OrderStatus.Placed]: [OrderStatus.Confirmed, OrderStatus.Rejected, OrderStatus.Cancelled],
  [OrderStatus.Confirmed]: [OrderStatus.Preparing, OrderStatus.Cancelled],
  [OrderStatus.Preparing]: [OrderStatus.Sent, OrderStatus.Cancelled],
  [OrderStatus.Sent]: [],
  [OrderStatus.Rejected]: [],
  [OrderStatus.Cancelled]: [],
};

/** Statuses in the order an order moves through them, final ones last. */
export const allOrderStatuses: OrderStatus[] = [
  OrderStatus.Placed,
  OrderStatus.Confirmed,
  OrderStatus.Preparing,
  OrderStatus.Sent,
  OrderStatus.Rejected,
  OrderStatus.Cancelled,
];

/** A sent, cancelled or rejected order can no longer be changed by the customer. */
export function isOrderFinished(status: OrderStatus): boolean {
  return (
    status === OrderStatus.Sent ||
    status === OrderStatus.Cancelled ||
    status === OrderStatus.Rejected
  );
}

export function formatMoney(amount: number, currency: string): string {
  return `${amount.toFixed(2)} ${currency}`;
}

export function formatDate(isoDate: string): string {
  // The backend sends UTC dates without a zone suffix
  const normalized = /[zZ]|[+-]\d\d:\d\d$/.test(isoDate) ? isoDate : `${isoDate}Z`;
  return new Date(normalized).toLocaleString();
}

export function shortId(id: string): string {
  return id.slice(0, 8).toUpperCase();
}

/** Price of one unit of a dish in the given size, the way the backend computes it. */
export function unitPrice(dish: dish, sizeName: string | null): number {
  const size = sizeName === null ? undefined : dish.sizes.find((s) => s.name === sizeName);
  return dish.basePriceAmount + (size?.priceAdded ?? 0);
}

export type estimatedTotal = {
  amount: number;
  currency: string;
};

/**
 * Estimates the total of a list of items. Returns null when a dish is unknown
 * or the dishes are priced in different currencies (the backend rejects those).
 */
export function estimateTotal(items: orderItem[], dishes: dish[]): estimatedTotal | null {
  let amount = 0;
  let currency: string | null = null;

  for (const item of items) {
    const itemDish = dishes.find((d) => d.id === item.dishId);
    if (!itemDish) return null;
    if (currency !== null && currency !== itemDish.basePriceCurrency) return null;
    currency = itemDish.basePriceCurrency;
    amount += unitPrice(itemDish, item.sizeName) * item.quantity;
  }

  return currency === null ? null : { amount, currency };
}

export function describeItem(item: orderItem, dishes: dish[]): string {
  const itemDish = dishes.find((d) => d.id === item.dishId);
  const name = itemDish?.name ?? "Unknown dish";
  return item.sizeName ? `${name} (${item.sizeName})` : name;
}

export function validateItems(items: orderItem[]): string[] {
  const errors: string[] = [];
  if (items.length === 0) errors.push("Add at least one dish.");
  if (items.some((i) => !Number.isInteger(i.quantity) || i.quantity <= 0)) {
    errors.push("Every quantity should be a whole number greater than 0.");
  }
  return errors;
}

export const addressFieldNames = {
  county: "County",
  city: "City",
  street: "Street",
  streetNumber: "Street number",
  zipCode: "Zip code",
} as const satisfies Record<keyof address, string>;

export function readAddress(formData: FormData): address {
  const read = (name: string) => ((formData.get(name) as string | null) ?? "").trim();
  return {
    county: read(addressFieldNames.county),
    city: read(addressFieldNames.city),
    street: read(addressFieldNames.street),
    streetNumber: read(addressFieldNames.streetNumber),
    zipCode: read(addressFieldNames.zipCode),
  };
}

export function validateAddress(value: address): string[] {
  const errors: string[] = [];
  for (const key of Object.keys(addressFieldNames) as (keyof address)[]) {
    if (value[key].length === 0) errors.push(`${addressFieldNames[key]} is required.`);
  }
  return errors;
}

export function sameAddress(a: address, b: address): boolean {
  return (Object.keys(addressFieldNames) as (keyof address)[]).every((key) => a[key] === b[key]);
}

export function errorMessage(error: unknown): string {
  if (error instanceof Error) return error.message;
  return "An unknown error occurred";
}
