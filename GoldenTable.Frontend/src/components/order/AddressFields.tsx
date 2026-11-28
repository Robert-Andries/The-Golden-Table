import type { ReactNode } from "react";
import Input from "../Input";
import { addressFieldNames } from "../../common/orders";
import type { address } from "../../types/order";
import styles from "./Order.module.css";

type props = {
  defaultAddress?: address;
};

export default function AddressFields({ defaultAddress }: props): ReactNode {
  return (
    <div className={styles.addressGrid}>
      <Input type="text" name={addressFieldNames.county} defaultText={defaultAddress?.county ?? ""} autoComplete="address-level1" />
      <Input type="text" name={addressFieldNames.city} defaultText={defaultAddress?.city ?? ""} autoComplete="address-level2" />
      <Input type="text" name={addressFieldNames.street} defaultText={defaultAddress?.street ?? ""} autoComplete="address-line1" />
      <Input type="text" name={addressFieldNames.streetNumber} defaultText={defaultAddress?.streetNumber ?? ""} />
      <Input type="text" name={addressFieldNames.zipCode} defaultText={defaultAddress?.zipCode ?? ""} autoComplete="postal-code" />
    </div>
  );
}
