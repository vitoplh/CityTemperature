### Run instructions

Set the absolute path for the source file and the authentication key in the appropriate appsettings.json file, see Development for a sample.  
OpenApi Doc is accessible via /scalar

### Comments

The file is processed with CSVHelper library as the common advice I have seen several times on dotnet subreddit is that parsing CSV files is not that trivial. I opted to calculated the average on the go, that way we avoid large sums and problems that would come with scaling. 

Regarding the data I noticed that the temperatures are basically random numbers between -99.9 and 99.9, as the average is around 0+/-2, I verified this by importing the CSV to duckdb:

`CREATE TABLE test AS SELECT * FROM read_csv('measurements.txt', header = false, delim = ';', names = ['City', 'Temp']);`  

`SELECT City, MIN(Temp), MAX(Temp), AVG(Temp) AS AvgTemp, COUNT(*) AS Count FROM test GROUP BY City ORDER BY AvgTemp ASC;`

For the sake of simplicity an in-memory database was used (at first I wondered, if I'd just stick with the dictionary but then started thinking about sorting and pagination). Since it's wrapped in a repository, it's easire to replace and integration tests with test containers can be used ([MS Docs](https://learn.microsoft.com/en-us/ef/core/testing/choosing-a-testing-strategy), [NDC video](https://www.youtube.com/watch?v=td9HE0vxsf4)). For a real database I'd also look into using EF Bulk Extension for improved performance.

As for how to trigger updates, I was a bit torn. At first I wanted to implement detection for a file change, but turns out it's not always reliable. In Azure, we'd probably upload the file to blob storage, then have an event triggering an function that would process and load the file to the SQL server, making it completely decoupled from the API side (and I believe Azure SQL uses RCSI by default, so updates would not block reads). I am ignoring the edge case, if someone was accesing data by using pagination and the data would be updated then

There have been some [changes](https://devblogs.microsoft.com/dotnet/dotnet9-openapi/) to the OpenApi in the -NET ecosystem so I used Scalar ([Nick's Video](https://www.youtube.com/watch?v=8yI4gD1HruY)). For responses I used ProblemDetails as it's an RFC standard and tried avoiding throwing Exceptions for flow control. I could have added state to the repository and made it unavailable before it's initialized but I did not want to overcomplicate things for a POC.

A simple api key authentication was used ([Nick's Video](https://www.youtube.com/watch?v=GrJJXixjR8M)). I also grouped the minimal API's using this guide [link](https://www.tessferrandez.com/blog/2023/10/31/organizing-minimal-apis.html).

### To-do:

- switch from in-memory DB to something better
- add cancellation tokens
- improve OpenApi spec (currently authentication is not included)
- model uses primitive types, DDD favors value types and entities
- add pagination to the get all cities method
- better refreshing in the repository layer
- cleanup Program.cs (will need to look up a video I saw for inspiration)
- add logging to exception middleware for production
