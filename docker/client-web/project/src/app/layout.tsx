import type { Metadata } from "next";
import "./globals.css";
import Header from "@/components/Header";
import Footer from "@/components/Footer";
import { AuthProvider } from "@/context/AuthContext";
import { BookingProvider } from "@/context/BookingContext";
import { LocaleProvider } from "@/context/LocaleContext";

export const metadata: Metadata = {
  title: "ViajesAltairis - Hotel Bookings",
  description: "Book the best hotels across Spain and France with ViajesAltairis",
  icons: { icon: "/logo.jpg" },
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body className="flex min-h-screen flex-col bg-white text-gray-900 antialiased" suppressHydrationWarning>
        <LocaleProvider>
          <AuthProvider>
            <BookingProvider>
              <Header />
              <main className="flex-1">{children}</main>
              <Footer />
            </BookingProvider>
          </AuthProvider>
        </LocaleProvider>
      </body>
    </html>
  );
}
