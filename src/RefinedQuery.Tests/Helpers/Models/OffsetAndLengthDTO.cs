
using System.ComponentModel.DataAnnotations;

namespace RefinedQuery.Tests.Helpers.Models
{
    public class OffsetAndLengthDTO
    {
        public int? Offset { get; set; }

        public int? PageSize { get; set; }
    }
}