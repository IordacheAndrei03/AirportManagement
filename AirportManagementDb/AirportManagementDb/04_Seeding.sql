
---------- Adress ----------

INSERT INTO dbo.Adress (Country, City, Street)
VALUES ('Romania', 'Bucharest', 'Otopeni Str 1');
DECLARE @AddressIdBucharestOtopeni INT = SCOPE_IDENTITY();

INSERT INTO dbo.Adress (Country, City, Street)
VALUES ('United Kingdom', 'London', 'Heathrow Road 1');
DECLARE @AddressIdLondonHeathrow INT = SCOPE_IDENTITY();

---------- Airport ----------

INSERT INTO dbo.Airport (IATACode, Name, TimeZone, AdressId)
VALUES (
    'OTP',
    'Henri Coanda International Airport',
    'Europe/Bucharest',
    @AddressIdBucharestOtopeni
);
DECLARE @AirportIdOtp INT = SCOPE_IDENTITY();

INSERT INTO dbo.Airport (IATACode, Name, TimeZone, AdressId)
VALUES (
    'LHR',
    'London Heathrow Airport',
    'Europe/London',
    @AddressIdLondonHeathrow
);
DECLARE @AirportIdLhr INT = SCOPE_IDENTITY();

---------- Aircraft ----------

INSERT INTO dbo.Aircraft (TailNumber, Model, SeatCapacity)
VALUES ('YR-BGJ', 'Airbus A320', 180);
DECLARE @AircraftIdAirbusA320 INT = SCOPE_IDENTITY();

INSERT INTO dbo.Aircraft (TailNumber, Model, SeatCapacity)
VALUES ('G-BAAA', 'Boeing 737-800', 186);
DECLARE @AircraftIdBoeing737800 INT = SCOPE_IDENTITY();

---------- Airline ----------

INSERT INTO dbo.Airline (IATACode, Name)
VALUES ('RO', 'TAROM');
DECLARE @AirlineIdTarom INT = SCOPE_IDENTITY();

INSERT INTO dbo.Airline (IATACode, Name)
VALUES ('BA', 'British Airways');
DECLARE @AirlineIdBritishAirways INT = SCOPE_IDENTITY();

---------- User ----------

INSERT INTO dbo.[User] (Name, Email)
VALUES ('Andrei Iordache', 'andrei.iordache@yahoo.com');
DECLARE @UserIdJohnDoe INT = SCOPE_IDENTITY();

INSERT INTO dbo.[User] (Name, Email)
VALUES ('Beatrice Constantin', 'beatrice.constantin@yahoo.com');
DECLARE @UserIdAnaPopescu INT = SCOPE_IDENTITY();

---------- Booking Status ----------

INSERT INTO dbo.BookingStatus (Status)
VALUES ('Active');
DECLARE @BookingStatusIdActive INT = SCOPE_IDENTITY();

INSERT INTO dbo.BookingStatus (Status)
VALUES ('Cancelled');
DECLARE @BookingStatusIdCancelled INT = SCOPE_IDENTITY();

---------- Flight Status ----------

INSERT INTO dbo.FlightStatus (Status)
VALUES ('Scheduled');
DECLARE @FlightStatusIdScheduled INT = SCOPE_IDENTITY();

INSERT INTO dbo.FlightStatus (Status)
VALUES ('Boarding');
DECLARE @FlightStatusIdBoarding INT = SCOPE_IDENTITY();

INSERT INTO dbo.FlightStatus (Status)
VALUES ('Departed');
DECLARE @FlightStatusIdDeparted INT = SCOPE_IDENTITY();

INSERT INTO dbo.FlightStatus (Status)
VALUES ('Canceled');
DECLARE @FlightStatusIdCanceled INT = SCOPE_IDENTITY();

INSERT INTO dbo.FlightStatus (Status)
VALUES ('Delayed');
DECLARE @FlightStatusIdDelayed INT = SCOPE_IDENTITY();

---------- Gate ----------

INSERT INTO dbo.Gate (AirportId, Code)
VALUES (@AirportIdOtp, 'A1');
DECLARE @GateIdOtpA1 INT = SCOPE_IDENTITY();

