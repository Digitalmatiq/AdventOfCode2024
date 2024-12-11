namespace AdventOfCode2024.Day1;

public static class Day1
{
   public static int GetDistancesSum()
   {
      var (firstList, secondList) = GetLists();

      var differences = firstList
         .Zip(secondList)
         .Sum(x => Math.Abs(x.First - x.Second));

      return differences;
   }

   public static int GetSimillarityScore()
   {
      var (firstList, secondList) = GetLists();

      var multiplicationTable = new Dictionary<int, int>();

      foreach (var x in secondList)
         multiplicationTable[x] = 1 + multiplicationTable.GetValueOrDefault(x, 0);

      var simillarity = firstList.Sum(x => x * multiplicationTable.GetValueOrDefault(x, 0));

      return simillarity;
   }

   private static (List<int> FirstList, List<int> SecondList) GetLists()
   {
      var lines = File.ReadAllLines(@"Day1\day1.txt");

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

      return (firstList, secondList);
   }
}
