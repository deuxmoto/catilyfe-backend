-- Gets image details.

CREATE PROCEDURE img.[getimage]
    @error_message NVARCHAR(2048) OUTPUT
   ,@id                    INT           = NULL
   ,@slug                  NVARCHAR(128) = NULL
   ,@top                   INT           = NULL
   ,@skip                  INT           = NULL
AS
    SET NOCOUNT ON
    -- Run as snapshot
    SET TRANSACTION ISOLATION LEVEL SNAPSHOT

    SET @top =          ISNULL(@top, 100)
    SET @skip =         ISNULL(@skip, 0)

    DECLARE @ids TABLE
    (
        id INT PRIMARY KEY
    )

    INSERT INTO @ids
    SELECT i.id FROM img.images i
    WHERE (i.slug = @slug OR @slug IS NULL)
       AND (i.id = @id OR @id IS NULL)
    ORDER BY i.slug ASC
    OFFSET (@skip) ROWS 
    FETCH NEXT (@top) ROWS ONLY

    SELECT
        l.id
       ,l.image
       ,l.width
       ,l.height
       ,l.fileformat
       ,l.adapter
       ,l.metadata
    FROM img.links l
    JOIN @ids i
      ON l.image = i.id

    SELECT
        img.id
       ,img.slug
       ,img.description
       ,img.whencreated
    FROM img.images img
    JOIN @ids i
      ON img.id = i.id

RETURN 0
