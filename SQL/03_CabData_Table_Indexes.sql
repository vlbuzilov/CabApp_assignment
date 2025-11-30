USE CabDB;

-- Index for searches by PULocationID
CREATE NONCLUSTERED INDEX IX_CabData_PULocationID ON dbo.CabData(PULocationID);

-- Covering index for average tip_amount per PULocationID
CREATE NONCLUSTERED INDEX IX_CabData_PULocationID_Tip ON dbo.CabData (PULocationID) INCLUDE (tip_amount, fare_amount);

-- Index for top 100 longest trips by distance
CREATE NONCLUSTERED INDEX IX_CabData_TripDistance ON dbo.CabData (trip_distance DESC);

-- Index for top 100 longest trips by travel time
CREATE NONCLUSTERED INDEX IX_CabData_TravelTime ON dbo.CabData (tpep_pickup_datetime, tpep_dropoff_datetime);