GO
Alter table RoleMaster add AddedOn DateTime NOT NULL DEFAULT GETDATE()
Alter table RoleMaster add ModifiedOn DateTime NULL DEFAULT GETDATE()
Alter table RoleMaster add IsDeleted BIT NOT NULL DEFAULT 0

Alter Table Usermaster Add AddedBy INT NOT NULL DEFAULT 1001
Alter Table Usermaster Add ModifiedBy INT NULL

Alter Table Rolemaster Add AddedBy INT NOT NULL DEFAULT 1001
Alter Table Rolemaster Add ModifiedBy INT NULL


GO

CREATE TABLE ProductRawMaterialMapping
(
ProductRMMappingID int not null primary key identity(1,1),
ProductID int not null, 
RawMaterialID int not null,
RMReqdForUOMQty int not null,
UnitType tinyint not null,
AddedOn	datetime default getdate() not null,
LastModifiedOn	datetime null
)
GO

CREATE TABLE Unit
(
UnitID int not null primary key identity(1,1),
UnitName varchar(10)
)
GO

INSERT INTO Unit
(UnitName)
VALUES ('Number'),('Percentage')

GO

CREATE TABLE MenuMaster
(
MenuID int not null primary key identity(1,1),
MenuCode varchar(20) not null,
MenuName varchar(50) not null,
MenuIcon varchar(100) null,
MenuParentID int null,
Link varchar(200) null, 
AddedOn	datetime default getdate() not null,
LastModifiedOn	datetime null,
IsActive BIT NOT NULL DEFAULT(1)
)
GO

INSERT INTO MenuMaster
(MenuCode,MenuName,MenuParentID,Link)
VALUES
('M_Master','Masters',null,null),
('M_Inventory','Inventory',null,null),
('M_Trasactions','Trasactions',null,null)
GO

INSERT INTO MenuMaster
(MenuCode,MenuName,MenuParentID,Link)
VALUES
('M_Pallets','Pallets',1,'GetAll/Pallet'),
('M_Racks','Racks',1,'GetAll/Rack'),
('M_RM','Raw Materials',1,'GetAll/RawMaterial'),
('M_Products','Products',1,'GetAll/Product'),
('M_Moulds','Moulds',1,'GetAll/Mould'),
('M_Machines','Machines',1,'GetAll/Machine'),
('M_Vendors','Vendors',1,'GetAll/Company'),
('M_Customers','Customers',1,'GetAll/Company'),
('M_Users','Users',1,'GetAll/UserMaster'),
('M_Roles','Roles',1,'GetAll/UserMaster'),
------------------------------------------------
('M_RMInventory','Raw Materials',2,'GetAll/RMInventory'),
------------------------------------------------
('M_PO','Purchase Orders',3,'GetAll/Purchase'),
('M_PR','Purchase Received',3,'GetAll/PurchaseRcv'),
('M_Sales','Sales Order',3,'GetAll/Sales'),
('M_OpenSales','Open Sales Orders',3,'GetAll/Dispatch'),
('M_Production','Production Programs',3,'GetAll/Production'),
('M_Dispatch','Dispatch',3,'GetAll/Packing')
------------------------------------------------

GO

CREATE TABLE RoleRights
(
RoleRightsID int not null primary key identity(1,1),
RoleID int not null,
MenuID int not null,
[Add] bit not null default(0),
[Update] bit not null default(0),
[Delete] bit not null default(0),
[View] bit not null default(0),
IsActive bit not null default(1),
AddedOn DATETIME NOT NULL DEFAULT(GETDATE()),
ModifiedOn DATETIME NULL,
AddedBy INT NOT NULL DEFAULT(1001),
ModifiedBy INT NULL
)

GO

INSERT INTO RoleRights
(RoleID,MenuID,[Add],[Update],[Delete],[View])

SELECT		RoleID,MenuID,1,1,1,1 
FROM		MenuMaster
CROSS JOIN	RoleMaster
Where		RoleID = 101

GO

Alter Table ProductRawMaterialMapping Add IsDeleted BIT NOT NULL DEFAULT 0
GO
----------  25-Jun-2023-- Added By Kunal ---------- 


ALTER TABLE ProductMaster
DROP CONSTRAINT FK_ProductMaster_RawMaterialMaster;
GO

ALTER TABLE ProductMaster
DROP COLUMN RawMaterialID, RMReqdForUOMQty
GO

CREATE TABLE ProductionProgramRMMapping
(
ProductionProgramRMMappingID int not null primary key identity(1,1),
ProductionProgramID int not null,
ProductID int not null,
RawMaterialID int not null,
RMQty int not null
)
GO

ALTER TABLE ProductionProgram
DROP CONSTRAINT FK_ProductionProgram_RawMaterialMaster
GO
ALTER TABLE MenuMaster ADD IsActive BIT NOT NULL DEFAULT 1
ALTER TABLE MenuMaster ADD AddedBy INT NOT NULL DEFAULT(1001)
ALTER TABLE MenuMaster ADD ModifiedBy INT NULL

