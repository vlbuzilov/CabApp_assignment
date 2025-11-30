
namespace CabETL.CabETL.DTO
{
    public class CabTripDTO
    {
        public string VendorID { get; set; } = null!;
        public string tpep_pickup_datetime { get; set; } = null!;
        public string tpep_dropoff_datetime { get; set; } = null!;
        public string passenger_count { get; set; } = null!;
        public string trip_distance { get; set; } = null!;
        public string RatecodeID { get; set; } = null!;
        public string store_and_fwd_flag { get; set; } = null!;
        public string PULocationID { get; set; } = null!;
        public string DOLocationID { get; set; } = null!;
        public string payment_type { get; set; } = null!;
        public string fare_amount { get; set; } = null!;
        public string extra { get; set; } = null!;
        public string mta_tax { get; set; } = null!;
        public string tip_amount { get; set; } = null!;
        public string tolls_amount { get; set; } = null!;
        public string improvement_surcharge { get; set; } = null!;
        public string total_amount { get; set; } = null!;
        public string congestion_surcharge { get; set; } = null!;
    }
}