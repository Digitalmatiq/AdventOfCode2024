using System.Text.RegularExpressions;

namespace AdventOfCode2024.Day14;

public static class Day14
{
   public static int CountFewestMoves()
   {

      var robots = LoadRobots();

      var robotMap = new Dictionary<Vector, List<Robot>>();
      foreach (var robot in robots)
      {
         if (!robotMap.TryAdd(robot.Location, [robot]) && robotMap.TryGetValue(robot.Location, out var list))
            list.Add(robot);
      }

      var rows = 103;
      var cols = 101;

      var map = LoadMap(rows, cols, robotMap);

      var firstCount = CountStepsUntilCTree(map, robots);
      return firstCount;
   }

   private static int CountStepsUntilCTree(Spot[,] map, List<Robot> robots)
   {
      var rows = 103;
      var cols = 101;

      bool IsInsideBounds(int row, int col) => row >= 0 && col >= 0 && row < rows && col < cols;

      var count = 0;
      while (true)
      {
         Simulate(map, robots);
         count++;

         var symmetries = 0;
         for (var i = 0; i < rows; i++)
         {
            for (var j = 0; j < cols; j++)
            {
               var spot = map[i, j];

               var (iSym, jSym) = (i, cols - j - 1); //Symmetric versus X axis
               if (IsInsideBounds(iSym, jSym))
               {
                  var symetricSpot = map[iSym, jSym];

                  if (spot.RobotsOnSpot > 0 && symetricSpot.RobotsOnSpot > 0)
                     symmetries++;
               }
            }
         }

         if (symmetries > 85)
         {
            PrintMap(map);
            break;
         }
      }

      return count;
   }

   public static int GetSafetyFactor()
   {
      var simulateCount = 100;
      var robots = LoadRobots();

      var robotMap = new Dictionary<Vector, List<Robot>>();
      foreach (var robot in robots)
      {
         if (!robotMap.TryAdd(robot.Location, [robot]) && robotMap.TryGetValue(robot.Location, out var list))
            list.Add(robot);
      }

      var rows = 103;
      var cols = 101;

      var map = LoadMap(rows, cols, robotMap);
      PrintMap(map);

      for (var i = 0; i < simulateCount; i++)
         Simulate(map, robots);

      PrintMap(map);
      var (q1, q2, q3, q4) = CountQuadrants(map);
      var safetyFactor = q1 * q2 * q3 * q4;

      return safetyFactor;
   }

   private static void PrintMap(Spot[,] map)
   {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);

      for (var i = 0; i < rows; i++)
      {
         for (var j = 0; j < cols; j++)
         {
            var count = map[i, j].RobotsOnSpot;
            if (count == 0)
               Console.Write(" . ");
            else
               Console.Write($" {count} ");
         }

         Console.Write("\n");
      }
   }

   private static (int Q1, int Q2, int Q3, int Q4) CountQuadrants(Spot[,] map)
   {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);

      var horizontalCutoff = (rows - 1) / 2;
      var verticalCutoff = (cols - 1) / 2;

      int CountInternal(int fromRow, int toRow, int fromCol, int toCol)
      {
         var robotsCount = 0;
         for (var i = fromRow; i < toRow; i++)
         {
            for (var j = fromCol; j < toCol; j++)
               robotsCount += map[i, j].RobotsOnSpot;
         }

         return robotsCount;
      }

      return (
         CountInternal(0, horizontalCutoff, 0, verticalCutoff),
         CountInternal(0, horizontalCutoff, verticalCutoff + 1, cols),
         CountInternal(horizontalCutoff + 1, rows, 0, verticalCutoff),
         CountInternal(horizontalCutoff + 1, rows, verticalCutoff + 1, cols));
   }

   private static void Simulate(Spot[,] map, List<Robot> robots)
   {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);

      foreach (var robot in robots)
      {
         if (robot.Velocity.X == 0 && robot.Velocity.Y == 0)
            continue;

         map[robot.Location.Y, robot.Location.X].RobotsOnSpot--;
         var newX = Mod(robot.Location.X + robot.Velocity.X, cols);
         var newY = Mod(robot.Location.Y + robot.Velocity.Y, rows);

         robot.Location = new Vector(newX, newY);
         map[robot.Location.Y, robot.Location.X].RobotsOnSpot++;
      }
   }

   static int Mod(int x, int m) => ((x % m) + m) % m;

   private static Spot[,] LoadMap(int rows, int cols, Dictionary<Vector, List<Robot>> robotMap)
   {
      var map = new Spot[rows, cols];

      for (var i = 0; i < rows; i++)
      {
         for (var j = 0; j < cols; j++)
         {
            var location = new Vector(j, i);
            map[i, j] = new Spot(location);

            if (robotMap.TryGetValue(location, out var robots))
               map[i, j].RobotsOnSpot = robots.Count;
         }
      }

      return map;
   }

   private static List<Robot> LoadRobots()
   {
      var lines = File.ReadAllLines(@"Day14\day14.txt");

      var robots = new List<Robot>();
      var regex = new Regex(@"-?\d+");

      foreach (var line in lines)
      {
         var allMatches = regex.Matches(line);
         var xPos = int.Parse(allMatches[0].Value);
         var yPos = int.Parse(allMatches[1].Value);
         var xVel = int.Parse(allMatches[2].Value);
         var yVel = int.Parse(allMatches[3].Value);

         var robot = new Robot(new Vector(xPos, yPos), new Vector(xVel, yVel));
         robots.Add(robot);
      }

      return robots;
   }

   private readonly record struct Vector(int X, int Y);

   private sealed class Robot(Vector location, Vector velocity)
   {
      public Vector Location { get; set; } = location;

      public Vector Velocity { get; set; } = velocity;
   }

   private sealed record Spot(Vector Location)
   {
      public int RobotsOnSpot { get; set; }
   }
}
