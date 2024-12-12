
namespace AdventOfCode2024.Day5;

public static class Day5
{
   public static int SumUpdates()
   {
      var (rules, updates) = LoadStructures();
      var dependencyMap = BuildDependencyMap(rules);

      var sum = updates
         .Where(x => IsUpdateValid(x, dependencyMap))
         .Sum(x => x.MiddlePage);

      return sum;
   }

   public static int SumWrongUpdates()
   {
      var (rules, updates) = LoadStructures();
      var dependencyMap = BuildDependencyMap(rules);

      var sum = updates
         .Where(x => !IsUpdateValid(x, dependencyMap))
         .Select(x => FixUpdate(x, dependencyMap))
         .Sum(x => x.MiddlePage);

      return sum;
   }

   private static Update FixUpdate(Update update, Dictionary<int, HashSet<int>> dependencyMap)
   {
      static Update FixUpdateInternal(Update update, Dictionary<int, HashSet<int>> dependencyMap)
      {
         var visited = new HashSet<int>();

         foreach (var page in update.Pages)
         {
            if (!dependencyMap.TryGetValue(page, out var dependents))
               continue;

            foreach (var (index, v) in visited.Index())
            {
               if (dependents.Contains(v))
               {
                  var newList = update.Pages.ToList();
                  newList.Remove(v);
                  newList.Add(v);

                  return FixUpdateInternal(new Update([.. newList]), dependencyMap);
               }
            }

            visited.Add(page);
         }

         return update;
      }

      return FixUpdateInternal(update, dependencyMap);
   }

   private static Dictionary<int, HashSet<int>> BuildDependencyMap(List<OrderRule> rules)
   {
      var dependencyMap = new Dictionary<int, HashSet<int>>();

      foreach (var (before, after) in rules)
      {
         if (!dependencyMap.TryGetValue(before, out var set))
         {
            set = [after];
            dependencyMap[before] = set;
         }

         set.Add(after);
      }

      return dependencyMap;
   }

   private static bool IsUpdateValid(Update update, Dictionary<int, HashSet<int>> dependencyMap)
   {
      var visited = new HashSet<int>();

      foreach (var page in update.Pages)
      {
         if (!dependencyMap.TryGetValue(page, out var dependents))
            continue;

         foreach (var v in visited)
         {
            if (dependents.Contains(v))
               return false;
         }

         visited.Add(page);
      }

      return true;
   }

   private static (List<OrderRule> PageRules, List<Update> Updates) LoadStructures()
   {
      var lines = File.ReadAllLines(@"Day5\day5.txt");

      var pageRules = new List<OrderRule>();
      var updates = new List<Update>();

      foreach (var line in lines)
      {
         if (line.Contains('|'))
         {
            var values = line.Split('|');
            pageRules.Add(new OrderRule(int.Parse(values[0]), int.Parse(values[1])));
         }
         else if (line.Contains(','))
         {
            var values = line.Split(',');
            updates.Add(new Update(values.Select(int.Parse).ToArray()));
         }
      }

      return (pageRules, updates);
   }

   private readonly record struct OrderRule(int Before, int After);

   private readonly record struct Update(int[] Pages)
   {
      public int MiddlePage => Pages[(Pages.Length - 1) / 2];
   }
}
