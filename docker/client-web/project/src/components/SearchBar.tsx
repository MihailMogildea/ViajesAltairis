"use client";

import { useState, useRef, useEffect } from "react";
import { getAllDestinations } from "@/data/areas";
import { getDefaultCheckIn, getDefaultCheckOut } from "@/lib/utils";
import { useLocale } from "@/context/LocaleContext";

interface SearchBarProps {
  initialDestination?: string;
  initialCheckIn?: string;
  initialCheckOut?: string;
  initialGuests?: number;
  onSearch?: (destination: string, checkIn: string, checkOut: string, guests: number) => void;
  compact?: boolean;
}

export default function SearchBar({
  initialDestination = "",
  initialCheckIn = "",
  initialCheckOut = "",
  initialGuests = 2,
  onSearch,
  compact = false,
}: SearchBarProps) {
  const { t } = useLocale();
  const [destination, setDestination] = useState(initialDestination);
  const [checkIn, setCheckIn] = useState(initialCheckIn || getDefaultCheckIn());
  const [checkOut, setCheckOut] = useState(initialCheckOut || getDefaultCheckOut());
  const [guests, setGuests] = useState(initialGuests);
  const [suggestions, setSuggestions] = useState<string[]>([]);
  const [showSuggestions, setShowSuggestions] = useState(false);
  const wrapperRef = useRef<HTMLDivElement>(null);

  const allDestinations = getAllDestinations();

  useEffect(() => {
    function handleClick(e: MouseEvent) {
      if (wrapperRef.current && !wrapperRef.current.contains(e.target as Node)) {
        setShowSuggestions(false);
      }
    }
    document.addEventListener("mousedown", handleClick);
    return () => document.removeEventListener("mousedown", handleClick);
  }, []);

  function handleDestinationChange(value: string) {
    setDestination(value);
    if (value.length > 0) {
      const filtered = allDestinations.filter((d) => d.toLowerCase().includes(value.toLowerCase()));
      setSuggestions(filtered.slice(0, 6));
      setShowSuggestions(true);
    } else {
      setShowSuggestions(false);
    }
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (onSearch) {
      onSearch(destination, checkIn, checkOut, guests);
    } else {
      const params = new URLSearchParams();
      if (destination) params.set("destination", destination);
      if (checkIn) params.set("checkIn", checkIn);
      if (checkOut) params.set("checkOut", checkOut);
      params.set("guests", String(guests));
      window.location.href = `/hotels?${params.toString()}`;
    }
  }

  const containerClass = compact
    ? "flex flex-col gap-2 sm:flex-row sm:items-end"
    : "flex flex-col gap-3 lg:flex-row lg:items-end";

  return (
    <form onSubmit={handleSubmit} className={containerClass}>
      <div className="relative flex-1" ref={wrapperRef}>
        <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.search.destination")}</label>
        <input
          type="text"
          value={destination}
          onChange={(e) => handleDestinationChange(e.target.value)}
          onFocus={() => destination.length > 0 && setShowSuggestions(true)}
          placeholder={t("client.search.placeholder")}
          className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
        />
        {showSuggestions && suggestions.length > 0 && (
          <ul className="absolute z-20 mt-1 w-full rounded-lg border border-gray-200 bg-white shadow-lg">
            {suggestions.map((s) => (
              <li key={s}>
                <button
                  type="button"
                  className="w-full px-4 py-2 text-left text-sm hover:bg-blue-50"
                  onClick={() => { setDestination(s); setShowSuggestions(false); }}
                >
                  {s}
                </button>
              </li>
            ))}
          </ul>
        )}
      </div>
      <div>
        <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.search.check_in")}</label>
        <input
          type="date"
          value={checkIn}
          onChange={(e) => setCheckIn(e.target.value)}
          className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
        />
      </div>
      <div>
        <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.search.check_out")}</label>
        <input
          type="date"
          value={checkOut}
          onChange={(e) => setCheckOut(e.target.value)}
          className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
        />
      </div>
      <div>
        <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.search.guests")}</label>
        <select
          value={guests}
          onChange={(e) => setGuests(Number(e.target.value))}
          className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
        >
          {[1, 2, 3, 4].map((n) => (
            <option key={n} value={n}>{n} {n === 1 ? t("client.search.guest") : t("client.search.guests_plural")}</option>
          ))}
        </select>
      </div>
      <button
        type="submit"
        className="rounded-lg bg-blue-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 transition-colors whitespace-nowrap"
      >
        {t("client.search.search_hotels")}
      </button>
    </form>
  );
}
