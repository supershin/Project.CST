using System.Linq.Expressions;

namespace Project.ConstructionTracking.Web.Commons
{
    public static class QueryExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
   (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IQueryable<T> Page<T, TResult>(this IQueryable<T> query,
                        int pageNum, int pageSize,
                        Expression<Func<T, TResult>> defaultSortingExpression,
                        string sortField,
                        bool isAscendingOrder, out int rowsCount, bool isOrder = true)
        {

            rowsCount = query.Count();

            if (isOrder)
            {
                // defaultSort
                if (isAscendingOrder)
                    query = query.OrderBy(defaultSortingExpression);
                else
                    query = query.OrderByDescending(defaultSortingExpression);

                //optionSort
                if (!string.IsNullOrEmpty(sortField))
                {
                    query = query.OrderBy(sortField, isAscendingOrder);

                }

            }

            if (pageSize <= 0)
            {
                return query;
            }

            if (rowsCount <= pageSize || pageNum <= 0) pageNum = 0;
            if (pageNum == 1)
            {
                pageNum = 0;
            }

            // int excludedRows = (pageNum - 1) * pageSize;
            return query.Skip(pageNum).Take(pageSize);

        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderPair, bool asc)
        {


            IQueryable<T> returnValue = null;

            string methodName = asc ? "OrderBy" : "OrderByDescending";
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "p");
            string propertyName = (orderPair.Split(' ')[0]).Trim();
            System.Reflection.PropertyInfo property;
            MemberExpression propertyAccess;

            if (propertyName.Contains('.'))
            {
                // support to be sorted on child fields. 
                String[] childProperties = propertyName.Split('.');
                property = typeof(T).GetProperty(childProperties[0]);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);

                for (int i = 1; i < childProperties.Length; i++)
                {
                    Type t = property.PropertyType;
                    if (!t.IsGenericType)
                    {
                        property = t.GetProperty(childProperties[i]);
                    }
                    else
                    {
                        property = t.GetGenericArguments().First().GetProperty(childProperties[i]);
                    }

                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }
            }
            else
            {
                property = type.GetProperty(propertyName);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
            }

            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), methodName, new Type[] { type, property.PropertyType },
            source.Expression, Expression.Quote(orderByExpression));
            returnValue = source.Provider.CreateQuery<T>(resultExpression);
            return returnValue;

        }
    }
}
