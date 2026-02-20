"use client";

import { useState } from "react";
import type { PaymentTransactionDto, PaymentTransactionFeeDto } from "@/types/payment";

interface TransactionsTabProps {
  transactions: PaymentTransactionDto[];
  fees: PaymentTransactionFeeDto[];
  t: Record<string, string>;
}

export function TransactionsTab({
  transactions,
  fees,
  t,
}: TransactionsTabProps) {
  const [expandedId, setExpandedId] = useState<number | null>(null);

  const feesByTransaction = new Map<number, PaymentTransactionFeeDto[]>();
  for (const fee of fees) {
    const list = feesByTransaction.get(fee.paymentTransactionId) ?? [];
    list.push(fee);
    feesByTransaction.set(fee.paymentTransactionId, list);
  }

  function toggleExpand(id: number) {
    setExpandedId((prev) => (prev === id ? null : id));
  }

  if (transactions.length === 0) {
    return (
      <div className="rounded-lg border border-gray-200 bg-white p-8 text-center text-sm text-gray-500">
        {t["admin.financial.transactions.empty"] ?? "No transactions found."}
      </div>
    );
  }

  return (
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
                t={t}
              />
            );
          })}
        </tbody>
      </table>
    </div>
  );
}

function TransactionRow({
  transaction: tx,
  fees,
  isExpanded,
  hasFees,
  onToggle,
  t,
}: {
  transaction: PaymentTransactionDto;
  fees: PaymentTransactionFeeDto[];
  isExpanded: boolean;
  hasFees: boolean;
  onToggle: () => void;
  t: Record<string, string>;
}) {
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
          {tx.amount.toFixed(2)}
        </td>
        <td className="px-4 py-3">
          <span className="inline-flex items-center rounded-full bg-blue-100 px-2 py-0.5 text-xs font-medium text-blue-700">
            {tx.status}
          </span>
        </td>
        <td className="px-4 py-3 text-xs text-gray-500">
          {new Date(tx.createdAt).toLocaleDateString()}
        </td>
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
