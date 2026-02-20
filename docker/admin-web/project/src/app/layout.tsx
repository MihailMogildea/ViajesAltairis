import type { Metadata } from "next";
import { getLocale } from "@/lib/locale-server";
import "./globals.css";

export const metadata: Metadata = {
  title: "ViajesAltairis Admin",
  description: "Admin dashboard for ViajesAltairis",
};

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const locale = await getLocale();

  return (
    <html lang={locale}>
      <body className="bg-gray-50 text-gray-900 antialiased">{children}</body>
    </html>
  );
}
