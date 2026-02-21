"use client";

import { useState } from "react";
import type { RoomConfig, RoomImage, BasketItemTax, BoardOption } from "@/types";
import { formatPrice } from "@/lib/utils";
import { useBooking } from "@/context/BookingContext";
import { useLocale } from "@/context/LocaleContext";
import ImageCarousel from "@/components/ImageCarousel";

interface RoomOffer {
  id: number;
  basePricePerNight: number;
  availableRooms: number;
  boardOptions: BoardOption[];
}

interface RoomTypeGroup {
  roomTypeDbId: number;
  roomTypeName: string;
  capacity: number;
  images: RoomImage[];
  offers: RoomOffer[];
}

interface RoomCardProps {
  room?: RoomConfig;
  group?: RoomTypeGroup;
  hotelId: number;
  hotelName: string;
  taxes: BasketItemTax[];
  checkIn: string;
  checkOut: string;
}

function OfferRow({
  offer,
  offerIndex,
  totalOffers,
  roomTypeName,
  capacity,
  hotelId,
  hotelName,
  taxes,
  checkIn,
  checkOut,
}: {
  offer: RoomOffer;
  offerIndex: number;
  totalOffers: number;
  roomTypeName: string;
  capacity: number;
  hotelId: number;
  hotelName: string;
  taxes: BasketItemTax[];
  checkIn: string;
  checkOut: string;
}) {
  const { addItem } = useBooking();
  const { locale, currency, t } = useLocale();
  const [selectedBoard, setSelectedBoard] = useState(offer.boardOptions[0]?.board_type_code || "room_only");
  const [numRooms, setNumRooms] = useState(1);
  const [added, setAdded] = useState(false);

  if (offer.boardOptions.length === 0) return null;

  const boardOption = offer.boardOptions.find((b) => b.board_type_code === selectedBoard) || offer.boardOptions[0];
  const totalPerNight = offer.basePricePerNight + (boardOption?.price_supplement || 0);

  function handleAdd() {
    addItem({
      hotel_id: hotelId,
      hotel_name: hotelName,
      room_config_id: offer.id,
      room_type_name: roomTypeName,
      board_type_id: boardOption.board_type_id,
      board_type_code: boardOption.board_type_code,
      board_type_name: boardOption.board_type_name,
      check_in: checkIn,
      check_out: checkOut,
      guests: capacity * numRooms,
      num_rooms: numRooms,
      price_per_night: offer.basePricePerNight,
      board_supplement: boardOption.price_supplement,
      taxes,
    });
    setAdded(true);
    setTimeout(() => setAdded(false), 2000);
  }

  return (
    <div className={`flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between ${offerIndex > 0 ? "border-t border-dashed border-gray-200 pt-3" : ""}`}>
      <div className="flex-1">
        {totalOffers > 1 && (
          <p className="mb-1 text-xs font-medium text-blue-600">
            {t("client.room.option")} {offerIndex + 1}
            <span className="ml-2 text-gray-400">{offer.availableRooms} {t("client.room.available")}</span>
          </p>
        )}
        <div className="flex flex-wrap gap-3">
          <div>
            <label className="mb-1 block text-xs font-medium text-gray-500">{t("client.room.meal_plan")}</label>
            <select
              value={selectedBoard}
              onChange={(e) => setSelectedBoard(e.target.value)}
              className="rounded-lg border border-gray-300 px-3 py-1.5 text-sm"
            >
              {offer.boardOptions.map((b) => (
                <option key={b.board_type_code} value={b.board_type_code}>
                  {b.board_type_name} {b.price_supplement > 0 ? `(+${formatPrice(b.price_supplement, currency.code, locale, currency.exchangeRateToEur)})` : ""}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-gray-500">{t("client.room.rooms")}</label>
            <select
              value={numRooms}
              onChange={(e) => setNumRooms(Number(e.target.value))}
              className="rounded-lg border border-gray-300 px-3 py-1.5 text-sm"
            >
              {Array.from({ length: Math.min(offer.availableRooms, 5) }, (_, i) => i + 1).map((n) => (
                <option key={n} value={n}>{n}</option>
              ))}
            </select>
          </div>
        </div>
      </div>
      <div className="text-right sm:ml-6">
        <p className="text-2xl font-bold text-gray-900">{formatPrice(totalPerNight, currency.code, locale, currency.exchangeRateToEur)}</p>
        <p className="text-xs text-gray-400">{t("client.room.per_room_night")}</p>
        <button
          onClick={handleAdd}
          className={`mt-2 rounded-lg px-5 py-2 text-sm font-semibold transition-colors ${
            added ? "bg-green-600 text-white" : "bg-blue-600 text-white hover:bg-blue-700"
          }`}
        >
          {added ? t("client.room.added") : t("client.room.add_to_basket")}
        </button>
      </div>
    </div>
  );
}

function LegacyRoomCard({ room, hotelId, hotelName, taxes, checkIn, checkOut }: { room: RoomConfig; hotelId: number; hotelName: string; taxes: BasketItemTax[]; checkIn: string; checkOut: string }) {
  const { addItem } = useBooking();
  const { locale, currency, t } = useLocale();
  const [selectedBoard, setSelectedBoard] = useState(room.board_options[0]?.board_type_code || "room_only");
  const [numRooms, setNumRooms] = useState(1);
  const [added, setAdded] = useState(false);

  if (room.board_options.length === 0) return null;

  const boardOption = room.board_options.find((b) => b.board_type_code === selectedBoard) || room.board_options[0];
  const totalPerNight = room.base_price_per_night + (boardOption?.price_supplement || 0);

  function handleAdd() {
    addItem({
      hotel_id: hotelId,
      hotel_name: hotelName,
      room_config_id: room.id,
      room_type_name: room.room_type_name,
      board_type_id: boardOption.board_type_id,
      board_type_code: boardOption.board_type_code,
      board_type_name: boardOption.board_type_name,
      check_in: checkIn,
      check_out: checkOut,
      guests: room.capacity * numRooms,
      num_rooms: numRooms,
      price_per_night: room.base_price_per_night,
      board_supplement: boardOption.price_supplement,
      taxes,
    });
    setAdded(true);
    setTimeout(() => setAdded(false), 2000);
  }

  return (
    <div className="rounded-xl border border-gray-200 p-5">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
        <div className="flex-1">
          <h3 className="text-lg font-semibold text-gray-900">{room.room_type_name}</h3>
          <div className="mt-1 flex flex-wrap gap-3 text-sm text-gray-500">
            <span>{t("client.room.capacity")} {room.capacity} {room.capacity === 1 ? t("client.room.guest") : t("client.room.guests")}</span>
            <span>&middot;</span>
            <span>{room.quantity} {t("client.room.available")}</span>
          </div>
          <div className="mt-3">
            <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.room.meal_plan")}</label>
            <select value={selectedBoard} onChange={(e) => setSelectedBoard(e.target.value)} className="rounded-lg border border-gray-300 px-3 py-1.5 text-sm">
              {room.board_options.map((b) => (
                <option key={b.board_type_code} value={b.board_type_code}>
                  {b.board_type_name} {b.price_supplement > 0 ? `(+${formatPrice(b.price_supplement, currency.code, locale, currency.exchangeRateToEur)})` : ""}
                </option>
              ))}
            </select>
          </div>
          <div className="mt-3">
            <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.room.rooms")}</label>
            <select value={numRooms} onChange={(e) => setNumRooms(Number(e.target.value))} className="rounded-lg border border-gray-300 px-3 py-2 text-sm">
              {Array.from({ length: Math.min(room.quantity, 5) }, (_, i) => i + 1).map((n) => (
                <option key={n} value={n}>{n}</option>
              ))}
            </select>
          </div>
        </div>
        <div className="text-right sm:ml-6">
          <p className="text-2xl font-bold text-gray-900">{formatPrice(totalPerNight, currency.code, locale, currency.exchangeRateToEur)}</p>
          <p className="text-xs text-gray-400">{t("client.room.per_room_night")}</p>
          <button
            onClick={handleAdd}
            className={`mt-3 rounded-lg px-5 py-2 text-sm font-semibold transition-colors ${added ? "bg-green-600 text-white" : "bg-blue-600 text-white hover:bg-blue-700"}`}
          >
            {added ? t("client.room.added") : t("client.room.add_to_basket")}
          </button>
        </div>
      </div>
    </div>
  );
}

export default function RoomCard({ room, group, hotelId, hotelName, taxes, checkIn, checkOut }: RoomCardProps) {
  const { t } = useLocale();

  if (group) {
    if (group.offers.length === 0) return null;
    return (
      <div className="rounded-xl border border-gray-200 overflow-hidden">
        {group.images.length > 0 && (
          <ImageCarousel images={group.images} aspectRatio="aspect-[21/9]" />
        )}
        <div className="p-5">
        <h3 className="text-lg font-semibold text-gray-900">{group.roomTypeName}</h3>
        <div className="mt-1 flex flex-wrap gap-3 text-sm text-gray-500">
          <span>{t("client.room.capacity")} {group.capacity} {group.capacity === 1 ? t("client.room.guest") : t("client.room.guests")}</span>
          {group.offers.length === 1 && (
            <>
              <span>&middot;</span>
              <span>{group.offers[0].availableRooms} {t("client.room.available")}</span>
            </>
          )}
        </div>
        <div className="mt-3 space-y-3">
          {group.offers.map((offer, i) => (
            <OfferRow
              key={offer.id}
              offer={offer}
              offerIndex={i}
              totalOffers={group.offers.length}
              roomTypeName={group.roomTypeName}
              capacity={group.capacity}
              hotelId={hotelId}
              hotelName={hotelName}
              taxes={taxes}
              checkIn={checkIn}
              checkOut={checkOut}
            />
          ))}
        </div>
        </div>
      </div>
    );
  }

  if (room) {
    return <LegacyRoomCard room={room} hotelId={hotelId} hotelName={hotelName} taxes={taxes} checkIn={checkIn} checkOut={checkOut} />;
  }

  return null;
}
