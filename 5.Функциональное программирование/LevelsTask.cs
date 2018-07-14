// Вставьте сюда финальное содержимое файла LevelsTask.cs
using System;
using System.Collections.Generic;

namespace func_rocket
{
	public class LevelsTask
	{
		static readonly Physics standardPhysics = new Physics();

		public static IEnumerable<Level> CreateLevels()
		{
			yield return new Level("Zero", 
				new Rocket(new Vector(200, 500), Vector.Zero, -0.5 * Math.PI),
				new Vector(600, 200), 
				(size, v) => Vector.Zero, standardPhysics);
            yield return new Level("Heavy",
                new Rocket(new Vector(200, 500), Vector.Zero, -0.5 * Math.PI),
                new Vector(600, 200),
                (size, v) => new Vector(0, 0.9), standardPhysics);
            yield return new Level("Up",
                new Rocket(new Vector(200, 500), Vector.Zero, -0.5 * Math.PI),
                new Vector(700, 500),
                (size, v) => new Vector(0, -300 / (size.Height - v.Y  +300)),
                standardPhysics);
            yield return new Level("WhiteHole",
                new Rocket(new Vector(200, 500), Vector.Zero, -0.5 * Math.PI),
                new Vector(600, 200),
                (size, v) =>
                {
                    var toWhiteHole = new Vector(v.X - 600, v.Y - 200);
                    return toWhiteHole.Normalize() * 140 * toWhiteHole.Length 
                    / (toWhiteHole.Length * toWhiteHole.Length + 1);
                },
                standardPhysics);
            yield return new Level("BlackHole",
                new Rocket(new Vector(200, 500), Vector.Zero, -0.5 * Math.PI),
                new Vector(600, 200),
                (size, v) =>
                {
                    var blackHolePosition = new Vector((600 + 200) / 2 , (500 + 200) / 2);
                    var toBlackHole = blackHolePosition - v;
                    return new Vector(blackHolePosition.X - v.X, blackHolePosition.Y - v.Y).Normalize() *
                        300 * toBlackHole.Length / (toBlackHole.Length * toBlackHole.Length + 1);
                }
                , standardPhysics);
            yield return new Level("BlackAndWhite",
                new Rocket(new Vector(200, 500), Vector.Zero, -0.5 * Math.PI),
                new Vector(600, 200),
                (size, v) =>
                {
                    var toWhiteHole = new Vector(v.X - 600, v.Y - 200);
                    var blackHolePosition = new Vector((600 + 200) / 2, (500 + 200) / 2);
                    var toBlackHole = blackHolePosition - v;
                    return (toWhiteHole.Normalize() * 140 * toWhiteHole.Length 
                    / (toWhiteHole.Length * toWhiteHole.Length + 1) +
                    new Vector(blackHolePosition.X - v.X, blackHolePosition.Y - v.Y).Normalize() *
                        300 * toBlackHole.Length / (toBlackHole.Length * toBlackHole.Length + 1)) / 2;
                }
                , standardPhysics);
        }
	}
}