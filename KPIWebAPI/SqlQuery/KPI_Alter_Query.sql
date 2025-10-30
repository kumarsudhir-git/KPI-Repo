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
ALTER TABLE RawMaterialInventoryMaster ADD AddedBy INT NULL, ModifiedBy INT NULL, LocationId INT NULL

GO
ALTER TABLE RawMaterialInventoryMaster ADD MinOrderlevel INT NULL

GO
INSERT INTO MenuMaster
(MenuCode,MenuName,MenuParentID,Link)
VALUES
('M_RMInventory','Master Batch',2,'GetAllMasterBatch/RMInventory'),
('M_RMInventory','Package Bags',2,'GetAllPackagBags/RMInventory'),
('M_RMInventory','Finished Goods',2,'GetAllFinishedGood/RMInventory')

GO

CREATE TABLE RMInventoryMasterBatch
(
MasterBatchId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
CodeNo NVARCHAR(250) NULL, 
Colour NVARCHAR(250) NULL, 
VendorId INT NULL, 
QtyInStock INT NULL, 
MinOrderLevel INT NULL,
LocationId INT NULL,
IsActive BIT NOT NULL DEFAULT(1),
AddedBy INT NULL,
AddedOn DATETIME NOT NULL DEFAULT(GETDATE()),
ModifiedBy INT NULL,
ModifiedOn DATETIME NULL
)

GO

CREATE TABLE RMInventoryPackageBags
(
PackageBagId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
Size INT NULL,  
VendorId INT NULL, 
QtyInStock INT NULL, 
MinOrderLevel INT NULL,
LocationId INT NULL,
IsActive BIT NOT NULL DEFAULT(1),
AddedBy INT NULL,
AddedOn DATETIME NOT NULL DEFAULT(GETDATE()),
ModifiedBy INT NULL,
ModifiedOn DATETIME NULL
)

GO

CREATE TABLE RMInventoryFinishedGood
(
FinishedGoodId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
ProductId INT NULL,  
RackId INT NULL,
Package INT NULL,
Qty INT NULL, 
MinOrderLevel INT NULL,
LocationId INT NULL,
IsActive BIT NOT NULL DEFAULT(1),
AddedBy INT NULL,
AddedOn DATETIME NOT NULL DEFAULT(GETDATE()),
ModifiedBy INT NULL,
ModifiedOn DATETIME NULL
)

GO

CREATE PROCEDURE usp_GetRMInventoryMasterBatch

AS
BEGIN

Select MB.MasterBatchId,
MB.CodeNo,
MB.Colour,
MB.QtyInStock,
MB.MinOrderLevel,
MB.VendorId,
VM.VendorName,
MB.LocationId,
LM.LocationName,
MB.AddedOn,
UM.Username AS 'AddedByName',
MB.ModifiedOn,
usrMstr.Username AS 'ModifiedByName'
from RMInventoryMasterBatch MB
LEFT JOIN UserMaster UM
ON MB.AddedBy = UM.UserID
LEFT JOIN UserMaster usrMstr
ON MB.ModifiedBy = usrMstr.UserID
LEFT JOIN VendorMaster VM
ON MB.VendorId = VM.VendorId
LEFT JOIN LocationMaster LM
ON MB.LocationId = LM.LocationId
WHERE MB.IsActive = 1
ORDER BY CASE WHEN MB.ModifiedOn != null THEN MB.ModifiedOn ELSE MB.AddedOn END DESC

END

GO

CREATE PROCEDURE usp_GetRMInventoryPackageBags

AS
BEGIN

Select PB.PackageBagId,
PB.Size,
VM.VendorName,
PB.QtyInStock,
PB.MinOrderLevel,
PB.LocationId,
LM.LocationName,
PB.AddedOn,
UM.Username AS 'AddedByName',
PB.ModifiedOn,
usrMstr.Username AS 'ModifiedByName'
from RMInventoryPackageBags PB
LEFT JOIN UserMaster UM
ON PB.AddedBy = UM.UserID
LEFT JOIN UserMaster usrMstr
ON PB.ModifiedBy = usrMstr.UserID
LEFT JOIN LocationMaster LM
ON PB.LocationId = LM.LocationId
LEFT JOIN VendorMaster VM
ON PB.VendorId = VM.VendorId
WHERE PB.IsActive = 1
ORDER BY CASE WHEN PB.ModifiedOn != null THEN PB.ModifiedOn ELSE PB.AddedOn END DESC

