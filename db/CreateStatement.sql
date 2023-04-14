USE master

IF EXISTS(select * from sys.databases where name='FoodDeliveryDB')
DROP DATABASE FoodDeliveryDB;

CREATE DATABASE FoodDeliveryDB;
GO

USE FoodDeliveryDB;

CREATE TABLE [Restaurant] (
  [restaurantId] integer PRIMARY KEY IDENTITY(1, 1),
  [restaurantUserName] varchar(50),
  [restaurantName] varchar(50),
  [restaurantAddress] integer,
  [restaurantDescription] varchar(255),
  [restaurantContactNumber] varchar(10)
)
GO

CREATE TABLE [User] (
  [userId] integer PRIMARY KEY IDENTITY(1, 1),
  [username] varchar(50),
  [userContactNumber] varchar(10)
)
GO

CREATE TABLE [Address] (
  [addressId] integer PRIMARY KEY IDENTITY(1, 1),
  [streetName] varchar(70),
  [city] varchar(50),
  [province] varchar(50),
  [postalCode] varchar(4)
)
GO

CREATE TABLE [Item] (
  [itemId] integer PRIMARY KEY IDENTITY(1, 1),
  [itemPrice] money,
  [restaurantId] integer,
  [itemStatusId] integer,
  [itemInformationId] integer
)
GO

CREATE TABLE [ItemStatus] (
  [itemStatusId] integer PRIMARY KEY IDENTITY(1, 1),
  [itemStatusName] varchar(30)
)
GO

CREATE TABLE [ItemInformation] (
  [itemInformationId] integer PRIMARY KEY IDENTITY(1, 1),
  [itemName] varchar(70),
  [itemDescription] varchar(255),
  [itemCategoryId] integer
)
GO

CREATE TABLE [ItemCategory] (
  [itemCategoryId] integer PRIMARY KEY IDENTITY(1, 1),
  [itemCategoryName] varchar(30)
)
GO

CREATE TABLE [Order] (
  [orderId] integer PRIMARY KEY IDENTITY(1, 1),
  [orderDate] datetime,
  [restaurantId] integer,
  [userId] integer,
  [personeelId] integer,
  [addressId] integer,
  [orderStatusId] integer
)
GO

CREATE TABLE [OrderItem] (
  [orderItemId] integer PRIMARY KEY IDENTITY(1, 1),
  [orderItemQuantity] integer,
  [orderItemPrice] money,
  [orderId] integer,
  [itemInformationId] integer
)
GO

CREATE TABLE [OrderStatus] (
  [orderStatusId] integer PRIMARY KEY IDENTITY(1, 1),
  [orderStatusName] varchar(70)
)
GO

CREATE TABLE [DeliveryPersoneel] (
  [personeelId] integer PRIMARY KEY IDENTITY(1, 1),
  [personeelName] varchar(50),
  [personeelContactNumber] varchar(10),
  [vehicleRegistrationNumber] varchar(8)
)
GO

ALTER TABLE [Restaurant] ADD FOREIGN KEY ([restaurantAddress]) REFERENCES [Address] ([addressId])
GO

ALTER TABLE [Item] ADD FOREIGN KEY ([restaurantId]) REFERENCES [Restaurant] ([restaurantId])
GO

ALTER TABLE [Item] ADD FOREIGN KEY (itemStatusId) REFERENCES [ItemStatus] ([itemStatusId])
GO

ALTER TABLE [Item] ADD FOREIGN KEY ([itemInformationId]) REFERENCES [ItemInformation] ([itemInformationId])
GO

ALTER TABLE [ItemInformation] ADD FOREIGN KEY ([itemCategoryId]) REFERENCES [ItemCategory] ([itemCategoryId])
GO

ALTER TABLE [Order] ADD FOREIGN KEY ([restaurantId]) REFERENCES [Restaurant] ([restaurantId])
GO

ALTER TABLE [Order] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([userId])
GO

ALTER TABLE [Order] ADD FOREIGN KEY ([personeelId]) REFERENCES [DeliveryPersoneel] ([personeelId])
GO

ALTER TABLE [Order] ADD FOREIGN KEY ([addressId]) REFERENCES [Address] ([addressId])
GO

ALTER TABLE [Order] ADD FOREIGN KEY ([orderStatusId]) REFERENCES [OrderStatus] ([orderStatusId])
GO

ALTER TABLE [OrderItem] ADD FOREIGN KEY ([orderId]) REFERENCES [Order] ([orderId])
GO

ALTER TABLE [OrderItem] ADD FOREIGN KEY ([itemInformationId]) REFERENCES [ItemInformation] ([itemInformationId])
GO
