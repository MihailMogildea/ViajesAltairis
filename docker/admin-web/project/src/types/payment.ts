export interface PaymentMethodDto {
  id: number;
  name: string;
  minDaysBeforeCheckin: number;
  enabled: boolean;
  createdAt: string;
}

export interface CreatePaymentMethodRequest {
  name: string;
  minDaysBeforeCheckin: number;
}

export interface PaymentTransactionDto {
  id: number;
  reservationId: number;
  paymentMethodId: number;
  transactionReference: string;
  amount: number;
  currencyId: number;
  exchangeRateId: number;
  status: string;
  createdAt: string;
  updatedAt: string;
}

export interface PaymentTransactionFeeDto {
  id: number;
  paymentTransactionId: number;
  feeType: string;
  feeAmount: number;
  feePercentage: number | null;
  fixedFeeAmount: number | null;
  currencyId: number;
  description: string | null;
  createdAt: string;
}
