ALTER TABLE img.images
    ADD CONSTRAINT [images_whencreated_default]
    DEFAULT GETUTCDATE()
    FOR whencreated
