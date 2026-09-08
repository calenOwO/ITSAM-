-- Direct SQL to add Remarks column to BorrowingRequests table
ALTER TABLE "BorrowingRequests"
ADD COLUMN IF NOT EXISTS "Remarks" character varying(500);

-- Update the migration history
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260911000000_AddRemarksToRequests', '8.0.28')
ON CONFLICT DO NOTHING;
