namespace AdventOfCode2024.Day10;

public static class Day10
{
   public static (int, int) GetTrailheadValueSumAndRatingSum()
   {
      var matrix = LoadMatrix();

      var length = matrix.GetLength(0);
      var trailPairings = new Dictionary<Index, HashSet<Index>>();

      for (var i = 0; i < length; i++)
      {
         for (var j = 0; j < length; j++)
         {
            if (matrix[i, j] == 0)
               trailPairings.Add(new Index(i, j), []);
         }
      }

      var distinctTrails = 0;

      void CountTrailsInternal(Index current, int value, HashSet<Index> endpoints)
      {
         static bool IsWithinBounds(Index index, int length) => index.Row >= 0 && index.Row < length && index.Col >= 0 && index.Col < length;

         if (value == 9)
         {
            endpoints.Add(current);
            distinctTrails++;
            return;
         }

         var length = matrix.GetLength(0);
         var nextValue = value + 1;

         var right = new Index(current.Row, current.Col + 1);
         if (IsWithinBounds(right, length) && matrix[right.Row, right.Col] == nextValue)
            CountTrailsInternal(right, nextValue, endpoints);

         var left = new Index(current.Row, current.Col - 1);
         if (IsWithinBounds(left, length) && matrix[left.Row, left.Col] == nextValue)
            CountTrailsInternal(left, nextValue, endpoints);

         var up = new Index(current.Row - 1, current.Col);
         if (IsWithinBounds(up, length) && matrix[up.Row, up.Col] == nextValue)
            CountTrailsInternal(up, nextValue, endpoints);

         var down = new Index(current.Row + 1, current.Col);
         if (IsWithinBounds(down, length) && matrix[down.Row, down.Col] == nextValue)
            CountTrailsInternal(down, nextValue, endpoints);
      }

      foreach (var (trailhead, endpoints) in trailPairings)
         CountTrailsInternal(trailhead, 0, endpoints);

      var valueSum = trailPairings.Values.Sum(x => x.Count);
      return (valueSum, distinctTrails);
   }

   private readonly record struct Index(int Row, int Col);

   private static int[,] LoadMatrix()
   {
      var lines = File.ReadAllLines(@"Day10\day10.txt");

      var matrix = new int[lines.Length, lines.Length];
      foreach (var (row, line) in lines.Index())
      {
         foreach (var (col, @int) in line.Index())
            matrix[row, col] = @int - '0';
      }

      return matrix;
   }
}