INSERT INTO dbo.Gate (AirportId, Code)
VALUES (@AirportIdLhr, 'B10');
DECLARE @GateIdLhrB10 INT = SCOPE_IDENTITY();

---------- Flight ----------

-- RO 391 OTP -> LHR
INSERT INTO dbo.Flight (
    AirlineId,
    FlightNumber,
    OriginAirport,
    DestinationAirport,
    IsActive,
    DefaultAircraftId
)
VALUES (
    @AirlineIdTarom,
    '391',
    @AirportIdOtp,
    @AirportIdLhr,
    1,
    @AircraftIdAirbusA320
);
DECLARE @FlightIdRo391 INT = SCOPE_IDENTITY();

-- BA 888 LHR -> OTP
INSERT INTO dbo.Flight (
    AirlineId,
    FlightNumber,
    OriginAirport,
    DestinationAirport,
    IsActive,
    DefaultAircraftId
)
VALUES (
    @AirlineIdBritishAirways,
    '888',
    @AirportIdLhr,
    @AirportIdOtp,
    1,
    @AircraftIdBoeing737800
);
DECLARE @FlightIdBa888 INT = SCOPE_IDENTITY();

---------- FlightSchedule ----------

INSERT INTO dbo.FlightSchedule (
    FlightId,
    ScheduledDepartureUtc,
    ScheduleArrivalUtc,
    GateId,
    AssignedAircraftId,
    FlightStatusId
)
VALUES (
    @FlightIdRo391,
    '2025-01-15T06:00:00',
    '2025-01-15T08:30:00',
    @GateIdOtpA1,
    @AircraftIdAirbusA320,
    @FlightStatusIdScheduled
);
DECLARE @FlightScheduleIdRo391_20250115 INT = SCOPE_IDENTITY();

INSERT INTO dbo.FlightSchedule (
    FlightId,
    ScheduledDepartureUtc,
    ScheduleArrivalUtc,
    GateId,
    AssignedAircraftId,
    FlightStatusId
)
VALUES (
    @FlightIdBa888,
    '2025-01-15T10:00:00',
    '2025-01-15T12:30:00',
    @GateIdLhrB10,
    @AircraftIdBoeing737800,
    @FlightStatusIdScheduled
);
DECLARE @FlightScheduleIdBa888_20250115 INT = SCOPE_IDENTITY();

---------- Booking ----------

INSERT INTO dbo.Booking (
    UserId,
    BookingStatusId,
    CreatedUtc,
    ConfirmationCode,
    Quantity
)
VALUES (
    @UserIdJohnDoe,
    @BookingStatusIdActive,
    SYSUTCDATETIME(),
    'ABC123',
    2
);
DECLARE @BookingIdJohnDoe INT = SCOPE_IDENTITY();

INSERT INTO dbo.Booking (
    UserId,
    BookingStatusId,
    CreatedUtc,
    ConfirmationCode,
    Quantity
)
VALUES (
    @UserIdAnaPopescu,
    @BookingStatusIdActive,
    SYSUTCDATETIME(),
    'ANA001',
    1
);
DECLARE @BookingIdAnaPopescu INT = SCOPE_IDENTITY();

---------- Ticket ----------

INSERT INTO dbo.Ticket (
    BookingId,
    FlightScheduleId,
    FareClass,
    BasePrice,
    Taxes,
    TotalPrice,
    Currency,
    IsRefundable,
    SeatNumber,
    PassangerFullName,
    PassangerEmail
)
VALUES (
    @BookingIdJohnDoe,
    @FlightScheduleIdRo391_20250115,
    'Economy',
    120.00,
    30.00,
    150.00,
    'EUR',
    1,
    '12A',
    'John Doe',
    'john.doe@example.com'
);

INSERT INTO dbo.Ticket (
    BookingId,
    FlightScheduleId,
    FareClass,
    BasePrice,
    Taxes,
    TotalPrice,
    Currency,
    IsRefundable,
    SeatNumber,
    PassangerFullName,
    PassangerEmail
)
VALUES (
    @BookingIdAnaPopescu,
    @FlightScheduleIdBa888_20250115,
    'Economy',
    100.00,
    25.00,
    125.00,
    'EUR',
    0,
    '18C',
    'Ana Popescu',
    'ana.popescu@example.com'
);
