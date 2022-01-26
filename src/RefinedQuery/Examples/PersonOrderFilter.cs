
using RefinedQuery.Ordering;

namespace RefinedQuery.Examples
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
}