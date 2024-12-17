using AdventOfCode2024.Day11;

internal class Program
{
   private static void Main(string[] args)
   {
      var result2 = Day11.CountStonesNoCaching();
      var result = Day11.CountStonesWithCaching();
      Console.WriteLine(result);
   }
}