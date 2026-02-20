"use client";

import { useState, useMemo } from "react";
import { StatusBadge } from "@/components/status-badge";
import { SectionHeader } from "@/components/section-header";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ToastMessage } from "@/components/toast-message";
import {
  changeStatus,
  submitReservation,
  cancelReservation,
  addLine,
  removeLine,
  addGuest,
  fetchReservationLines,
  fetchReservationGuests,
} from "../actions";
import type {
  ReservationAdminDto,
  ReservationStatusDto,
  ReservationLineAdminDto,
  ReservationGuestAdminDto,
  SubmitReservationRequest,
  AddLineRequest,
} from "@/types/reservation";
import type {
  HotelOption,
  RoomConfigOption,
  BoardOption,
  PaymentMethodOption,
  HotelProviderOption,
  RoomTypeOption,
  BoardTypeOption,
} from "../actions";

type Variant = "enabled" | "disabled" | "success" | "warning" | "danger" | "info";

function statusVariant(statusName: string): Variant {
  const lower = statusName.toLowerCase();
  if (lower.includes("draft")) return "info";
  if (lower.includes("pending")) return "warning";
  if (lower.includes("confirmed")) return "success";
  if (lower.includes("checked")) return "success";
  if (lower.includes("completed")) return "enabled";
  if (lower.includes("cancelled")) return "danger";
  return "disabled";
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleString();
}

function formatDateShort(dateStr: string) {
  return new Date(dateStr).toLocaleDateString();
}

const EMPTY_SUBMIT: SubmitReservationRequest = {
  paymentMethodId: 0,
  cardNumber: null,
  cardExpiry: null,
  cardCvv: null,
  cardHolderName: null,
};

const EMPTY_LINE: AddLineRequest = {
  roomConfigurationId: 0,
  boardTypeId: 0,
  checkIn: "",
  checkOut: "",
  guestCount: 1,
};

interface AddGuestForm {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
}

const EMPTY_GUEST: AddGuestForm = { firstName: "", lastName: "", email: "", phone: "" };

function canEdit(access: string | null): boolean {
  return access === "full" || access === "own";
}

