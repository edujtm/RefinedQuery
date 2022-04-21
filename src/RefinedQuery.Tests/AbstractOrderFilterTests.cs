using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;
using FluentAssertions;

using RefinedQuery.Linq;
using RefinedQuery.Exceptions;
using RefinedQuery.Ordering;
using RefinedQuery.Tests.Helpers;
using RefinedQuery.Tests.Helpers.Models;
using RefinedQuery.Tests.Helpers.Xunit;

namespace RefinedQuery.Tests
{

    public class PersonOrderFilter : AbstractOrderFilter<Person>
    {
        public PersonOrderFilter()
        {
            OnField("name")
                .OrderBy(person => person.FirstName);

            OnField("age")
                .OrderBy(person => person.Age);
        }
    }

    [Collection("DB")]
    public class AbstractOrderFilterTests : RespawnedTest
    {
        private readonly ICollection<Person> _persons = Fake.Persons();
        private readonly PersonOrderFilter _orderFilter = new PersonOrderFilter();

        private readonly DbContextFixture _fixture;


        public AbstractOrderFilterTests(DbContextFixture fixture)
            : base(fixture) 
        {
            _fixture = fixture;

            _fixture.Context.Persons.AddRange(_persons);

            _fixture.Context.SaveChanges();
            _fixture.Context.DetachEntities();
        }

        [Fact]
        public void Should_Not_Order_If_No_Values_Given_In_Ordering_List()
        {
            var query = _fixture.Context.Persons;

            // Given: no ordering fields were given.
            var ordering = new List<string>();

            var resultQueryable = query.OrderBy(_orderFilter, ordering);

            // Then: the original query is not affected.
            resultQueryable.IsOrdered().Should().BeFalse();
            // Then: the original query is not modified.
            resultQueryable.Expression.Should().Be(query.AsQueryable().Expression);
        }

        [Fact]
        public void Should_Not_Change_IQueryable_If_Ordering_List_Is_Null()
        {
            var query = _fixture.Context.Persons;

            // Given: ordering field list is null 
            // When: the ordering list is null
            var resultQueryable = query.OrderBy(_orderFilter, null);

            // Then: the original query is not affected.
            resultQueryable.IsOrdered().Should().BeFalse();
            // Then: the original query is not modified.
            resultQueryable.Expression.Should().Be(query.AsQueryable().Expression);
        }

        [Fact]
        public void Should_Order_Values_When_Given_Correct_Ordering_Fields()
        {
            var query = _fixture.Context.Persons;

            // Given: ordering fields are given in the correct format
            var ordering = new List<string> { "name:asc" };

            // When: the query is ordered using the fields
            var resultQueryable = query.OrderBy(_orderFilter, ordering);

            // Then: the original query is ordered by the first name, according to the configuration.
            resultQueryable.ToList().Should().BeInAscendingOrder(person => person.FirstName);
        }

        [Fact]
        public void Should_Order_Values_When_Given_Multiple_Correct_Ordering_Fields()
        {
            var query = _fixture.Context.Persons;

            // Given: multiple ordering fields are given in the correct format
            var ordering = new List<string> { "name:asc", "age:desc" };

            // When: the query is ordered using the fields
            var resultQueryable = query.OrderBy(_orderFilter, ordering);

            var expectedOrdering = query.OrderBy(person => person.FirstName)
                                        .ThenBy(person => person.Age)
                                        .ToList();

            // Then: the original query is ordered by the first name and then by age, 
            // according to the configuration.
            resultQueryable.ToList().Should().BeEquivalentTo(expectedOrdering);
        }

        [Theory]
        [MemberData(nameof(InvalidOrderFields))]
        public void Should_Raise_Error_On_Invalid_Ordering_Field(List<string> invalidOrdering)
        {
            var query = _fixture.Context.Persons;

            // Given: an invalid ordering
            // When: the query is ordered using the fields
            Action action = () => query.OrderBy(_orderFilter, invalidOrdering);

            action.Should().Throw<InvalidQueryFieldException>();
        }


        public static IEnumerable<object[]> InvalidOrderFields()
        {
            yield return new object[] { new List<string>() { "name:asc", ":desc" } };
            yield return new object[] { new List<string>() { "asc", "age:desc" } };
            yield return new object[] { new List<string>() { "", "age:desc" } };
            yield return new object[] { new List<string>() { "name:desc", "age" } };
            yield return new object[] { new List<string>() { "nam:asc" } };
            yield return new object[] { new List<string>() { "name:ascii" } };
        }
    }
}