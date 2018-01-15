-- Remove after deployment

ALTER TABLE cati.postmeta
    ADD CONSTRAINT publisheduser_default_removeme
    DEFAULT 5
    FOR publisheduser