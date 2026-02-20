export interface BusinessPartnerDto {
  id: number;
  companyName: string;
  taxId: string | null;
  discount: number;
  address: string;
  city: string;
  postalCode: string | null;
  country: string;
  contactEmail: string;
  contactPhone: string | null;
  enabled: boolean;
  createdAt: string;
}

export interface CreateBusinessPartnerRequest {
  companyName: string;
  taxId: string | null;
  discount: number;
  address: string;
  city: string;
  postalCode: string | null;
  country: string;
  contactEmail: string;
  contactPhone: string | null;
}
