-- Trending Spots Check-In System Database Schema
-- Run this in your Supabase SQL Editor

-- 1. Create TrendingSpots Table
CREATE TABLE IF NOT EXISTS "TrendingSpots" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Type" VARCHAR(50),
    "ImageUrl" TEXT,
    "Campus" VARCHAR(100),
    "CreatedAt" TIMESTAMP DEFAULT NOW()
);

-- 2. Create SpotCheckIns Table
CREATE TABLE IF NOT EXISTS "SpotCheckIns" (
    "Id" SERIAL PRIMARY KEY,
    "StudentId" INTEGER NOT NULL REFERENCES "Students"("Id") ON DELETE CASCADE,
    "SpotId" INTEGER NOT NULL REFERENCES "TrendingSpots"("Id") ON DELETE CASCADE,
    "CheckedInAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "ExpiresAt" TIMESTAMP NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT true,
    UNIQUE("StudentId", "SpotId", "IsActive")
);

-- 3. Create Indexes for Performance
CREATE INDEX IF NOT EXISTS idx_spot_checkins_student ON "SpotCheckIns"("StudentId");
CREATE INDEX IF NOT EXISTS idx_spot_checkins_spot ON "SpotCheckIns"("SpotId");
CREATE INDEX IF NOT EXISTS idx_spot_checkins_active ON "SpotCheckIns"("IsActive", "ExpiresAt");

-- 4. Enable RLS
ALTER TABLE "TrendingSpots" ENABLE ROW LEVEL SECURITY;
ALTER TABLE "SpotCheckIns" ENABLE ROW LEVEL SECURITY;

-- 5. RLS Policies for TrendingSpots (everyone can read)
CREATE POLICY "Anyone can view trending spots"
ON "TrendingSpots"
FOR SELECT
TO public
USING (true);

-- 6. RLS Policies for SpotCheckIns
-- Allow authenticated users to view active check-ins
CREATE POLICY "Authenticated users can view active check-ins"
ON "SpotCheckIns"
FOR SELECT
TO authenticated
USING ("IsActive" = true AND "ExpiresAt" > NOW());

-- Allow users to check in (create their own check-in)
CREATE POLICY "Users can check in to spots"
ON "SpotCheckIns"
FOR INSERT
TO authenticated
WITH CHECK (auth.uid()::text = (SELECT "Email" FROM "Students" WHERE "Id" = "StudentId")::text);

-- Allow users to update their own check-ins (for check-out)
CREATE POLICY "Users can update their own check-ins"
ON "SpotCheckIns"
FOR UPDATE
TO authenticated
USING (auth.uid()::text = (SELECT "Email" FROM "Students" WHERE "Id" = "StudentId")::text);

-- 7. Seed Initial Trending Spots
INSERT INTO "TrendingSpots" ("Name", "Type", "ImageUrl", "Campus") VALUES
('The Daily Grind', 'Coffee Shop', 'https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=400', 'Main Campus'),
('Main Library', 'Quiet Study', 'https://images.unsplash.com/photo-1521587760476-6c12a4b040da?w=400', 'Main Campus'),
('Student Union', 'Social Hub', 'https://images.unsplash.com/photo-1523050854058-8df90110c9f1?w=400', 'Main Campus'),
('Engineering Lab', 'Study Space', 'https://images.unsplash.com/photo-1581092160562-40aa08e78837?w=400', 'Engineering Building'),
('Campus Gym', 'Fitness', 'https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=400', 'Recreation Center'),
('Food Court', 'Dining', 'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=400', 'Student Center')
ON CONFLICT DO NOTHING;

-- 8. Function to Auto-Expire Old Check-Ins (optional cleanup)
CREATE OR REPLACE FUNCTION cleanup_expired_checkins()
RETURNS void AS $$
BEGIN
    UPDATE "SpotCheckIns"
    SET "IsActive" = false
    WHERE "IsActive" = true AND "ExpiresAt" < NOW();
END;
$$ LANGUAGE plpgsql;

-- Success Message
DO $$
BEGIN
    RAISE NOTICE 'Trending Spots tables created successfully!';
    RAISE NOTICE 'Run this query to verify: SELECT * FROM "TrendingSpots";';
END $$;
