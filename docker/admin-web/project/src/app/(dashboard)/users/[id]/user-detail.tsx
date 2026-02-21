"use client";

import { useState } from "react";
import {
  updateUser,
  createUserHotel,
  deleteUserHotel,
  createUserSubscription,
  deleteUserSubscription,
} from "./actions";
import { TabBar } from "@/components/tab-bar";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { StatusBadge } from "@/components/status-badge";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import type { UserDto, UserTypeDto, UserHotelDto, UserSubscriptionDto } from "@/types/user";
import type { LanguageDto } from "@/types/system";
import type { HotelDto } from "@/types/hotel";
import type { SubscriptionTypeDto } from "@/types/subscription";
import type { BusinessPartnerDto } from "@/types/business-partner";
import type { ProviderDto } from "@/types/provider";
import type { AccessLevel } from "@/lib/permissions";

const TABS = [
  { key: "info", label: "Info" },
  { key: "hotels", label: "Hotel Access" },
  { key: "subscriptions", label: "Subscriptions" },
];

interface InfoFormState {
  userTypeId: number;
  email: string;
  firstName: string;
  lastName: string;
  phone: string;
  taxId: string;
  address: string;
  city: string;
  postalCode: string;
  country: string;
  languageId: string;
  businessPartnerId: string;
  providerId: string;
  discount: number;
}

function userToForm(user: UserDto): InfoFormState {
  return {
    userTypeId: user.userTypeId,
    email: user.email,
    firstName: user.firstName,
    lastName: user.lastName,
    phone: user.phone ?? "",
    taxId: user.taxId ?? "",
    address: user.address ?? "",
    city: user.city ?? "",
    postalCode: user.postalCode ?? "",
    country: user.country ?? "",
    languageId: user.languageId != null ? String(user.languageId) : "",
    businessPartnerId: user.businessPartnerId != null ? String(user.businessPartnerId) : "",
    providerId: user.providerId != null ? String(user.providerId) : "",
    discount: user.discount,
  };
}

export function UserDetail({
  user: initialUser,
  userTypes,
  languages,
  hotels,
  userHotels: initialUserHotels,
  subscriptionTypes,
  userSubscriptions: initialUserSubscriptions,
  businessPartners,
  providers,
  utNames,
  access,
  t,
}: {
  user: UserDto;
  userTypes: UserTypeDto[];
  languages: LanguageDto[];
  hotels: HotelDto[];
  userHotels: UserHotelDto[];
  subscriptionTypes: SubscriptionTypeDto[];
  userSubscriptions: UserSubscriptionDto[];
  businessPartners: BusinessPartnerDto[];
  providers: ProviderDto[];
  utNames: Record<number, string>;
  access: AccessLevel | null;
  t: Record<string, string>;
}) {
  const [activeTab, setActiveTab] = useState("info");
  const [user, setUser] = useState(initialUser);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const isFull = access === "full";

  const tabs = TABS.map((tab) => ({
    ...tab,
    label:
      tab.key === "info"
        ? t["admin.users.tab.info"] ?? tab.label
        : tab.key === "hotels"
          ? t["admin.users.tab.hotels"] ?? tab.label
          : t["admin.users.tab.subscriptions"] ?? tab.label,
  }));

  return (
    <>
      <ToastMessage message={message} onDismiss={() => setMessage(null)} />
      <TabBar tabs={tabs} active={activeTab} onChange={setActiveTab} />

      {activeTab === "info" && (
        <InfoTab
          user={user}
          setUser={setUser}
          userTypes={userTypes}
          languages={languages}
          businessPartners={businessPartners}
          providers={providers}
          utNames={utNames}
          isFull={isFull}
          pending={pending}
          setPending={setPending}
          setMessage={setMessage}
          t={t}
        />
      )}

      {activeTab === "hotels" && (
        <HotelAccessTab
          user={user}
          initialUserHotels={initialUserHotels}
          hotels={hotels}
          isFull={isFull}
          pending={pending}
          setPending={setPending}
          setMessage={setMessage}
          t={t}
        />
      )}

      {activeTab === "subscriptions" && (
        <SubscriptionsTab
          user={user}
          initialUserSubscriptions={initialUserSubscriptions}
          subscriptionTypes={subscriptionTypes}
          isFull={isFull}
          pending={pending}
          setPending={setPending}
          setMessage={setMessage}
          t={t}
        />
      )}
    </>
  );
}

/* ---- Info Tab ---- */