END

GO

CREATE PROCEDURE usp_GetRMInventoryFinishedGood

AS
BEGIN

Select FG.FinishedGoodId,
FG.ProductId,
PM.ProductName,
FG.RackId,
RM.RackNo AS 'RackNumber',
FG.Package,
FG.Qty,
FG.MinOrderLevel,
FG.LocationId,
LM.LocationName,
FG.AddedOn,
UM.Username AS 'AddedByName',
FG.ModifiedOn,
usrMstr.Username AS 'ModifiedByName'
from RMInventoryFinishedGood FG
LEFT JOIN UserMaster UM
ON FG.AddedBy = UM.UserID
LEFT JOIN UserMaster usrMstr
ON FG.ModifiedBy = usrMstr.UserID
LEFT JOIN LocationMaster LM
ON FG.LocationId = LM.LocationId
LEFT JOIN ProductMaster PM
ON FG.ProductId = PM.ProductID
LEFT JOIN RackMaster RM
ON FG.RackId = RM.RackID
WHERE FG.IsActive = 1
ORDER BY CASE WHEN FG.ModifiedOn != null THEN FG.ModifiedOn ELSE FG.AddedOn END DESC

END

GO

ALTER TABLE RawMaterialMaster DROP COLUMN SupplierDetails
GO

ALTER TABLE RawMaterialMaster ADD VendorId INT NULL

GO

ALTER TABLE MouldMaster DROP COLUMN Location
GO

ALTER TABLE MouldMaster ADD LocationId INT NULL

GO

-------------------------------03-09-2024---------------------------------------------
GO

ALTER TABLE VendorMaster ADD VendorType NVARCHAR(10) NULL

GO

--------------------------------------17-09-2024-----------------------------------------

ALTER TABLE PurchaseMaster
DROP CONSTRAINT FK_PurchaseMaster_CompanyLocationMaster

GO

---------------------------------------21-09-2024---------------------------------------

GO
ALTER TABLE PurchaseRcvdMaster ADD CompanyLocationId INT NULL

ALTER TABLE PurchaseRcvdMaster ADD LocationId INT NULL

ALTER TABLE PurchaseRcvdMaster ADD QCReceived BIT NOT NULL DEFAULT(0)

ALTER TABLE PurchaseRcvdMaster ADD QCStatus NVARCHAR(250) NULL

GO

------------------------------------21-09-2024--------------------------------

ALTER TABLE SalesMaster ADD Colour NVARCHAR(250) NULL
ALTER TABLE SalesMaster ADD GMS NVARCHAR(250) NULL
ALTER TABLE SalesMaster ADD GMSInfo NVARCHAR(250) NULL
ALTER TABLE SalesMaster ADD Package NVARCHAR(250) NULL
ALTER TABLE SalesMaster ADD Quantity NVARCHAR(250) NULL
ALTER TABLE SalesMaster ADD SampleRequired BIT NOT NULL DEFAULT(0)
ALTER TABLE SalesMaster ADD DeliveryAddress NVARCHAR(250) NULL
ALTER TABLE SalesMaster ADD Transporter NVARCHAR(250) NULL
ALTER TABLE SalesMaster ADD CommittedDate DATETIME NULL
ALTER TABLE SalesMaster ADD Rate NVARCHAR(250) NULL
ALTER TABLE SalesMaster ADD PaymentStatus NVARCHAR(250) NULL

GO

CREATE TABLE SalesRMMapping
(
SalesRMId INT NOT NULL PRIMARY KEY IDENTITY(1,1),
SalesId INT NULL,
RMId INT NULL,
IsActive BIT NOT NULL DEFAULT(1),
CreatedBy INT NULL,
CreatedDate DATETIME NOT NULL DEFAULT(GETDATE()),
ModifiedBy INT NULL,
ModifiedOn DATETIME NULL
)

GO

CREATE TABLE LookUpMaster (
    LookUpID INT PRIMARY KEY IDENTITY(1,1),
    LookUpType VARCHAR(50) NOT NULL,
	LookUpName VARCHAR(100) NOT NULL,
    LookUpValue VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    IsActive BIT NOT NULL DEFAULT 1,
	CreatedBy INT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
	ModifiedBy INT NULL,
    ModifiedDate DATETIME NULL
)

