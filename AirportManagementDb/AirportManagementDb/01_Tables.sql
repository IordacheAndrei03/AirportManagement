USE AirportManagement;
GO

CREATE TABLE dbo.Adress(
    Id int IDENTITY(1,1) NOT NULL,
    Country nvarchar(50) NOT NULL,
    City nvarchar(50) NOT NULL,
    Street nvarchar(50) NOT NULL
);
GO

CREATE TABLE dbo.Airport(
    Id int IDENTITY(1,1) NOT NULL,
    IATACode Char(3) NOT NULL,
    Name nvarchar(100) NOT NULL,
    TimeZone nvarchar(100) NOT NULL,
    AdressId int NOT NULL
);
GO

CREATE TABLE dbo.Aircraft(
    Id int IDENTITY(1,1) NOT NULL,
    TailNumber nvarchar(20) NOT NULL,
    Model nvarchar(50) NOT NULL,
    SeatCapacity int NOT NULL
);
GO

CREATE TABLE dbo.Airline(
    Id int IDENTITY(1,1) NOT NULL,
    IATACode char(2) NOT NULL,
    Name nvarchar(50) NOT NULL
);
GO

CREATE TABLE dbo.[User](
    Id int IDENTITY(1,1) NOT NULL,
    Name nvarchar(50) NOT NULL,
    Email nvarchar(50) NOT NULL
);
GO

CREATE TABLE dbo.BookingStatus(
    Id int IDENTITY(1,1) NOT NULL,
    Status nvarchar(20) NOT NULL
);
GO

CREATE TABLE dbo.FlightStatus(
    Id int IDENTITY(1,1) NOT NULL,
    Status nvarchar(20) NOT NULL
);
GO

CREATE TABLE dbo.Gate(
    Id int IDENTITY(1,1) NOT NULL,
    AirportId int NOT NULL,
    Code nvarchar(20) NOT NULL
);
GO

CREATE TABLE dbo.Flight(
    Id int IDENTITY(1,1) NOT NULL,
    AirlineId int NOT NULL,
    FlightNumber nvarchar(20) NOT NULL,
    OriginAirport int NOT NULL,
    DestinationAirport int NOT NULL,
    IsActive bit NOT NULL,
    DefaultAircraftId int NOT NULL
);
GO

CREATE TABLE dbo.FlightSchedule(
    Id int IDENTITY(1,1) NOT NULL,
    FlightId int NOT NULL,
    ScheduledDepartureUtc datetime2(0) NOT NULL,
    ScheduleArrivalUtc datetime2(0) NOT NULL,
    GateId int NOT NULL,
    AssignedAircraftId int NOT NULL,
    FlightStatusId int NOT NULL
);
GO

CREATE TABLE dbo.Booking(
    Id int IDENTITY(1,1) NOT NULL,
    UserId int NOT NULL,
    BookingStatusId int NOT NULL,
    CreatedUtc datetime2(0) NOT NULL,
    ConfirmationCode nvarchar(20) NOT NULL,
    Quantity int NOT NULL
);
GO

CREATE TABLE dbo.Ticket(
    Id int IDENTITY(1,1) NOT NULL,
    BookingId int NOT NULL,
    FlightScheduleId int NOT NULL,
    FareClass nvarchar(50) NOT NULL,
    BasePrice decimal(10,2) NOT NULL,
    Taxes decimal(10,2) NOT NULL,
    TotalPrice decimal(10,2) NOT NULL,
    Currency char(3) NOT NULL,
    IsRefundable bit NOT NULL,
    SeatNumber nvarchar(10) NOT NULL,
    PassangerFullName nvarchar(50) NOT NULL,
    PassangerEmail nvarchar(50) NOT NULL
);
GO
