export interface LanguageDto {
  id: number;
  isoCode: string;
  name: string;
  createdAt: string;
}

export interface CurrencyDto {
  id: number;
  isoCode: string;
  name: string;
  symbol: string;
  createdAt: string;
}

export interface CountryDto {
  id: number;
  isoCode: string;
  name: string;
  currencyId: number;
  enabled: boolean;
  createdAt: string;
}

export interface ExchangeRateDto {
  id: number;
  currencyId: number;
  rateToEur: number;
  validFrom: string;
  validTo: string;
  createdAt: string;
}

export interface AdministrativeDivisionTypeDto {
  id: number;
  name: string;
  createdAt: string;
}

export interface AdministrativeDivisionDto {
  id: number;
  countryId: number;
  parentId: number | null;
  name: string;
  typeId: number;
  level: number;
  enabled: boolean;
  createdAt: string;
}

export interface CityDto {
  id: number;
  administrativeDivisionId: number;
  name: string;
  enabled: boolean;
  createdAt: string;
}

export interface TaxDto {
  id: number;
  taxTypeId: number;
  countryId: number | null;
  administrativeDivisionId: number | null;
  cityId: number | null;
  rate: number;
  isPercentage: boolean;
  enabled: boolean;
  createdAt: string;
}

export interface TaxTypeDto {
  id: number;
  name: string;
  createdAt: string;
}

export interface EmailTemplateDto {
  id: number;
  name: string;
}

export interface TranslationDto {
  id: number;
  entityType: string;
  entityId: number;
  field: string;
  languageId: number;
  value: string;
  createdAt: string;
}

export interface WebTranslationDto {
  id: number;
  translationKey: string;
  languageId: number;
  value: string;
  createdAt: string;
}

export interface NotificationLogDto {
  id: number;
  userId: number;
  emailTemplateId: number;
  recipientEmail: string;
  subject: string;
  body: string;
  createdAt: string;
}

export interface BoardTypeDto {
  id: number;
  name: string;
}

export interface RoomTypeDto {
  id: number;
  name: string;
  createdAt: string;
}

export interface AmenityCategoryDto {
  id: number;
  name: string;
  createdAt: string;
}

export interface AmenityDto {
  id: number;
  categoryId: number;
  name: string;
  createdAt: string;
}
