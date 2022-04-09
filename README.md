

## RefinedQuery

Allows for transformation of an IQueryable based on a given URL query string.

## Example

Given an entity object and a corresponding query object (i.e. DTO).

```cs
public class Person
{
    public string FirstName { get; set; }

    public int Age { get; set; }
}

public class PersonDTO
{
    public string Name { get; set; }

    public int Age { get; set; }

    public string Search { get; set; }

    public IEnumerable<string> OrderBy { get; set; }

    public int Offset { get; set; }

    public int Length { get; set; }
}
```

It's possible to define a class that specifies how queries will be applied
to an IQueryable collection.

```cs
public class PersonQueryHandler : QueryHandler<Person, PersonDTO>
{
    public PersonQueryHandler()
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
                PagingBy.OffsetAndLength(
                    offset: query => query.Offset,
                    length: query => query.Length
                )
            );
        });
    }
}
```

And then use it to manipulate the IQueryable.

```cs
public class Sample {
  private readonly PersonQueryHandler queryHandler = new PersonQueryHandler();

  public void Query(PersonDTO dto)
  {
      IQueryable<Person> persons = \* Create IQueryable *\

      var queryResult = persons
        .QueryBy(queryHandler, dto)
        .SearchBy(queryHandler, dto.Search)
        .SortBy(queryHandler, dto.OrderBy)
        .PageBy(queryHandler, dto)
        .ToList()
      
      // Or alternatively

      var queryResult = persons.Apply(queryHandler, dto)

      return queryResult;
  }
}
```