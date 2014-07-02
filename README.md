ormsbenchmark
=============

Simple benchmark EF & Linq2Sql

DB to test: Northwind
(if you don't have it in SQL Server, please attach the given DB file 'Northwind.mdf' in folder 'Db'.


RESULT:
=============

Entity Framework 6 - Fetching with conditions: with 1000 iterations

Total ellapsed Time in Milliseconds : 3451

Warming: 1377.0788 - Fastes : 1 - Slowest: 1377.0788 - AVERAGE: 3.4511974000001 in Milliseconds

#######

Linq2Sql Compiled Query - Fetching with conditions: with 1000 iterations

Total ellapsed Time in Milliseconds : 656

Warming: 28.0016 - Fastes : 0 - Slowest: 28.0016 - AVERAGE: 0.656037499999995 in  Milliseconds

