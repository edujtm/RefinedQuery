using System.Collections.Generic;

namespace RefinedQuery.Examples
{
    public class PersonQuery
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public int Offset { get; set; }

        public int Length { get; set; }

        public string Search { get; set; }
        public IEnumerable<string> OrderBy { get; set; }
    }
}
