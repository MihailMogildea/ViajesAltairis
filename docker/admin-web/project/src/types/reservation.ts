export interface ReservationAdminDto {
  id: number;
  reservationCode: string;
  statusId: number;
  statusName: string;
  bookedByUserId: number;
  bookedByFirstName: string;
  bookedByLastName: string;
  ownerUserId: number | null;
  ownerFirstName: string;
  ownerLastName: string;
  ownerEmail: string | null;
  ownerPhone: string | null;
  ownerTaxId: string | null;
  ownerAddress: string | null;
  ownerCity: string | null;
  ownerPostalCode: string | null;
  ownerCountry: string | null;
  subtotal: number;
  taxAmount: number;
  marginAmount: number;
  discountAmount: number;
  totalPrice: number;
  currencyId: number;
  currencyCode: string;
  exchangeRateId: number;
  promoCodeId: number | null;
  promoCode: string | null;
  notes: string | null;
  lineCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateReservationRequest {
  currencyCode: string;
  promoCode: string | null;
  ownerUserId: number | null;
  ownerFirstName: string | null;
  ownerLastName: string | null;
  ownerEmail: string | null;
  ownerPhone: string | null;
  ownerTaxId: string | null;
  ownerAddress: string | null;
  ownerCity: string | null;
  ownerPostalCode: string | null;
  ownerCountry: string | null;
}

export interface AddLineRequest {
  roomConfigurationId: number;
  boardTypeId: number;
  checkIn: string;
  checkOut: string;
  guestCount: number;
}

export interface AddGuestRequest {
  firstName: string;
  lastName: string;
  email: string | null;
  phone: string | null;
}

export interface ReservationLineAdminDto {
  reservationLineId: number;
  reservationId: number;
  hotelId: number;
  hotelName: string;
  roomTypeId: number;
  roomTypeName: string;
  boardTypeId: number;
  boardTypeName: string;
  providerId: number;
  providerName: string;
  checkInDate: string;
  checkOutDate: string;
  numRooms: number;
  numGuests: number;
  pricePerNight: number;
  boardPricePerNight: number;
  numNights: number;
  subtotal: number;
  taxAmount: number;
  marginAmount: number;
  discountAmount: number;
  totalPrice: number;
  currencyCode: string;
}

export interface ReservationGuestAdminDto {
  guestId: number;
  reservationLineId: number;
  firstName: string;
  lastName: string;
  email: string | null;
  phone: string | null;
  hotelName: string;
  roomTypeName: string;
}

export interface SubmitReservationRequest {
  paymentMethodId: number;
  cardNumber: string | null;
  cardExpiry: string | null;
  cardCvv: string | null;
  cardHolderName: string | null;
}

export interface CancelReservationRequest {
  reason: string | null;
}

export interface ReservationStatusDto {
  id: number;
  name: string;
  createdAt: string;
}

export interface CancellationDto {
  id: number;
  reservationId: number;
  cancelledByUserId: number;
  cancelledByUserEmail: string;
  reason: string | null;
  penaltyPercentage: number;
  penaltyAmount: number;
  refundAmount: number;
  currencyId: number;
  createdAt: string;
}

export interface CancellationPolicyDto {
  id: number;
  hotelId: number;
  freeCancellationHours: number;
  penaltyPercentage: number;
  enabled: boolean;
  createdAt: string;
}
