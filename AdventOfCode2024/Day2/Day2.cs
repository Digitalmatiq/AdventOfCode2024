namespace AdventOfCode2024.Day2;

public static class Day2
{
   public static int GetSafeReports()
   {
      var reports = GetReports();
      var safeReports = reports.Count(IsReportSafe);

      return safeReports;
   }

   public static int GetSafeReportsDampener()
   {
      var reports = GetReports();
      var safeReports = reports.Count(IsReportSafeDampener);

      return safeReports;
   }

   private static bool IsReportSafe(List<int> report)
   {
      var previous = report.First();

      bool? willIncrease = null;
      for (var i = 1; i < report.Count; i++)
      {
         var level = report[i];
         var delta = level - previous;

         if (delta == 0 || Math.Abs(delta) > 3)
            return false;

         var didIncrease = delta > 0;
         if (willIncrease.HasValue)
         {
            if (willIncrease.Value != didIncrease)
               return false;
         }
         else
         {
            willIncrease = didIncrease;
         }

         previous = level;
      }

      return true;
   }

   private static bool IsReportSafeDampener(List<int> report)
   {
      static bool IsReportSafeDampenerInternal(List<int> report, bool removedOnce)
      {
         var previous = report.First();

         bool? willIncrease = null;
         for (var i = 1; i < report.Count; i++)
         {
            var level = report[i];
            var delta = level - previous;

            if (delta == 0 || Math.Abs(delta) > 3)
            {
               if (removedOnce)
                  return false;

               var withoutThis = report.Where((item, index) => index != i).ToList();
               var withoutLast = report.Where((item, index) => index != i - 1).ToList();
               var withoutSecondLast = report.Where((item, index) => index != i - 2).ToList();

               var afterRemoval = IsReportSafeDampenerInternal(withoutThis, true) ||
                  IsReportSafeDampenerInternal(withoutLast, true) ||
                  IsReportSafeDampenerInternal(withoutSecondLast, true);

               return afterRemoval;
            }

            var didIncrease = delta > 0;
            if (willIncrease.HasValue)
            {
               if (willIncrease.Value != didIncrease)
               {
                  if (removedOnce)
                     return false;

                  var withoutThis = report.Where((item, index) => index != i).ToList();
                  var withoutLast = report.Where((item, index) => index != i - 1).ToList();
                  var withoutSecondLast = report.Where((item, index) => index != i - 2).ToList();

                  var afterRemoval = IsReportSafeDampenerInternal(withoutThis, true) ||
                     IsReportSafeDampenerInternal(withoutLast, true) ||
                     IsReportSafeDampenerInternal(withoutSecondLast, true);

                  return afterRemoval;
               }
            }
            else
            {
               willIncrease = didIncrease;
            }

            previous = level;
         }

         return true;
      }

      return IsReportSafeDampenerInternal(report, false);
   }

   private static List<List<int>> GetReports()
   {
      var lines = File.ReadAllLines(@"Day2\day2.txt");

      var reports = lines
         .Select(x => x
            .Split(' ')
            .Select(int.Parse)
            .ToList())
         .ToList();

      return reports;
   }
}
