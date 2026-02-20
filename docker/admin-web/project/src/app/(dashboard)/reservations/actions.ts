"use server";

import { apiFetch } from "@/lib/api";
import type {
  ReservationAdminDto,
  ReservationStatusDto,
  ReservationLineAdminDto,
  ReservationGuestAdminDto,
  CreateReservationRequest,
  AddLineRequest,
  AddGuestRequest,
  SubmitReservationRequest,
  CancelReservationRequest,
} from "@/types/reservation";

export async function fetchReservations(): Promise<ReservationAdminDto[]> {
  return apiFetch<ReservationAdminDto[]>("/api/Reservations", {
    cache: "no-store",
  });
}

export async function fetchReservation(id: number): Promise<ReservationAdminDto> {
  return apiFetch<ReservationAdminDto>(`/api/Reservations/${id}`, {
    cache: "no-store",
  });
}

export async function fetchStatuses(): Promise<ReservationStatusDto[]> {
  return apiFetch<ReservationStatusDto[]>("/api/ReservationStatuses", {
    cache: "no-store",
  });
}

export async function fetchReservationLines(id: number): Promise<ReservationLineAdminDto[]> {
  return apiFetch<ReservationLineAdminDto[]>(`/api/Reservations/${id}/lines`, {
    cache: "no-store",
  });
}

export async function fetchReservationGuests(id: number): Promise<ReservationGuestAdminDto[]> {
  return apiFetch<ReservationGuestAdminDto[]>(`/api/Reservations/${id}/guests`, {
    cache: "no-store",
  });
}

export async function createReservation(
  payload: CreateReservationRequest
): Promise<ReservationAdminDto> {
  return apiFetch<ReservationAdminDto>("/api/Reservations", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function addLine(
  id: number,
  payload: AddLineRequest
): Promise<number> {
  return apiFetch<number>(`/api/Reservations/${id}/lines`, {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function removeLine(id: number, lineId: number): Promise<void> {
  await apiFetch(`/api/Reservations/${id}/lines/${lineId}`, {
    method: "DELETE",
  });
}

export async function addGuest(
  id: number,
  lineId: number,
  payload: AddGuestRequest
): Promise<void> {
  await apiFetch(`/api/Reservations/${id}/lines/${lineId}/guests`, {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function changeStatus(
  id: number,
  statusId: number
): Promise<void> {
  await apiFetch(`/api/Reservations/${id}/status`, {
    method: "PATCH",
    body: JSON.stringify({ statusId }),
  });
}

export async function submitReservation(
  id: number,
  payload: SubmitReservationRequest
): Promise<void> {
  await apiFetch(`/api/Reservations/${id}/submit`, {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function cancelReservation(
  id: number,
  payload: CancelReservationRequest
): Promise<void> {
  await apiFetch(`/api/Reservations/${id}/cancel`, {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export interface HotelOption {
  id: number;
  name: string;
  enabled: boolean;
}

export interface RoomConfigOption {
  id: number;
  hotelProviderId: number;
  roomTypeId: number;
  capacity: number;
  quantity: number;
  pricePerNight: number;
  currencyId: number;
  enabled: boolean;
}

export interface BoardOption {
  id: number;
  hotelProviderRoomTypeId: number;
  boardTypeId: number;
  pricePerNight: number;
  enabled: boolean;
}

export interface PaymentMethodOption {
  id: number;
  name: string;
  enabled: boolean;
}

export async function fetchHotels(): Promise<HotelOption[]> {
  return apiFetch<HotelOption[]>("/api/Hotels", { cache: "no-store" });
}

export async function fetchRoomConfigs(): Promise<RoomConfigOption[]> {
  return apiFetch<RoomConfigOption[]>("/api/HotelProviderRoomTypes", { cache: "no-store" });
}

export async function fetchBoardOptions(): Promise<BoardOption[]> {
  return apiFetch<BoardOption[]>("/api/HotelProviderRoomTypeBoards", { cache: "no-store" });
}

export async function fetchPaymentMethods(): Promise<PaymentMethodOption[]> {
  return apiFetch<PaymentMethodOption[]>("/api/PaymentMethods", { cache: "no-store" });
}

export interface HotelProviderOption {
  id: number;
  hotelId: number;
  providerId: number;
  enabled: boolean;
}

export interface RoomTypeOption {
  id: number;
  name: string;
}

export interface BoardTypeOption {
  id: number;
  name: string;
}

export async function fetchHotelProviders(): Promise<HotelProviderOption[]> {
  return apiFetch<HotelProviderOption[]>("/api/HotelProviders", { cache: "no-store" });
}

export async function fetchRoomTypes(): Promise<RoomTypeOption[]> {
  return apiFetch<RoomTypeOption[]>("/api/RoomTypes", { cache: "no-store" });
}

export async function fetchBoardTypes(): Promise<BoardTypeOption[]> {
  return apiFetch<BoardTypeOption[]>("/api/BoardTypes", { cache: "no-store" });
}
