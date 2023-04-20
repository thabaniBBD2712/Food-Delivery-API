
USE [FoodDeliveryDB];
GO

CREATE OR ALTER VIEW [ItemView] AS
	SELECT I.*, II.itemName, II.itemDescription, IC.itemCategoryID, IC.itemCategoryName, IST.itemStatusName 
	FROM [Item] I 
	JOIN [ItemInformation] II	ON I.itemInformationId = II.itemInformationId
	JOIN [ItemCategory]	IC		ON II.itemCategoryId = IC.itemCategoryId
	JOIN [ItemStatus] IST		ON I.itemStatusId = IST.itemStatusId
GO
