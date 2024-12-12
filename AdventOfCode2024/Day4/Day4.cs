namespace AdventOfCode2024.Day4;

public static class Day4
{
   public static int GetX_MASOccurences()
   {
      var matrix = LoadMatrix();
      var length = Math.Sqrt(matrix.Length);

      var xmasOccurences = 0;
      for (var row = 0; row < length; row++)
      {
         for (var col = 0; col < length; col++)
         {
            if (matrix[row, col] == 'A')
            {
               if (IsC1XMAS(matrix, row, col) || IsC2XMAS(matrix, row, col) || IsC3XMAS(matrix, row, col) || IsC4XMAS(matrix, row, col))
                  xmasOccurences++;
            }
         }
      }

      return xmasOccurences;
   }

   private static char[,] LoadMatrix()
   {
      var lines = File.ReadAllLines(@"Day4\day4.txt");
      var length = lines.Length;

      var matrix = new char[length, length];
      foreach (var (row, line) in lines.Index())
      {
         foreach (var (col, @char) in line.Index())
            matrix[row, col] = @char;
      }

      return matrix;
   }

   public static int GetXMASOccurences()
   {
      var matrix = LoadMatrix();
      var length = Math.Sqrt(matrix.Length);

      var xmasOccurences = 0;
      for (var row = 0; row < length; row++)
      {
         for (var col = 0; col < length; col++)
         {
            if (matrix[row, col] == 'X')
            {
               if (IsXMASToTheLeft(matrix, row, col))
                  xmasOccurences++;

               if (IsXMASToTheRight(matrix, row, col))
                  xmasOccurences++;

               if (IsXMASToTheUp(matrix, row, col))
                  xmasOccurences++;

               if (IsXMASToTheDown(matrix, row, col))
                  xmasOccurences++;

               if (IsXMASOnLeftDownDiagonal(matrix, row, col))
                  xmasOccurences++;

               if (IsXMASOnLeftUpDiagonal(matrix, row, col))
                  xmasOccurences++;

               if (IsXMASOnRightDownDiagonal(matrix, row, col))
                  xmasOccurences++;

               if (IsXMASOnRightUpDiagonal(matrix, row, col))
                  xmasOccurences++;
            }
         }
      }

      return xmasOccurences;
   }

   private static bool IsCharPresent(char[,] array, char c, int row, int col)
   {
      var length = Math.Sqrt(array.Length);
      return row >= 0 && row < length && col >= 0 && col < length && array[row, col] == c;
   }

   private static bool IsC1XMAS(char[,] array, int row, int col) =>
      IsCharPresent(array, 'A', row, col) &&
      IsCharPresent(array, 'M', row - 1, col - 1) &&
      IsCharPresent(array, 'S', row + 1, col + 1) &&
      IsCharPresent(array, 'S', row - 1, col + 1) &&
      IsCharPresent(array, 'M', row + 1, col - 1);

   private static bool IsC2XMAS(char[,] array, int row, int col) =>
      IsCharPresent(array, 'A', row, col) &&
      IsCharPresent(array, 'M', row - 1, col - 1) &&
      IsCharPresent(array, 'S', row + 1, col + 1) &&
      IsCharPresent(array, 'M', row - 1, col + 1) &&
      IsCharPresent(array, 'S', row + 1, col - 1);

   private static bool IsC3XMAS(char[,] array, int row, int col) =>
      IsCharPresent(array, 'A', row, col) &&
      IsCharPresent(array, 'S', row - 1, col - 1) &&
      IsCharPresent(array, 'M', row + 1, col + 1) &&
      IsCharPresent(array, 'M', row - 1, col + 1) &&
      IsCharPresent(array, 'S', row + 1, col - 1);

   private static bool IsC4XMAS(char[,] array, int row, int col) =>
      IsCharPresent(array, 'A', row, col) &&
      IsCharPresent(array, 'S', row - 1, col - 1) &&
      IsCharPresent(array, 'M', row + 1, col + 1) &&
      IsCharPresent(array, 'S', row - 1, col + 1) &&
      IsCharPresent(array, 'M', row + 1, col - 1);

   private static bool IsXMASToTheLeft(char[,] array, int row, int col) =>
      IsCharPresent(array, 'X', row, col) &&
      IsCharPresent(array, 'M', row, col + 1) &&
      IsCharPresent(array, 'A', row, col + 2) &&
      IsCharPresent(array, 'S', row, col + 3);

   private static bool IsXMASToTheRight(char[,] array, int row, int col) =>
      IsCharPresent(array, 'X', row, col) &&
      IsCharPresent(array, 'M', row, col - 1) &&
      IsCharPresent(array, 'A', row, col - 2) &&
      IsCharPresent(array, 'S', row, col - 3);

   private static bool IsXMASToTheDown(char[,] array, int row, int col) =>
      IsCharPresent(array, 'X', row, col) &&
      IsCharPresent(array, 'M', row + 1, col) &&
      IsCharPresent(array, 'A', row + 2, col) &&
      IsCharPresent(array, 'S', row + 3, col);

   private static bool IsXMASToTheUp(char[,] array, int row, int col) =>
      IsCharPresent(array, 'X', row, col) &&
      IsCharPresent(array, 'M', row - 1, col) &&
      IsCharPresent(array, 'A', row - 2, col) &&
      IsCharPresent(array, 'S', row - 3, col);

   private static bool IsXMASOnLeftDownDiagonal(char[,] array, int row, int col) =>
     IsCharPresent(array, 'X', row, col) &&
     IsCharPresent(array, 'M', row + 1, col - 1) &&
     IsCharPresent(array, 'A', row + 2, col - 2) &&
     IsCharPresent(array, 'S', row + 3, col - 3);

   private static bool IsXMASOnLeftUpDiagonal(char[,] array, int row, int col) =>
     IsCharPresent(array, 'X', row, col) &&
     IsCharPresent(array, 'M', row - 1, col - 1) &&
     IsCharPresent(array, 'A', row - 2, col - 2) &&
     IsCharPresent(array, 'S', row - 3, col - 3);

   private static bool IsXMASOnRightDownDiagonal(char[,] array, int row, int col) =>
     IsCharPresent(array, 'X', row, col) &&
     IsCharPresent(array, 'M', row + 1, col + 1) &&
     IsCharPresent(array, 'A', row + 2, col + 2) &&
     IsCharPresent(array, 'S', row + 3, col + 3);

   private static bool IsXMASOnRightUpDiagonal(char[,] array, int row, int col) =>
     IsCharPresent(array, 'X', row, col) &&
     IsCharPresent(array, 'M', row - 1, col + 1) &&
     IsCharPresent(array, 'A', row - 2, col + 2) &&
     IsCharPresent(array, 'S', row - 3, col + 3);
}
