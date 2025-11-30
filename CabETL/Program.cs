using System.Globalization;
using CabETL.CabETL.Data;
using CabETL.CabETL.Domain;
using CabETL.CabETL.DTO;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.secret.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("CabDB");

        string csvPath = configuration["Paths:CsvFile"]!;
        string duplicatesPath = configuration["Paths:DuplicatesFile"]!;

        var optionsBuilder = new DbContextOptionsBuilder<CabDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        using var context = new CabDbContext(optionsBuilder.Options);

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim
        };

        var hashSet = new HashSet<string>();
        const int batchSize = 1000;
        var batch = new List<CabTrip>();
        int totalInserted = 0; 

        var estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        try
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, csvConfig);

            await using var writer = new StreamWriter(duplicatesPath);
            await using var csvDup = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csvDup.WriteHeader<CabTripDTO>();
            await csvDup.NextRecordAsync();

            await foreach (var record in csv.GetRecordsAsync<CabTripDTO>())
            {
                try
                {
                    if (!DateTime.TryParse(record.tpep_pickup_datetime, out var pickup) ||
                        !DateTime.TryParse(record.tpep_dropoff_datetime, out var dropoff) ||
                        !decimal.TryParse(record.trip_distance, NumberStyles.Any, CultureInfo.InvariantCulture, out var td) || td < 0 ||
                        !decimal.TryParse(record.fare_amount, NumberStyles.Any, CultureInfo.InvariantCulture, out var fa) || fa < 0 ||
                        !decimal.TryParse(record.tip_amount, NumberStyles.Any, CultureInfo.InvariantCulture, out var ta) || ta < 0 ||
                        !int.TryParse(record.passenger_count, NumberStyles.Any, CultureInfo.InvariantCulture, out var pc) || pc < 0 ||
                        !int.TryParse(record.PULocationID, NumberStyles.Any, CultureInfo.InvariantCulture, out var puLocationID) || puLocationID < 0 ||
                        !int.TryParse(record.DOLocationID, NumberStyles.Any, CultureInfo.InvariantCulture, out var doLocationID) || doLocationID < 0)
                    {
                        continue;
                    }


                    var pickupUtc = TimeZoneInfo.ConvertTimeToUtc(pickup, estZone);
                    var dropoffUtc = TimeZoneInfo.ConvertTimeToUtc(dropoff, estZone);

                    var flag = record.store_and_fwd_flag.Trim().ToUpper() == "Y" ? "Yes" : "No";
                    string key = $"{pickup}-{dropoff}-{record.passenger_count}";

                    if (hashSet.Contains(key))
                    {
                        csvDup.WriteRecord(record);
                        await csvDup.NextRecordAsync();
                        continue;
                    }

                    hashSet.Add(key);

                    batch.Add(new CabTrip
                    {
                        tpep_pickup_datetime = pickupUtc,
                        tpep_dropoff_datetime = dropoffUtc,
                        passenger_count = pc,
                        trip_distance = td,
                        store_and_fwd_flag = flag,
                        PULocationID = puLocationID,
                        DOLocationID = doLocationID,
                        fare_amount = fa,
                        tip_amount = ta
                    });

                    if (batch.Count >= batchSize)
                    {
                        await context.CabTrips.AddRangeAsync(batch);
                        await context.SaveChangesAsync();
                        totalInserted += batch.Count; 
                        batch.Clear();
                    }
                }
                catch (Exception ex)
                {
                Console.WriteLine($"Error processing record: {ex.Message}");
                }
            }

        if (batch.Count > 0)
        {
            try
            {    
                await context.CabTrips.AddRangeAsync(batch);
                await context.SaveChangesAsync();
                totalInserted += batch.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting final batch: {ex.Message}");
            }
        }

        Console.WriteLine($"Inserted {totalInserted} rows into CabData table.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading CSV file: {ex.Message}");
        }
    }
}