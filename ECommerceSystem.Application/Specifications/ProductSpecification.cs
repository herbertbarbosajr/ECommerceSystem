using ECommerceSystem.Core.Entities;
using ECommerceSystem.Core.Specifications;
using ECommerceSystem.Application.Queries;
using System.Linq.Expressions;

namespace ECommerceSystem.Application.Specifications;

public class ProductSpecification : BaseSpecification<Product>
{
    public ProductSpecification(GetProductsQuery query)
    {
        // Criteria
        Expression<Func<Product, bool>>? criteria = null;

        if (query.OnlyActive)
        {
            criteria = p => p.IsActive;
        }

        if (!string.IsNullOrEmpty(query.Search))
        {
            Expression<Func<Product, bool>> searchCriteria = p => p.Name.Contains(query.Search) || p.Description.Contains(query.Search);
            criteria = criteria == null ? searchCriteria : CombineExpressions(criteria, searchCriteria);
        }

        if (query.MinPrice.HasValue)
        {
            Expression<Func<Product, bool>> priceCriteria = p => p.Price >= query.MinPrice.Value;
            criteria = criteria == null ? priceCriteria : CombineExpressions(criteria, priceCriteria);
        }

        if (query.MaxPrice.HasValue)
        {
            Expression<Func<Product, bool>> maxPriceCriteria = p => p.Price <= query.MaxPrice.Value;
            criteria = criteria == null ? maxPriceCriteria : CombineExpressions(criteria, maxPriceCriteria);
        }

        if (criteria != null)
        {
            Criteria = criteria;
        }

        // Ordering
        if (!string.IsNullOrEmpty(query.SortBy))
        {
            switch (query.SortBy.ToLower())
            {
                case "name":
                    if (query.SortDescending)
                        OrderByDescending = p => p.Name;
                    else
                        OrderBy = p => p.Name;
                    break;
                case "price":
                    if (query.SortDescending)
                        OrderByDescending = p => p.Price;
                    else
                        OrderBy = p => p.Price;
                    break;
                case "createdat":
                    if (query.SortDescending)
                        OrderByDescending = p => p.CreatedAt;
                    else
                        OrderBy = p => p.CreatedAt;
                    break;
                default:
                    OrderBy = p => p.Name;
                    break;
            }
        }
        else
        {
            OrderBy = p => p.Name;
        }
    }

    private Expression<Func<Product, bool>> CombineExpressions(Expression<Func<Product, bool>> expr1, Expression<Func<Product, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(Product));
        var body = Expression.AndAlso(
            Expression.Invoke(expr1, parameter),
            Expression.Invoke(expr2, parameter)
        );
        return Expression.Lambda<Func<Product, bool>>(body, parameter);
    }
}
