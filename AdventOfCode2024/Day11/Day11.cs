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

   public static int CountStonesWithCaching()
   {
      var stones = ParseStoneInput();
      var cache = new Dictionary<long, BlinkingStoneStep>();

      static void Transform(long originalNumber, long number, int step, int stoppingStep, List<long> partialTransformed)
      {
         if (step == stoppingStep)
            return;

         partialTransformed.Remove(number);
         var nextStep = step + 1;

         if (number == 0)
         {
            partialTransformed.Add(1);
            Transform(originalNumber, 1, nextStep, stoppingStep, partialTransformed);

            return;
         }

         if (IsNumberSplittable(number, out var firstHalf, out var secondHalf))
         {
            partialTransformed.Add(firstHalf);
            Transform(originalNumber, firstHalf, nextStep, stoppingStep, partialTransformed);

            partialTransformed.Add(secondHalf);
            Transform(originalNumber, secondHalf, nextStep, stoppingStep, partialTransformed);
            return;
         }

         var newNumber = number * 2024;
         partialTransformed.Add(newNumber);

         Transform(originalNumber, newNumber, nextStep, stoppingStep, partialTransformed);
      }

      List<long> finalStones = [];
      foreach (var stone in stones)
      {
         for (var i = 0; i < 75; i++)
         {
            List<long> transformed = [];
            Transform(stone, stone, 0, i, transformed);
            cache.Add(stone, new BlinkingStoneStep());
         }
      }

      return finalStones.Count;
   }

   private readonly record struct BlinkingStoneStep(int Step, List<long> Transformed);

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
