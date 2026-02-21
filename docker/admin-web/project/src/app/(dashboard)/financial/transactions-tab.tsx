"use client";

import { useState } from "react";
import type {
  PaymentTransactionDto,
  PaymentTransactionFeeDto,
  PaymentTransactionStatusDto,
} from "@/types/payment";
import { fetchTransactions } from "./actions";

interface TransactionsTabProps {
  transactions: PaymentTransactionDto[];
  fees: PaymentTransactionFeeDto[];
  transactionStatuses: PaymentTransactionStatusDto[];
  ptsNames: Record<number, string>;
  t: Record<string, string>;
  onConfirm?: (id: number) => Promise<void>;
}

export function TransactionsTab({
  transactions: initialTransactions,
  fees,
  transactionStatuses,
  ptsNames,
  t,
  onConfirm,
}: TransactionsTabProps) {
  const [expandedId, setExpandedId] = useState<number | null>(null);
  const [transactions, setTransactions] = useState(initialTransactions);
  const [filterFrom, setFilterFrom] = useState("");
  const [filterTo, setFilterTo] = useState("");
  const [filterStatusId, setFilterStatusId] = useState<number | "">("");
  const [filtering, setFiltering] = useState(false);

  const feesByTransaction = new Map<number, PaymentTransactionFeeDto[]>();
  for (const fee of fees) {
    const list = feesByTransaction.get(fee.paymentTransactionId) ?? [];
    list.push(fee);
    feesByTransaction.set(fee.paymentTransactionId, list);
  }

  function toggleExpand(id: number) {
    setExpandedId((prev) => (prev === id ? null : id));
  }

  async function applyFilters() {
    setFiltering(true);
    try {
      const result = await fetchTransactions({
        from: filterFrom || undefined,
        to: filterTo || undefined,
        statusId: filterStatusId !== "" ? filterStatusId : undefined,
      });
      setTransactions(result);
    } finally {
      setFiltering(false);
    }
  }

  async function handleConfirm(id: number) {
    if (!onConfirm) return;
    await onConfirm(id);
    const refreshed = await fetchTransactions({
      from: filterFrom || undefined,
      to: filterTo || undefined,
      statusId: filterStatusId !== "" ? filterStatusId : undefined,
    });
    setTransactions(refreshed);
  }

  function statusLabel(statusId: number): string {
    return ptsNames[statusId] ?? transactionStatuses.find((s) => s.id === statusId)?.name ?? String(statusId);
  }

  function statusBadgeClass(statusId: number): string {
    switch (statusId) {
      case 1: return "bg-amber-100 text-amber-700";   // pending
      case 2: return "bg-green-100 text-green-700";    // completed
      case 3: return "bg-red-100 text-red-700";        // cancelled
      case 4: return "bg-blue-100 text-blue-700";      // refunded
      default: return "bg-gray-100 text-gray-700";
    }
  }

  return (
    <div className="space-y-4">
      {/* Filters */}
      <div className="flex flex-wrap items-end gap-4 rounded-lg border border-gray-200 bg-white p-4">
        <div>
          <label className="mb-1 block text-xs font-medium text-gray-500">
            {t["admin.financial.filter.from"] ?? "From"}
          </label>
          <input
            type="date"
            value={filterFrom}
            onChange={(e) => setFilterFrom(e.target.value)}
            className="rounded border border-gray-300 px-3 py-1.5 text-sm"
          />
        </div>
        <div>
          <label className="mb-1 block text-xs font-medium text-gray-500">
            {t["admin.financial.filter.to"] ?? "To"}
          </label>
          <input
            type="date"
            value={filterTo}
            onChange={(e) => setFilterTo(e.target.value)}
            className="rounded border border-gray-300 px-3 py-1.5 text-sm"
          />
        </div>
        <div>
          <label className="mb-1 block text-xs font-medium text-gray-500">
            {t["admin.financial.filter.status"] ?? "Status"}
          </label>
          <select
            value={filterStatusId}
            onChange={(e) => setFilterStatusId(e.target.value ? Number(e.target.value) : "")}
            className="rounded border border-gray-300 px-3 py-1.5 text-sm"
          >
            <option value="">{t["admin.financial.filter.all_statuses"] ?? "All statuses"}</option>
            {transactionStatuses.map((s) => (
              <option key={s.id} value={s.id}>
                {ptsNames[s.id] ?? s.name}
              </option>
            ))}
          </select>
        </div>
        <button
          onClick={applyFilters}
          disabled={filtering}
          className="rounded bg-indigo-600 px-4 py-1.5 text-sm font-medium text-white hover:bg-indigo-700 disabled:opacity-50"
        >
          {filtering ? (t["admin.label.loading"] ?? "Loading...") : (t["admin.action.search"] ?? "Search")}
        </button>
      </div>

      {/* Table */}
      {transactions.length === 0 ? (
        <div className="rounded-lg border border-gray-200 bg-white p-8 text-center text-sm text-gray-500">
          {t["admin.financial.transactions.empty"] ?? "No transactions found."}
        </div>
      ) : (
        <div className="overflow-hidden rounded-lg border border-gray-200 bg-white">
          <table className="w-full text-left text-sm">
            <thead className="border-b border-gray-200 bg-gray-50 text-xs font-medium uppercase text-gray-500">
              <tr>
                <th className="w-8 px-4 py-3" />
                <th className="px-4 py-3">
                  {t["admin.field.transactionReference"] ?? "Reference"}
                </th>
                <th className="px-4 py-3">
                  {t["admin.field.reservationId"] ?? "Reservation"}
                </th>
                <th className="px-4 py-3 text-right">
                  {t["admin.field.amount"] ?? "Amount"}
                </th>
                <th className="px-4 py-3">
                  {t["admin.field.status"] ?? "Status"}
                </th>
                <th className="px-4 py-3">
                  {t["admin.field.createdAt"] ?? "Created"}
                </th>
                {onConfirm && (
                  <th className="px-4 py-3">
                    {t["admin.field.actions"] ?? "Actions"}
                  </th>
                )}
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {transactions.map((tx) => {
                const txFees = feesByTransaction.get(tx.id) ?? [];
                const isExpanded = expandedId === tx.id;
                const hasFees = txFees.length > 0;

                return (
                  <TransactionRow
                    key={tx.id}
                    transaction={tx}
                    fees={txFees}
                    isExpanded={isExpanded}
                    hasFees={hasFees}
                    onToggle={() => toggleExpand(tx.id)}
                    onConfirm={onConfirm ? handleConfirm : undefined}
                    statusLabel={statusLabel(tx.statusId)}
                    statusBadgeClass={statusBadgeClass(tx.statusId)}
                    t={t}
                  />
                );
              })}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

function TransactionRow({
  transaction: tx,
  fees,
  isExpanded,
  hasFees,
  onToggle,
  onConfirm,
  statusLabel,
  statusBadgeClass,
  t,
}: {
  transaction: PaymentTransactionDto;
  fees: PaymentTransactionFeeDto[];
  isExpanded: boolean;
  hasFees: boolean;
  onToggle: () => void;
  onConfirm?: (id: number) => Promise<void>;
  statusLabel: string;
  statusBadgeClass: string;
  t: Record<string, string>;
}) {
  const [confirming, setConfirming] = useState(false);
  return (
    <>
      <tr className="hover:bg-gray-50">
        <td className="px-4 py-3">
          {hasFees && (
            <button
              onClick={onToggle}
              className="text-gray-400 hover:text-gray-600"
              title={
                isExpanded
                  ? t["admin.action.collapse"] ?? "Collapse"
                  : t["admin.action.expand"] ?? "Expand"
              }
            >
              {isExpanded ? "\u25BC" : "\u25B6"}
            </button>
          )}
        </td>
        <td className="px-4 py-3">
          <span className="font-medium text-gray-900">
            {tx.transactionReference}
          </span>
        </td>
        <td className="px-4 py-3">{tx.reservationId}</td>
        <td className="px-4 py-3 text-right font-medium">
          {tx.amount.toFixed(2)} {tx.currencyCode}
        </td>
        <td className="px-4 py-3">
          <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${statusBadgeClass}`}>
            {statusLabel}
          </span>
        </td>
        <td className="px-4 py-3 text-xs text-gray-500">
          {new Date(tx.createdAt).toLocaleDateString()}
        </td>
        {onConfirm && (
          <td className="px-4 py-3">
            {tx.statusId === 1 && (
              <button
                onClick={async () => {
                  setConfirming(true);
                  try {
                    await onConfirm(tx.id);
                  } finally {
                    setConfirming(false);
                  }
                }}
                disabled={confirming}
                className="rounded bg-green-600 px-3 py-1 text-xs font-medium text-white hover:bg-green-700 disabled:opacity-50"
              >
                {confirming
                  ? (t["admin.action.confirming"] ?? "Confirming...")
                  : (t["admin.action.confirm"] ?? "Confirm")}
              </button>
            )}
          </td>
        )}
      </tr>
      {isExpanded &&
        fees.map((fee) => (
          <tr key={fee.id} className="bg-gray-50">
            <td className="px-4 py-2" />
            <td className="px-4 py-2 pl-10 text-xs text-gray-600" colSpan={2}>
              <span className="font-medium">{fee.feeType}</span>
              {fee.description && (
                <span className="ml-2 text-gray-400">
                  {fee.description}
                </span>
              )}
            </td>
            <td className="px-4 py-2 text-right text-xs text-gray-600">
              {fee.feeAmount.toFixed(2)}
              {fee.feePercentage != null && (
                <span className="ml-1 text-gray-400">
                  ({fee.feePercentage}%)
                </span>
              )}
            </td>
            <td className="px-4 py-2" />
            <td className="px-4 py-2 text-xs text-gray-400">
              {new Date(fee.createdAt).toLocaleDateString()}
            </td>
          </tr>
        ))}
    </>
  );
}
