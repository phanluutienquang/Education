-- Seed data cho Client Authentication
-- Chạy file này sau khi đã tạo tables (migration)

-- ========================================
-- Client 1: Mobile App
-- ========================================
INSERT INTO "ClientSources" (
  "ClientId",
  "ClientName",
  "ApiSecret",
  "IsEnabled",
  "ValidFrom",
  "ValidTo",
  "AllowedScopes",
  "AllowedIPs",
  "RateLimitPerMinute",
  "CreatedAt",
  "UpdatedAt"
) VALUES (
  'mobile-app',                              -- ClientId
  'Mobile Application (Flutter)',            -- ClientName
  'mobile_secret_2024_xyz',                 -- ApiSecret (nên hash trong production)
  true,                                      -- IsEnabled
  NOW(),                                     -- ValidFrom
  NULL,                                      -- ValidTo (vô thời hạn)
  'courses.read,courses.write,exams.submit', -- AllowedScopes
  '',                                        -- AllowedIPs (empty = tất cả IPs)
  100,                                       -- RateLimitPerMinute
  NOW(),                                     -- CreatedAt
  NOW()                                      -- UpdatedAt
);

-- ========================================
-- Client 2: Web Portal
-- ========================================
INSERT INTO "ClientSources" (
  "ClientId",
  "ClientName",
  "ApiSecret",
  "IsEnabled",
  "ValidFrom",
  "ValidTo",
  "AllowedScopes",
  "AllowedIPs",
  "RateLimitPerMinute",
  "CreatedAt",
  "UpdatedAt"
) VALUES (
  'web-portal',                              -- ClientId
  'Web Portal (React/Angular)',              -- ClientName
  'web_secret_2024_abc',                     -- ApiSecret
  true,                                      -- IsEnabled
  NOW(),                                     -- ValidFrom
  NULL,                                      -- ValidTo
  'courses.read,courses.write,courses.delete,users.manage,exams.create,exams.grade',
  '',                                        -- AllowedIPs
  500,                                       -- RateLimitPerMinute
  NOW(),
  NOW()
);

-- ========================================
-- Client 3: Partner API (Garena - Liên Quân)
-- ========================================
INSERT INTO "ClientSources" (
  "ClientId",
  "ClientName",
  "ApiSecret",
  "IsEnabled",
  "ValidFrom",
  "ValidTo",
  "AllowedScopes",
  "AllowedIPs",
  "RateLimitPerMinute",
  "Metadata",
  "CreatedAt",
  "UpdatedAt"
) VALUES (
  'lien-quan-mobile',                        -- ClientId
  'Liên Quân Mobile - Garena',               -- ClientName
  'garena_enterprise_secret_789',            -- ApiSecret
  true,                                      -- IsEnabled
  NOW(),                                     -- ValidFrom
  '2027-12-31',                              -- ValidTo (có hạn)
  'courses.read,exams.submit,certifications.verify',
  '',                                        -- AllowedIPs
  5000,                                      -- RateLimitPerMinute (cao)
  '{"company":"Garena","tier":"enterprise","contact":"partner@garena.com"}',
  NOW(),
  NOW()
);

-- ========================================
-- Client 4: Test Client (cho developers)
-- ========================================
INSERT INTO "ClientSources" (
  "ClientId",
  "ClientName",
  "ApiSecret",
  "IsEnabled",
  "ValidFrom",
  "ValidTo",
  "AllowedScopes",
  "AllowedIPs",
  "RateLimitPerMinute",
  "CreatedAt",
  "UpdatedAt"
) VALUES (
  'test-client',                             -- ClientId
  'Test Client (Development)',               -- ClientName
  'test_secret_123',                         -- ApiSecret
  true,                                      -- IsEnabled
  NOW(),                                     -- ValidFrom
  '2026-12-31',                              -- ValidTo (đến cuối năm nay)
  'courses.read',                            -- Chỉ được read thôi
  '127.0.0.1,::1',                           -- Chỉ localhost
  10,                                        -- Rate limit thấp
  NOW(),
  NOW()
);

-- ========================================
-- Verify data đã insert
-- ========================================
SELECT 
  "ClientId",
  "ClientName",
  "IsEnabled",
  "AllowedScopes",
  "RateLimitPerMinute",
  "ValidTo"
FROM "ClientSources"
ORDER BY "CreatedAt" DESC;
