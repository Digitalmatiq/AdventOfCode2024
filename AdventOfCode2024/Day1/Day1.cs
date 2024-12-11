namespace AdventOfCode2024.Day1;

public static class Day1
{
   public static async Task<int> GetAnswer()
   {
      var lines = await File.ReadAllLinesAsync(@"Day1\day1.txt");

      var pairs = lines
         .Select(x => x
            .Split("   ")
            .Select(int.Parse)
            .ToList())
         .ToList();

      var firstList = pairs
         .Select(x => x[0])
         .ToList();

      var secondList = pairs
         .Select(x => x[1])
         .ToList();

      firstList.Sort();
      secondList.Sort();

      var differences = firstList.Zip(secondList).Sum(x => Math.Abs(x.First - x.Second));

      return differences;
   }
}
