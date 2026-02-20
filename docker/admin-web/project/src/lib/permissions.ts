/**
 * Role-based access control matrix for admin-web.
 *
 * Must stay in sync with admin-api authorization middleware.
 * user_type values match DB: admin=1, manager=2, agent=3, hotel_staff=4.
 * client (5) has NO access to admin-web.
 */

export type UserRole = "admin" | "manager" | "agent" | "hotel_staff";

export const USER_TYPE_ID: Record<number, UserRole> = {
  1: "admin",
  2: "manager",
  3: "agent",
  4: "hotel_staff",
};

export type Section =
  | "dashboard"
  | "hotels"
  | "providers"
  | "reservations"
  | "users"
  | "business-partners"
  | "pricing"
  | "subscriptions"
  | "financial"
  | "operations"
  | "reviews"
  | "system"
  | "audit"
  | "statistics";

export type AccessLevel = "full" | "read" | "own";

/**
 * Access matrix: which roles can access which sections, and at what level.
 *
 * - full:  CRUD on all records
 * - read:  read-only access
 * - own:   scoped to user's hotels (hotel_staff), partner (agent), or similar
 * - (absent): no access
 */
const ACCESS_MATRIX: Record<Section, Partial<Record<UserRole, AccessLevel>>> = {
  dashboard:            { admin: "full", manager: "full",  agent: "own",  hotel_staff: "own"  },
  hotels:               { admin: "full", manager: "read",  agent: "read", hotel_staff: "own"  },
  providers:            { admin: "full" },
  reservations:         { admin: "full", manager: "full",  agent: "own",  hotel_staff: "own"  },
  users:                { admin: "full", manager: "read" },
  "business-partners":  { admin: "full", manager: "read",  agent: "own"  },
  pricing:              { admin: "full", manager: "read" },
  subscriptions:        { admin: "full", manager: "read",  agent: "read" },
  financial:            { admin: "full", manager: "read",  agent: "own",  hotel_staff: "own"  },
  operations:           { admin: "full", manager: "full",  hotel_staff: "own"  },
  reviews:              { admin: "full", manager: "full",  hotel_staff: "own"  },
  system:               { admin: "full" },
  audit:                { admin: "full" },
  statistics:           { admin: "read", manager: "read" },
};

export function getAccessLevel(
  role: UserRole,
  section: Section
): AccessLevel | null {
  return ACCESS_MATRIX[section][role] ?? null;
}

export function canAccess(role: UserRole, section: Section): boolean {
  return getAccessLevel(role, section) !== null;
}

export function getAccessibleSections(role: UserRole): Section[] {
  return (Object.keys(ACCESS_MATRIX) as Section[]).filter((s) =>
    canAccess(role, s)
  );
}

/** Map URL path prefix to section. */
export function sectionFromPath(pathname: string): Section | null {
  // strip leading slash, take first segment
  const segment = pathname.replace(/^\//, "").split("/")[0];
  if (!segment || segment === "") return "dashboard";
  const sections = Object.keys(ACCESS_MATRIX) as Section[];
  return sections.find((s) => s === segment) ?? null;
}

/** Sidebar navigation items with labels and icons. */
export interface NavItem {
  section: Section;
  label: string;
  href: string;
}

export const NAV_ITEMS: NavItem[] = [
  { section: "dashboard",          label: "Dashboard",          href: "/"                    },
  { section: "hotels",             label: "Hotels",             href: "/hotels"              },
  { section: "providers",          label: "Providers",          href: "/providers"           },
  { section: "reservations",       label: "Reservations",       href: "/reservations"        },
  { section: "users",              label: "Users",              href: "/users"               },
  { section: "business-partners",  label: "Business Partners",  href: "/business-partners"   },
  { section: "pricing",            label: "Pricing & Margins",  href: "/pricing"             },
  { section: "subscriptions",      label: "Subscriptions",      href: "/subscriptions"       },
  { section: "financial",          label: "Financial",          href: "/financial"           },
  { section: "operations",         label: "Operations",         href: "/operations"          },
  { section: "reviews",            label: "Reviews",            href: "/reviews"             },
  { section: "system",             label: "System Config",      href: "/system"              },
  { section: "audit",              label: "Audit Log",          href: "/audit"               },
  { section: "statistics",         label: "Statistics",         href: "/statistics"          },
];
