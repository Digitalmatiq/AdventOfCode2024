namespace AdventOfCode2024.Day9;

public static class Day9
{
   public static long GetChecksum()
   {
      var data = LoadExpandedData();

      var left = data.First;
      var right = data.Last;

      while (left.Value.FileId != right.Value.Va.FileId)
      {
         while (left.Value.Values.Any(x => x.Value == -1))
         {
            var leftValues = left.Value.Values;

            while (right.Value.Values.All(x => x.Value == -1))
               right = right.Previous;

            var rightValues = right.Value.Values;

            for (var i = 0; i < leftValues.Count; i++)
            {
               if (leftValues[i].Value != -1)
                  continue;

               for (var j = 0; j < rightValues.Count; j++)
               {
                  if (rightValues[j].Value == -1)
                     continue;

                  leftValues[i] = rightValues[j];
                  rightValues.RemoveAt(j);
                  break;
               }

               break;
            }
         }

         left = left.Next;
      }

      while (true)
      {
         var values = data.Last.Value.Values;

         if (!values.All(x => x.Value == -1))
         {
            var count = 0;

            while (values.Any(x => x.Value == -1))
            {
               if (values[count].Value == -1)
               {
                  values.RemoveAt(count);
                  count = 0;
                  continue;
               }

               count++;
            }

            break;
         }

         data.RemoveLast();
      }

      var checkSum = 0;
      foreach (var file in data)
      {
         foreach (var value in file.Values)
         {
            checkSum += value.FileId * value.Value;
         }
      }

      return checkSum;
   }

   private static LinkedList<DiskFile> LoadExpandedData()
   {
      var @string = File.ReadAllText(@"Day9\day9.txt");

      var list = new LinkedList<DiskFile>();
      int i;

      for (i = 0; i < @string.Length - 1; i += 2)
      {
         var identity = i / 2;
         var identityMultiple = Convert.ToInt32(@string[i] - '0');
         var slotsMultiple = Convert.ToInt32(@string[i + 1] - '0');

         var values = Enumerable
            .Range(0, identityMultiple)
            .Select(x => new FileValue(identity, identity))
            .Concat(Enumerable
               .Range(0, slotsMultiple)
               .Select(x => new FileValue(identity, -1)))
            .ToList();

         list.AddLast(new DiskFile(values));
      }

      if (@string.Length % 2 != 0)
      {
         var identity = i / 2;
         var identityMultiple = Convert.ToInt32(@string[^1] - '0');

         var values = Enumerable
            .Range(0, identityMultiple)
            .Select(x => new FileValue(identity, identity))
            .ToList();

         list.AddLast(new DiskFile(values));
      }

      return list;
   }

   private readonly record struct FileValue(int FileId, int Value);

   private sealed record DiskFile(List<FileValue> Values);
}
