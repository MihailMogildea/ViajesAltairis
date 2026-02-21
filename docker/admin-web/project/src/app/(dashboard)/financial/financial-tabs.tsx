"use client";

import { useState } from "react";
import { TabBar } from "@/components/tab-bar";
import { InvoicesTab } from "./invoices-tab";
import { PaymentMethodsTab } from "./payment-methods-tab";
import { TransactionsTab } from "./transactions-tab";
import { BillingExportTab } from "./billing-export-tab";
import type { AccessLevel } from "@/lib/permissions";
import type { InvoiceDto, InvoiceStatusDto } from "@/types/invoice";
import type {
  PaymentMethodDto,
  PaymentTransactionDto,
  PaymentTransactionFeeDto,
  PaymentTransactionStatusDto,
} from "@/types/payment";
import { confirmTransaction } from "./actions";

interface FinancialTabsProps {
  invoices: InvoiceDto[];
  invoiceStatuses: InvoiceStatusDto[];
  paymentMethods: PaymentMethodDto[];
  transactions: PaymentTransactionDto[];
  fees: PaymentTransactionFeeDto[];
  transactionStatuses: PaymentTransactionStatusDto[];
  pmNames: Record<number, string>;
  isNames: Record<number, string>;
  ptsNames: Record<number, string>;
  access: AccessLevel;
  t: Record<string, string>;
}

const TABS = [
  { key: "invoices", label: "Invoices" },
  { key: "payment-methods", label: "Payment Methods" },
  { key: "transactions", label: "Transactions" },
  { key: "billing-export", label: "Billing Export" },
];

export function FinancialTabs({
  invoices,
  invoiceStatuses,
  paymentMethods,
  transactions,
  fees,
  transactionStatuses,
  pmNames,
  isNames,
  ptsNames,
  access,
  t,
}: FinancialTabsProps) {
  const [activeTab, setActiveTab] = useState("invoices");

  async function handleConfirmTransaction(id: number) {
    await confirmTransaction(id);
  }

  const tabs = TABS.map((tab) => ({
    ...tab,
    label: t[`admin.financial.tab.${tab.key}`] ?? tab.label,
  }));

  return (
    <>
      <TabBar tabs={tabs} active={activeTab} onChange={setActiveTab} />

      {activeTab === "invoices" && (
        <InvoicesTab
          invoices={invoices}
          statuses={invoiceStatuses}
          isNames={isNames}
          access={access}
          t={t}
        />
      )}

      {activeTab === "payment-methods" && (
        <PaymentMethodsTab
          paymentMethods={paymentMethods}
          pmNames={pmNames}
          access={access}
          t={t}
        />
      )}

      {activeTab === "transactions" && (
        <TransactionsTab
          transactions={transactions}
          fees={fees}
          transactionStatuses={transactionStatuses}
          ptsNames={ptsNames}
          t={t}
          onConfirm={handleConfirmTransaction}
        />
      )}

      {activeTab === "billing-export" && access === "full" && (
        <BillingExportTab t={t} />
      )}
    </>
  );
}
