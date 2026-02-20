export interface SeasonalMarginDto {
  id: number;
  administrativeDivisionId: number;
  startMonthDay: string;
  endMonthDay: string;
  margin: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateSeasonalMarginRequest {
  administrativeDivisionId: number;
  startMonthDay: string;
  endMonthDay: string;
  margin: number;
}

export interface PromoCodeDto {
  id: number;
  code: string;
  discountPercentage: number | null;
  discountAmount: number | null;
  currencyId: number | null;
  validFrom: string;
  validTo: string;
  maxUses: number | null;
  currentUses: number;
  enabled: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreatePromoCodeRequest {
  code: string;
  discountPercentage: number | null;
  discountAmount: number | null;
  currencyId: number | null;
  validFrom: string;
  validTo: string;
  maxUses: number | null;
}
