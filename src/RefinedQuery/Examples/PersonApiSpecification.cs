
using RefinedQuery.Specification;

namespace RefinedQuery.Examples
{
    public class PersonApiSpecification : ApiSpecification<Person, PersonQuery>
    {
        public PersonApiSpecification()
        {

            // Configures query string for querying (eg. /route?name="eduardo"&age=20)
            UseQuery((spec) => {

                spec.OnQuery(person => person.Name)
                    .IgnoreWhen((query) => string.IsNullOrWhiteSpace(query))
                    .SearchFor((p) => p.FirstName)
                    .ThatMatches((name, query) => name.ToLower().Contains(query.ToLower()));

                spec.OnQuery(person => person.Age)
                    .InvalidWhen((age) => age <= 0)
                    .SearchFor((p) => p.Age)
                    .ThatMatches((age, query) => age == query);
            });

            // Configures query string for searching (eg. /route?search="joao")
            UseSearch(query => query.Search, (spec) => {
                spec.SearchFor(person => person.FirstName)
                    .ThatMatches((name, searchTerm) => name.ToLower().Contains(searchTerm.ToLower()));

                spec.SearchFor(person => person.Age)
                    .ThatMatches((age, searchTerm) => age.ToString() == searchTerm);
            });

            // Configures query string for ordering (e.g. /route?orderBy="name:asc"&orderBy="age:desc")
            UseOrdering(query => query.OrderBy, (spec) => {

                spec.OnField("name")
                    .OrderBy(person => person.FirstName);

                spec.OnField("age")
                    .OrderBy(person => person.Age);
            });

            // Configures query for pagination (e.g. /route?offset=20&length=10)
            UsePagination((spec) => {
                spec.WithPagination(
                    spec.PagingBy.OffsetAndLength(
                        offset: query => query.Offset,
                        length: query => query.Length
                    )
                );
            });
        }
    }
}