ALTER TABLE ProductionProgram
DROP COLUMN RawMaterialID, RMQty
GO
---- Added By Kunal On 26 Jun -----
update MenuMaster
SET Link = 'GetUsers/UserMaster' Where MenuCode = 'M_Users'
GO
update MenuMaster
SET Link = 'GetRoles/UserMaster' Where MenuCode = 'M_Roles'

GO

--ALTER TABLE RoleRights ADD AddedOn DATETIME NOT NULL DEFAULT(GETDATE())
--ALTER TABLE RoleRights ADD ModifiedOn DATETIME NULL

--ALTER TABLE RoleRights ADD AddedBy INT NOT NULL DEFAULT(1001)
--ALTER TABLE RoleRights ADD ModifiedBy INT NULL

---- Added By Kunal On 27 Jun -----

ALTER TABLE ProductionProgramRMMapping
ADD CONSTRAINT FK_ProductionProgam_ID FOREIGN KEY (ProductionProgramID)
    REFERENCES ProductionProgram(ProductionProgramID);

ALTER TABLE ProductionProgramRMMapping
ADD CONSTRAINT FK_RawMaterial_ID FOREIGN KEY (RawMaterialID)
    REFERENCES RawMaterialMaster(RawMaterialID);

ALTER TABLE ProductRawMaterialMapping
ADD CONSTRAINT FK_ProductMaster_ID FOREIGN KEY (ProductID)
    REFERENCES ProductMaster(ProductID);

ALTER TABLE ProductRawMaterialMapping
ADD CONSTRAINT FK_ProductRawMaterial_ID FOREIGN KEY (RawMaterialID)
    REFERENCES RawMaterialMaster(RawMaterialID);

Go
-- 10-Sept-2023
update MenuMaster SET Link = 'GetAllProduction/Production' WHERE MenuCode = 'M_Production'
Go

CREATE TABLE ProductionProgramBatch
(
ProgramBatchID bigint primary key identity(1,1) not null,
ProductionProgramID int foreign key references ProductionProgram(ProductionProgramID),
InProductionQty int not null,
ProductQtyCompleted int not null,
UserID int not null,
AddedOn Date default getdate()
)
Go
------------------------28-07-2024-------------------------------------------------
Alter table RackMaster Add Location NVARCHAR(MAX) NULL
Alter table RawMaterialMaster Add SupplierDetails NVARCHAR(MAX) NULL
Alter table ProductRawMaterialMapping ADD RMGradeUsed INT NULL
ALTER Table MouldMaster ADD Location NVARCHAR(MAX) NULL
ALTER Table MouldMaster ADD TotalCavities NVARCHAR(MAX) NULL
ALTER Table MouldMaster ADD RunningCavities NVARCHAR(MAX) NULL
ALTER Table MouldMaster ADD CorePins NVARCHAR(MAX) NULL

GO


CREATE TABLE VendorMaster
(
VendorId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
VendorName NVARCHAR(MAX) NOT NULL,
Notes NVARCHAR(MAX) NULL,
Address NVARCHAR(MAX) NULL,
ContactNumber NVARCHAR(MAX) NULL,
IsDiscontinued BIT NOT NULL DEFAULT(0),
AddedBy INT NOT NULL,
AddedOn DATETIME NOT NULL DEFAULT(getDate()),
LastModifiedBy INT NULL,
LastModifiedOn DATETIME NULL
)

update MenuMaster set Link = 'GetAll/VendorMaster' where MenuID = 10


-------------------------------------------------20-08-2024-----------------------------------

GO

CREATE TABLE LocationMaster
(
LocationId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
LocationName NvarChar(250) NULL,
IsActive BIT NOT NULL DEFAULT(1),
AddedBy INT NULL,
AddedOn DateTime NOT NULL DEFAULT(GETDATE()),
ModifiedBy INT NULL,
ModifiedOn DateTime NULL
)

GO

Alter table RackMaster Drop Column Location

GO

Alter table RackMaster Add LocationId INT NULL
GO

CREATE PROCEDURE usp_GetLocationMasterAllData

AS
BEGIN

Select LM.LocationId,
LM.LocationName,
ISNULL(UM.Username,'') AS 'AddedByName',
ISNULL(UsrMstr.Username,'') AS 'ModifiedByName',
LM.AddedOn,
LM.ModifiedOn
from
LocationMaster LM
LEFT JOIN UserMaster UM
ON LM.AddedBy = UM.UserID
LEFT JOIN UserMaster UsrMstr
ON LM.ModifiedBy = UsrMstr.UserID
WHERE LM.IsActive = 1

END

GO
INSERT INTO MenuMaster
(MenuCode,MenuName,MenuParentID,Link)
VALUES
('M_Location','Location',1,'GetAll/LocationMaster')

GO

------------------------------------------------------END-----------------------------


