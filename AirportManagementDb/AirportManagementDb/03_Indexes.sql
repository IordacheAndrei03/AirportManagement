USE AirportManagement;
GO

---------- AIPORT INDEXES ----------
CREATE UNIQUE INDEX UX_Airport_IATACode
    ON dbo.Airport(IATACode);

CREATE UNIQUE INDEX UX_Airport_AdressId
    ON dbo.Airport(AdressId);

---------- AIRCRAFT INDEXES ----------

CREATE UNIQUE INDEX UX_Aircraft_Model
    ON dbo.Aircraft(Model);

---------- AIRLINEINDEXES ----------

CREATE UNIQUE INDEX UX_Airline_IATACode
    ON dbo.Airline(IATACode);

---------- USER INDEXES ----------

CREATE UNIQUE INDEX UX_User_Email
    ON dbo.[User](Email);

---------- GATE INDEXES ----------

CREATE UNIQUE INDEX UX_Gate_Airport_Code
    ON dbo.Gate(AirportId, Code);

---------- FLIGHT INDEXES ----------

CREATE NONCLUSTERED INDEX IX_Flight_Airline_FlightNumber
    ON dbo.Flight(AirlineId, FlightNumber);

CREATE NONCLUSTERED INDEX IX_Flight_Airline_FlightNumber_IsActive
    ON dbo.Flight(AirlineId, FlightNumber)
    WHERE IsActive = 1;

CREATE NONCLUSTERED INDEX IX_Flight_Origin_Destination
    ON dbo.Flight(OriginAirport, DestinationAirport);

---------- FLIGHTSCHEDULE INDEXES ----------

CREATE NONCLUSTERED INDEX IX_FlightSchedule_Flight_DepartureUtc
    ON dbo.FlightSchedule(FlightId, ScheduledDepartureUtc);

CREATE NONCLUSTERED INDEX IX_FlightSchedule_Gate_DepartureUtc
    ON dbo.FlightSchedule(GateId, ScheduledDepartureUtc);

---------- TICKET INDEXES ----------

CREATE NONCLUSTERED INDEX IX_Ticket_FlightSchedule_FareClass
    ON dbo.Ticket(FlightScheduleId, FareClass);

CREATE UNIQUE INDEX UX_Booking_ConfirmationCode
    ON dbo.Booking(ConfirmationCode);
