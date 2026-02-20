"use server";

import { apiFetch } from "@/lib/api";
import type { InvoiceDto, InvoiceStatusDto } from "@/types/invoice";
import type {
  PaymentMethodDto,
  CreatePaymentMethodRequest,
  PaymentTransactionDto,
  PaymentTransactionFeeDto,
} from "@/types/payment";

// --- Invoices ---

export async function fetchInvoices(): Promise<InvoiceDto[]> {
  return apiFetch<InvoiceDto[]>("/api/Invoices", { cache: "no-store" });
}

export async function fetchInvoiceStatuses(): Promise<InvoiceStatusDto[]> {
  return apiFetch<InvoiceStatusDto[]>("/api/InvoiceStatuses", {
    cache: "no-store",
  });
}

export async function updateInvoiceStatus(
  id: number,
  statusId: number
): Promise<void> {
  await apiFetch(`/api/Invoices/${id}/status`, {
    method: "PATCH",
    body: JSON.stringify({ statusId }),
  });
}

// --- Payment Methods ---

export async function fetchPaymentMethods(): Promise<PaymentMethodDto[]> {
  return apiFetch<PaymentMethodDto[]>("/api/PaymentMethods", {
    cache: "no-store",
  });
}

export async function createPaymentMethod(
  payload: CreatePaymentMethodRequest
): Promise<PaymentMethodDto> {
  return apiFetch<PaymentMethodDto>("/api/PaymentMethods", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updatePaymentMethod(
  id: number,
  payload: CreatePaymentMethodRequest
): Promise<PaymentMethodDto> {
  return apiFetch<PaymentMethodDto>(`/api/PaymentMethods/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deletePaymentMethod(id: number): Promise<void> {
  await apiFetch(`/api/PaymentMethods/${id}`, { method: "DELETE" });
}

export async function togglePaymentMethodEnabled(
  id: number
): Promise<void> {
  await apiFetch(`/api/PaymentMethods/${id}/enabled`, { method: "PATCH" });
}

// --- Transactions ---

export async function fetchTransactions(): Promise<PaymentTransactionDto[]> {
  return apiFetch<PaymentTransactionDto[]>("/api/PaymentTransactions", {
    cache: "no-store",
  });
}

export async function fetchTransactionFees(): Promise<
  PaymentTransactionFeeDto[]
> {
  return apiFetch<PaymentTransactionFeeDto[]>("/api/PaymentTransactionFees", {
    cache: "no-store",
  });
}
