export interface UserDto {
  id: number;
  userTypeId: number;
  email: string;
  firstName: string;
  lastName: string;
  phone: string | null;
  taxId: string | null;
  address: string | null;
  city: string | null;
  postalCode: string | null;
  country: string | null;
  languageId: number | null;
  businessPartnerId: number | null;
  providerId: number | null;
  discount: number;
  enabled: boolean;
  createdAt: string;
}

export interface CreateUserRequest {
  userTypeId: number;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phone: string | null;
  taxId: string | null;
  address: string | null;
  city: string | null;
  postalCode: string | null;
  country: string | null;
  languageId: number | null;
  businessPartnerId: number | null;
  providerId: number | null;
  discount: number;
}

export interface UpdateUserRequest {
  userTypeId: number;
  email: string;
  firstName: string;
  lastName: string;
  phone: string | null;
  taxId: string | null;
  address: string | null;
  city: string | null;
  postalCode: string | null;
  country: string | null;
  languageId: number | null;
  businessPartnerId: number | null;
  providerId: number | null;
  discount: number;
}

export interface UserTypeDto {
  id: number;
  name: string;
  createdAt: string;
}

export interface UserHotelDto {
  id: number;
  userId: number;
  hotelId: number;
  createdAt: string;
}

export interface UserSubscriptionDto {
  id: number;
  userId: number;
  subscriptionTypeId: number;
  startDate: string;
  endDate: string | null;
  active: boolean;
  createdAt: string;
  updatedAt: string;
}
