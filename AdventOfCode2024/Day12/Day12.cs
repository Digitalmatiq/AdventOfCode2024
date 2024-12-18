namespace AdventOfCode2024.Day12;

public static class Day12
{
   public static int GetPriceWithPerimiter()
   {
      var regions = LoadRegions();
      var total = regions.Sum(x => x.Area * x.Permiter);

      return total;
   }

   public static int GetPriceWithSides()
   {
      var regions = LoadRegions();
      var total = regions.Sum(x => x.Area * x.Sides);

      return total;
   }

   private static HashSet<Region> LoadRegions()
   {
      var map = LoadMap();
      var length = map.GetLength(0);

      var regionsMap = new Dictionary<Index, Region>();

      bool IsWithinBounds(Index pos) => pos.Row >= 0 && pos.Col >= 0 && pos.Row < length && pos.Col < length;

      char ValueAt(Index pos) => map[pos.Row, pos.Col];

      void TraverseInternal(Index pos, char C, HashSet<Index> region)
      {
         if (ValueAt(pos) == C)
         {
            if (!region.Add(pos))
               return;
         }

         var right = new Index(pos.Row, pos.Col + 1);
         if (IsWithinBounds(right) && ValueAt(right) == C)
            TraverseInternal(right, C, region);

         var left = new Index(pos.Row, pos.Col - 1);
         if (IsWithinBounds(left) && ValueAt(left) == C)
            TraverseInternal(left, C, region);

         var up = new Index(pos.Row - 1, pos.Col);
         if (IsWithinBounds(up) && ValueAt(up) == C)
            TraverseInternal(up, C, region);

         var down = new Index(pos.Row + 1, pos.Col);
         if (IsWithinBounds(down) && ValueAt(down) == C)
            TraverseInternal(down, C, region);
      }

      for (var row = 0; row < length; row++)
      {
         for (var col = 0; col < length; col++)
         {
            var index = new Index(row, col);
            if (!regionsMap.ContainsKey(index))
            {
               var regionIndexes = new HashSet<Index>();
               TraverseInternal(index, ValueAt(index), regionIndexes);

               var region = new Region(regionIndexes, ValueAt(index));
               foreach (var regionIndex in regionIndexes)
                  regionsMap[regionIndex] = region;
            }
         }
      }

      var regions = regionsMap.Values.ToHashSet();
      return regions;
   }

   private readonly record struct Index(int Row, int Col);

   private sealed record Region(HashSet<Index> Indexes, char Plant)
   {
      private readonly record struct Adjacency(Index First, Index Second)
      {
         public override int GetHashCode() => First.GetHashCode() + Second.GetHashCode();
      }

      private readonly record struct Boundary(Index Index, Boundary.BPosition Position)
      {
         public enum BPosition
         {
            Right,
            Left,
            Up,
            Down
         }

         public override int GetHashCode() => Position switch
         {
            BPosition.Right => new Index(Index.Row, Index.Col + 1).GetHashCode(),
            BPosition.Left => new Index(Index.Row, Index.Col - 1).GetHashCode(),
            BPosition.Up => new Index(Index.Row - 1, Index.Col).GetHashCode(),
            BPosition.Down => new Index(Index.Row + 1, Index.Col).GetHashCode(),
         } + new Index(Index.Row, Index.Col).GetHashCode();

         public bool Equals(Boundary obj)
         {
            if (obj.Index.Col == Index.Col)
            {
               if (obj.Index.Row == Index.Row - 1 && obj.Position == BPosition.Down && Position == BPosition.Up)
                  return true;

               if (obj.Index.Row == Index.Row + 1 && obj.Position == BPosition.Up && Position == BPosition.Down)
                  return true;
            }

            if (obj.Index.Row == Index.Row)
            {
               if (obj.Index.Col == Index.Col + 1 && obj.Position == BPosition.Left && Position == BPosition.Right)
                  return true;

               if (obj.Index.Col == Index.Col - 1 && obj.Position == BPosition.Right && Position == BPosition.Left)
                  return true;
            }

            return obj.Index.Row == Index.Row && obj.Index.Col == Index.Col && obj.Position == Position;
         }
      }

      public Guid Guid { get; } = Guid.NewGuid();

      public int Area { get; } = Indexes.Count;

      public int Sides
      {
         get
         {
            var segments = new HashSet<Boundary>();

            foreach (var index in Indexes)
            {
               var right = new Boundary(index, Boundary.BPosition.Right);
               var left = new Boundary(index, Boundary.BPosition.Left);
               var up = new Boundary(index, Boundary.BPosition.Up);
               var down = new Boundary(index, Boundary.BPosition.Down);

               if (!segments.Add(right))
                  segments.Remove(right);

               if (!segments.Add(left))
                  segments.Remove(left);

               if (!segments.Add(up))
                  segments.Remove(up);

               if (!segments.Add(down))
                  segments.Remove(down);
            }

            var horizontalSegments = segments
               .Where(x => x.Position == Boundary.BPosition.Up || x.Position == Boundary.BPosition.Down)
               .GroupBy(x => HashCode.Combine(x.Index.Row, x.Position))
               .Sum(x => CountIndividualSides(x.ToList(), horizontal: true));

            var verticalSegments = segments
               .Where(x => x.Position == Boundary.BPosition.Left || x.Position == Boundary.BPosition.Right)
               .GroupBy(x => HashCode.Combine(x.Index.Col, x.Position))
               .Sum(x => CountIndividualSides(x.ToList(), horizontal: false));

            return horizontalSegments + verticalSegments;
         }
      }

      public int Permiter => (4 * Area) - GetInternalAdjacencies().Count;

      private int CountIndividualSides(List<Boundary> segments, bool horizontal)
      {
         if (segments.Count == 0)
            return 0;

         if (segments.Count == 1)
            return 1;

         if (horizontal)
            segments.Sort((a, b) => a.Index.Col.CompareTo(b.Index.Col));
         else
            segments.Sort((a, b) => a.Index.Row.CompareTo(b.Index.Row));

         var count = 1;
         for (var i = 0; i < segments.Count - 1; i++)
         {
            var current = segments[i];
            var next = segments[i + 1];

            if (horizontal)
            {
               if (current.Index.Col + 1 != next.Index.Col)
                  count++;
            }
            else
            {
               if (current.Index.Row + 1 != next.Index.Row)
                  count++;
            }
         }

         return count;
      }

      private HashSet<Adjacency> GetInternalAdjacencies()
      {
         var adjacencies = new HashSet<Adjacency>();
         foreach (var pos in Indexes)
         {
            var right = new Index(pos.Row, pos.Col + 1);
            var left = new Index(pos.Row, pos.Col - 1);
            var up = new Index(pos.Row - 1, pos.Col);
            var down = new Index(pos.Row + 1, pos.Col);

            if (Indexes.Contains(right))
               adjacencies.Add(new Adjacency(pos, right));

            if (Indexes.Contains(left))
               adjacencies.Add(new Adjacency(pos, left));

            if (Indexes.Contains(up))
               adjacencies.Add(new Adjacency(pos, up));

            if (Indexes.Contains(down))
               adjacencies.Add(new Adjacency(pos, down));
         }

         return adjacencies;
      }

      public override int GetHashCode() => Guid.GetHashCode();
   }

   private static char[,] LoadMap()
   {
      var lines = File.ReadAllLines(@"Day12\day12.txt");
      var matrix = new char[lines.Length, lines.Length];

      foreach (var (row, line) in lines.Index())
      {
         foreach (var (col, @char) in line.Index())
            matrix[row, col] = @char;
      }

      return matrix;
   }
}
