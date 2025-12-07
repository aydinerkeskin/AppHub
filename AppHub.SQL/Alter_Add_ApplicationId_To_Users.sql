-- Adds application_id column to users table and enforces the foreign key to applications.
-- Before running the NOT NULL/foreign key step, ensure every existing user row has a valid application_id.

BEGIN;

ALTER TABLE users
    ADD COLUMN IF NOT EXISTS application_id INTEGER;

-- Example data backfill (replace with a valid application id):
-- UPDATE users SET application_id = 1 WHERE application_id IS NULL;

ALTER TABLE users
    ALTER COLUMN application_id SET NOT NULL,
    ADD CONSTRAINT IF NOT EXISTS fk_users_application
        FOREIGN KEY (application_id) REFERENCES applications(id) ON DELETE RESTRICT;

CREATE INDEX IF NOT EXISTS idx_users_application_id ON users(application_id);

COMMIT;
