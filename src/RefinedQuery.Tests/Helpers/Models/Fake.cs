using System.Collections.Generic;

namespace RefinedQuery.Tests.Helpers.Models
{
    public static class Fake
    {
        public static ICollection<Person> Persons()
        {
            return new List<Person>()
            {
                new Person { FirstName = "Eduardo", LastName = "Macedo", Age = 25 },
                new Person { FirstName = "John", LastName = "Doe", Age = 37 },
                new Person { FirstName = "Mary", LastName = "Lucy", Age = 37 },
                new Person { FirstName = "Doe", LastName = "Johnson", Age = 37 },
                new Person { FirstName = "Aderbal", LastName = "Joilson", Age = 25 },
                new Person { FirstName = "Jediscleuza", LastName = "Santos", Age = 37 },
                new Person { FirstName = "Newell", LastName = "Robinson", Age = 37 },
                new Person { FirstName = "Joilson", LastName = "Dantas", Age = 37 },
            };
        }
    }
}