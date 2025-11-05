using ECommerceSystem.Core.Entities;
using ECommerceSystem.Core.Specifications;
using ECommerceSystem.Application.Queries;
using System.Linq.Expressions;

namespace ECommerceSystem.Application.Specifications;

public class OrderSpecification : BaseSpecification<Order>
{
    public OrderSpecification(GetOrdersByUserQuery query)
    {
        // Criteria
        Expression<Func<Order, bool>>? criteria = null;

        // Always filter by UserId
        criteria = o => o.UserId == query.UserId;

        if (query.Status.HasValue)
        {
            Expression<Func<Order, bool>> statusCriteria = o => o.Status == query.Status.Value;
            criteria = CombineExpressions(criteria, statusCriteria);
        }

        if (query.FromDate.HasValue)
        {
            Expression<Func<Order, bool>> fromDateCriteria = o => o.OrderDate >= query.FromDate.Value;
            criteria = CombineExpressions(criteria, fromDateCriteria);
        }

        if (query.ToDate.HasValue)
        {
            Expression<Func<Order, bool>> toDateCriteria = o => o.OrderDate <= query.ToDate.Value;
            criteria = CombineExpressions(criteria, toDateCriteria);
        }

        if (criteria != null)
        {
            Criteria = criteria;
        }

        // Includes
        AddInclude(o => o.OrderItems);
        AddInclude("OrderItems.Product");

        // Ordering
        if (!string.IsNullOrEmpty(query.SortBy))
        {
            switch (query.SortBy.ToLower())
            {
                case "orderdate":
                    if (query.SortDescending)
                        OrderByDescending = o => o.OrderDate;
                    else
                        OrderBy = o => o.OrderDate;
                    break;
                case "totalamount":
                    if (query.SortDescending)
                        OrderByDescending = o => o.TotalAmount;
                    else
                        OrderBy = o => o.TotalAmount;
                    break;
                default:
                    OrderByDescending = o => o.OrderDate;
                    break;
            }
        }
        else
        {
            OrderByDescending = o => o.OrderDate;
        }
    }

    private Expression<Func<Order, bool>> CombineExpressions(Expression<Func<Order, bool>> expr1, Expression<Func<Order, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(Order));
        var body = Expression.AndAlso(
            Expression.Invoke(expr1, parameter),
            Expression.Invoke(expr2, parameter)
        );
        return Expression.Lambda<Func<Order, bool>>(body, parameter);
    }
}
