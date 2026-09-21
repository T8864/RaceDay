-- RaceDay API Seed Data
-- Run after registering users through the API

USE RaceDay_API;
GO

-- Categories
INSERT INTO Categories (CategoryName, Description)
VALUES
('5KM Fun Run', 'Beginner friendly 5 kilometer run'),
('10KM Race', 'Intermediate 10 kilometer race'),
('Half Marathon', 'Advanced 21.1 kilometer half marathon');
GO

-- Events
INSERT INTO Events (EventName, EventDate, Location, Distance, Status, MaxParticipants, OrganiserID, CategoryID, CreatedAt)
VALUES
('Cape Town Spring Race', '2026-10-15', 'Cape Town', '42.2km', 'Upcoming', 500, 1, 1, GETDATE()),
('Joburg City Run', '2026-11-20', 'Johannesburg', '10km', 'Upcoming', 300, 2, 2, GETDATE()),
('Pretoria Heritage Run', '2026-12-05', 'Pretoria', '5km', 'Upcoming', 400, 1, 3, GETDATE());
GO

-- Enrolments
INSERT INTO Enrolments (UserID, EventID, CategoryID, EnrolmentDate, Status)
VALUES
(3, 1, 1, GETDATE(), 'Confirmed'),
(4, 1, 2, GETDATE(), 'Confirmed'),
(3, 2, 3, GETDATE(), 'Confirmed'),
(4, 3, 1, GETDATE(), 'Confirmed');
GO

-- Results
INSERT INTO Results (EnrolmentID, FinishTime, PositionOverall, PositionCategory, Notes)
VALUES
(1, '00:32:15', 1, 1, 'Personal Best'),
(2, '01:05:30', 2, 2, 'Strong finish');
GO