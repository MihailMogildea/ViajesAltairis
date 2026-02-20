"use client";

import { useState } from "react";
import { TabBar } from "@/components/tab-bar";
import { InvoicesTab } from "./invoices-tab";
import { PaymentMethodsTab } from "./payment-methods-tab";
import { TransactionsTab } from "./transactions-tab";
import type { AccessLevel } from "@/lib/permissions";
import type { InvoiceDto, InvoiceStatusDto } from "@/types/invoice";
import type {
  PaymentMethodDto,
  PaymentTransactionDto,
  PaymentTransactionFeeDto,
} from "@/types/payment";

interface FinancialTabsProps {
  invoices: InvoiceDto[];
  invoiceStatuses: InvoiceStatusDto[];
  paymentMethods: PaymentMethodDto[];
  transactions: PaymentTransactionDto[];
  fees: PaymentTransactionFeeDto[];
  access: AccessLevel;
  t: Record<string, string>;
}

const TABS = [
  { key: "invoices", label: "Invoices" },
  { key: "payment-methods", label: "Payment Methods" },
  { key: "transactions", label: "Transactions" },
];

export function FinancialTabs({
  invoices,
  invoiceStatuses,
  paymentMethods,
  transactions,
  fees,
  access,
  t,
}: FinancialTabsProps) {
  const [activeTab, setActiveTab] = useState("invoices");

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
          access={access}
          t={t}
        />
      )}

      {activeTab === "payment-methods" && (
        <PaymentMethodsTab
          paymentMethods={paymentMethods}
          access={access}
          t={t}
        />
      )}

      {activeTab === "transactions" && (
        <TransactionsTab
          transactions={transactions}
          fees={fees}
          t={t}
        />
      )}
    </>
  );
}