function InfoTab({
  user,
  setUser,
  userTypes,
  languages,
  businessPartners,
  providers,
  utNames,
  isFull,
  pending,
  setPending,
  setMessage,
  t,
}: {
  user: UserDto;
  setUser: (u: UserDto) => void;
  userTypes: UserTypeDto[];
  languages: LanguageDto[];
  businessPartners: BusinessPartnerDto[];
  providers: ProviderDto[];
  utNames: Record<number, string>;
  isFull: boolean;
  pending: boolean;
  setPending: (v: boolean) => void;
  setMessage: (v: string | null) => void;
  t: Record<string, string>;
}) {
  const [form, setForm] = useState<InfoFormState>(userToForm(user));

  async function handleSave() {
    setPending(true);
    setMessage(null);
    try {
      const payload = {
        userTypeId: form.userTypeId,
        email: form.email,
        firstName: form.firstName,
        lastName: form.lastName,
        phone: form.phone || null,
        taxId: form.taxId || null,
        address: form.address || null,
        city: form.city || null,
        postalCode: form.postalCode || null,
        country: form.country || null,
        languageId: form.languageId ? Number(form.languageId) : null,
        businessPartnerId: form.businessPartnerId ? Number(form.businessPartnerId) : null,
        providerId: form.providerId ? Number(form.providerId) : null,
        discount: form.discount,
      };
      const updated = await updateUser(user.id, payload);
      setUser({ ...user, ...updated });
      setMessage(t["admin.message.updated"] ?? "Updated successfully.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Update failed");
    } finally {
      setPending(false);
    }
  }

  return (
    <div className="max-w-2xl rounded-lg border border-gray-200 bg-white p-6">
      <div className="space-y-4">
        <FormField
          label={t["admin.field.userType"] ?? "User Type"}
          type="select"
          value={form.userTypeId}
          onChange={(v) => setForm((f) => ({ ...f, userTypeId: Number(v) }))}
          options={userTypes.map((ut) => ({ value: ut.id, label: utNames[ut.id] ?? ut.name }))}
          disabled={!isFull}
        />
        <FormField
          label={t["admin.field.email"] ?? "Email"}
          type="email"
          value={form.email}
          onChange={(v) => setForm((f) => ({ ...f, email: String(v) }))}
          disabled={!isFull}
          required
        />
        <FormField
          label={t["admin.field.firstName"] ?? "First Name"}
          value={form.firstName}
          onChange={(v) => setForm((f) => ({ ...f, firstName: String(v) }))}
          disabled={!isFull}
          required
        />
        <FormField
          label={t["admin.field.lastName"] ?? "Last Name"}
          value={form.lastName}
          onChange={(v) => setForm((f) => ({ ...f, lastName: String(v) }))}
          disabled={!isFull}
          required
        />
        <FormField
          label={t["admin.field.phone"] ?? "Phone"}
          value={form.phone}
          onChange={(v) => setForm((f) => ({ ...f, phone: String(v) }))}
          disabled={!isFull}
        />
        <FormField
          label={t["admin.field.taxId"] ?? "Tax ID"}
          value={form.taxId}
          onChange={(v) => setForm((f) => ({ ...f, taxId: String(v) }))}
          disabled={!isFull}
        />
        <FormField
          label={t["admin.field.address"] ?? "Address"}
          value={form.address}
          onChange={(v) => setForm((f) => ({ ...f, address: String(v) }))}
          disabled={!isFull}
        />
        <FormField
          label={t["admin.field.city"] ?? "City"}
          value={form.city}
          onChange={(v) => setForm((f) => ({ ...f, city: String(v) }))}
          disabled={!isFull}
        />
        <FormField
          label={t["admin.field.postalCode"] ?? "Postal Code"}
          value={form.postalCode}
          onChange={(v) => setForm((f) => ({ ...f, postalCode: String(v) }))}
          disabled={!isFull}
        />
        <FormField
          label={t["admin.field.country"] ?? "Country"}
          value={form.country}
          onChange={(v) => setForm((f) => ({ ...f, country: String(v) }))}
          disabled={!isFull}
        />
        <FormField
          label={t["admin.field.language"] ?? "Language"}
          type="select"
          value={form.languageId}
          onChange={(v) => setForm((f) => ({ ...f, languageId: String(v) }))}
          options={languages.map((l) => ({ value: l.id, label: l.name }))}
          disabled={!isFull}
        />
        <FormField
          label={t["admin.field.businessPartner"] ?? "Business Partner"}
          type="select"
          value={form.businessPartnerId}
          onChange={(v) => setForm((f) => ({ ...f, businessPartnerId: String(v) }))}
          options={businessPartners.map((bp) => ({ value: bp.id, label: bp.companyName }))}
          disabled={!isFull}
        />
        <FormField
          label={t["admin.field.provider"] ?? "Provider"}
          type="select"
          value={form.providerId}
          onChange={(v) => setForm((f) => ({ ...f, providerId: String(v) }))}
          options={providers.map((p) => ({ value: p.id, label: p.name }))}
          disabled={!isFull}
        />
        <FormField
          label={t["admin.field.discount"] ?? "Discount (%)"}
          type="number"
          step="0.01"
          value={form.discount}
          onChange={(v) => setForm((f) => ({ ...f, discount: Number(v) }))}
          disabled={!isFull}
        />
      </div>

      {isFull && (
        <div className="mt-6 flex justify-end">
          <button
            onClick={handleSave}
            disabled={pending}
            className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
          >
            {pending ? "..." : t["admin.action.save"] ?? "Save"}
          </button>
        </div>
      )}
    </div>
  );
}

