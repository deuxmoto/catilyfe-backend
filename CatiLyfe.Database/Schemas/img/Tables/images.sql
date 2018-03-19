CREATE TABLE img.images
(
    id          INT           NOT NULL IDENTITY(1,1)
   ,slug        NVARCHAR(128) NOT NULL
   ,description NVARCHAR(256) NOT NULL
   ,whencreated DATETIME2     NOT NULL
)
