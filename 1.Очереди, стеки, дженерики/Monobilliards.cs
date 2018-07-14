// Вставьте сюда финальное содержимое файла Monobilliards.cs
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Monobilliards
{
    public class Monobilliards : IMonobilliards
    {
        public bool IsCheater(IList<int> inspectedBalls)
        {
            var stack = new Stack<int>();
            var removedBalls = new HashSet<int>();
            int peek = 0;
            
            foreach (var ball in inspectedBalls)
            {
                if (stack.Count == 0)
                {
                    for (int i = 1; i < ball; i++)
                        stack.Push(i);
                }

                else
                {
                    if (ball == stack.First()) stack.Pop();

                    else if (ball > stack.First())
                    {
                        if (peek < stack.First()) peek = stack.First();

                        for (int i = peek + 1; i < ball; i++)

                            if (!removedBalls.Contains(i)) stack.Push(i);
                            else removedBalls.Remove(i);

                        peek = ball;
                    }
                    else return true;
                }
                removedBalls.Add(ball);
            }
            return false;
        }
    }
}