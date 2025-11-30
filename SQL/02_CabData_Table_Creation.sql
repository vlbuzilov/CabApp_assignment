
USE CabDB;

CREATE TABLE CabData (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    tpep_pickup_datetime DATETIME2(2) NOT NULL,
    tpep_dropoff_datetime DATETIME2(2) NOT NULL,
    passenger_count TINYINT NULL,
    trip_distance FLOAT NULL,   
    store_and_fwd_flag VARCHAR(3) NULL,
    PULocationID INT NULL,
    DOLocationID INT NULL,
    fare_amount DECIMAL(10,2) NULL,
    tip_amount DECIMAL(10,2) NULL
);