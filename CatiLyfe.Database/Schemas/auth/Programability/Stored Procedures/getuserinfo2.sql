CREATE PROCEDURE auth.getuserinfo2
    @error_message  NVARCHAR(2048) OUTPUT
   ,@fuzzy          BIT           = 0
   ,@token          VARBINARY(64) = NULL
   ,@ids            auth.idlist     READONLY
   ,@emails         auth.stringlist READONLY
   ,@names          auth.stringlist READONLY
AS
    SET NOCOUNT ON
    SET TRANSACTION ISOLATION LEVEL SNAPSHOT

    DECLARE @error          INT = 0

    DECLARE @selectedids auth.idlist

    INSERT INTO @selectedids
    (
        id
    )
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
