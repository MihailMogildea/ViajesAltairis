export interface InvoiceDto {
  id: number;
  invoiceNumber: string;
  statusId: number;
  reservationId: number;
  businessPartnerId: number | null;
  subtotal: number;
  taxAmount: number;
  discountAmount: number;
  totalAmount: number;
  periodStart: string;
  periodEnd: string;
  createdAt: string;
  updatedAt: string;
}

export interface InvoiceStatusDto {
  id: number;
  name: string;
  createdAt: string;
}