GO

INSERT INTO LookupMaster (LookUpType,LookUpName,LookUpValue,Description,IsActive,CreatedBy,CreatedDate)
VALUES('VendorType','Vendor','VD001','VendorType',1,1001,GETDATE()),
('VendorType','KPI-Labour','VD002','VendorType',1,1001,GETDATE())

GO

INSERT INTO LookupMaster (LookUpType,LookUpName,LookUpValue,Description,IsActive,CreatedBy,CreatedDate)
VALUES('SMRateAccess','SMRateAccess','101,102','Role Based Access for Sales Rate',1,1001,GETDATE())

GO

INSERT INTO LookupMaster (LookUpType,LookUpName,LookUpValue,Description,IsActive,CreatedBy,CreatedDate)
VALUES('GMSType','Standard','GSM001','GMSType',1,1001,GETDATE()),
('GMSType','Heavy','GSM002','GMSType',1,1001,GETDATE()),
('GMSType','Light','GSM003','GMSType',1,1001,GETDATE())

--------------------------------11-10-2014----------------------------------------------------

GO

INSERT INTO LookupMaster (LookUpType,LookUpName,LookUpValue,Description,IsActive,CreatedBy,CreatedDate)
VALUES('Color','White','CLR001','ColorType',1,1001,GETDATE()),
('Color','Black','CLR002','ColorType',1,1001,GETDATE()),
('Color','Blue','CLR003','ColorType',1,1001,GETDATE()),
('Color','Saffron','CLR004','ColorType',1,1001,GETDATE()),
('Color','Red','CLR005','ColorType',1,1001,GETDATE()),
('Color','Green','CLR006','ColorType',1,1001,GETDATE()),
('Color','Yellow','CLR007','ColorType',1,1001,GETDATE())

GO
ALTER TABLE SalesMaster
DROP CONSTRAINT FK_SalesMaster_CompanyLocationMaster

-------------------------------------01-12-2024---------------------------------------------------

ALTER TABLE MachineHistory ADD IsDeleted BIT DEFAULT(0)

----------------------------------------------------------IF-REQUIRED---------------------------

INSERT INTO MachineTypeMaster (MachineType,Description,IsDiscontinued,AddedOn,LastModifiedOn)
VALUES ('INJECTION BLOWING MACHINE','INJECTION BLOWING MACHINE',0,GETDATE(),NULL),
('EXTRUSION BLOWING MACHINE','EXTRUSION BLOWING MACHINE',0,GETDATE(),NULL),
('INJECTION MOULDING MACHINE','INJECTION MOULDING MACHINE',0,GETDATE(),NULL)

INSERT INTO MachineStatusMaster (MachineStatus,AddedOn,LastModifiedOn)
VALUES('NotInUse', GETDATE(), NULL),  
('InUse', GETDATE(), NULL),  
('InMaintainance', GETDATE(), NULL),  
('Discontinued', GETDATE(), NULL)

------------------------------------------------------END------------------------------------------------------------

--------------------------------------13-01-2025----------------------------------------------------------------------

INSERT INTO MouldTypeMaster (MouldType,Description,IsDiscontinued,AddedOn,LastModifiedOn)
VALUES ('Injection Mould','Injection Mould',0,GETDATE(),NULL),
('Blow Mould','Blow Mould',0,GETDATE(),NULL)

INSERT INTO UOMMaster (UnitsName,IsDiscontinued,AddedOn,LastModifiedOn)
VALUES ('Nos',0,GETDATE(),NULL),
('Gms',1,GETDATE(),NULL),
('Kgs',0,GETDATE(),NULL)


INSERT INTO TagColourMaster (TagColour,IsDiscontinued)
VALUES('RED',0),
('GREEN',0),
('BLUE',0),
('YELLOW',0)

-----------------------------------------------------17-01-2025-------------------------------------------------------------

GO
ALTER TABLE RMInventoryPackageBags
ALTER COLUMN Size NVARCHAR(MAX);

GO
ALTER TABLE RMInventoryPackageBags ADD ColorId INT NULL

