export interface AuditLogDto {
  id: number;
  userId: number | null;
  userEmail: string | null;
  entityType: string;
  entityId: number;
  action: string;
  oldValues: string | null;
  newValues: string | null;
  createdAt: string;
}
