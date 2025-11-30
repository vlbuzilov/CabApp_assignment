**Handling Large Files (e.g., 10GB CSV)**

The current implementation is already designed to work efficiently with large datasets.

The program streams the CSV file record-by-record using CsvHelper.GetRecordsAsync, which ensures that only a small portion of the file is kept in memory at any time. Duplicate detection is also handled in a streaming manner, and batches of 1000 records are inserted into the database, preventing excessive memory usage.

If I needed to scale this solution to reliably handle a 10GB CSV file or larger, the first change I would make is to replace Entity Framework batch inserts with SqlBulkCopy, which provides a highly optimized, low-overhead bulk insertion mechanism. This would significantly increase throughput and reduce insertion time for extremely large datasets.

Additional improvements could include parallelizing CPU-bound data transformations or using a producer–consumer pipeline, but those would be secondary optimizations. The core design—streaming input, validating incrementally, and writing duplicates immediately—already supports very large file processing efficiently.
