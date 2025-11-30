using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CabETL.CabETL.Domain;
using Microsoft.EntityFrameworkCore;

namespace CabETL.CabETL.Data
{
    public class CabDbContext : DbContext
    {
        public CabDbContext(DbContextOptions<CabDbContext> options)
            : base(options)
        {
        }

        public DbSet<CabTrip> CabTrips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CabTrip>(entity =>
            {
                entity.ToTable("CabData");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.tpep_pickup_datetime).HasColumnType("datetime2(2)").IsRequired();
                entity.Property(e => e.tpep_dropoff_datetime).HasColumnType("datetime2(2)").IsRequired();
                entity.Property(e => e.passenger_count).HasColumnType("tinyint");
                entity.Property(e => e.trip_distance).HasColumnType("decimal(10,2)");
                entity.Property(e => e.store_and_fwd_flag).HasMaxLength(3);
                entity.Property(e => e.PULocationID);
                entity.Property(e => e.DOLocationID);
                entity.Property(e => e.fare_amount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.tip_amount).HasColumnType("decimal(10,2)");

                entity.HasIndex(e => e.PULocationID)
                    .HasDatabaseName("IX_CabData_PULocationID");

                entity.HasIndex(e => e.trip_distance)
                    .HasDatabaseName("IX_CabData_TripDistance");

                entity.HasIndex(e => new { e.tpep_pickup_datetime, e.tpep_dropoff_datetime })
                    .HasDatabaseName("IX_CabData_TravelTime");
            });
        }
    }
}