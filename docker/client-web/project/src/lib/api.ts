import type {
  ApiLoginResponse,
  ApiRegisterResponse,
  ApiSearchResponse,
  ApiHotelDetail,
  ApiRoomAvailability,
  ApiReviewsResponse,
  ApiPromoValidation,
  ApiSubmitResponse,
  ApiReservationSummary,
  ApiReservationDetail,
  ApiProfileResponse,
  ApiUpdateProfileRequest,
  ApiInvoiceSummary,
  ApiInvoiceDetail,
  ApiSubscriptionPlan,
  ApiMySubscription,
  ApiSubscribeResponse,
  ApiCancellationPolicy,
  ApiCountry,
  ApiHotelTax,
  ApiPaymentMethod,
} from "@/types";
import { LOCALE_STORAGE_KEY, DEFAULT_LOCALE } from "./locale";

const API_BASE =
  process.env.NEXT_PUBLIC_API_URL || "http://localhost:5002/api";

// --- Token management ---

const TOKEN_KEY = "va_token";

export function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem(TOKEN_KEY);
}

export function setToken(token: string) {
  localStorage.setItem(TOKEN_KEY, token);
}

export function clearToken() {
  localStorage.removeItem(TOKEN_KEY);
}

// --- Fetch helpers ---

class ApiError extends Error {
  constructor(public status: number, message: string) {
    super(message);
  }
}

// 401 listener — AuthContext subscribes to clear auth state on token expiry
type AuthErrorListener = () => void;
const authErrorListeners: Set<AuthErrorListener> = new Set();

export function onAuthError(listener: AuthErrorListener): () => void {
  authErrorListeners.add(listener);
  return () => { authErrorListeners.delete(listener); };
}

function getLocaleForHeader(): string {
  if (typeof window === "undefined") return DEFAULT_LOCALE;
  return localStorage.getItem(LOCALE_STORAGE_KEY) || DEFAULT_LOCALE;
}

async function apiFetch<T>(
  path: string,
  options: RequestInit = {}
): Promise<T> {
  const token = getToken();
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    "Accept-Language": getLocaleForHeader(),
    ...((options.headers as Record<string, string>) || {}),
  };
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const res = await fetch(`${API_BASE}${path}`, { ...options, headers });

  if (!res.ok) {
    // Handle expired/invalid token
    if (res.status === 401 && token) {
      clearToken();
      authErrorListeners.forEach((fn) => fn());
    }
    const text = await res.text().catch(() => res.statusText);
    throw new ApiError(res.status, text);
  }

  if (res.status === 204) return undefined as T;

  const text = await res.text();
  if (!text) return undefined as T;
  return JSON.parse(text);
}

// --- Auth ---

export async function apiLogin(
  email: string,
  password: string
): Promise<ApiLoginResponse> {
  return apiFetch<ApiLoginResponse>("/auth/login", {
    method: "POST",
    body: JSON.stringify({ email, password }),
  });
}

export async function apiRegister(params: {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phone?: string;
  preferredLanguageId?: number;
  preferredCurrencyId?: number;
}): Promise<ApiRegisterResponse> {
  return apiFetch<ApiRegisterResponse>("/auth/register", {
    method: "POST",
    body: JSON.stringify(params),
  });
}

// --- Profile ---

export async function apiGetProfile(): Promise<ApiProfileResponse> {
  return apiFetch<ApiProfileResponse>("/profile");
}

export async function apiUpdateProfile(
  data: ApiUpdateProfileRequest
): Promise<ApiProfileResponse> {
  return apiFetch<ApiProfileResponse>("/profile", {
    method: "PUT",
    body: JSON.stringify(data),
  });
}

export async function apiChangePassword(
  currentPassword: string,
  newPassword: string
): Promise<void> {
  await apiFetch("/profile/password", {
    method: "PUT",
    body: JSON.stringify({ currentPassword, newPassword }),
  });
}

// --- Hotels ---

export async function apiSearchHotels(params: {
  cityId?: number;
  countryId?: number;
  checkIn?: string;
  checkOut?: string;
  guests?: number;
  stars?: number[];
  amenityIds?: number[];
  page?: number;
  pageSize?: number;
}): Promise<ApiSearchResponse> {
  const qs = new URLSearchParams();
  if (params.cityId) qs.set("cityId", String(params.cityId));
  if (params.countryId) qs.set("countryId", String(params.countryId));
  if (params.checkIn) qs.set("checkIn", params.checkIn);
  if (params.checkOut) qs.set("checkOut", params.checkOut);
  if (params.guests) qs.set("guests", String(params.guests));
  if (params.stars && params.stars.length > 0) {
    params.stars.forEach((s) => qs.append("stars", String(s)));
  }
  if (params.amenityIds && params.amenityIds.length > 0) {
    params.amenityIds.forEach((id) => qs.append("amenityIds", String(id)));
  }
  if (params.page) qs.set("page", String(params.page));
  if (params.pageSize) qs.set("pageSize", String(params.pageSize));
  const query = qs.toString();
  return apiFetch<ApiSearchResponse>(`/hotels${query ? `?${query}` : ""}`);
}

