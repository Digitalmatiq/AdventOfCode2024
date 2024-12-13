namespace AdventOfCode2024.Day7;

public static class Day7
{
   public static long SumTests()
   {
      var tests = LoadTests();
      var sum = tests
         .Where(IsTestTrue)
         .Sum(x => x.Value);

      return sum;
   }

   private static bool IsTestTrue(TestResult test)
   {
      var value = test.Value;
      var biggest = test.Nums.Aggregate(1L, (part, x) => part * x);

      if (value >= biggest)
         return value == biggest;

      static bool IsTestTrueInternal(TestResult test, Operation[] ops)
      {
         var total = test.Nums.First();

         for (var i = 1; i < test.Nums.Count; i++)
         {
            var current = test.Nums[i];
            var result = ops[i - 1] switch
            {
               Operation.Add => total += current,
               Operation.Multiply => total *= current,
            };
         }

         return total == test.Value;
      }

      var variants = GeneratePermutations(test.Nums.Count - 1);
      foreach (var variant in variants)
      {
         if (IsTestTrueInternal(test, variant))
            return true;
      }

      return false;
   }

   private static List<Operation[]> GeneratePermutations(int n)
   {
      var combinations = new List<Operation[]>();
      var allOps = Enum.GetValues<Operation>();

      void GenerateCombinationsInternal(Operation[] array, int index)
      {
         if (index == n)
         {
            combinations.Add(array.ToArray());
            return;
         }

         foreach (var op in allOps)
         {
            array[index] = op;
            GenerateCombinationsInternal(array, index + 1);
         }
      }

      GenerateCombinationsInternal(new Operation[n], 0);

      return combinations;
   }

   private enum Operation
   {
      Add,
      Multiply
   }

   private static List<TestResult> LoadTests()
   {
      var lines = File.ReadAllLines(@"Day7\day7.txt");

      var results = new List<TestResult>();
      foreach (var line in lines)
      {
         var data = line.Split(':');
         var value = long.Parse(data[0]);

         var nums = data[1]
            .TrimStart()
            .Split(' ')
            .Select(long.Parse)
            .ToList();

         results.Add(new TestResult(value, nums));
      }

      return results;
   }

   private readonly record struct TestResult(long Value, List<long> Nums);
}
