import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { SectionHeader } from "@/components/section-header";
import { FinancialTabs } from "./financial-tabs";
import type { InvoiceDto, InvoiceStatusDto } from "@/types/invoice";
import type {
  PaymentMethodDto,
  PaymentTransactionDto,
  PaymentTransactionFeeDto,
  PaymentTransactionStatusDto,
} from "@/types/payment";
import type { TranslationDto, LanguageDto } from "@/types/system";

export default async function FinancialPage() {
  const session = await getSession();
  const access = session ? getAccessLevel(session.role, "financial") : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  if (!access) {
    return (
      <div>
        <SectionHeader
          title={t["admin.section.financial"] ?? "Financial"}
          description={t["admin.label.no_access"] ?? "You do not have access to this section."}
        />
      </div>
    );
  }

  let invoices: InvoiceDto[] = [];
  let invoiceStatuses: InvoiceStatusDto[] = [];
  let paymentMethods: PaymentMethodDto[] = [];
  let transactions: PaymentTransactionDto[] = [];
  let fees: PaymentTransactionFeeDto[] = [];
  let transactionStatuses: PaymentTransactionStatusDto[] = [];
  let translations: TranslationDto[] = [];
  let languages: LanguageDto[] = [];
  let error: string | null = null;

  try {
    [invoices, invoiceStatuses, paymentMethods, transactions, fees, transactionStatuses, translations, languages] =
      await Promise.all([
        apiFetch<InvoiceDto[]>("/api/Invoices", { cache: "no-store" }),
        apiFetch<InvoiceStatusDto[]>("/api/InvoiceStatuses", { cache: "no-store" }),
        apiFetch<PaymentMethodDto[]>("/api/PaymentMethods", { cache: "no-store" }),
        apiFetch<PaymentTransactionDto[]>("/api/PaymentTransactions", { cache: "no-store" }),
        apiFetch<PaymentTransactionFeeDto[]>("/api/PaymentTransactionFees", { cache: "no-store" }),
        apiFetch<PaymentTransactionStatusDto[]>("/api/PaymentTransactions/statuses", { cache: "no-store" }),
        apiFetch<TranslationDto[]>("/api/Translations", { cache: "no-store" }),
        apiFetch<LanguageDto[]>("/api/Languages", { cache: "no-store" }),
      ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load financial data";
  }

  const langId = languages.find((l) => l.isoCode === locale)?.id ?? 1;
  const pmNames: Record<number, string> = {};
  const isNames: Record<number, string> = {};
  const ptsNames: Record<number, string> = {};
  for (const tr of translations) {
    if (tr.field === "name" && tr.languageId === langId) {
      if (tr.entityType === "payment_method") {
        pmNames[tr.entityId] = tr.value;
      } else if (tr.entityType === "invoice_status") {
        isNames[tr.entityId] = tr.value;
      } else if (tr.entityType === "payment_transaction_status") {
        ptsNames[tr.entityId] = tr.value;
      }
    }
  }

  return (
    <div>
      <SectionHeader
        title={t["admin.section.financial"] ?? "Financial"}
        description={t["admin.section.financial.desc"] ?? "Invoices, payments, and revenue reports."}
      />

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <FinancialTabs
          invoices={invoices}
          invoiceStatuses={invoiceStatuses}
          paymentMethods={paymentMethods}
          transactions={transactions}
          fees={fees}
          transactionStatuses={transactionStatuses}
          pmNames={pmNames}
          isNames={isNames}
          ptsNames={ptsNames}
          access={access}
          t={t}
        />
      )}
    </div>
  );
}
