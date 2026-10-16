/*
 BUOI 5 - MiniSupermarketDb
 Chạy script này trong SSMS sau khi đã chọn đúng database MiniSupermarketDb.
 Script tương thích với bảng Products, Customers, Categories của project Buoi4.
*/
IF DB_ID(N'MiniSupermarketDb') IS NULL
    THROW 50001, 'Chua co database MiniSupermarketDb. Hay tao database va chay migration Buoi4 truoc.', 1;
GO
USE MiniSupermarketDb;
GO

IF OBJECT_ID('dbo.Orders', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders
    (
        OrderId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Orders PRIMARY KEY,
        OrderCode VARCHAR(30) NOT NULL,
        CreatedDate DATETIME NOT NULL CONSTRAINT DF_Orders_CreatedDate DEFAULT(GETDATE()),
        CashierUsername VARCHAR(50) NOT NULL,
        CustomerId INT NOT NULL,
        TotalItems INT NOT NULL,
        FinalAmount DECIMAL(18,2) NOT NULL,
        RewardPoints INT NOT NULL CONSTRAINT DF_Orders_RewardPoints DEFAULT(0),
        CONSTRAINT UQ_Orders_OrderCode UNIQUE(OrderCode),
        CONSTRAINT FK_Orders_Customers FOREIGN KEY(CustomerId) REFERENCES dbo.Customers(CustomerId),
        CONSTRAINT CK_Orders_TotalItems CHECK(TotalItems >= 0),
        CONSTRAINT CK_Orders_FinalAmount CHECK(FinalAmount >= 0),
        CONSTRAINT CK_Orders_RewardPoints CHECK(RewardPoints >= 0)
    );
END;
GO

IF OBJECT_ID('dbo.OrderDetails', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderDetails
    (
        OrderDetailId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrderDetails PRIMARY KEY,
        OrderId INT NOT NULL,
        ProductId INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        LineTotal DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY(OrderId) REFERENCES dbo.Orders(OrderId),
        CONSTRAINT FK_OrderDetails_Products FOREIGN KEY(ProductId) REFERENCES dbo.Products(ProductId),
        CONSTRAINT CK_OrderDetails_Quantity CHECK(Quantity > 0),
        CONSTRAINT CK_OrderDetails_UnitPrice CHECK(UnitPrice >= 0),
        CONSTRAINT CK_OrderDetails_LineTotal CHECK(LineTotal >= 0)
    );
END;
GO

/* Dữ liệu demo báo cáo: tạo 15 hóa đơn khi chưa có hóa đơn và đã có khách hàng/sản phẩm. */
IF NOT EXISTS (SELECT 1 FROM dbo.Orders)
AND EXISTS (SELECT 1 FROM dbo.Customers)
AND EXISTS (SELECT 1 FROM dbo.Products)
BEGIN
    DECLARE @CustomerId INT = (SELECT TOP 1 CustomerId FROM dbo.Customers ORDER BY CustomerId);
    DECLARE @ProductId INT = (SELECT TOP 1 ProductId FROM dbo.Products ORDER BY ProductId);
    DECLARE @i INT = 1;
    WHILE @i <= 15
    BEGIN
        DECLARE @Cashier VARCHAR(50) = CONCAT('cashier', RIGHT(CONCAT('0', ((@i - 1) % 5) + 1), 2));
        DECLARE @Amount DECIMAL(18,2) = CAST((100000 + (@i * 13750)) AS DECIMAL(18,2));
        DECLARE @Points INT = CAST(@Amount / 10000 AS INT);
        DECLARE @Code VARCHAR(30) = CONCAT('HD-SEED-', RIGHT(CONCAT('000', @i), 3));
        INSERT INTO dbo.Orders(OrderCode, CreatedDate, CashierUsername, CustomerId, TotalItems, FinalAmount, RewardPoints)
        VALUES(@Code, DATEADD(DAY, -(@i % 30), GETDATE()), @Cashier, @CustomerId, 1, @Amount, @Points);
        DECLARE @NewOrderId INT = SCOPE_IDENTITY();
        INSERT INTO dbo.OrderDetails(OrderId, ProductId, Quantity, UnitPrice, LineTotal)
        SELECT @NewOrderId, @ProductId, 1, @Amount, @Amount;
        SET @i += 1;
    END;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetRevenueByCashier
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CashierUsername,
           COUNT(OrderId) AS TotalOrders,
           SUM(FinalAmount) AS TotalRevenue,
           SUM(RewardPoints) AS TotalPointsAwarded
    FROM dbo.Orders
    GROUP BY CashierUsername
    ORDER BY TotalRevenue DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetRevenueByDateRange
    @FromDate DATETIME,
    @ToDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CashierUsername,
           COUNT(OrderId) AS TotalOrders,
           SUM(FinalAmount) AS TotalRevenue
    FROM dbo.Orders
    WHERE CreatedDate >= @FromDate AND CreatedDate <= @ToDate
    GROUP BY CashierUsername
    ORDER BY TotalRevenue DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetTop5SellingProducts
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (5)
        p.ProductId, p.Barcode, p.ProductName,
        SUM(od.Quantity) AS TotalQuantitySold,
        SUM(od.LineTotal) AS TotalRevenue
    FROM dbo.OrderDetails od
    INNER JOIN dbo.Products p ON p.ProductId = od.ProductId
    GROUP BY p.ProductId, p.Barcode, p.ProductName
    ORDER BY TotalQuantitySold DESC, TotalRevenue DESC;
END;
GO

/* Kiểm tra nhanh */
SELECT TOP 10 * FROM dbo.Orders ORDER BY OrderId DESC;
SELECT TOP 10 * FROM dbo.OrderDetails ORDER BY OrderDetailId DESC;
EXEC dbo.sp_GetRevenueByCashier;
GO