GO
ALTER PROCEDURE usp_GetRMInventoryPackageBags  
  
AS  
BEGIN  
  
Select PB.PackageBagId,  
PB.Size,  
VM.VendorName,  
PB.QtyInStock,  
PB.MinOrderLevel,  
PB.LocationId,  
LM.LocationName,
TCM.TagColour as 'ColorName',
PB.AddedOn,  
UM.Username AS 'AddedByName',  
PB.ModifiedOn,  
usrMstr.Username AS 'ModifiedByName'  
from RMInventoryPackageBags PB  
LEFT JOIN UserMaster UM  
ON PB.AddedBy = UM.UserID  
LEFT JOIN UserMaster usrMstr  
ON PB.ModifiedBy = usrMstr.UserID  
LEFT JOIN LocationMaster LM  
ON PB.LocationId = LM.LocationId  
LEFT JOIN VendorMaster VM  
ON PB.VendorId = VM.VendorId 
LEFT JOIN TagColourMaster TCM
ON PB.ColorId = TCM.TagColourID
WHERE PB.IsActive = 1  
ORDER BY CASE WHEN PB.ModifiedOn != null THEN PB.ModifiedOn ELSE PB.AddedOn END DESC  
  
END

----------------------------------------------------------------------01-02-2025---------------------------------------
GO

ALTER PROCEDURE usp_GetRMInventoryPackageBags    
    
AS    
BEGIN    
    
Select PB.PackageBagId,    
PB.Size,    
VM.VendorName,    
PB.QtyInStock,    
PB.MinOrderLevel,    
PB.LocationId,    
LM.LocationName,  
--TCM.TagColour as 'ColorName',  
PB.AddedOn,    
UM.Username AS 'AddedByName',    
PB.ModifiedOn,    
usrMstr.Username AS 'ModifiedByName'    
from RMInventoryPackageBags PB    
LEFT JOIN UserMaster UM    
ON PB.AddedBy = UM.UserID    
LEFT JOIN UserMaster usrMstr    
ON PB.ModifiedBy = usrMstr.UserID    
LEFT JOIN LocationMaster LM    
ON PB.LocationId = LM.LocationId    
LEFT JOIN VendorMaster VM    
ON PB.VendorId = VM.VendorId   
--LEFT JOIN TagColourMaster TCM  
--ON PB.ColorId = TCM.TagColourID  
WHERE PB.IsActive = 1    
ORDER BY CASE WHEN PB.ModifiedOn != null THEN PB.ModifiedOn ELSE PB.AddedOn END DESC    
    
END

GO

insert into LookUpMaster (LookUpType,LookUpName,LookUpValue,Description,IsActive,CreatedBy,CreatedDate)
values('GMSType','Custom','GSM004','Custom GMSType',1,1001,GETDATE())

------------------------------------------------------END----------------------------------------------------------------

------------------------------------------------------28/02/2025-------------------------------------------------------------------

alter table ProductMaster
drop constraint FK_ProductMaster_ProductCategoryMaster 

----------------------------------------------------------------06-03-2025-------------------------------------------

INSERT INTO PurchaseStatusMaster (PurchaseStatusID,PurchaseStatus,AddedOn)
VALUES (10,'Open',GETDATE())

----------------------------------------------------------------05-04-2025-----------------------------------------------------------

INSERT INTO PurchaseStatusMaster (PurchaseStatusID,PurchaseStatus,AddedOn,LastModifiedOn)
VALUES(10,'Ack Pending',GETDATE(),NULL),
(20,'Partial Received',GETDATE(),NULL),
(30,'Full Received',GETDATE(),NULL),
(40,'In Stock', GETDATE(),null),
(50,'To Be Produced', GETDATE(),null),
(60,'In Production', GETDATE(),null),
(70,'Mould Not Available', GETDATE(),null)

----------------------------------------------------------------20-04-2025-----------------------------------------------------------

ALTER TABLE SalesDetails ADD Color NVARCHAR(250) NULL
ALTER TABLE SalesDetails ADD Gms NVARCHAR(250) NULL
ALTER TABLE SalesDetails ADD Package NVARCHAR(250) NULL
ALTER TABLE SalesDetails ADD Rate NVARCHAR(250) NULL
ALTER TABLE SalesDetails ADD SampleReq BIT NOT NULL DEFAULT(0)

