import { useState, type ReactNode } from "react";

import styles from "./Shop.module.css";
import type { dishResponse } from "../../communication/common/dishResponse";
import searchDish from "../../common/searchDish";
import useGetAllDishes from "../../communication/getAllDishes/useGetAllDishes";
import ShopDishCard from "./ShopDishCard";

type props = {
  dishes: dishResponse[]
}

function DisplayShop({dishes} : props) : ReactNode {
  const [searchTerm, setSearchTerm] = useState<string>("");
  const displayedDishes = searchDish(dishes ?? [], searchTerm);

  return (
    <>
      <input type="text" placeholder="Search a dish" onChange={e => setSearchTerm(e.target.value)}/>
      <div className={styles.container}>
        {displayedDishes.map((dish) => <ShopDishCard dish={dish} />)}
      </div>
    </>
  );
}

function Shop(): ReactNode {
  const { data, isPending, isError, error } = useGetAllDishes();
  
  if (isPending) {
    return <div className={styles.loading}>Loading dishes, please wait...</div>;
  }
  if (isError) {
    return (
      <div
        className={styles.error}
      >{`An error occured while getting dishes, ${error.message}`}</div>
    );
  }
  if (data.length === 0) {
    return <div className={styles.emptyState}>No dishes found!</div>;
  }

  return <DisplayShop dishes={data} />
}



export default Shop;
