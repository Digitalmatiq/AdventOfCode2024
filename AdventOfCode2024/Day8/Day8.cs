namespace AdventOfCode2024.Day8;

public static class Day8
{
   public static int CountAntiNodes()
   {
      var (matrix, antennas) = LoadAntennas();

      var length = matrix.GetLength(0);
      var antinodes = new HashSet<Node>();

      void AddAntinodes(Node node, Node other)
      {
         var dx = other.Col - node.Col;
         var dy = other.Row - node.Row;

         var antiNode = new Node('#', other.Row + dy, other.Col + dx);
         if (antiNode.Row < 0 || antiNode.Row >= length || antiNode.Col < 0 || antiNode.Col >= length)
            return;

         antinodes.Add(antiNode);
      }

      foreach (var (f, nodes) in antennas)
      {
         if (nodes.Count == 1)
            continue;

         for (var i = 0; i < nodes.Count; i++)
         {
            var node = nodes[i];
            for (var j = i + 1; j < nodes.Count; j++)
            {
               var other = nodes[j];
               AddAntinodes(node, other);
               AddAntinodes(other, node);
            }
         }
      }

      return antinodes.Count;
   }

   public static int CountUpdatedAntinodes()
   {
      var (matrix, antennas) = LoadAntennas();

      var length = matrix.GetLength(0);
      var antinodes = new HashSet<Node>();

      void AddAntinodes(Node node, Node other)
      {
         var dx = other.Col - node.Col;
         var dy = other.Row - node.Row;

         var current = other;
         while (true)
         {
            var antiNode = new Node('#', current.Row + dy, current.Col + dx);
            if (antiNode.Row < 0 || antiNode.Row >= length || antiNode.Col < 0 || antiNode.Col >= length)
               break;

            antinodes.Add(antiNode);
            current = antiNode;
         }

         antinodes.Add(new Node('#', node.Row, node.Col));
         antinodes.Add(new Node('#', other.Row, other.Col));
      }

      foreach (var (f, nodes) in antennas)
      {
         if (nodes.Count == 1)
            continue;

         for (var i = 0; i < nodes.Count; i++)
         {
            var node = nodes[i];
            for (var j = i + 1; j < nodes.Count; j++)
            {
               var other = nodes[j];
               AddAntinodes(node, other);
               AddAntinodes(other, node);
            }
         }
      }

      return antinodes.Count;
   }

   private static (char[,] Matrix, Dictionary<char, List<Node>> Antenas) LoadAntennas()
   {
      var lines = File.ReadAllLines(@"Day8\day8.txt");

      var matrix = new char[lines.Length, lines.Length];
      var antennas = new Dictionary<char, List<Node>>();

      foreach (var (row, line) in lines.Index())
      {
         foreach (var (col, @char) in line.Index())
         {
            matrix[row, col] = @char;

            if (@char != '.')
            {
               if (!antennas.TryGetValue(@char, out var list))
               {
                  list = [];
                  antennas[@char] = list;
               }

               list.Add(new(@char, row, col));
            }
         }
      }

      return (matrix, antennas);
   }

   private readonly record struct Node(char C, int Row, int Col)
   {
      public override int GetHashCode() => HashCode.Combine(Row, Col);
   }
}
