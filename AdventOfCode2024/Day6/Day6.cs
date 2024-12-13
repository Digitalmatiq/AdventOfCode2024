using System.Buffers;
using System.Text;

namespace AdventOfCode2024.Day6;

public static class Day6
{
   public static int CountObstructions()
   {
      var (matrix, text) = LoadMatrix();
      var length = Math.Sqrt(matrix.Length);

      bool HasCycleInternal(char[,] matrix, Index index, Orientation orientation, HashSet<int> cycleHashes)
      {
         var (row, col) = index;
         var currentPosition = matrix[row, col];
         var (lastRow, lastCol) = (row, col);

         while (currentPosition != '#')
         {
            (lastRow, lastCol) = (row, col);

            switch (orientation)
            {
               case Orientation.Up:
                  row -= 1;
                  break;
               case Orientation.Right:
                  col += 1;
                  break;
               case Orientation.Down:
                  row += 1;
                  break;
               case Orientation.Left:
                  col -= 1;
                  break;
            }

            if (row < 0 || row >= length || col < 0 || col >= length)
               return false;

            currentPosition = matrix[row, col];
         }

         var hash = HashCode.Combine(index, orientation);
         if (cycleHashes.Contains(hash))
            return true;

         cycleHashes.Add(hash);
         var newOrdientation = (int)(++orientation) % 4;
         return HasCycleInternal(matrix, new Index(lastRow, lastCol), (Orientation)newOrdientation, cycleHashes);
      }

      var sv = SearchValues.Create('^');
      var pos = text.AsSpan().IndexOfAny(sv);
      var row = pos / (int)length;
      var col = pos % (int)length;

      var count = 0;
      for (var i = 0; i < length; i++)
      {
         for (var j = 0; j < length; j++)
         {
            var value = matrix[i, j];
            if (value == '#' || value == '^')
               continue;

            var temp = matrix[i, j];
            matrix[i, j] = '#';
            if (HasCycleInternal(matrix, new Index(row, col), Orientation.Up, []))
               count++;

            matrix[i, j] = temp;
         }
      }

      return count;
   }

   public static int CountPositions()
   {
      var (matrix, text) = LoadMatrix();

      var length = Math.Sqrt(matrix.Length);
      var visited = new HashSet<Index>();

      void TracePositionsInternal(char[,] matrix, Index index, Orientation orientation)
      {
         var (row, col) = index;
         var currentPosition = matrix[row, col];
         var (lastRow, lastCol) = (row, col);

         while (currentPosition != '#')
         {
            (lastRow, lastCol) = (row, col);
            visited.Add(new(row, col));

            switch (orientation)
            {
               case Orientation.Up:
                  row -= 1;
                  break;
               case Orientation.Right:
                  col += 1;
                  break;
               case Orientation.Down:
                  row += 1;
                  break;
               case Orientation.Left:
                  col -= 1;
                  break;
            }

            if (row < 0 || row >= length || col < 0 || col >= length)
               return;

            currentPosition = matrix[row, col];
         }

         var newOrdientation = (int)(++orientation) % 4;
         TracePositionsInternal(matrix, new Index(lastRow, lastCol), (Orientation)newOrdientation);
      }

      var sv = SearchValues.Create('^');
      var pos = text.AsSpan().IndexOfAny(sv);
      var row = pos / (int)length;
      var col = pos % (int)length;

      TracePositionsInternal(matrix, new Index(row, col), Orientation.Up);
      return visited.Count;
   }

   private readonly record struct Index(int Row, int Col);

   private enum Orientation
   {
      Up,
      Right,
      Down,
      Left
   }

   private static (char[,] Matrix, string FlatText) LoadMatrix()
   {
      var lines = File.ReadAllLines(@"Day6\day6.txt");
      var length = lines.Length;

      var matrix = new char[length, length];

      foreach (var (row, line) in lines.Index())
      {
         foreach (var (col, @char) in line.Index())
            matrix[row, col] = @char;
      }

      var builder = new StringBuilder();

      foreach (var line in lines)
      {
         builder.Append(line
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .Trim());
      }

      var flatText = builder.ToString();

      return (matrix, flatText);
   }
}
