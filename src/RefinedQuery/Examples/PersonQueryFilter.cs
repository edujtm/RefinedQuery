
using RefinedQuery.Query;

namespace RefinedQuery.Examples
{
    public class PersonQueryFilter : AbstractQueryFilter<Person, PersonQuery>
    {
        public PersonQueryFilter()
        {
            OnQuery((q) => q.Name)
              .IgnoreWhen((query) => string.IsNullOrWhiteSpace(query))
              .SearchFor((p) => p.FirstName)
              .ThatMatches((name, query) => name.ToLower().Contains(query.ToLower()));

            OnQuery((q) => q.Age)
              .InvalidWhen((age) => age <= 0)
              .SearchFor((p) => p.Age)
              .ThatMatches((age, query) => age == query);
        }
    }
}
