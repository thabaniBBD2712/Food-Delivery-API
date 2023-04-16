# Food delivery API

## Description
A food delivery API (Application Programming Interface) is a software interface that enables communication between a food delivery service and other applications, such as mobile apps or websites. The API provides developers with the tools to access and integrate data and functionalities related to food ordering and delivery services.

## Prerequisites
* dotnet version 6^
* C# version 10^

## Developing
Open terminal and execute `dotnet restore` to install dependencies <br/>
If using VisualStudio, make sure to open `FoodDeliveryAPI.csproj` as the project, and not just the project folder

### Initialising DB
1. Open your favourite SQL Server manager and connect to `(localdb)\MSSQLLocalDB` 
2. Run script `db\CreateStatement.sql` to create database and tables
3. Run script `db\InsertStatements.sql` to populate database with mock data

## Running
Open terminal and execute `dotnet run`<br/>
Navigate to [swagger](http://localhost:5263/swagger/index.html) for documentation :)

## Authors
* [Anne-Mien Carr](https://github.com/AnneMienBBD)
* [Hewitt Nyambalo](https://github.com/hewitt-BBD)
* [Kyle Pottinger](https://github.com/Kyle-Pottinger)
* [Thabani Nkonde](https://github.com/thabaniBBD2712)
* [Tlholo Koma](https://github.com/Tlholo-Koma)
* [Willa Lyle](https://github.com/willacharlotte)
* [Zeerak Baig](https://github.com/ZeerakBaig-BBD)
