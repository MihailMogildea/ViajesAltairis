import type { User } from "@/types";

export const users: User[] = [
  {
    id: 8,
    email: "client1@example.com",
    first_name: "Juan",
    last_name: "Martínez",
    user_type: "client",
    discount: 0,
    subscription_type: "premium",
    subscription_discount: 7,
  },
  {
    id: 9,
    email: "client2@example.com",
    first_name: "Emma",
    last_name: "Wilson",
    user_type: "client",
    discount: 0,
  },
  {
    id: 10,
    email: "ana@viajessol.com",
    first_name: "Ana",
    last_name: "Fernández",
    user_type: "agent",
    discount: 0,
    business_partner: "Viajes Sol",
    business_partner_discount: 8,
  },
];

export function findUserByEmail(email: string): User | undefined {
  return users.find((u) => u.email === email);
}
