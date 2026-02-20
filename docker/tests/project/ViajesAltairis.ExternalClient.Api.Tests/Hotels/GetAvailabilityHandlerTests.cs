// GetAvailability handler uses MariaDB-specific SQL:
//   HAVING AvailableRooms > 0  (without GROUP BY)
// SQLite does not support HAVING on non-aggregate queries.
// These tests require integration testing against MariaDB.
