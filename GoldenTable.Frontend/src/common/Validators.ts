import { validCurrencies } from "./constants";

export function isGreaterThen0(number: number | null): boolean {
  if (number === null) return false;
  return number > 0;
}

export function isGreaterOrEqualTo0(number: number | null): boolean {
  if (number === null) return false;
  return number >= 0;
}

export function hasMinLenght(text: string | null, lenght: number): boolean {
  if (text === null) return false;
  return text.trim().length >= lenght;
}

export function isValidCurrency(currency: string | null): boolean {
  if (currency === null) return false;
  return validCurrencies.includes(currency);
}

export function isStringArray(obj: any): obj is string[] {
  return Array.isArray(obj) && obj.every((item) => typeof item === "string");
}

export function isValidUri(uri : string | null | undefined) {
  if(uri === undefined || uri === null) return false;
  try {
    new URL(uri);
    return true;
  } catch {
    return false;
  }
}