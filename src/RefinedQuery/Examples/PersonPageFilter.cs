using RefinedQuery.Pagination;


namespace RefinedQuery.Examples
{
    public class PersonPageFilter : AbstractPageFilter<Person, PersonQuery>
    {
        public PersonPageFilter()
        {
            WithPagination(
                PagingBy.OffsetAndLength(
                    offset: query => query.Offset,
                    length: query => query.Length
                )
            );
        }
    }
}