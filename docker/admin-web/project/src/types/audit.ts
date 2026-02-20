export interface AuditLogDto {
  id: number;
  userId: number | null;
  entityType: string;
  entityId: number;
  action: string;
  oldValues: string | null;
  newValues: string | null;
  createdAt: string;
}
