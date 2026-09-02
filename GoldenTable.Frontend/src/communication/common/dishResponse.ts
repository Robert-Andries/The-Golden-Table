export type size = {
  name: string;
  priceAdded: number;
  weight: number;
};

export type energy = {
  kcal: number;
  kj: number;
};

export type carbohydrates = {
  total: number;
  ofWhichSugar: number;
};

export type nutritionalInformation = {
  energy: energy;
  gramsOfFat: number;
  gramsOfCarbohydrates: carbohydrates;
  gramsOfProtein: number;
  gramsOfSalt: number;
};

export type dishResponse = {
  id: string;
  name: string;
  description: string;
  basePriceAmount: number;
  basePriceCurrency: string;
  category: string;
  tags: string[];
  nutritionalInformation: nutritionalInformation;
  imagesUris: string[];
  imageIds: string[];
  sizes: size[];
};
