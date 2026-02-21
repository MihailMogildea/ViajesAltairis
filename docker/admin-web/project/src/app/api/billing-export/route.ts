import { NextRequest, NextResponse } from "next/server";
import { getSession } from "@/lib/auth";
import { getLocale } from "@/lib/locale-server";

const API_BASE_URL =
  process.env.NEXT_PUBLIC_ADMIN_API_URL ?? "http://admin-api:8080";

export async function GET(request: NextRequest) {
  const session = await getSession();
  if (!session) {
    return NextResponse.json({ error: "Unauthorized" }, { status: 401 });
  }

  const { searchParams } = request.nextUrl;
  const from = searchParams.get("from");
  const to = searchParams.get("to");
  const channel = searchParams.get("channel");

  if (!from || !to || !channel) {
    return NextResponse.json({ error: "Missing parameters" }, { status: 400 });
  }

  const locale = await getLocale();

  const res = await fetch(
    `${API_BASE_URL}/api/Invoices/billing-export?from=${from}&to=${to}&channel=${channel}`,
    {
      headers: {
        Authorization: `Bearer ${session.token}`,
        "Accept-Language": locale,
      },
    }
  );

  if (!res.ok) {
    return NextResponse.json(
      { error: `API error: ${res.status}` },
      { status: res.status }
    );
  }

  const blob = await res.arrayBuffer();
  const fileName =
    res.headers.get("Content-Disposition")?.match(/filename="?([^"]+)"?/)?.[1] ??
    "billing.zip";

  return new NextResponse(blob, {
    headers: {
      "Content-Type": "application/zip",
      "Content-Disposition": `attachment; filename="${fileName}"`,
    },
  });
}
