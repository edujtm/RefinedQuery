
namespace RefinedQuery.Ordering
{
    public interface IOrderFieldHandler
    {
        (string Field, OrderDef Order) GetOrderFor(string queryField);
    }
}