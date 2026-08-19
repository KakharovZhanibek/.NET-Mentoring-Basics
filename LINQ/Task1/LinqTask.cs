using System;
using System.Collections.Generic;
using System.Linq;
using Task1.DoNotChange;

namespace Task1
{
    public static class LinqTask
    {
        public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
        {
            if (customers == null) throw new ArgumentNullException(nameof(customers));
            return customers.Where(c => c.Orders.Sum(o => o.Total) > limit);
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            if (customers == null) throw new ArgumentNullException(nameof(customers));
            return from c in customers
                   join s in suppliers ?? Enumerable.Empty<Supplier>()
                       on new { Country = c.Country?.ToLower(), City = c.City?.ToLower() }
                       equals new { Country = s.Country?.ToLower(), City = s.City?.ToLower() }
                       into matchedSuppliers
                   select (c, (IEnumerable<Supplier>)matchedSuppliers);
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            if (customers == null) throw new ArgumentNullException(nameof(customers));

            var supplierGroups =
                from s in suppliers ?? Enumerable.Empty<Supplier>()
                group s by new { Country = s.Country.ToLower(), City = s.City.ToLower() } into sg
                select sg;

            return from c in customers
                   join sg in supplierGroups
                       on new { Country = c.Country?.ToLower(), City = c.City?.ToLower() }
                       equals sg.Key into matchedGroups
                   select (c, (IEnumerable<Supplier>)matchedGroups.SelectMany(g => g));
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            if (customers == null) throw new ArgumentNullException(nameof(customers));
            return customers.Where(c => c.Orders.Any(o => o.Total > limit));
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            if (customers == null) throw new ArgumentNullException(nameof(customers));
            return customers
                .Where(c => c.Orders != null && c.Orders.Length > 0)
                .Select(c => (c, c.Orders.Min(o => o.OrderDate)));
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            if (customers == null) throw new ArgumentNullException(nameof(customers));
            return customers
                .Where(c => c.Orders != null && c.Orders.Length > 0)
                .Select(c => (c, c.Orders.Min(o => o.OrderDate)))
                .OrderBy(x => x.Item2.Year)
                .ThenBy(x => x.Item2.Month)
                .ThenByDescending(x => x.c.Orders.Sum(o => o.Total))
                .ThenBy(x => x.c.CompanyName);
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            if (customers == null) throw new ArgumentNullException(nameof(customers));
            return customers.Where(c =>
                (c.PostalCode != null && c.PostalCode.Any(ch => !char.IsDigit(ch))) ||
                string.IsNullOrEmpty(c.Region) ||
                (c.Phone != null && !c.Phone.Contains("("))
            );
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            if (products == null) throw new ArgumentNullException(nameof(products));
            return products
                .GroupBy(p => p.Category)
                .Select(cg => new Linq7CategoryGroup
                {
                    Category = cg.Key,
                    UnitsInStockGroup = cg
                        .GroupBy(p => p.UnitsInStock)
                        .Select(sg => new Linq7UnitsInStockGroup
                        {
                            UnitsInStock = sg.Key,
                            Prices = sg.OrderBy(p => p.UnitPrice).Select(p => p.UnitPrice)
                        })
                });
        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            if (products == null) throw new ArgumentNullException(nameof(products));
            return new[]
            {
                (cheap, products.Where(p => p.UnitPrice <= cheap)),
                (middle, products.Where(p => p.UnitPrice > cheap && p.UnitPrice <= middle)),
                (expensive, products.Where(p => p.UnitPrice > middle && p.UnitPrice <= expensive))
            };
        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {
            if (customers == null) throw new ArgumentNullException(nameof(customers));
            return customers
                .GroupBy(c => c.City)
                .Select(g => (
                    g.Key,
                    (int)Math.Round(g.Average(c => c.Orders.Sum(o => o.Total))),
                    (int)Math.Round(g.Average(c => c.Orders.Length))
                ));
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            if (suppliers == null) throw new ArgumentNullException(nameof(suppliers));
            return suppliers
                .Select(s => s.Country)
                .Distinct()
                .OrderBy(c => c.Length)
                .ThenBy(c => c)
                .Aggregate(string.Empty, (acc, c) => acc + c);
        }
    }
}