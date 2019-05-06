using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LINQTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(ExpressionSample(3, 5));

            WhereWithExpression();

            Console.ReadKey();
        }

        private static int ExpressionSample(int a, int b)
        {
            Func<int, int> square = x => x * x;
            Expression<Func<int, int, int>> add = (x, y) => x + y;

            return add.Compile()(a, b);
        }

        // Ezt helyettesítjük expression-nel:
        // companies.Where(company => (company.ToLower() == "coho winery" || company.Length > 16)).OrderBy(company => company)
        private static void WhereWithExpression()
        {
            string[] companies = { "Consolidated Messenger", "Alpine Ski House", "Southridge Video", "City Power & Light",
                   "Coho Winery", "Wide World Importers", "Graphic Design Institute", "Adventure Works",
                   "Humongous Insurance", "Woodgrove Bank", "Margie's Travel", "Northwind Traders",
                   "Blue Yonder Airlines", "Trey Research", "The Phone Company",
                   "Wingtip Toys", "Lucerne Publishing", "Fourth Coffee" };

            // Azért, hogy használhassunk LINQ-t
            IQueryable<string> queryableData = companies.AsQueryable<string>();

            // létrehozzuk a company változót a lekésérbe
            ParameterExpression pe = Expression.Parameter(typeof(string), "company");

            // ***** Where(company => (company.ToLower() == "coho winery" || company.Length > 16)) *****
            Expression left = Expression.Call(pe, typeof(string).GetMethod("ToLower", Type.EmptyTypes)); // a kifejezés bal oldala
            Expression right = Expression.Constant("coho winery"); // a kifejezés jobb oldala
            Expression e1 = Expression.Equal(left, right); // maga az ellenőrzés

            // következő kifejezés
            left = Expression.Property(pe, typeof(string).GetProperty("Length"));
            right = Expression.Constant(16, typeof(int));
            Expression e2 = Expression.GreaterThan(left, right);

            Expression predicateBody = Expression.OrElse(e1, e2); // két kikötés összefűzése OR-ral

            MethodCallExpression whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { queryableData.ElementType },
                queryableData.Expression,
                Expression.Lambda<Func<string, bool>>(predicateBody, new ParameterExpression[] { pe }));
            // ***** End Where *****

            // ***** OrderBy(company => company) *****
            MethodCallExpression orderByCallExpression = Expression.Call(
                typeof(Queryable),
                "OrderBy",
                new Type[] { queryableData.ElementType, queryableData.ElementType },
                whereCallExpression,
                Expression.Lambda<Func<string, string>>(pe, new ParameterExpression[] { pe }));
            // ***** End OrderBy *****

            IQueryable<string> results = queryableData.Provider.CreateQuery<string>(orderByCallExpression);

            foreach (string company in results)
            {
                Console.WriteLine(company);
            }
        }
    }
}