-----------------------------------------------------------------27-04-2025--------------------------------------------------------------

INSERT INTO SalesStatusMaster (SalesStatusID, SalesStatus, AddedOn)
VALUES (1,'Confirmed',GETDATE()),
(10,'Procure Material', GETDATE()),
(20,'Awaiting Production', GETDATE()),
(30,'In Production', GETDATE()),
(40,'Completed', GETDATE()),
(50,'Ready For Despatch', GETDATE()), 
(60,'Despatched', GETDATE()),
(70,'On Hold', GETDATE()),
(80,'Partially Delivered', GETDATE()),
(999,'Cancelled', GETDATE())

----------------------------------------------------06-05-2025--------------------------------------------------------------------------------

Update MenuMaster set Link = 'GetAll/Dispatch' where MenuID = 20

GO

SET IDENTITY_INSERT MouldStatusMaster ON;

INSERT INTO MouldStatusMaster (MouldStatusID, MouldStatus, AddedOn, LastModifiedOn)
VALUES
(101, 'NotInUse', GETDATE(), GETDATE()),
(102, 'InUse', GETDATE(), GETDATE()),
(103, 'InMaintainance', GETDATE(), GETDATE()),
(104, 'Loaned', GETDATE(), GETDATE()),
(105, 'Discontinued', GETDATE(), GETDATE());

SET IDENTITY_INSERT MouldStatusMaster OFF;

GO
INSERT INTO MouldHistory (MouldID,MouldStatusID,Description,AddedOn,NextReminderOn)
Select MouldID,101,'First Entry',GETDATE(), NULL from MouldMaster

GO
--TRUNCATE TABLE MachineHistory

GO
INSERT INTO MachineHistory (MachineID,MachineStatusID,Description,AddedOn,NextReminderOn,IsDeleted)
Select MachineID,101,'First Entry',GETDATE(), NULL,0 from MachineMaster

--------------------------------------------------26-05-2025---------------
GO

ALTER TABLE MachineMouldMapping ADD AddedBy INT NULL
ALTER TABLE MachineMouldMapping ADD ModifiedBy INT NULL

GO

CREATE PROC usp_GetMachineMouldMapData
@orderBy NVARCHAR(20) = 'Machine'

AS
BEGIN

Select MMM.MachineMouldMappingID, MMM.MachineId, MM.MachineName, MMM.MouldId, MldMstr.MouldName,
ISNULL(UM.Username,'') AS 'AddedBy', MMM.AddedOn --,ISNULL(UsrMstr.Username,'') AS 'ModifiedBy', MMM.LastModifiedOn
from MachineMouldMapping MMM
--group by MMM.MachineID
JOIN MachineMaster MM
ON MMM.MachineID = MM.MachineID
JOIN MouldMaster MldMstr
ON MMM.MouldID = MldMstr.MouldID
JOIN UserMaster UM
ON MMM.AddedBy = UM.UserID
--LEFT JOIN UserMaster UsrMstr
--ON MMM.ModifiedBy = UsrMstr.UserID
where MMM.IsDiscontinued = 0
Order BY CASE WHEN @orderBy = 'Machine' THEN MM.MachineName ELSE MldMstr.MouldName END asc

END

-----------------------------------------------28-05-2025----------------------------------------------------------------

INSERT INTO MenuMaster
(MenuCode,MenuName,MenuParentID,Link)
VALUES
('M_Machines','Machine Mould Map',1,'GetAllMachineMouldMappedData/Machine'),
('M_Moulds','Mould Machine Map',1,'GetAllMouldMachineMappedData/Mould')

------------------------------------------------25-08-2025----------------------------------------------------------------

ALTER TABLE RawMaterialMaster ADD RMGrade NVARCHAR(MAX) NULL

ALTER TABLE ProductMaster
ADD MinimumSellingPrice DECIMAL(18,2) NOT NULL DEFAULT(0.00);	

--EXEC sp_rename 'ProductMaster.MaximumSellingPrice', 'MinimumSellingPrice', 'COLUMN';

ALTER TABLE ProductMaster ADD Colour NVARCHAR(250) NULL;

ALTER TABLE ProductRawMaterialMapping
ADD RMQty DECIMAL(18,2) NOT NULL DEFAULT(0.00),
    RMGrade NVARCHAR(20) NULL;

