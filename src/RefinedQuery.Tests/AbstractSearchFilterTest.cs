using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;
using FluentAssertions;

using RefinedQuery.Searching;
using RefinedQuery.Linq;
using RefinedQuery.Tests.Helpers;
using RefinedQuery.Tests.Helpers.Xunit;
using RefinedQuery.Tests.Helpers.Models;

namespace RefinedQuery.Tests
{

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

    [Collection("DB")]
    public class AbstractSearchFilterTest : RespawnedTest
    {

        IEnumerable<Person> persons = Fake.Persons(); 

        private readonly DbContextFixture _fixture;

        public AbstractSearchFilterTest(DbContextFixture fixture)
            : base(fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void AbstractSearchFilter_ShouldReturn_MatchesOnAnyColumn()
        {
            _fixture.Context.Persons.AddRange(persons);

            _fixture.Context.SaveChanges();
            _fixture.Context.DetachEntities();

            var filter = new PersonSearchFilter();

            var searchTerm = "Doe";

            var query = _fixture.Context.Persons;
            var result = query.SearchBy(filter, searchTerm);

            // Avoids flakiness due to database not ensuring
            // the same order of insertion on retrieval
            var orderedResult = result.OrderBy(person => person.FirstName);

            result.Should().SatisfyRespectively(
                first => { first.FirstName.Should().Be("Doe"); },
                second => { second.LastName.Should().Be("Doe"); }
            );
        }

        [Fact]
        public void AbstractSeachFilter_ShouldNot_ModifyQuery_WhenSearchTermIsNull()
        {
            _fixture.Context.Persons.AddRange(persons);

            _fixture.Context.SaveChanges();
            _fixture.Context.DetachEntities();

            var filter = new PersonSearchFilter();

            string searchTerm = null;

            var query = _fixture.Context.Persons;        
            var result = query.SearchBy(filter, searchTerm);

            result.Should().HaveCount(8);
        }

        [Fact]
        public void AbstractSearchFilter_ShouldNot_ModifyQuery_WhenGivenNoSearchRules()
        {
            _fixture.Context.Persons.AddRange(persons);

            _fixture.Context.SaveChanges();
            _fixture.Context.DetachEntities();

            var filter = new EmptySearchFilter();

            var searchTerm = "Mary";

            var query = _fixture.Context.Persons;
            var result = query.SearchBy(filter, searchTerm);

            result.Should().BeEquivalentTo(persons);
        }

        [Fact]
        public void AbstractSearchFilter_ShouldIgnore_NullProperties()
        {
            var clone = persons.ToList();
            clone.Add(new Person { FirstName = null, LastName = null, Age = 30 });

            _fixture.Context.Persons.AddRange(clone);

            _fixture.Context.SaveChanges();
            _fixture.Context.DetachEntities();

            var filter = new PersonSearchFilter();

            var searchTerm = "something";

            var query = _fixture.Context.Persons;
            var result = query.SearchBy(filter, searchTerm);

            result.Should().BeEmpty();
        }
    }
}