export async function apiGetHotelDetail(
  id: number
): Promise<ApiHotelDetail> {
  return apiFetch<ApiHotelDetail>(`/hotels/${id}`);
}

export async function apiGetRoomAvailability(
  hotelId: number,
  checkIn: string,
  checkOut: string,
  guests: number
): Promise<ApiRoomAvailability[]> {
  const qs = new URLSearchParams({
    checkIn,
    checkOut,
    guests: String(guests),
  });
  const res = await apiFetch<{ rooms: ApiRoomAvailability[] }>(
    `/hotels/${hotelId}/availability?${qs}`
  );
  return res.rooms;
}

export async function apiGetHotelReviews(
  hotelId: number,
  page = 1,
  pageSize = 10
): Promise<ApiReviewsResponse> {
  const qs = new URLSearchParams({
    page: String(page),
    pageSize: String(pageSize),
  });
  return apiFetch<ApiReviewsResponse>(
    `/hotels/${hotelId}/reviews?${qs}`
  );
}

// --- Promo Codes ---

export async function apiValidatePromoCode(
  code: string
): Promise<ApiPromoValidation> {
  const qs = new URLSearchParams({ code });
  return apiFetch<ApiPromoValidation>(`/promo-codes/validate?${qs}`);
}

// --- Reservations ---

export async function apiCreateDraftReservation(
  currencyCode: string,
  promoCode?: string
): Promise<number> {
  return apiFetch<number>("/reservations", {
    method: "POST",
    body: JSON.stringify({ currencyCode, promoCode }),
  });
}

export async function apiAddReservationLine(
  reservationId: number,
  roomConfigurationId: number,
  boardTypeId: number,
  checkIn: string,
  checkOut: string,
  guestCount: number
): Promise<number> {
  return apiFetch<number>(`/reservations/${reservationId}/lines`, {
    method: "POST",
    body: JSON.stringify({
      roomConfigurationId,
      boardTypeId,
      checkIn,
      checkOut,
      guestCount,
    }),
  });
}

export async function apiAddReservationGuest(
  reservationId: number,
  lineId: number,
  firstName: string,
  lastName: string
): Promise<void> {
  await apiFetch(`/reservations/${reservationId}/lines/${lineId}/guests`, {
    method: "POST",
    body: JSON.stringify({ firstName, lastName }),
  });
}

export async function apiSubmitReservation(
  reservationId: number,
  paymentMethodId: number,
  cardNumber?: string,
  cardExpiry?: string,
  cardCvv?: string,
  cardHolderName?: string
): Promise<ApiSubmitResponse> {
  return apiFetch<ApiSubmitResponse>(
    `/reservations/${reservationId}/submit`,
    {
      method: "POST",
      body: JSON.stringify({
        paymentMethodId,
        cardNumber: cardNumber || null,
        cardExpiry: cardExpiry || null,
        cardCvv: cardCvv || null,
        cardHolderName: cardHolderName || null,
      }),
    }
  );
}

export async function apiGetMyReservations(
  page = 1,
  pageSize = 10,
  status?: string
): Promise<{ reservations: ApiReservationSummary[]; totalCount: number }> {
  const qs = new URLSearchParams({
    page: String(page),
    pageSize: String(pageSize),
  });
  if (status) qs.set("status", status);
  return apiFetch(`/reservations?${qs}`);
}

export async function apiGetReservationDetail(
  id: number
): Promise<ApiReservationDetail> {
  return apiFetch<ApiReservationDetail>(`/reservations/${id}`);
}

export async function apiCancelReservation(
  id: number,
  reason?: string
): Promise<void> {
  await apiFetch(`/reservations/${id}/cancel`, {
    method: "POST",
    body: JSON.stringify(reason ? { reason } : {}),
  });
}

// --- Reference data (i18n) ---

export async function apiGetWebTranslations(): Promise<Record<string, string>> {
  return apiFetch<Record<string, string>>("/reference/translations");
}

export interface ApiLanguage {
  id: number;
  code: string;
  name: string;
}

export async function apiGetLanguages(): Promise<ApiLanguage[]> {
  const res = await apiFetch<{ languages: ApiLanguage[] }>("/reference/languages");
  return res.languages;
}