------------------------------------------------19-09-2025----------------------------------------------------------------
ALTER TABLE MouldMaster
ADD
    MaintenanceFrequency INT NULL,
    LastMaintenanceDate DATE NULL,
    PurchaseDate DATE NULL;

-------------------------------------------------27-09-2025----------------------------------------------------------------

ALTER TABLE VendorMaster ADD MOQ NVARCHAR(MAX) NULL
ALTER TABLE VendorMaster ADD ItemType NVARCHAR(50) NULL


INSERT INTO LookupMaster (LookUpType,LookUpName,LookUpValue,Description,IsActive,CreatedBy,CreatedDate)
VALUES('VendorItemType','RM','VIT001','VendorItemType',1,1001,GETDATE()),
('VendorItemType','FG','VIT002','VendorItemType',1,1001,GETDATE()),
('VendorItemType','CONSUMABLES','VIT003','VendorItemType',1,1001,GETDATE()),
('VendorItemType','CONSULTANTS','VIT004','VendorItemType',1,1001,GETDATE()),
('VendorItemType','LOGISTICS','VIT005','VendorItemType',1,1001,GETDATE()),
('VendorItemType','MACHINES','VIT006','VendorItemType',1,1001,GETDATE()),
('VendorItemType','SPARES','VIT007','VendorItemType',1,1001,GETDATE())

---------------------------------------------------14-10-2025--------------------------------------------------------
ALTER TABLE CompanyLocationMaster ADD Designation NVARCHAR(MAX) NULL

ALTER TABLE CompanyLocationMaster ADD PreferredTransporter NVARCHAR(MAX) NULL

---------------------------------------------------26-10-2025----------------------------------------------------------
GO
ALTER TABLE MachineMaster ADD VendorId INT NULL
ALTER TABLE MachineMaster ADD LocationId INT NULL

GO
update MenuMaster SET MenuName = 'CONSUMABLES' where MenuName = 'Package Bags'

GO

INSERT INTO LookUpMaster (LookUpType,LookUpName,LookUpValue,Description,IsActive,CreatedBy,CreatedDate)
VALUES
('ItemType','Package bags','IT001','ItemType',1,1001,GETDATE()),
('ItemType','Spares','IT002','ItemType',1,1001,GETDATE()),
('ItemType','Tapes','IT003','ItemType',1,1001,GETDATE()),
('ItemType','Cartons','IT004','ItemType',1,1001,GETDATE()),
('ItemType','Stationery','IT005','ItemType',1,1001,GETDATE()),
('ItemType','General','IT006','ItemType',1,1001,GETDATE())

GO
ALTER TABLE RMInventoryPackageBags ADD ItemType NVARCHAR(250) NULL

GO
ALTER PROCEDURE usp_GetRMInventoryPackageBags    
    
AS    
BEGIN    
    
Select PB.PackageBagId,    
PB.Size,    
VM.VendorName,    
PB.QtyInStock,    
PB.MinOrderLevel,    
PB.LocationId,    
LM.LocationName,  
TCM.TagColour AS 'ColorName',
LKMSTR.LookUpName AS 'ItemTypeName',
PB.AddedOn,    
UM.Username AS 'AddedByName',    
PB.ModifiedOn,    
usrMstr.Username AS 'ModifiedByName'    
from RMInventoryPackageBags PB    
LEFT JOIN UserMaster UM    
ON PB.AddedBy = UM.UserID    
LEFT JOIN UserMaster usrMstr    
ON PB.ModifiedBy = usrMstr.UserID    
LEFT JOIN LocationMaster LM    
ON PB.LocationId = LM.LocationId    
LEFT JOIN VendorMaster VM    
ON PB.VendorId = VM.VendorId   
LEFT JOIN TagColourMaster TCM  
ON PB.ColorId = TCM.TagColourID
LEFT JOIN LookUpMaster LKMSTR
ON PB.ItemType = LKMSTR.LookUpValue
WHERE PB.IsActive = 1    
ORDER BY CASE WHEN PB.ModifiedOn != null THEN PB.ModifiedOn ELSE PB.AddedOn END DESC    
    
END
------------------------------------------------------END----------------------------------------------------------------

