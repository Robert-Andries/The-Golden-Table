import { useState, type FormEvent, type ReactNode } from "react";
import { Link, useNavigate } from "react-router-dom";

import { useAuth } from "../../auth/useAuth";
import { useCart } from "../../cart/useCart";
import { errorMessage, readAddress, validateAddress, validateItems } from "../../common/orders";
import useGetAllDishes from "../../communication/dish/getAllDishes/useGetAllDishes";
import useCreateOrder from "../../communication/order/create/useCreateOrder";
import AddressFields from "../../components/order/AddressFields";
import FormErrors from "../../components/order/FormErrors";
import OrderItemsEditor from "../../components/order/OrderItemsEditor";
import type { dish } from "../../types/dish";
import styles from "../orders/Orders.module.css";

type props = {
  dishes: dish[];
};

function Checkout(): ReactNode {
  const { authenticated, hasRole, login } = useAuth();

  if (!authenticated) {
    return (
      <div className={styles.actions}>
        <p className={styles.note}>Connect to your account to place the order.</p>
        <button type="button" className={styles.primaryBtn} onClick={() => login()}>
          Connect
        </button>
      </div>
    );
  }

  if (!hasRole("orders:create")) {
    return <p className={styles.note}>Your account is not allowed to place orders.</p>;
  }

  return null;
}

function DisplayCart({ dishes }: props): ReactNode {
  const { authenticated, hasRole } = useAuth();
  const { items, setItems, clear } = useCart();
  const navigate = useNavigate();
  const createOrder = useCreateOrder();
  const [errors, setErrors] = useState<string[]>([]);

  const canOrder = authenticated && hasRole("orders:create");

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const address = readAddress(new FormData(event.currentTarget));
    const validationErrors = [...validateItems(items), ...validateAddress(address)];
    setErrors(validationErrors);
    if (validationErrors.length > 0) return;

    createOrder.mutate(
      { items, address },
      {
        onSuccess: (order) => {
          clear();
          navigate(`/orders/${order.id}`, { state: { message: "Your order was placed!" } });
        },
        onError: (error) => setErrors([errorMessage(error)]),
      },
    );
  }

  if (items.length === 0) {
    return (
      <div className={styles.emptyState}>
        <p>Your cart is empty.</p>
        <Link to="/shop" className={styles.primaryBtn}>
          Browse the shop
        </Link>
      </div>
    );
  }

  return (
    <form className={styles.form} onSubmit={handleSubmit}>
      <section className={styles.section}>
        <h2 className={styles.sectionTitle}>Dishes</h2>
        <OrderItemsEditor items={items} dishes={dishes} onChange={setItems} disabled={createOrder.isPending} />
      </section>

      {canOrder && (
        <section className={styles.section}>
          <h2 className={styles.sectionTitle}>Delivery address</h2>
          <AddressFields />
        </section>
      )}

      <FormErrors errors={errors} />

      <div className={styles.buttonGroup}>
        <button type="button" className={styles.dangerBtn} onClick={clear} disabled={createOrder.isPending}>
          Empty cart
        </button>
        {canOrder ? (
          <button type="submit" className={styles.primaryBtn} disabled={createOrder.isPending}>
            {createOrder.isPending ? "Placing order..." : "Place order"}
          </button>
        ) : (
          <Checkout />
        )}
      </div>
    </form>
  );
}

export default function Cart(): ReactNode {
  const { data, isPending, isError, error } = useGetAllDishes();

  let content: ReactNode;
  if (isPending) {
    content = <div className={styles.loading}>Loading dishes, please wait...</div>;
  } else if (isError) {
    content = <div className={styles.error}>{`An error occurred while getting dishes, ${error.message}`}</div>;
  } else {
    content = <DisplayCart dishes={data} />;
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1 className={styles.title}>Your cart</h1>
        <Link to="/shop" className={styles.backLink}>
          ← Continue shopping
        </Link>
      </div>
      {content}
    </div>
  );
}
