export default function LoginLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  // Login has no sidebar — just render children directly
  return <>{children}</>;
}