/* ---- Hotel Access Tab ---- */

function HotelAccessTab({
  user,
  initialUserHotels,
  hotels,
  isFull,
  pending,
  setPending,
  setMessage,
  t,
}: {
  user: UserDto;
  initialUserHotels: UserHotelDto[];
  hotels: HotelDto[];
  isFull: boolean;
  pending: boolean;
  setPending: (v: boolean) => void;
  setMessage: (v: string | null) => void;
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initialUserHotels);
  const [modalOpen, setModalOpen] = useState(false);
  const [hotelId, setHotelId] = useState<number>(0);
  const [deleting, setDeleting] = useState<UserHotelDto | null>(null);

  const hotelMap = Object.fromEntries(hotels.map((h) => [h.id, h.name]));

  async function handleAssign() {
    setPending(true);
    setMessage(null);
    try {
      const created = await createUserHotel(user.id, hotelId);
      setItems((prev) => [...prev, created]);
      setMessage(t["admin.message.created"] ?? "Created successfully.");
      setModalOpen(false);
      setHotelId(0);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Operation failed");
    } finally {
      setPending(false);
    }
  }

  async function handleDelete() {
    if (!deleting) return;
    setPending(true);
    setMessage(null);
    try {
      await deleteUserHotel(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<UserHotelDto>[] = [
    {
      key: "hotelId",
      header: t["admin.field.hotel"] ?? "Hotel",
      render: (item) => <span>{hotelMap[item.hotelId] ?? item.hotelId}</span>,
    },
  ];

  return (
    <>
      {isFull && (
        <SectionHeader
          title=""
          action={
            <button
              onClick={() => {
                setHotelId(0);
                setModalOpen(true);
              }}
              className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
            >
              {t["admin.action.assign"] ?? "Assign"}
            </button>
          }
        />
      )}

      <DataTable
        columns={columns}
        data={items}
        keyField="id"
        emptyMessage={t["admin.users.hotels_empty"] ?? "No hotel access assigned."}
        actions={
          isFull
            ? (item) => (
                <button
                  onClick={() => setDeleting(item)}
                  className="rounded border border-red-300 px-3 py-1 text-xs font-medium text-red-700 hover:bg-red-50"
                >
                  {t["admin.action.delete"] ?? "Delete"}
                </button>
              )
            : undefined
        }
      />

      <FormModal
        open={modalOpen}
        title={t["admin.users.assign_hotel"] ?? "Assign Hotel Access"}
        onClose={() => {
          setModalOpen(false);
          setHotelId(0);
        }}
        onSubmit={handleAssign}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.hotel"] ?? "Hotel"}
          type="select"
          value={hotelId}
          onChange={(v) => setHotelId(Number(v))}
          options={hotels.map((h) => ({ value: h.id, label: h.name }))}
          required
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
        message={
          (t["admin.confirm.delete.message"] ?? "Are you sure you want to delete \"{name}\"?").replace(
            "{name}",
            deleting ? (hotelMap[deleting.hotelId] ?? String(deleting.hotelId)) : ""
          )
        }
        onConfirm={handleDelete}
        onCancel={() => setDeleting(null)}
        loading={pending}
        confirmLabel={t["admin.action.delete"] ?? "Delete"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      />
    </>
  );
}

/* ---- Subscriptions Tab ---- */

function SubscriptionsTab({
  user,
  initialUserSubscriptions,
  subscriptionTypes,
  isFull,
  pending,
  setPending,
  setMessage,
  t,
}: {
  user: UserDto;
  initialUserSubscriptions: UserSubscriptionDto[];
  subscriptionTypes: SubscriptionTypeDto[];
  isFull: boolean;
  pending: boolean;
  setPending: (v: boolean) => void;
  setMessage: (v: string | null) => void;
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initialUserSubscriptions);
  const [modalOpen, setModalOpen] = useState(false);
  const [subForm, setSubForm] = useState({ subscriptionTypeId: 0, startDate: "", endDate: "" });
  const [deleting, setDeleting] = useState<UserSubscriptionDto | null>(null);

  const subTypeMap = Object.fromEntries(subscriptionTypes.map((st) => [st.id, st.name]));

  async function handleAssign() {
    setPending(true);
    setMessage(null);
    try {
      const created = await createUserSubscription(
        user.id,
        subForm.subscriptionTypeId,
        subForm.startDate,
        subForm.endDate || null
      );
      setItems((prev) => [...prev, created]);
      setMessage(t["admin.message.created"] ?? "Created successfully.");
      setModalOpen(false);
      setSubForm({ subscriptionTypeId: 0, startDate: "", endDate: "" });
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Operation failed");
    } finally {
      setPending(false);
    }
  }

  async function handleDelete() {
    if (!deleting) return;
    setPending(true);
    setMessage(null);
    try {
      await deleteUserSubscription(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<UserSubscriptionDto>[] = [
    {
      key: "subscriptionTypeId",
      header: t["admin.field.subscriptionType"] ?? "Subscription Type",
      render: (item) => <span>{subTypeMap[item.subscriptionTypeId] ?? item.subscriptionTypeId}</span>,
    },
    {
      key: "startDate",
      header: t["admin.field.startDate"] ?? "Start Date",
      render: (item) => <span>{item.startDate}</span>,
    },
    {
      key: "endDate",
      header: t["admin.field.endDate"] ?? "End Date",
      render: (item) => <span>{item.endDate ?? "-"}</span>,
    },
    {
      key: "active",
      header: t["admin.field.active"] ?? "Active",
      render: (item) => (
        <StatusBadge variant={item.active ? "success" : "disabled"}>
          {item.active
            ? t["admin.label.active"] ?? "Active"
            : t["admin.label.inactive"] ?? "Inactive"}
        </StatusBadge>
      ),
    },
  ];

  return (
    <>
      {isFull && (
        <SectionHeader
          title=""
          action={
            <button
              onClick={() => {
                setSubForm({ subscriptionTypeId: 0, startDate: "", endDate: "" });
                setModalOpen(true);
              }}
              className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
            >
              {t["admin.action.assign"] ?? "Assign"}
            </button>
          }
        />
      )}

      <DataTable
        columns={columns}
        data={items}
        keyField="id"
        emptyMessage={t["admin.users.subscriptions_empty"] ?? "No subscriptions assigned."}
        actions={
          isFull
            ? (item) => (
                <button
                  onClick={() => setDeleting(item)}
                  className="rounded border border-red-300 px-3 py-1 text-xs font-medium text-red-700 hover:bg-red-50"
                >
                  {t["admin.action.delete"] ?? "Delete"}
                </button>
              )
            : undefined
        }
      />

      <FormModal
        open={modalOpen}
        title={t["admin.users.assign_subscription"] ?? "Assign Subscription"}
        onClose={() => {
          setModalOpen(false);
          setSubForm({ subscriptionTypeId: 0, startDate: "", endDate: "" });
        }}
        onSubmit={handleAssign}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.subscriptionType"] ?? "Subscription Type"}
          type="select"
          value={subForm.subscriptionTypeId}
          onChange={(v) => setSubForm((f) => ({ ...f, subscriptionTypeId: Number(v) }))}
          options={subscriptionTypes.map((st) => ({ value: st.id, label: st.name }))}
          required
        />
        <FormField
          label={t["admin.field.startDate"] ?? "Start Date"}
          type="date"
          value={subForm.startDate}
          onChange={(v) => setSubForm((f) => ({ ...f, startDate: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.endDate"] ?? "End Date"}
          type="date"
          value={subForm.endDate}
          onChange={(v) => setSubForm((f) => ({ ...f, endDate: String(v) }))}
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
        message={
          (t["admin.confirm.delete.message"] ?? "Are you sure you want to delete \"{name}\"?").replace(
            "{name}",
            deleting ? (subTypeMap[deleting.subscriptionTypeId] ?? String(deleting.subscriptionTypeId)) : ""
          )
        }
        onConfirm={handleDelete}
        onCancel={() => setDeleting(null)}
        loading={pending}
        confirmLabel={t["admin.action.delete"] ?? "Delete"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      />
    </>
  );
}
