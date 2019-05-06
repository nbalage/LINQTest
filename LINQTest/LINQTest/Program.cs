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

            Console.ReadKey();
        }

        private static int ExpressionSample(int a, int b)
        {
            Func<int, int> square = x => x * x;
            Expression<Func<int, int, int>> add = (x, y) => x + y;

            return add.Compile()(a, b);
        }
    }
}
