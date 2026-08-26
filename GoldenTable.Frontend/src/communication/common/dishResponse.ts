export type size = {
    Name: string,
    PriceAdded: number,
    Weight: number
}

export type energy = {
    Kcal: number,
    Kj: number
}

export type carbohydrates = {
    Total: number,
    OfWhichSugar: number
}

export type nutritionalInformation = {
    Energy: energy,
    GramsOfFat: number,
    GramsOfCarbohydrates: carbohydrates,
    GramsOfProtein: number,
    GramsOfSalt: number
}

export type dishResponse = {
  Name: string,
  Description: string,
  BasePriceAmount: number,
  BasePriceCurrency: string,
  Category: string,
  Tags: string[],
  NutritionalInformation: nutritionalInformation,
  ImagesUris: string[],
  Sizes: size[]
}