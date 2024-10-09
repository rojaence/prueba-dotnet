USE master;
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'pruebaDotnetDB')
BEGIN
    ALTER DATABASE pruebaDotnetDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE pruebaDotnetDB;
END
GO

-- 3. Crear la nueva BD
CREATE DATABASE pruebaDotnetDB;
GO

-- 4. Usar la BD
USE pruebaDotnetDB;
GO


CREATE TABLE Users (
    IdUser INT PRIMARY KEY IDENTITY,
    Username VARCHAR(50) NOT NULL,
    Password VARCHAR(100) NOT NULL,
    SessionActive BIT DEFAULT 0 NOT NULL,
    Email VARCHAR(120) NOT NULL,
    Status BIT DEFAULT 1 NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    MiddleName VARCHAR(50) NOT NULL,
    FirstLastname VARCHAR(50) NOT NULL,
    SecondLastname VARCHAR(50) NOT NULL,
    IdCard VARCHAR(10) NOT NULL,
    BirthDate DATE NOT NULL
)
GO

CREATE TABLE Sessions (
    IdSession INT PRIMARY KEY IDENTITY,
    IdUser INT NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NULL,
    FOREIGN KEY (IdUser) REFERENCES Users(IdUser)
)
GO

CREATE TABLE Permissions (
    IdPermission INT PRIMARY KEY IDENTITY,
    Name VARCHAR(50) NOT NULL UNIQUE,
)
GO

CREATE TABLE Roles (
    IdRole INT PRIMARY KEY IDENTITY,
    RoleName VARCHAR(50) NOT NULL UNIQUE
)
GO

CREATE TABLE Role_Permission (
    IdRole INT NOT NULL,
    IdPermission INT NOT NULL,
    FOREIGN KEY (IdRole) REFERENCES Roles(IdRole),
    FOREIGN KEY (IdPermission) REFERENCES Permissions(IdPermission)
)
GO

CREATE TABLE Role_User (
    IdRole INT NOT NULL,
    IdUser INT NOT NULL,
    FOREIGN KEY (IdRole) REFERENCES Roles(IdRole),
    FOREIGN KEY (IdUser) REFERENCES Users(IdUser)
)
GO


INSERT INTO Roles (RoleName) VALUES ('Admin'), ('User')
GO
INSERT INTO Permissions (Name) VALUES 
('canViewWelcome'),
('canViewDashboard'),
('searchUsers'),
('canUpdateOtherUserProfiles')
GO

INSERT INTO Role_Permission (IdRole, IdPermission) VALUES
(1, 1),
(1, 2),
(1, 3),
(1, 4),
(2, 1)
GO

INSERT INTO Users (Username, Password, SessionActive, Email, Status, FirstName, MiddleName, FirstLastname, SecondLastname, IdCard, BirthDate)
VALUES ('rendarac', '$2a$13$zTlIlDv4ksRfiLcX0YJjqe.Wrky7V8Oq4XTottr5MalEeb6p1B0xe', 0, 'rendarac@mail.com', 1, 'Ronny', 'Jacinto', 'Endara', 'Celi', '1234567890', '1999-02-19')
GO

INSERT INTO Role_User (IdRole, IdUser) VALUES (1, 1)
GO