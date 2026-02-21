export interface Country {
  id: number;
  name: string;
  iso_code: string;
  currency_id: number;
  enabled: boolean;
}

export interface AdministrativeDivision {
  id: number;
  name: string;
  country_id: number;
  type_id: number;
  parent_id: number | null;
  level: number;
}

export interface City {
  id: number;
  name: string;
  administrative_division_id: number;
}

export interface Region {
  id: number;
  name: string;
  country: string;
  cities: City[];
}

export interface Hotel {
  id: number;
  name: string;
  city_id: number;
  city_name: string;
  region_name: string;
  country_name: string;
  stars: number;
  latitude: number;
  longitude: number;
  check_in_time: string;
  check_out_time: string;
  phone: string;
  email: string;
  website: string;
  margin: number;
  enabled: boolean;
  summary: string;
  description: string;
  images: HotelImage[];
  amenities: Amenity[];
  min_price: number;
  avg_rating: number;
  review_count: number;
}

export interface HotelImage {
  id: number;
  hotel_id: number;
  url: string;
  alt_text: string;
  sort_order: number;
}

export interface RoomType {
  id: number;
  name: string;
  code: string;
}

export interface RoomConfig {
  id: number;
  hotel_id: number;
  room_type_id: number;
  room_type_name: string;
  capacity: number;
  quantity: number;
  base_price_per_night: number;
  currency_code: string;
  images: RoomImage[];
  amenities: Amenity[];
  board_options: BoardOption[];
}

export interface RoomImage {
  id: number;
  room_config_id: number;
  url: string;
  alt_text: string;
  sort_order: number;
}

export interface BoardType {
  id: number;
  name: string;
  code: string;
}

export interface BoardOption {
  board_type_id: number;
  board_type_name: string;
  board_type_code: string;
  price_supplement: number;
}

export interface AmenityCategory {
  id: number;
  name: string;
}

export interface Amenity {
  id: number;
  name: string;
  category: "hotel" | "room";
  icon: string;
}

export interface Review {
  id: number;
  hotel_id: number;
  user_name: string;
  rating: number;
  title: string;
  comment: string;
  created_at: string;
  response?: ReviewResponse;
}

export interface ReviewResponse {
  id: number;
  review_id: number;
  responder_name: string;
  comment: string;
  created_at: string;
}

export interface User {
  id: number;
  email: string;
  first_name: string;
  last_name: string;
  user_type: string;
  discount: number;
  subscription_type?: string;
  subscription_discount?: number;
  business_partner?: string;
  business_partner_discount?: number;
}

export interface PromoCode {
  id: number;
  code: string;
  discount: number;
  valid_from: string;
  valid_to: string;
  max_uses: number;
  current_uses: number;
  enabled: boolean;
}

export interface ReservationStatus {
  id: number;
  name: string;
  code: string;
}

export interface Reservation {
  id: number;
  code: string;
  user_id: number;
  status: string;
  owner_first_name: string;
  owner_last_name: string;
  owner_email: string;
  currency_code: string;
  exchange_rate: number;
  subtotal: number;
  discount_total: number;
  tax_total: number;
  total: number;
  promo_code?: string;
  created_at: string;
  lines: ReservationLine[];
  hotel_names?: string;
}

export interface ReservationLine {
  id: number;
  reservation_id: number;
  hotel_name: string;
  room_type_name: string;
  board_type_name: string;
  check_in: string;
  check_out: string;
  num_rooms: number;
  price_per_night: number;
  nights: number;
  line_total: number;
  guests: ReservationGuest[];
}

export interface ReservationGuest {
  id: number;
  first_name: string;
  last_name: string;
  email?: string;
  phone?: string;
}

export interface SubscriptionType {
  id: number;
  name: string;
  code: string;
  price: number;
  discount: number;
}

export interface SearchFilters {
  destination: string;
  checkIn: string;
  checkOut: string;
  guests: number;
  stars: number[];
  minPrice: number;
  maxPrice: number;
  amenities: number[];
  boardType: string;
  sortBy: "price_asc" | "price_desc" | "rating" | "stars";
}

export interface PaginationState {
  page: number;
  perPage: number;
  total: number;
}

export interface BasketItemTax {
  taxTypeName: string;
  rate: number;
  isPercentage: boolean;
}

export interface BasketItem {
  id: string;
  hotel_id: number;
  hotel_name: string;
  room_config_id: number;
  room_type_name: string;
  board_type_id: number;
  board_type_code: string;
  board_type_name: string;
  check_in: string;
  check_out: string;
  guests: number;
  num_rooms: number;
  price_per_night: number;
  board_supplement: number;
  nights: number;
  line_total: number;
  taxes?: BasketItemTax[];
}

