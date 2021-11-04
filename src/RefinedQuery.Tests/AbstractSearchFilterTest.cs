using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;
using FluentAssertions;

using RefinedQuery.Search;
using RefinedQuery.Linq;

namespace RefinedQuery.Tests
{
    public class Person
    {
      public string FirstName { get; set; }

      public string LastName { get; set; }

      public int Age { get; set; }

    }

    public class PersonSearchFilter : AbstractSearchFilter<Person>
    {
        public PersonSearchFilter()
        {

          SearchFor((p) => p.FirstName)
            .ThatMatches((fname, search) => fname.ToLower().Contains(search.ToLower()));

          SearchFor((p) => p.LastName)
            .ThatMatches((lname, search) => lname.ToLower().Contains(search.ToLower()));

          SearchFor((p) => p.Age)
            .ThatMatches((age, search) => age.ToString() == search);
        }
    }

    public class EmptySearchFilter : AbstractSearchFilter<Person>
    {
        public EmptySearchFilter() {}
    }

    public class AbstractSearchFilterTest
    {

        IEnumerable<Person> persons = new List<Person>()
        {
            new Person { FirstName = "Eduardo", LastName = "Macedo", Age = 25 },
            new Person { FirstName = "John", LastName = "Doe", Age = 37 },
            new Person { FirstName = "Mary", LastName = "Lucy", Age = 37 },
            new Person { FirstName = "Doe", LastName = "Johnson", Age = 37 },
        };

        [Fact]
        public void AbstractSearchFilter_ShouldReturn_MatchesOnAnyColumn()
        {
            var query = persons.AsQueryable();        
            var filter = new PersonSearchFilter();

            var searchTerm = "Doe";

            var result = filter.ApplyFilter(query, searchTerm);

            result.Should().SatisfyRespectively(
                first => { first.LastName.Should().Be("Doe"); },
                second => { second.FirstName.Should().Be("Doe"); }
            );
        }

        [Fact]
        public void AbstractSeachFilter_ShouldNot_ModifyQuery_WhenSearchTermIsNull()
        {
            var query = persons.AsQueryable();        
            var filter = new PersonSearchFilter();

            string searchTerm = null;

            var result = query.Search(filter, searchTerm);

            result.Should().HaveCount(4);
        }

        [Fact]
        public void AbstractSearchFilter_ShouldNot_ModifyQuery_WhenGivenNoSearchRules()
        {
            
            var query = persons.AsQueryable();        
            var filter = new EmptySearchFilter();

            var searchTerm = "Mary";

            var result = query.Search(filter, searchTerm);

            result.Should().Equal(persons);
        }

        [Fact]
        public void AbstractSearchFilter_ShouldIgnore_NullProperties()
        {
            var clone = persons.ToList();
            clone.Add(new Person { FirstName = null, LastName = null, Age = 30 });

            var query = clone.AsQueryable();
            var filter = new PersonSearchFilter();

            var searchTerm = "something";

            var result = filter.ApplyFilter(query, searchTerm);

            result.Should().BeEmpty();
        }
    }
}
