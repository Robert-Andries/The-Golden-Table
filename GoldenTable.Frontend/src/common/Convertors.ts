export function KCalToKJ(kcal: number): number {
  return kcal * 4.184;
}

export function KJToKCal(kJ: number): number {
  return kJ / 4.184;
}