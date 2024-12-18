namespace AdventOfCode2024.Day11;

public static class Day11
{
   public static int CountStonesNoCaching()
   {
      var stones = new LinkedList<long>(ParseStoneInput());

      for (var i = 0; i < 25; i++)
      {
         for (var current = stones.First; current != null; current = current.Next)
         {
            if (current.Value == 0)
            {
               current.Value++;
            }
            else if (IsNumberSplittable(current.Value, out var firstHalf, out var secondHalf))
            {
               stones.AddBefore(current, firstHalf);
               var lastAdded = stones.AddAfter(current, secondHalf);
               stones.Remove(current);
               current = lastAdded;
            }
            else
            {
               current.Value *= 2024;
            }
         }
      }

      return stones.Count;
   }

   private readonly record struct CacheEntry(long Number, int Step);

   public static long CountStonesWithCaching()
   {
      var stones = ParseStoneInput();

      var cache = new Dictionary<CacheEntry, long>();

      long CountInternal(long number, int step)
      {
         if (step == 0)
            return 1;

         if (cache.TryGetValue(new CacheEntry(number, step), out var value))
            return value;

         var nextStep = step - 1;
         if (number == 0)
         {
            value = CountInternal(1, nextStep);
            cache.TryAdd(new CacheEntry(1, nextStep), value);

            return value;
         }

         if (IsNumberSplittable(number, out var firstHalf, out var secondHalf))
         {
            var first = CountInternal(firstHalf, nextStep);
            cache.TryAdd(new CacheEntry(firstHalf, nextStep), first);

            var second = CountInternal(secondHalf, nextStep);
            cache.TryAdd(new CacheEntry(secondHalf, nextStep), second);

            return first + second;
         }

         var newNumber = number * 2024;
         value = CountInternal(newNumber, nextStep);
         cache.TryAdd(new CacheEntry(newNumber, nextStep), value);

         return value;
      }

      var count = stones.Sum(x => CountInternal(x, 75));

      return count;
   }

   private static bool IsNumberSplittable(long number, out int firstHalf, out int secondHalf)
   {
      firstHalf = 0;
      secondHalf = 0;

      var numberStr = number.ToString();
      if (numberStr.Length % 2 == 0)
      {
         var cutOff = numberStr.Length / 2;
         firstHalf = int.Parse(numberStr.Substring(0, cutOff));
         secondHalf = int.Parse(numberStr.Substring(cutOff, cutOff));

         return true;
      }

      return false;
   }

   private static List<long> ParseStoneInput() => File
      .ReadAllLines(@"Day11\day11.txt")
      .SelectMany(x => x.Split(' '))
      .Select(long.Parse)
      .ToList();
}
