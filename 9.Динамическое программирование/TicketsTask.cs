// Вставьте сюда финальное содержимое файла TicketsTask.cs
using System.Numerics;
 
namespace Tickets
{
    internal class TicketsTask
    {
        private const int MaxLen = 100;
        private const int MaxSum = 2000;
 
        public static BigInteger Solve(int halfLen, int totalSum)
        {
            if (totalSum % 2 != 0)
                return 0;
 
            var happyTickets = InitializeHappyTicketsContainer();
           
            var halfResult = CountHappyTickets(happyTickets, halfLen, totalSum / 2);
            return halfResult * halfResult;
        }
 
        private static BigInteger[,] InitializeHappyTicketsContainer()
        {
            var happyTickets = new BigInteger[MaxLen + 1, MaxSum + 1];
 
            for (var i = 0; i < MaxLen; i++)
            for (var j = 0; j < MaxSum; j++)
            {
                happyTickets[i, j] = -1;
            }
 
            return happyTickets;
        }
 
        private static BigInteger CountHappyTickets(BigInteger[,] happyTickets, int len, int sum)
        {
            if (happyTickets[len, sum] >= 0) return happyTickets[len, sum];
            if (sum == 0) return 1;
            if (len == 0) return 0;
 
            happyTickets[len, sum] = 0;
            for (var i = 0; i < 10; i++)
            {
                if (sum - i >= 0)
                {
                    happyTickets[len, sum] += CountHappyTickets(happyTickets, len - 1, sum - i);
                }              
            }
 
            return happyTickets[len, sum];
        }
    }
}