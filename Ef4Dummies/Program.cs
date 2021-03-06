﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Ef4Dummies
{
    class Program
    {
        static void Main(string[] args)
        {
            // disables migration check
            Database.SetInitializer<Ef.Context>(null);

            Benchmark(() => FetchWithConditionsViaEf(), 1000, "Entity Framework 6 - Fetching with conditions");
            Benchmark(() => FetchWithConditionsViaPrecompiledQuery(), 1000, "Linq2Sql Compiled Query - Fetching with conditions");

            Console.ReadLine();
        }

        private static void Benchmark(Action call, int repeats, string what)
        {
            var records = new List<double>();

            Console.WriteLine("-------------------------");
            Console.WriteLine(what + ": with " + repeats + " iterations");

            var watch = new Stopwatch();
            watch.Start();
            
            for (var i = 0; i < repeats; i++)
            {
                var start = DateTime.UtcNow;
                call();
                records.Add(DateTime.UtcNow.Subtract(start).TotalMilliseconds);
            }

            Console.WriteLine("Total ellapsed Time in Milliseconds : {0}", watch.ElapsedMilliseconds);
            Console.WriteLine("Warming: {0} - Fastes : {1} - Slowest: {2} - AVERAGE: {3} in Milliseconds", records.First(), records.Min(), records.Max(), records.Average());
            Console.WriteLine("-------------------------");

            watch.Stop();
        }

        private static int FetchWithConditionsViaEf()
        {
            using (var context = new Ef.Context())
            {
                // querying only
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;
                context.Configuration.LazyLoadingEnabled = false;
                //context.Database.Log = Console.WriteLine;
                // also does not track query
                var results = context.Orders.AsNoTracking()
                                     .Where(order => order.EmployeeID >= 1 && order.ShipVia >= 1)
                                     .OrderBy(order => order.OrderDate)
                                     .Skip(5)
                                     .Take(100)
                                     .ToArray();
                return results.Length;
            }
        }

        private static int FetchWithConditionsViaPrecompiledQuery()
        {
            using (var context = new Linq2Sql.ContextDataContext())
            {
                // does not track query
                context.ObjectTrackingEnabled = false;
                //context.Log = new DebugTextWriter();
                var results = CompiledQuery(context).ToArray();
                return results.Length;
            }
        }

        private static readonly Func<Linq2Sql.ContextDataContext, IQueryable<Linq2Sql.Order>> CompiledQuery =
            System.Data.Linq.CompiledQuery.Compile<Linq2Sql.ContextDataContext, IQueryable<Linq2Sql.Order>>
            (
                context => context.Orders
                                  .Where(order => order.EmployeeID >= 1 && order.ShipVia >= 1)
                                  .Skip(5)
                                  .Take(100)
            );

        class DebugTextWriter : System.IO.TextWriter
        {
            private readonly string _custom;

            public DebugTextWriter(string custom = null)
            {
                _custom = custom;
                TryPrintCustomText();
            }

            public override void Write(char[] buffer, int index, int count)
            {
                Console.Write(new String(buffer, index, count));
            }

            public override void Write(string value)
            {
                Console.Write(value);
            }

            public override Encoding Encoding
            {
                get { return Encoding.Default; }
            }

            private void TryPrintCustomText()
            {
                if (!string.IsNullOrWhiteSpace(_custom))
                {
                    Console.WriteLine("");
                    Console.WriteLine(_custom);
                }
            }
        }

    }
}
