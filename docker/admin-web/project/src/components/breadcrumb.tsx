import Link from "next/link";

interface BreadcrumbItem {
  label: string;
  href?: string;
}

export function Breadcrumb({ items }: { items: BreadcrumbItem[] }) {
  return (
    <nav className="mb-4 text-sm text-gray-500">
      {items.map((item, i) => (
        <span key={i}>
          {i > 0 && <span className="mx-1">/</span>}
          {item.href ? (
            <Link href={item.href} className="hover:text-gray-700">
              {item.label}
            </Link>
          ) : (
            <span className="text-gray-900">{item.label}</span>
          )}
        </span>
      ))}
    </nav>
  );
}
