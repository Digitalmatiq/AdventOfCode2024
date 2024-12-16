namespace AdventOfCode2024.Day9;

public static class Day9
{
   public static long GetChecksumDefragmented()
   {
      var data = LoadExpandedData();
      CompactSpace(data, withFragmentation: false);

      return GetChecksum(data);
   }

   public static long GetChecksum()
   {
      var data = LoadExpandedData();
      CompactSpace(data, withFragmentation: true);

      return GetChecksum(data);
   }

   private static long GetChecksum(LinkedList<DiskSpace> data)
   {
      var checkSum = 0L;
      var count = 0;
      for (var item = data.First; item != null; item = item.Next)
      {
         var length = item.Value.Length;

         for (var i = 0; i < length; i++)
         {
            if (item.Value.FileId != -1)
               checkSum += count * item.Value.FileId;

            count++;
         }
      }

      return checkSum;
   }

   private static void CompactSpace(LinkedList<DiskSpace> data, bool withFragmentation)
   {
      var left = data.First!;
      var right = data.Last!;

      while (left.Value != right.Value)
      {
         if (left.Value.FileId != -1)
         {
            left = left.Next;
            continue;
         }

         if (right.Value.FileId == -1)
         {
            right = right.Previous;
            continue;
         }

         var delta = right.Value.Length - left.Value.Length;
         if (delta > 0 && withFragmentation)
         {
            data.AddAfter(right, new DiskSpace(-1, left.Value.Length));
            left.Value = new DiskSpace(right.Value.FileId, left.Value.Length);
            right.Value = new DiskSpace(right.Value.FileId, delta);
         }
         else if (delta <= 0)
         {
            data.AddBefore(left, new DiskSpace(right.Value.FileId, right.Value.Length));
            left.Value = new DiskSpace(-1, -delta);
            right.Value = new DiskSpace(-1, right.Value.Length);

            if (!withFragmentation)
               left = data.First;
         }
         else
         {
            if (!withFragmentation)
            {
               var first = data.First;
               var second = right;

               var hasGap = true;

               while (first.Value.Length < second.Value.Length || first.Value.FileId != -1)
               {
                  if (first.Value == second.Value)
                  {
                     hasGap = false;
                     break;
                  }

                  first = first.Next;
               }

               if (!hasGap)
               {
                  right = right.Previous;
                  continue;
               }

               left = first;
            }
         }
      }
   }

   private static LinkedList<DiskSpace> LoadExpandedData()
   {
      var @string = File.ReadAllText(@"Day9\day9.txt");

      var list = new LinkedList<DiskSpace>();
      int i;

      for (i = 0; i < @string.Length - 1; i += 2)
      {
         var identity = i / 2;
         var identityMultiple = Convert.ToInt32(@string[i] - '0');
         var slotsMultiple = Convert.ToInt32(@string[i + 1] - '0');

         list.AddLast(new DiskSpace(identity, identityMultiple));
         list.AddLast(new DiskSpace(-1, slotsMultiple));
      }

      if (@string.Length % 2 != 0)
      {
         var identity = i / 2;
         var identityMultiple = Convert.ToInt32(@string[^1] - '0');

         list.AddLast(new DiskSpace(identity, identityMultiple));
      }

      return list;
   }

   private readonly record struct DiskSpace(int FileId, int Length)
   {
      public Guid Guid { get; } = Guid.NewGuid();
   }
}
