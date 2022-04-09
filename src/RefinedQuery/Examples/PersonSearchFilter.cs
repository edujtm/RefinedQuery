
using RefinedQuery.Searching;


namespace RefinedQuery.Examples
{
    public class PersonSearchFilter : AbstractSearchFilter<Person>
    {
        public PersonSearchFilter()
        {
            SearchFor(person => person.FirstName)
                .ThatMatches((name, searchTerm) => name.ToLower().Contains(searchTerm.ToLower()));

            SearchFor(person => person.Age)
                .ThatMatches((age, searchTerm) => age.ToString() == searchTerm);
        }
    }
}
