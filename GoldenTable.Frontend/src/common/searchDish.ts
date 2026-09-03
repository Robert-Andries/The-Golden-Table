import type { dishResponse } from "../communication/common/dishResponse";

export default function searchDish(
  dishes: dishResponse[],
  searchTerm: string,
): dishResponse[] {
  let output: dishResponse[] = [];
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
