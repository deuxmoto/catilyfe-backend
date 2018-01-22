-- Gets image details.

CREATE PROCEDURE img.setimagedetails
    @error_message         NVARCHAR(2048) OUTPUT
   ,@id                    INT = NULL
   ,@slug                  NVARCHAR(128)
   ,@description           NVARCHAR(256)
AS
    SET NOCOUNT ON
    -- Run as snapshot
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED

    DECLARE @error INT = 0
    DECLARE @itemnotfound INT = 50001

    BEGIN TRANSACTION
    BEGIN TRY

        IF (@id IS NOT NULL AND NOT EXISTS (SELECT TOP 1 1 FROM img.images WHERE id = @id))
        BEGIN
            SET @error = @itemnotfound
            SET @error_message = CONCAT(N'The item with the id ''', @id, N''' does not exist.')
            GOTO ErrorHandler
        END

        MERGE INTO img.images i
        USING (VALUES (@slug)) AS src(slug)
          ON src.slug = i.slug
        WHEN NOT MATCHED THEN
        INSERT
        (
            slug
           ,description
        )
        VALUES
        (
            @slug
           ,@description
        )
        WHEN MATCHED THEN
        UPDATE
          SET slug = @slug
             ,description = @description
        ;

    COMMIT TRANSACTION

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
