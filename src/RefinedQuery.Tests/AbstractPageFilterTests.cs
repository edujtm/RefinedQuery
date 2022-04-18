using System.Collections.Generic;

using Xunit;
using FluentAssertions;

using RefinedQuery.Pagination;
using RefinedQuery.Linq;
using RefinedQuery.Tests.Helpers;
using RefinedQuery.Tests.Helpers.Xunit;
using RefinedQuery.Tests.Helpers.Models;

namespace RefinedQuery.Tests
{
    public class OffsetAndLengthPageFilter : AbstractPageFilter<Person, OffsetAndLengthDTO>
    {
        public OffsetAndLengthPageFilter()
        {
            WithPagination(
                PagingBy.OffsetAndLength(
                    offset: query => query.Offset,
                    length: query => query.PageSize
                )
            );
        }
    }

    public class PageAndSizePageFilter : AbstractPageFilter<Person, PageAndSizeDTO>
    {
        public PageAndSizePageFilter()
        {
            WithPagination(
                PagingBy.PageAndSize(
                    page: query => query.Page,
                    size: query => query.PageSize
                )
            );
        }
    }

    [Collection("DB")]
    public class AbstractPageFilterTests : RespawnedTest
    {

        private readonly OffsetAndLengthPageFilter olPageFilter = new(); 
        private readonly PageAndSizePageFilter psPageFilter = new(); 

        ICollection<Person> _persons = Fake.Persons();

        private readonly DbContextFixture _fixture;

        public AbstractPageFilterTests(DbContextFixture fixture)
            : base(fixture)
        {
            _fixture = fixture;
        }

        #region OFFSET_AND_LENGTH

        [Fact]
        public void Should_Use_Default_Values_For_Offset_Length_If_Not_Given()
        {

            _fixture.Context.Persons.AddRange(_persons);

            _fixture.Context.SaveChanges();
            _fixture.Context.DetachEntities();

            // Given: query does not specify offset and length.
            var query = new OffsetAndLengthDTO()
            {
                Offset = null,
                PageSize = null
            };

            var data = _fixture.Context.Persons;
            var page = data.PaginateBy(olPageFilter, query);

            page.Should().HaveCount(8);
        }

        [Theory]
        [InlineData(0, 10, 8)]      // Returns the entire collection
        [InlineData(0, 2, 2)]       // Returns the full page from the beginning
        [InlineData(5, 2, 2)]       // Has 8 itens, so returns the full page size.
        [InlineData(5, 5, 3)]       // Only has 8 itens, so it returns 3
        [InlineData(null, 5, 5)]    // Defaults to 0 if offset is not given
        [InlineData(0, null, 8)]    // Defaults to 20 if length is not given
        public void Offset_Length_Pagination_Should_Return_Correct_Quantity_In_Page(
            int? offset, 
            int? pageSize, 
            int expectedSize
        )
        {
            _fixture.Context.Persons.AddRange(_persons);

            _fixture.Context.SaveChanges();
            _fixture.Context.DetachEntities();

            // Given: query does not specify offset and length.
            var query = new OffsetAndLengthDTO()
            {
                Offset = offset,
                PageSize = pageSize
            };

            var data = _fixture.Context.Persons;
            var page = data.PaginateBy(olPageFilter, query);

            page.Should().HaveCount(expectedSize);
        }

        #endregion

        #region PAGE_AND_SIZE

        [Fact]
        public void Should_Use_Default_Values_For_Page_And_Size_If_Not_Given()
        {
            _fixture.Context.Persons.AddRange(_persons);

            _fixture.Context.SaveChanges();
            _fixture.Context.DetachEntities();

            // Given: query does not specify page and page size.
            var query = new PageAndSizeDTO()
            {
                Page = null,
                PageSize = null
            };

            var data = _fixture.Context.Persons;
            var page = data.PaginateBy(psPageFilter, query);

            // Then: query will use the default values (page: 0, page size: 20)
            page.Should().HaveCount(8);
        }

        [Theory]
        [InlineData(0, 10, 8)]      // Returns the entire collection
        [InlineData(0, 2, 2)]       // Returns the full page from the beginning
        [InlineData(1, 5, 3)]       // Has 8 itens. Returns the last 3 after skipping first page
        [InlineData(2, 5, 0)]       // Has 8 itens. Returns no itens since it skipped entire collection
        public void Page_PageSize_Pagination_Should_Return_Correct_Quantity_In_Page(
            int? page, 
            int? pageSize, 
            int expectedSize
        )
        {
            _fixture.Context.Persons.AddRange(_persons);

            _fixture.Context.SaveChanges();
            _fixture.Context.DetachEntities();

            // Given: query does not specify offset and length.
            var query = new PageAndSizeDTO()
            {
                Page = page,
                PageSize = pageSize
            };

            var data = _fixture.Context.Persons;
            var paginatedResponse = data.PaginateBy(psPageFilter, query);

            paginatedResponse.Should().HaveCount(expectedSize);
        }

        #endregion
    }
}