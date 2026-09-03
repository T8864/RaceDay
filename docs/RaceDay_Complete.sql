-- ================================================
-- RaceDay Database Complete Script
-- Created by: [Boitumelo Mathebula]
-- Date: 2026-09-03
-- ================================================

-- Create Database
CREATE DATABASE RaceDay;
GO

USE RaceDay;
GO

-- ================================================
-- CREATE TABLES
-- ================================================

-- Organisers Table
CREATE TABLE Organisers (
    OrganiserID INT PRIMARY KEY IDENTITY(1,1),
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Phone VARCHAR(20),
    OrganisationName VARCHAR(100) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- Participants Table
CREATE TABLE Participants (
    ParticipantID INT PRIMARY KEY IDENTITY(1,1),
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Phone VARCHAR(20),
    DateOfBirth DATE NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- Categories Table
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName VARCHAR(50) NOT NULL,
    Distance DECIMAL(5,2) NOT NULL,
    Description VARCHAR(255)
);
GO

-- Events Table
CREATE TABLE Events (
    EventID INT PRIMARY KEY IDENTITY(1,1),
    EventName VARCHAR(100) NOT NULL,
    EventDate DATE NOT NULL,
    Location VARCHAR(100) NOT NULL,
    MaxParticipants INT NOT NULL,
    OrganiserID INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (OrganiserID) REFERENCES Organisers(OrganiserID)
);
GO

-- Enrolments Table
CREATE TABLE Enrolments (
    EnrolmentID INT PRIMARY KEY IDENTITY(1,1),
    ParticipantID INT NOT NULL,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    EnrolmentDate DATETIME DEFAULT GETDATE(),
    Status VARCHAR(20) DEFAULT 'Registered',
    FOREIGN KEY (ParticipantID) REFERENCES Participants(ParticipantID),
    FOREIGN KEY (EventID) REFERENCES Events(EventID),
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);
GO

-- Results Table
CREATE TABLE Results (
    ResultID INT PRIMARY KEY IDENTITY(1,1),
    EnrolmentID INT NOT NULL,
    FinishTime TIME,
    Position INT,
    Notes VARCHAR(255),
    FOREIGN KEY (EnrolmentID) REFERENCES Enrolments(EnrolmentID)
);
GO

-- ================================================
-- SEED DATA
-- ================================================

-- Insert Organisers
INSERT INTO Organisers (FirstName, LastName, Email, Phone, OrganisationName)
VALUES 
('John', 'Smith', 'john.smith@raceday.com', '0821234567', 'Cape Town Running Club'),
('Sarah', 'Johnson', 'sarah.johnson@raceday.com', '0839876543', 'Joburg Athletics');
GO

-- Insert Participants
INSERT INTO Participants (FirstName, LastName, Email, Phone, DateOfBirth)
VALUES
('Michael', 'Brown', 'michael.brown@gmail.com', '0711234567', '1990-05-15'),
('Emily', 'Davis', 'emily.davis@gmail.com', '0729876543', '1995-08-22'),
('James', 'Wilson', 'james.wilson@gmail.com', '0731234567', '1988-03-10'),
('Lisa', 'Anderson', 'lisa.anderson@gmail.com', '0749876543', '2000-11-30');
GO

-- Insert Categories
INSERT INTO Categories (CategoryName, Distance, Description)
VALUES
('5KM Fun Run', 5.00, 'Beginner friendly 5 kilometer run'),
('10KM Race', 10.00, 'Intermediate 10 kilometer race'),
('Half Marathon', 21.10, 'Advanced 21.1 kilometer half marathon');
GO

-- Insert Events
INSERT INTO Events (EventName, EventDate, Location, MaxParticipants, OrganiserID)
VALUES
('Cape Town Spring Race', '2026-10-15', 'Cape Town', 500, 1),
('Joburg City Run', '2026-11-20', 'Johannesburg', 300, 2),
('Pretoria Heritage Run', '2026-12-05', 'Pretoria', 400, 1);
GO

-- Insert Enrolments
INSERT INTO Enrolments (ParticipantID, EventID, CategoryID, Status)
VALUES
(1, 1, 1, 'Registered'),
(2, 1, 2, 'Registered'),
(3, 2, 3, 'Registered'),
(4, 2, 1, 'Registered'),
(1, 3, 2, 'Registered'),
(3, 3, 3, 'Registered');
GO

-- Insert Results
INSERT INTO Results (EnrolmentID, FinishTime, Position, Notes)
VALUES
(1, '00:32:15', 1, 'Personal Best'),
(2, '01:05:30', 2, 'Strong finish'),
(3, '02:15:45', 1, 'Winner');
GO