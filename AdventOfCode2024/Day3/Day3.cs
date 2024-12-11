using System.Buffers;
using System.Text;

namespace AdventOfCode2024.Day3;

public static class Day3
{
   public static long GetMultiplication()
   {
      var data = File.ReadAllText(@"Day3\day3.txt");
      var searchValues = SearchValues.Create(["mul(", "do()", "don't()"], StringComparison.Ordinal);
      var finalResult = 0L;

      var enabledMuls = true;
      int pos;
      ReadOnlySpan<char> remaining = data;
      while ((pos = remaining.IndexOfAny(searchValues)) >= 0)
      {
         if (ParseEnabled(remaining, pos, out var changedEnabled))
            enabledMuls = changedEnabled!.Value;

         if (enabledMuls && ParseMul(remaining, pos, ',', out var first) && ParseMul(remaining, pos + first!.Length + 1, ')', out var second))
         {
            var firstNum = long.Parse(first);
            var secondNum = long.Parse(second!);

            finalResult += firstNum * secondNum;
         }

         remaining = remaining.Slice(pos + 1);
      }

      return finalResult;
   }

   private static bool ParseEnabled(ReadOnlySpan<char> text, int pos, out bool? enabled)
   {
      enabled = null;

      const int doLength = 4;
      const int dontLength = 7;

      var doSubstring = text.Slice(pos, doLength);
      if (MemoryExtensions.Equals(doSubstring, "do()", StringComparison.InvariantCulture))
      {
         enabled = true;
         return true;
      }

      var dontSubstring = text.Slice(pos, dontLength);
      if (MemoryExtensions.Equals(dontSubstring, "don't()", StringComparison.InvariantCulture))
      {
         enabled = false;
         return true;
      }

      return false;
   }

   private static bool ParseMul(ReadOnlySpan<char> text, int pos, char c, out string? valueStr)
   {
      const int mulLength = 4;

      valueStr = null;
      var builder = new StringBuilder();
      var count = 0;

      do
      {
         var finalChar = text[mulLength + pos + count++];

         if (!char.IsDigit(finalChar))
         {
            if (finalChar == c)
            {
               valueStr = builder.ToString();
               return true;
            }

            break;
         }

         builder.Append(finalChar);
      }
      while (true);

      return false;
   }
}
