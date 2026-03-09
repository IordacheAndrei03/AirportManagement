using System;
using System.Collections.Generic;
using AirportManagement.Infrastructure.ScaffoldDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.ScaffoldDb.Context;

public partial class AirportManagementContext : DbContext
{
    public AirportManagementContext()
    {
    }

    public AirportManagementContext(DbContextOptions<AirportManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adress> Adresses { get; set; }

    public virtual DbSet<Aircraft> Aircraft { get; set; }

    public virtual DbSet<Airline> Airlines { get; set; }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingStatus> BookingStatuses { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<FlightSchedule> FlightSchedules { get; set; }

    public virtual DbSet<FlightStatus> FlightStatuses { get; set; }

    public virtual DbSet<Gate> Gates { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adress>(entity =>
        {
            entity.ToTable("Adress");

            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Street).HasMaxLength(50);
        });

        modelBuilder.Entity<Aircraft>(entity =>
        {
            entity.HasIndex(e => e.Model, "UX_Aircraft_Model").IsUnique();

            entity.Property(e => e.Model).HasMaxLength(50);
            entity.Property(e => e.TailNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<Airline>(entity =>
        {
            entity.ToTable("Airline");

            entity.HasIndex(e => e.Iatacode, "UX_Airline_IATACode").IsUnique();

            entity.Property(e => e.Iatacode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("IATACode");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Airport>(entity =>
        {
            entity.ToTable("Airport");

            entity.HasIndex(e => e.AdressId, "UX_Airport_AdressId").IsUnique();

            entity.HasIndex(e => e.Iatacode, "UX_Airport_IATACode").IsUnique();

            entity.Property(e => e.Iatacode)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("IATACode");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.TimeZone).HasMaxLength(100);

            entity.HasOne(d => d.Adress).WithOne(p => p.Airport)
                .HasForeignKey<Airport>(d => d.AdressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Airport_Adress");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("Booking");

            entity.HasIndex(e => e.ConfirmationCode, "UX_Booking_ConfirmationCode").IsUnique();

            entity.Property(e => e.ConfirmationCode).HasMaxLength(20);
            entity.Property(e => e.CreatedUtc)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.BookingStatus).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BookingStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_BookingStatus");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BookingStatus>(entity =>
        {
            entity.ToTable("BookingStatus");

            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.ToTable("Flight");

            entity.HasIndex(e => new { e.AirlineId, e.FlightNumber }, "IX_Flight_Airline_FlightNumber");

            entity.HasIndex(e => new { e.AirlineId, e.FlightNumber }, "IX_Flight_Airline_FlightNumber_IsActive").HasFilter("([IsActive]=(1))");

            entity.HasIndex(e => new { e.OriginAirport, e.DestinationAirport }, "IX_Flight_Origin_Destination");

            entity.Property(e => e.FlightNumber).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Airline).WithMany(p => p.Flights)
                .HasForeignKey(d => d.AirlineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Flight_Airline");

            entity.HasOne(d => d.DefaultAircraft).WithMany(p => p.Flights)
                .HasForeignKey(d => d.DefaultAircraftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Flight_DefaultAircraft");

            entity.HasOne(d => d.DestinationAirportNavigation).WithMany(p => p.FlightDestinationAirportNavigations)
                .HasForeignKey(d => d.DestinationAirport)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Flight_DestinationAirport");

            entity.HasOne(d => d.OriginAirportNavigation).WithMany(p => p.FlightOriginAirportNavigations)
                .HasForeignKey(d => d.OriginAirport)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Flight_OriginAirport");
        });

        modelBuilder.Entity<FlightSchedule>(entity =>
        {
            entity.ToTable("FlightSchedule");

            entity.HasIndex(e => new { e.FlightId, e.ScheduledDepartureUtc }, "IX_FlightSchedule_Flight_DepartureUtc");

            entity.HasIndex(e => new { e.GateId, e.ScheduledDepartureUtc }, "IX_FlightSchedule_Gate_DepartureUtc");

            entity.Property(e => e.ScheduleArrivalUtc).HasPrecision(0);
            entity.Property(e => e.ScheduledDepartureUtc).HasPrecision(0);

            entity.HasOne(d => d.AssignedAircraft).WithMany(p => p.FlightSchedules)
                .HasForeignKey(d => d.AssignedAircraftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FlightSchedule_Aircraft");

            entity.HasOne(d => d.Flight).WithMany(p => p.FlightSchedules)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Flightschedule_Flight");

            entity.HasOne(d => d.FlightStatus).WithMany(p => p.FlightSchedules)
                .HasForeignKey(d => d.FlightStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FlightScheduled_Status");

            entity.HasOne(d => d.Gate).WithMany(p => p.FlightSchedules)
                .HasForeignKey(d => d.GateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FlightSchedule_Gate");
        });

        modelBuilder.Entity<FlightStatus>(entity =>
        {
            entity.ToTable("FlightStatus");

            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<Gate>(entity =>
        {
            entity.ToTable("Gate");

            entity.HasIndex(e => new { e.AirportId, e.Code }, "UX_Gate_Airport_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(20);

            entity.HasOne(d => d.Airport).WithMany(p => p.Gates)
                .HasForeignKey(d => d.AirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gate_Airport");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Ticket");

            entity.HasIndex(e => new { e.FlightScheduleId, e.FareClass }, "IX_Ticket_FlightSchedule_FareClass");

            entity.Property(e => e.BasePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FareClass).HasMaxLength(50);
            entity.Property(e => e.PassangerEmail).HasMaxLength(50);
            entity.Property(e => e.PassangerFullName).HasMaxLength(50);
            entity.Property(e => e.SeatNumber).HasMaxLength(10);
            entity.Property(e => e.Taxes).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Booking).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Booking");

            entity.HasOne(d => d.FlightSchedule).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.FlightScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_FlightSchedule");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
