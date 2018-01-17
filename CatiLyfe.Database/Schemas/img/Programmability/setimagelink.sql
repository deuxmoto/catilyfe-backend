-- Set an image link
CREATE PROCEDURE img.setimagelink
    @error_message         NVARCHAR(2048) OUTPUT
   ,@imageid               INT
   ,@linkid                INT = NULL
   ,@filetype              NVARCHAR(16)
   ,@width                 INT
   ,@height                INT
   ,@adapter               NVARCHAR(64)
   ,@metadata              NVARCHAR(MAX)
AS
    SET NOCOUNT ON
    -- Run as snapshot
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED

    DECLARE @error        INT = 0
    DECLARE @itemnotfound INT = 50001

    DECLARE @slug         NVARCHAR(128)

    BEGIN TRANSACTION
    BEGIN TRY

        IF (@linkid IS NOT NULL AND NOT EXISTS (SELECT TOP 1 1 FROM img.links WHERE id = @linkid AND image = @imageid))
        BEGIN
            SET @error = @itemnotfound
            SET @error_message = CONCAT(N'The item with the id ''', @linkid, N''' does not exist.')
            GOTO ErrorHandler
        END

        MERGE INTO img.links lnk
        USING (VALUES (@imageid, @linkid)) AS src(imageid, linkid)
          ON src.imageid = lnk.image AND src.linkid = lnk.id
        WHEN NOT MATCHED THEN
        INSERT
        (
            image
           ,width
           ,height
           ,fileformat
           ,adapter
           ,metadata
        )
        VALUES
        (
            @imageid
           ,@width
           ,@height
           ,@filetype
           ,@adapter
           ,@metadata
        )
        WHEN MATCHED THEN
        UPDATE
          SET width = @width
             ,height = @height
             ,fileformat = @filetype
             ,adapter = @adapter
             ,metadata = @metadata
        ;

    SELECT TOP 1 @slug = slug FROM img.images WHERE id = @imageid

    COMMIT TRANSACTION

    -- Get everything backs
    EXECUTE @error = img.getimage @slug = @slug

    END TRY
    BEGIN CATCH

        SET @error = ERROR_NUMBER()
        SET @error_message = ERROR_MESSAGE()
        GOTO ErrorHandler

    END CATCH

RETURN @error

ErrorHandler:
    ROLLBACK TRANSACTION
RETURN @error
