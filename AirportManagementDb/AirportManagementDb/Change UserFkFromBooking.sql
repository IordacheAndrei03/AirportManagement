-- 1) Drop FK vechi Booking -> [User]
ALTER TABLE dbo.Booking
DROP CONSTRAINT FK_Booking_User;

-- 2) Creeaz? coloan? nou? compatibil? cu AspNetUsers.Id (default Identity)
ALTER TABLE dbo.Booking
ADD UserId_New nvarchar(450) NULL;

-- 3) ?terge coloana veche UserId (cea care referea dbo.[User])
ALTER TABLE dbo.Booking
DROP COLUMN UserId;

-- 4) Rename coloan? nou? la UserId (p?strezi numele)
EXEC sp_rename 'dbo.Booking.UserId_New', 'UserId', 'COLUMN';

-- 5) Creeaz? FK nou Booking(UserId) -> AspNetUsers(Id)
ALTER TABLE dbo.Booking
ADD CONSTRAINT FK_Booking_AspNetUsers_UserId
FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id);

-- 6) ?terge tabela veche [User]
DROP TABLE dbo.[User];

DELETE FROM dbo.Ticket
WHERE BookingId IN (SELECT Id FROM dbo.Booking);

DELETE FROM dbo.Booking WHERE UserId IS NULL;

ALTER TABLE dbo.Booking
ALTER COLUMN UserId nvarchar(450) NOT NULL;


SELECT OBJECT_ID('dbo.[User]') AS UserTableObjectId;

SELECT 
  c.name,
  t.name AS DataType,
  c.max_length,
  c.is_nullable
FROM sys.columns c
JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Booking')
  AND c.name = 'UserId';

  SELECT 
  fk.name AS FKName,
  OBJECT_NAME(fk.parent_object_id) AS ParentTable,
  OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable
FROM sys.foreign_keys fk
WHERE fk.parent_object_id = OBJECT_ID('dbo.Booking');