export interface ApiCurrency {
  id: number;
  code: string;
  name: string;
  symbol: string;
  exchangeRateToEur: number;
}

export async function apiGetCurrencies(): Promise<ApiCurrency[]> {
  const res = await apiFetch<{ currencies: ApiCurrency[] }>("/reference/currencies");
  return res.currencies;
}

// --- Forgot / Reset password ---

export async function apiForgotPassword(email: string): Promise<void> {
  await apiFetch("/auth/forgot-password", {
    method: "POST",
    body: JSON.stringify({ email }),
  });
}

export async function apiResetPassword(token: string, newPassword: string): Promise<void> {
  await apiFetch("/auth/reset-password", {
    method: "POST",
    body: JSON.stringify({ token, newPassword }),
  });
}

// --- Reservation lines ---

export async function apiRemoveReservationLine(
  reservationId: number,
  lineId: number
): Promise<void> {
  await apiFetch(`/reservations/${reservationId}/lines/${lineId}`, {
    method: "DELETE",
  });
}

// --- Invoices ---

export async function apiGetInvoices(
  page = 1,
  pageSize = 10
): Promise<{ invoices: ApiInvoiceSummary[]; totalCount: number }> {
  const qs = new URLSearchParams({ page: String(page), pageSize: String(pageSize) });
  return apiFetch(`/invoices?${qs}`);
}

export async function apiGetInvoiceDetail(id: number): Promise<ApiInvoiceDetail> {
  return apiFetch<ApiInvoiceDetail>(`/invoices/${id}`);
}

export async function apiDownloadInvoicePdf(id: number): Promise<void> {
  const token = getToken();
  const headers: Record<string, string> = {
    "Accept-Language": getLocaleForHeader(),
  };
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }
  const res = await fetch(`${API_BASE}/invoices/${id}/pdf`, { headers });
  if (!res.ok) {
    throw new Error(`Failed to download PDF: ${res.status}`);
  }
  const blob = await res.blob();
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = `invoice-${id}.pdf`;
  document.body.appendChild(a);
  a.click();
  document.body.removeChild(a);
  URL.revokeObjectURL(url);
}

export async function apiGenerateInvoice(reservationId: number): Promise<ApiInvoiceDetail> {
  return apiFetch<ApiInvoiceDetail>("/invoices/generate", {
    method: "POST",
    body: JSON.stringify({ reservationId }),
  });
}

// --- Reviews ---

export async function apiSubmitReview(data: {
  reservationLineId: number;
  rating: number;
  title?: string;
  comment?: string;
}): Promise<number> {
  return apiFetch<number>("/reviews", {
    method: "POST",
    body: JSON.stringify(data),
  });
}

// --- Subscriptions ---

export async function apiGetSubscriptionPlans(): Promise<{ plans: ApiSubscriptionPlan[] }> {
  return apiFetch<{ plans: ApiSubscriptionPlan[] }>("/subscriptions/plans");
}

export async function apiGetMySubscription(): Promise<ApiMySubscription> {
  return apiFetch<ApiMySubscription>("/subscriptions/me");
}

export async function apiSubscribe(
  subscriptionTypeId: number,
  paymentMethodId: number,
  cardNumber: string,
  cardExpiry: string,
  cardCvv: string,
  cardHolderName: string
): Promise<ApiSubscribeResponse> {
  return apiFetch<ApiSubscribeResponse>("/subscriptions", {
    method: "POST",
    body: JSON.stringify({ subscriptionTypeId, paymentMethodId, cardNumber, cardExpiry, cardCvv, cardHolderName }),
  });
}

export async function apiCancelSubscription(): Promise<void> {
  await apiFetch("/subscriptions/me", { method: "DELETE" });
}

// --- Cancellation policy ---

export async function apiGetCancellationPolicy(
  hotelId: number
): Promise<ApiCancellationPolicy[]> {
  const res = await apiFetch<{ policies: ApiCancellationPolicy[] }>(
    `/hotels/${hotelId}/cancellation-policy`
  );
  return res.policies;
}

// --- Countries ---

export async function apiGetCountries(): Promise<ApiCountry[]> {
  const res = await apiFetch<{ countries: ApiCountry[] }>("/reference/countries");
  return res.countries;
}

// --- Payment methods ---

export async function apiGetPaymentMethods(): Promise<ApiPaymentMethod[]> {
  const res = await apiFetch<{ paymentMethods: ApiPaymentMethod[] }>("/reference/payment-methods");
  return res.paymentMethods;
}

// --- Hotel taxes ---

export async function apiGetHotelTaxes(
  hotelId: number
): Promise<ApiHotelTax[]> {
  return apiFetch<ApiHotelTax[]>(`/reference/taxes/${hotelId}`);
}
