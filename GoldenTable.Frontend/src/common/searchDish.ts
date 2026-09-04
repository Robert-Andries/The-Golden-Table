import type { dish } from "../types/dish";

export default function searchDish(
  dishes: dish[],
  searchTerm: string,
): dish[] {
  let output: dish[] = [];
  const searchTermLower = searchTerm.toLowerCase();
  dishes.forEach((dish) => {
    if (
      dish.name.toLowerCase().includes(searchTermLower) ||
      dish.description.toLowerCase().includes(searchTermLower) ||
      dish.category.toLowerCase().includes(searchTerm)
    ) {
      output.push(dish);
      return;
    }
    for (const tag of dish.tags) {
      if (tag.toLowerCase().includes(searchTermLower)) {
        output.push(dish);
        break;
      }
    }
  });
  return output;
}
