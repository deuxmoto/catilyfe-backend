CREATE PROCEDURE auth.getuserinfo2
    @error_message  NVARCHAR(2048) OUTPUT
   ,@fuzzy          BIT           = 0
   ,@token          VARBINARY(64) = NULL
   ,@top            INT           = 100
   ,@skip           INT           = 0
   ,@ids            auth.idlist     READONLY
   ,@emails         auth.stringlist READONLY
   ,@names          auth.stringlist READONLY
AS
    SET NOCOUNT ON
    SET TRANSACTION ISOLATION LEVEL SNAPSHOT

    DECLARE @error          INT = 0

    DECLARE @selectedids auth.idlist

    IF (@token IS NULL AND (NOT EXISTS (SELECT TOP 1 1 FROM @ids)) 
            AND (NOT EXISTS (SELECT TOP 1 1 FROM @ids))
            AND (NOT EXISTS (SELECT TOP 1 1 FROM @emails))
            AND (NOT EXISTS (SELECT TOP 1 1 FROM @names)))
    BEGIN
        INSERT INTO @selectedids
        (
            id
        )
        SELECT id 
        FROM auth.users
        ORDER BY id
        OFFSET (@skip) ROWS
        FETCH NEXT (@top) ROWS ONLY
    END
    ELSE
    BEGIN
        WITH filteredIds AS
        (
            SELECT u.id
            FROM auth.users u
            JOIN @ids i
              ON i.id = u.id
            UNION
            SELECT u.id
            FROM auth.users u
            JOIN @emails e
              ON (@fuzzy = 0 AND e.string = u.email) OR (@fuzzy = 1 AND CONCAT('%', e.string, '%') LIKE u.email)
            UNION
            SELECT u.id
            FROM auth.users u
            JOIN @names n
              ON (@fuzzy = 0 AND n.string = u.name) OR (@fuzzy = 1 AND CONCAT('%', n.string, '%') LIKE u.name)
            UNION
            SELECT t.userid AS id
            FROM auth.tokens t
            WHERE t.token = @token
        )
        INSERT INTO @selectedids
        (
            id
        )
        SELECT id 
        FROM filteredIds
        ORDER BY id
        OFFSET (@skip) ROWS
        FETCH NEXT (@top) ROWS ONLY
    END

    -- OUTPUT all of the roles
    SELECT
        ur.userid
       ,r.role
    FROM auth.userroles ur
    JOIN auth.roles r
      ON r.id = ur.roleid
    JOIN @selectedids s
      ON s.id = ur.userid

    -- Output user details
    SELECT
        u.id
       ,u.name
       ,u.email
       ,u.salt
       ,u.pass
    FROM auth.users u
    JOIN @selectedids si
      ON si.id = u.id

ErrorHandler:
    RETURN @error