// --- API response types (match client-api DTOs) ---

export interface ApiLoginResponse {
  token: string;
  expiresAt: string;
}

export interface ApiHotelSummary {
  id: number;
  name: string;
  stars: number;
  cityId: number;
  city: string;
  cityImageUrl: string | null;
  countryId: number;
  country: string;
  priceFrom: number | null;
  mainImageUrl: string | null;
}

export interface ApiSearchResponse {
  hotels: ApiHotelSummary[];
  totalCount: number;
}

export interface ApiHotelDetail {
  id: number;
  name: string;
  stars: number;
  description: string;
  address: string;
  city: string;
  country: string;
  latitude: number | null;
  longitude: number | null;
  phone: string | null;
  email: string | null;
  website: string | null;
  images: string[];
  amenities: string[];
}

export interface ApiRoomAvailability {
  roomTypeId: number;
  roomTypeDbId: number;
  roomTypeName: string;
  basePricePerNight: number;
  maxGuests: number;
  availableRooms: number;
  images: string[];
  boardOptions: ApiBoardOption[];
}

export interface ApiBoardOption {
  boardTypeId: number;
  boardTypeName: string;
  pricePerNight: number;
}

export interface ApiReviewDto {
  id: number;
  userName: string;
  rating: number;
  comment: string | null;
  createdAt: string;
  responseComment: string | null;
  responseDate: string | null;
}

export interface ApiReviewsResponse {
  reviews: ApiReviewDto[];
  totalCount: number;
  averageRating: number;
}

export interface ApiReservationSummary {
  id: number;
  status: string;
  createdAt: string;
  totalAmount: number;
  currency: string;
  lineCount: number;
  hotelNames: string | null;
}

export interface ApiReservationDetail {
  id: number;
  status: string;
  createdAt: string;
  totalAmount: number;
  totalDiscount: number;
  currency: string;
  exchangeRate: number;
  promoCode: string | null;
  lines: ApiReservationLine[];
}

export interface ApiReservationLine {
  id: number;
  hotelName: string;
  roomType: string;
  boardType: string;
  checkIn: string;
  checkOut: string;
  guestCount: number;
  lineTotal: number;
  guests?: { id: number; firstName: string; lastName: string }[];
}

export interface ApiSubmitResponse {
  reservationId: number;
  status: string;
  totalAmount: number;
  currency: string;
}

export interface ApiPromoValidation {
  isValid: boolean;
  discountPercentage: number | null;
  discountAmount: number | null;
  expiresAt: string | null;
  message: string | null;
}

export interface ApiProfileResponse {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  phone: string | null;
  country: string | null;
  preferredLanguage: string;
  preferredCurrency: string;
  discount: number;
  subscriptionType?: string;
  subscriptionDiscount?: number;
  createdAt: string;
}

export interface ApiRegisterResponse {
  userId: number;
  token: string;
}

export interface ApiUpdateProfileRequest {
  firstName: string;
  lastName: string;
  phone?: string;
  countryId?: number;
  preferredLanguageId?: number;
  preferredCurrencyId?: number;
}

export interface ApiInvoiceSummary {
  id: number;
  invoiceNumber: string;
  statusId: number;
  status: string;
  totalAmount: number;
  currency: string;
  issuedAt: string;
}

export interface ApiInvoiceDetail {
  id: number;
  invoiceNumber: string;
  status: string;
  subTotal: number;
  taxAmount: number;
  totalAmount: number;
  currency: string;
  exchangeRateToEur: number;
  issuedAt: string;
  paidAt: string | null;
  reservationId: number;
}

export interface ApiSubscriptionPlan {
  id: number;
  name: string;
  price: number;
  discount: number;
  currencyId: number;
  currencyCode: string;
}

export interface ApiMySubscription {
  subscriptionId?: number;
  subscriptionTypeId?: number;
  planName?: string;
  discount?: number;
  startDate?: string;
  endDate?: string;
  isActive: boolean;
}

export interface ApiSubscribeResponse {
  subscriptionId: number;
  startDate: string;
  endDate: string;
  paymentReference?: string;
}

export interface ApiCancellationPolicy {
  hoursBeforeCheckIn: number;
  penaltyPercentage: number;
}

export interface ApiCountry {
  id: number;
  code: string;
  name: string;
}

export interface ApiHotelTax {
  taxTypeName: string;
  rate: number;
  isPercentage: boolean;
}

export interface ApiPaymentMethod {
  id: number;
  code: string;
  name: string;
  minDaysBeforeCheckin: number;
}
