export interface ProviderDto {
  id: number;
  typeId: number;
  currencyId: number;
  name: string;
  apiUrl: string | null;
  apiUsername: string | null;
  margin: number;
  enabled: boolean;
  syncStatus: string | null;
  lastSyncedAt: string | null;
  createdAt: string;
}

export interface CreateProviderRequest {
  typeId: number;
  currencyId: number;
  name: string;
  apiUrl: string | null;
  apiUsername: string | null;
  apiPassword: string | null;
  margin: number;
}

export interface ProviderTypeDto {
  id: number;
  name: string;
  createdAt: string;
}
