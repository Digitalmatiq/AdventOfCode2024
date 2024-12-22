namespace AdventOfCode2024.Day15;

public static class Day15
{
   public static long SumOfBoxesCoordinates()
   {
      var (map, sequence) = LoadData();


      return 0;
   }

   private static (char[,] Map, List<char> Sequence) LoadData()
   {
      var lines = File.ReadAllLines(@"Day15\day15.txt");

      var map = new char[lines.Length, lines.Length];
      var lastRow = 0;

      foreach (var (row, line) in lines.Index())
      {
         if (line.Length == 0)
         {
            lastRow = row;
            break;
         }

         foreach (var (col, @char) in line.Index())
            map[row, col] = @char;
      }

      var sequence = new List<char>();
      for (var i = lastRow + 1; i < lines.Length; i++)
      {
         var line = lines[i];
         foreach (var @char in line)
            sequence.Add(@char);
      }

      return (map, sequence);
   }
}
