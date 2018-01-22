CREATE TABLE img.links
(
    image       INT           NOT NULL
   ,id          INT           NOT NULL IDENTITY(1,1)
   ,width       INT           NOT NULL
   ,height      INT           NOT NULL
   ,fileformat   NVARCHAR(16)  NOT NULL
   ,adapter     NVARCHAR(64)  NOT NULL
   ,metadata    NVARCHAR(MAX) NOT NULL
)
