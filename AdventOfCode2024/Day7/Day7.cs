namespace AdventOfCode2024.Day7;

public static class Day7
{
   public static long SumTests()
   {
      var tests = LoadTests();
      var sum = tests
         .Where(x => IsTestTrue(x, withConcat: false))
         .Sum(x => x.Value);

      return sum;
   }

   public static long SumTestsWithConcat()
   {
      var tests = LoadTests();
      var sum = tests
         .Where(x => IsTestTrue(x, withConcat: true))
         .Sum(x => x.Value);

      return sum;
   }

   private static bool IsTestTrue(TestResult test, bool withConcat)
   {
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
               Operation.Concat => total = long.Parse(total.ToString() + current.ToString()),
            };

            if (total > test.Value)
               break;
         }

         return total == test.Value;
      }

      var variants = GenerateCombinations(test.Nums.Count - 1, withConcat);
      foreach (var variant in variants)
      {
         if (IsTestTrueInternal(test, variant))
            return true;
      }

      return false;
   }

   private static List<Operation[]> GenerateCombinations(int n, bool withConcat)
   {
      var combinations = new List<Operation[]>();
      var allOps = Enum.GetValues<Operation>();
      if (!withConcat)
         allOps = allOps.Where(x => x != Operation.Concat).ToArray();

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
      Multiply,
      Concat
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
