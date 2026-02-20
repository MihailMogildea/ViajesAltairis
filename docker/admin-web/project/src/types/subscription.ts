export interface SubscriptionTypeDto {
  id: number;
  name: string;
  pricePerMonth: number;
  discount: number;
  currencyId: number;
  enabled: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CreateSubscriptionTypeRequest {
  name: string;
  pricePerMonth: number;
  discount: number;
  currencyId: number;
}
