// Вставьте сюда финальное содержимое файла FlagsTask.cs
namespace Flags
{
    public static class FlagsTask
    {
        public static long Solve(int a)
        {
            long prev = 1, current = 1, result = 1;
            for (long i = 2; i < a; i++)
            {
                result = prev + current;
                prev = current;
                current = result;
            }
            return result * 2;
        }
    }
}