export function ReservationDetail({
  reservation: initial,
  statuses,
  initialLines,
  initialGuests,
  hotels,
  roomConfigs,
  boardOptions,
  paymentMethods,
  hotelProviders,
  roomTypes,
  boardTypes,
  access,
  t,
}: {
  reservation: ReservationAdminDto;
  statuses: ReservationStatusDto[];
  initialLines: ReservationLineAdminDto[];
  initialGuests: ReservationGuestAdminDto[];
  hotels: HotelOption[];
  roomConfigs: RoomConfigOption[];
  boardOptions: BoardOption[];
  paymentMethods: PaymentMethodOption[];
  hotelProviders: HotelProviderOption[];
  roomTypes: RoomTypeOption[];
  boardTypes: BoardTypeOption[];
  access: string | null;
  t: Record<string, string>;
}) {
  const [reservation, setReservation] = useState(initial);
  const [lines, setLines] = useState(initialLines);
  const [guests, setGuests] = useState(initialGuests);
  const [selectedStatusId, setSelectedStatusId] = useState(reservation.statusId);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  // Cancel dialog
  const [showCancel, setShowCancel] = useState(false);
  const [cancelReason, setCancelReason] = useState("");

  // Submit modal
  const [showSubmit, setShowSubmit] = useState(false);
  const [submitForm, setSubmitForm] = useState<SubmitReservationRequest>({ ...EMPTY_SUBMIT });

  // Add line modal
  const [showAddLine, setShowAddLine] = useState(false);
  const [lineForm, setLineForm] = useState<AddLineRequest>({ ...EMPTY_LINE });
  const [selectedHotelId, setSelectedHotelId] = useState<number>(0);

  // Add guest modal
  const [showAddGuest, setShowAddGuest] = useState(false);
  const [guestForm, setGuestForm] = useState<AddGuestForm>({ ...EMPTY_GUEST });
  const [guestLineId, setGuestLineId] = useState<number>(0);

  const isDraft = reservation.statusName.toLowerCase().includes("draft");

  // Build lookup maps
  const roomTypeMap = useMemo(() => new Map(roomTypes.map((rt) => [rt.id, rt.name])), [roomTypes]);
  const boardTypeMap = useMemo(() => new Map(boardTypes.map((bt) => [bt.id, bt.name])), [boardTypes]);

  // Hotel provider IDs for selected hotel
  const hotelProviderIds = useMemo(() => {
    if (!selectedHotelId) return new Set<number>();
    return new Set(hotelProviders.filter((hp) => hp.hotelId === selectedHotelId && hp.enabled).map((hp) => hp.id));
  }, [selectedHotelId, hotelProviders]);

  // Room configs for selected hotel (enabled only)
  const filteredRoomConfigs = useMemo(
    () => roomConfigs.filter((rc) => hotelProviderIds.has(rc.hotelProviderId) && rc.enabled),
    [roomConfigs, hotelProviderIds]
  );

  // Board options for selected room config (enabled only)
  const filteredBoardOptions = useMemo(
    () => boardOptions.filter((bo) => bo.hotelProviderRoomTypeId === lineForm.roomConfigurationId && bo.enabled),
    [boardOptions, lineForm.roomConfigurationId]
  );

  // Enabled hotels only
  const enabledHotels = useMemo(() => hotels.filter((h) => h.enabled), [hotels]);
  const enabledPaymentMethods = useMemo(() => paymentMethods.filter((pm) => pm.enabled), [paymentMethods]);

  async function handleChangeStatus() {
    if (selectedStatusId === reservation.statusId) return;
    setPending(true);
    setMessage(null);
    try {
      await changeStatus(reservation.id, selectedStatusId);
      const newStatus = statuses.find((s) => s.id === selectedStatusId);
      setReservation((prev) => ({
        ...prev,
        statusId: selectedStatusId,
        statusName: newStatus?.name ?? prev.statusName,
      }));
      setMessage(t["admin.reservations.status_updated"] ?? "Status updated.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Status change failed");
    } finally {
      setPending(false);
    }
  }

  async function handleCancel() {
    setPending(true);
    setMessage(null);
    try {
      await cancelReservation(reservation.id, {
        reason: cancelReason || null,
      });
      const cancelledStatus = statuses.find((s) =>
        s.name.toLowerCase().includes("cancelled")
      );
      setReservation((prev) => ({
        ...prev,
        statusId: cancelledStatus?.id ?? prev.statusId,
        statusName: cancelledStatus?.name ?? "Cancelled",
      }));
      setShowCancel(false);
      setCancelReason("");
      setMessage(t["admin.reservations.cancelled"] ?? "Reservation cancelled.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Cancel failed");
    } finally {
      setPending(false);
    }
  }

  async function handleSubmit() {
    setPending(true);
    setMessage(null);
    try {
      await submitReservation(reservation.id, {
        ...submitForm,
        cardNumber: submitForm.cardNumber || null,
        cardExpiry: submitForm.cardExpiry || null,
        cardCvv: submitForm.cardCvv || null,
        cardHolderName: submitForm.cardHolderName || null,
      });
      const pendingStatus = statuses.find((s) =>
        s.name.toLowerCase().includes("pending")
      );
      setReservation((prev) => ({
        ...prev,
        statusId: pendingStatus?.id ?? prev.statusId,
        statusName: pendingStatus?.name ?? "Pending",
      }));
      setShowSubmit(false);
      setSubmitForm({ ...EMPTY_SUBMIT });
      setMessage(t["admin.reservations.submitted"] ?? "Reservation submitted.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Submit failed");
    } finally {
      setPending(false);
    }
  }

  async function handleAddLine() {
    if (!lineForm.roomConfigurationId || !lineForm.boardTypeId || !lineForm.checkIn || !lineForm.checkOut) return;
    setPending(true);
    setMessage(null);
    try {
      await addLine(reservation.id, lineForm);
      // Refresh lines and reservation
      const [updatedLines, updatedGuests] = await Promise.all([
        fetchReservationLines(reservation.id),
        fetchReservationGuests(reservation.id),
      ]);
      setLines(updatedLines);
      setGuests(updatedGuests);
      setReservation((prev) => ({ ...prev, lineCount: updatedLines.length }));
      setShowAddLine(false);
      setLineForm({ ...EMPTY_LINE });
      setSelectedHotelId(0);
      setMessage(t["admin.reservations.line_added"] ?? "Line added.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Add line failed");
    } finally {
      setPending(false);
    }
  }

  async function handleRemoveLine(lineId: number) {
    if (!confirm(t["admin.reservations.confirm_remove_line"] ?? "Remove this line?")) return;
    setPending(true);
    setMessage(null);
    try {
      await removeLine(reservation.id, lineId);
      const [updatedLines, updatedGuests] = await Promise.all([
        fetchReservationLines(reservation.id),
        fetchReservationGuests(reservation.id),
      ]);
      setLines(updatedLines);
      setGuests(updatedGuests);
      setReservation((prev) => ({ ...prev, lineCount: updatedLines.length }));
      setMessage(t["admin.reservations.line_removed"] ?? "Line removed.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Remove line failed");
    } finally {
      setPending(false);
    }
  }

  async function handleAddGuest() {
    if (!guestForm.firstName || !guestForm.lastName) return;
    setPending(true);
    setMessage(null);
    try {
      await addGuest(reservation.id, guestLineId, {
        firstName: guestForm.firstName,
        lastName: guestForm.lastName,
        email: guestForm.email || null,
        phone: guestForm.phone || null,
      });
      const updatedGuests = await fetchReservationGuests(reservation.id);
      setGuests(updatedGuests);
      setShowAddGuest(false);
      setGuestForm({ ...EMPTY_GUEST });
      setGuestLineId(0);
      setMessage(t["admin.reservations.guest_added"] ?? "Guest added.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Add guest failed");
    } finally {
      setPending(false);
    }
  }

  function openAddGuest(lineId: number) {
    setGuestLineId(lineId);
    setGuestForm({ ...EMPTY_GUEST });
    setShowAddGuest(true);
  }

  return (
    <div className="space-y-8">
      <ToastMessage message={message} onDismiss={() => setMessage(null)} />

      {/* Section 1: Header */}
      <div className="rounded-lg border border-gray-200 bg-white p-6">
        <div className="flex flex-wrap items-center justify-between gap-4">
          <div>
            <h2 className="text-2xl font-semibold">{reservation.reservationCode}</h2>
            <div className="mt-2 flex items-center gap-3">
              <StatusBadge variant={statusVariant(reservation.statusName)}>
                {reservation.statusName}
              </StatusBadge>
              <span className="text-xs text-gray-500">
                {t["admin.reservations.label.created"] ?? "Created"}: {formatDate(reservation.createdAt)}
              </span>
              <span className="text-xs text-gray-500">
                {t["admin.reservations.label.updated"] ?? "Updated"}: {formatDate(reservation.updatedAt)}
              </span>
            </div>
          </div>

          {access === "full" && (
            <div className="flex items-center gap-2">
              <select
                value={selectedStatusId}
                onChange={(e) => setSelectedStatusId(Number(e.target.value))}
                className="rounded border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
              >
                {statuses.map((s) => (
                  <option key={s.id} value={s.id}>
                    {s.name}
                  </option>
                ))}
              </select>
              <button
                onClick={handleChangeStatus}
                disabled={pending || selectedStatusId === reservation.statusId}
                className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
              >
                {t["admin.reservations.change_status"] ?? "Change Status"}
              </button>
            </div>
          )}
        </div>
      </div>

      {/* Section 2: Owner Snapshot */}
      <div>
        <SectionHeader title={t["admin.reservations.section.owner"] ?? "Owner Snapshot"} />
        <div className="rounded-lg border border-gray-200 bg-white p-6">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
            <Field label={t["admin.reservations.field.first_name"] ?? "First Name"} value={reservation.ownerFirstName} />
            <Field label={t["admin.reservations.field.last_name"] ?? "Last Name"} value={reservation.ownerLastName} />
            <Field label={t["admin.reservations.field.email"] ?? "Email"} value={reservation.ownerEmail} />
            <Field label={t["admin.reservations.field.phone"] ?? "Phone"} value={reservation.ownerPhone} />
            <Field label={t["admin.reservations.field.tax_id"] ?? "Tax ID"} value={reservation.ownerTaxId} />
            <Field label={t["admin.reservations.field.address"] ?? "Address"} value={reservation.ownerAddress} />
            <Field label={t["admin.reservations.field.city"] ?? "City"} value={reservation.ownerCity} />
            <Field label={t["admin.reservations.field.postal_code"] ?? "Postal Code"} value={reservation.ownerPostalCode} />
            <Field label={t["admin.reservations.field.country"] ?? "Country"} value={reservation.ownerCountry} />
          </div>
        </div>
      </div>

      {/* Section 3: Reservation Lines */}
      <div>
        <SectionHeader title={t["admin.reservations.section.lines"] ?? "Reservation Lines"} />
        <div className="rounded-lg border border-gray-200 bg-white p-6">
          {lines.length === 0 ? (
            <p className="text-sm text-gray-500">{t["admin.reservations.no_lines"] ?? "No lines yet."}</p>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-left text-sm">
                <thead className="border-b border-gray-200 text-xs font-medium uppercase text-gray-500">
                  <tr>
                    <th className="px-3 py-2">{t["admin.reservations.col.hotel"] ?? "Hotel"}</th>
                    <th className="px-3 py-2">{t["admin.reservations.col.room_type"] ?? "Room Type"}</th>
                    <th className="px-3 py-2">{t["admin.reservations.col.board"] ?? "Board"}</th>
                    <th className="px-3 py-2">{t["admin.reservations.col.check_in"] ?? "Check-in"}</th>
                    <th className="px-3 py-2">{t["admin.reservations.col.check_out"] ?? "Check-out"}</th>
                    <th className="px-3 py-2 text-right">{t["admin.reservations.col.nights"] ?? "Nights"}</th>
                    <th className="px-3 py-2 text-right">{t["admin.reservations.col.guests"] ?? "Guests"}</th>
                    <th className="px-3 py-2 text-right">{t["admin.reservations.col.total"] ?? "Total"}</th>
                    {isDraft && canEdit(access) && <th className="px-3 py-2"></th>}
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100">
                  {lines.map((line) => (
                    <tr key={line.reservationLineId}>
                      <td className="px-3 py-2">{line.hotelName}</td>
                      <td className="px-3 py-2">{line.roomTypeName}</td>
                      <td className="px-3 py-2">{line.boardTypeName}</td>
                      <td className="px-3 py-2">{formatDateShort(line.checkInDate)}</td>
                      <td className="px-3 py-2">{formatDateShort(line.checkOutDate)}</td>
                      <td className="px-3 py-2 text-right">{line.numNights}</td>
                      <td className="px-3 py-2 text-right">{line.numGuests}</td>
                      <td className="px-3 py-2 text-right">{line.totalPrice.toFixed(2)} {line.currencyCode}</td>
                      {isDraft && canEdit(access) && (
                        <td className="px-3 py-2 text-right">
                          <button
                            onClick={() => handleRemoveLine(line.reservationLineId)}
                            disabled={pending}
                            className="text-xs text-red-600 hover:underline disabled:opacity-50"
                          >
                            {t["admin.action.remove"] ?? "Remove"}
                          </button>
                        </td>
                      )}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          {isDraft && canEdit(access) && (
            <div className="mt-4">
              <button
                onClick={() => {
                  setLineForm({ ...EMPTY_LINE });
                  setSelectedHotelId(0);
                  setShowAddLine(true);
                }}
                disabled={pending}
                className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
              >
                {t["admin.reservations.add_line"] ?? "Add Line"}
              </button>
            </div>
          )}
        </div>
      </div>

      {/* Section 4: Guests */}
      {lines.length > 0 && (
        <div>
          <SectionHeader title={t["admin.reservations.section.guests"] ?? "Guests"} />
          <div className="rounded-lg border border-gray-200 bg-white p-6 space-y-6">
            {lines.map((line) => {
              const lineGuests = guests.filter((g) => g.reservationLineId === line.reservationLineId);
              return (
                <div key={line.reservationLineId}>
                  <h4 className="mb-2 text-sm font-medium text-gray-700">
                    {line.hotelName} — {line.roomTypeName}
                  </h4>
                  {lineGuests.length === 0 ? (
                    <p className="text-xs text-gray-400">{t["admin.reservations.no_guests"] ?? "No guests registered."}</p>
                  ) : (
                    <table className="w-full text-left text-sm">
                      <thead className="border-b border-gray-200 text-xs font-medium uppercase text-gray-500">
                        <tr>
                          <th className="px-3 py-1">{t["admin.reservations.field.first_name"] ?? "First Name"}</th>
                          <th className="px-3 py-1">{t["admin.reservations.field.last_name"] ?? "Last Name"}</th>
                          <th className="px-3 py-1">{t["admin.reservations.field.email"] ?? "Email"}</th>
                          <th className="px-3 py-1">{t["admin.reservations.field.phone"] ?? "Phone"}</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-gray-100">
                        {lineGuests.map((g) => (
                          <tr key={g.guestId}>
                            <td className="px-3 py-1">{g.firstName}</td>
                            <td className="px-3 py-1">{g.lastName}</td>
                            <td className="px-3 py-1">{g.email ?? "-"}</td>
                            <td className="px-3 py-1">{g.phone ?? "-"}</td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  )}
                  {isDraft && canEdit(access) && (
                    <button
                      onClick={() => openAddGuest(line.reservationLineId)}
                      disabled={pending}
                      className="mt-2 text-xs text-blue-600 hover:underline disabled:opacity-50"
                    >
                      {t["admin.reservations.add_guest"] ?? "Add Guest"}
                    </button>
                  )}
                </div>
              );
            })}
          </div>
        </div>
      )}

      {/* Section 5: Pricing Breakdown */}
      <div>
        <SectionHeader title={t["admin.reservations.section.pricing"] ?? "Pricing Breakdown"} />
        <div className="rounded-lg border border-gray-200 bg-white p-6">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
            <Field label={t["admin.reservations.field.subtotal"] ?? "Subtotal"} value={`${reservation.subtotal.toFixed(2)} ${reservation.currencyCode}`} />
            <Field label={t["admin.reservations.field.tax"] ?? "Tax Amount"} value={`${reservation.taxAmount.toFixed(2)} ${reservation.currencyCode}`} />
            <Field label={t["admin.reservations.field.margin"] ?? "Margin Amount"} value={`${reservation.marginAmount.toFixed(2)} ${reservation.currencyCode}`} />
            <Field label={t["admin.reservations.field.discount"] ?? "Discount Amount"} value={`${reservation.discountAmount.toFixed(2)} ${reservation.currencyCode}`} />
            <Field label={t["admin.reservations.field.total"] ?? "Total Price"} value={`${reservation.totalPrice.toFixed(2)} ${reservation.currencyCode}`} />
            {reservation.promoCode && (
              <Field label={t["admin.reservations.field.promo_code"] ?? "Promo Code"} value={reservation.promoCode} />
            )}
          </div>
        </div>
      </div>

      {/* Section 6: Actions */}
      {canEdit(access) && (
        <div>
          <SectionHeader title={t["admin.reservations.section.actions"] ?? "Actions"} />
          <div className="rounded-lg border border-gray-200 bg-white p-6">
            <div className="flex flex-wrap gap-3">
              {isDraft && (
                <button
                  onClick={() => setShowSubmit(true)}
                  disabled={pending}
                  className="rounded bg-green-600 px-4 py-2 text-sm font-medium text-white hover:bg-green-700 disabled:opacity-50"
                >
                  {t["admin.reservations.submit"] ?? "Submit Reservation"}
                </button>
              )}
              <button
                onClick={() => setShowCancel(true)}
                disabled={pending}
                className="rounded bg-red-600 px-4 py-2 text-sm font-medium text-white hover:bg-red-700 disabled:opacity-50"
              >
                {t["admin.reservations.cancel"] ?? "Cancel Reservation"}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Cancel Modal */}
      <FormModal
        open={showCancel}
        title={t["admin.reservations.cancel_title"] ?? "Cancel Reservation"}
        onClose={() => {
          setShowCancel(false);
          setCancelReason("");
        }}
        onSubmit={handleCancel}
        loading={pending}
        saveLabel={t["admin.reservations.confirm_cancel"] ?? "Cancel Reservation"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <p className="text-sm text-gray-600">
          {t["admin.reservations.cancel_message"] ?? "Are you sure you want to cancel this reservation? Please provide a reason."}
        </p>
        <FormField
          label={t["admin.reservations.field.cancel_reason"] ?? "Reason"}
          type="textarea"
          value={cancelReason}
          onChange={(v) => setCancelReason(String(v))}
          placeholder={t["admin.reservations.cancel_reason_placeholder"] ?? "Reason for cancellation..."}
        />
      </FormModal>

      {/* Submit Modal */}
      <FormModal
        open={showSubmit}
        title={t["admin.reservations.submit_title"] ?? "Submit Reservation"}
        onClose={() => {
          setShowSubmit(false);
          setSubmitForm({ ...EMPTY_SUBMIT });
        }}
        onSubmit={handleSubmit}
        loading={pending}
        saveLabel={t["admin.reservations.submit"] ?? "Submit"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.reservations.field.payment_method"] ?? "Payment Method"}
          type="select"
          value={submitForm.paymentMethodId}
          onChange={(v) => setSubmitForm({ ...submitForm, paymentMethodId: Number(v) })}
          options={enabledPaymentMethods.map((pm) => ({ value: pm.id, label: pm.name }))}
          placeholder={t["admin.reservations.select_payment_method"] ?? "Select payment method..."}
        />
        <FormField
          label={t["admin.reservations.field.card_number"] ?? "Card Number"}
          value={submitForm.cardNumber ?? ""}
          onChange={(v) => setSubmitForm({ ...submitForm, cardNumber: String(v) || null })}
        />
        <FormField
          label={t["admin.reservations.field.card_expiry"] ?? "Card Expiry"}
          value={submitForm.cardExpiry ?? ""}
          onChange={(v) => setSubmitForm({ ...submitForm, cardExpiry: String(v) || null })}
        />
        <FormField
          label={t["admin.reservations.field.card_cvv"] ?? "Card CVV"}
          value={submitForm.cardCvv ?? ""}
          onChange={(v) => setSubmitForm({ ...submitForm, cardCvv: String(v) || null })}
        />
        <FormField
          label={t["admin.reservations.field.card_holder"] ?? "Card Holder Name"}
          value={submitForm.cardHolderName ?? ""}
          onChange={(v) => setSubmitForm({ ...submitForm, cardHolderName: String(v) || null })}
        />
      </FormModal>

      {/* Add Line Modal */}
      <FormModal
        open={showAddLine}
        title={t["admin.reservations.add_line"] ?? "Add Line"}
        onClose={() => {
          setShowAddLine(false);
          setLineForm({ ...EMPTY_LINE });
          setSelectedHotelId(0);
        }}
        onSubmit={handleAddLine}
        loading={pending}
        saveLabel={t["admin.action.create"] ?? "Create"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.reservations.field.hotel"] ?? "Hotel"}
          type="select"
          value={selectedHotelId}
          onChange={(v) => {
            setSelectedHotelId(Number(v));
            setLineForm({ ...lineForm, roomConfigurationId: 0, boardTypeId: 0 });
          }}
          options={enabledHotels.map((h) => ({ value: h.id, label: h.name }))}
          placeholder={t["admin.reservations.select_hotel"] ?? "Select hotel..."}
        />
        <FormField
          label={t["admin.reservations.field.room_config"] ?? "Room Configuration"}
          type="select"
          value={lineForm.roomConfigurationId}
          onChange={(v) => {
            setLineForm({ ...lineForm, roomConfigurationId: Number(v), boardTypeId: 0 });
          }}
          options={filteredRoomConfigs.map((rc) => ({
            value: rc.id,
            label: `${roomTypeMap.get(rc.roomTypeId) ?? `Room #${rc.roomTypeId}`} — Cap: ${rc.capacity}, ${rc.pricePerNight}/night`,
          }))}
          placeholder={t["admin.reservations.select_room"] ?? "Select room..."}
          disabled={!selectedHotelId}
        />
        <FormField
          label={t["admin.reservations.field.board_type"] ?? "Board Type"}
          type="select"
          value={lineForm.boardTypeId}
          onChange={(v) => setLineForm({ ...lineForm, boardTypeId: Number(v) })}
          options={filteredBoardOptions.map((bo) => ({
            value: bo.boardTypeId,
            label: `${boardTypeMap.get(bo.boardTypeId) ?? `Board #${bo.boardTypeId}`} — ${bo.pricePerNight}/night`,
          }))}
          placeholder={t["admin.reservations.select_board"] ?? "Select board type..."}
          disabled={!lineForm.roomConfigurationId}
        />
        <FormField
          label={t["admin.reservations.field.check_in"] ?? "Check-in Date"}
          type="date"
          value={lineForm.checkIn}
          onChange={(v) => setLineForm({ ...lineForm, checkIn: String(v) })}
        />
        <FormField
          label={t["admin.reservations.field.check_out"] ?? "Check-out Date"}
          type="date"
          value={lineForm.checkOut}
          onChange={(v) => setLineForm({ ...lineForm, checkOut: String(v) })}
        />
        <FormField
          label={t["admin.reservations.field.guest_count"] ?? "Guest Count"}
          type="number"
          value={lineForm.guestCount}
          onChange={(v) => setLineForm({ ...lineForm, guestCount: Math.max(1, Number(v)) })}
        />
      </FormModal>

      {/* Add Guest Modal */}
      <FormModal
        open={showAddGuest}
        title={t["admin.reservations.add_guest"] ?? "Add Guest"}
        onClose={() => {
          setShowAddGuest(false);
          setGuestForm({ ...EMPTY_GUEST });
          setGuestLineId(0);
        }}
        onSubmit={handleAddGuest}
        loading={pending}
        saveLabel={t["admin.action.create"] ?? "Create"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.reservations.field.first_name"] ?? "First Name"}
          value={guestForm.firstName}
          onChange={(v) => setGuestForm({ ...guestForm, firstName: String(v) })}
          required
        />
        <FormField
          label={t["admin.reservations.field.last_name"] ?? "Last Name"}
          value={guestForm.lastName}
          onChange={(v) => setGuestForm({ ...guestForm, lastName: String(v) })}
          required
        />
        <FormField
          label={t["admin.reservations.field.email"] ?? "Email"}
          type="email"
          value={guestForm.email}
          onChange={(v) => setGuestForm({ ...guestForm, email: String(v) })}
        />
        <FormField
          label={t["admin.reservations.field.phone"] ?? "Phone"}
          value={guestForm.phone}
          onChange={(v) => setGuestForm({ ...guestForm, phone: String(v) })}
        />
      </FormModal>
    </div>
  );
}

function Field({ label, value }: { label: string; value: string | null }) {
  return (
    <div>
      <dt className="text-xs font-medium text-gray-500">{label}</dt>
      <dd className="mt-1 text-sm text-gray-900">{value || "-"}</dd>
    </div>
  );
}
