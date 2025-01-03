﻿namespace AdventOfCode2024.Day15;

public static class Day15
{
   public static long SumOfExpandedBoxesCoordinates()
   {
      var (map, sequence, currentPosition) = LoadData();

      (map, currentPosition) = ExpandMap(map);

      PrintMap(map);

      foreach (var (index, action) in sequence.Index())
      {
         switch (action)
         {
            case '<':
               currentPosition = HandleLeftExpanded(map, currentPosition);
               break;

            case '>':
               currentPosition = HandleRightExpanded(map, currentPosition);
               break;

            case '^':
               currentPosition = HandleTopExpanded(map, currentPosition);
               break;

            case 'v':
               currentPosition = HandleDownExpanded(map, currentPosition);
               break;
         }

         PrintMap(map);
      }

      return 0;
   }

   private static (char[,] Map, Index CurrentPosition) ExpandMap(char[,] map)
   {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);

      var resized = new char[rows, 2 * cols];
      Index currentPosition = default;

      for (var i = 0; i < rows; i++)
      {
         for (var j = 0; j < cols; j++)
         {
            var value = map[i, j];

            var valueLeft = value;
            var valueRight = value;

            if (value == '@')
            {
               valueRight = '.';
               currentPosition = new Index(i, 2 * j);
            }
            else if (value == 'O')
            {
               valueLeft = '[';
               valueRight = ']';
            }

            resized[i, 2 * j] = valueLeft;
            resized[i, (2 * j) + 1] = valueRight;
         }
      }

      return (resized, currentPosition);
   }

   public static long SumOfBoxesCoordinates()
   {
      var (map, sequence, currentPosition) = LoadData();

      foreach (var (index, action) in sequence.Index())
      {
         switch (action)
         {
            case '<':
               currentPosition = HandleLeft(map, currentPosition);
               break;

            case '>':
               currentPosition = HandleRight(map, currentPosition);
               break;

            case '^':
               currentPosition = HandleTop(map, currentPosition);
               break;

            case 'v':
               currentPosition = HandleDown(map, currentPosition);
               break;
         }
      }

      var sum = GetSumOfCoordinates(map);

      return sum;
   }

   private static int GetSumOfCoordinates(char[,] map)
   {
      var sum = 0;

      for (var i = 0; i < map.GetLength(0); i++)
      {
         for (var j = 0; j < map.GetLength(1); j++)
         {
            var value = map[i, j];
            if (value == 'O')
               sum += (i * 100) + j;
         }
      }

      return sum;
   }

   private static void PrintMap(char[,] map)
   {
      for (var i = 0; i < map.GetLength(0); i++)
      {
         for (var j = 0; j < map.GetLength(1); j++)
            Console.Write($" {map[i, j]} ");

         Console.Write('\n');
      }
   }

   private static Index HandleLeft(char[,] map, Index pos)
   {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);

      var (lastIndexToUpdate, canMove) = GetLastMovingIndex(Direction.Left, map, pos);
      var finalPosition = pos;

      if (canMove)
      {
         map[pos.Row, pos.Col] = '.';

         for (var i = pos.Col - 2; i >= lastIndexToUpdate.Col; i--)
            map[pos.Row, i] = 'O';

         finalPosition = new Index(pos.Row, pos.Col - 1);
         map[finalPosition.Row, finalPosition.Col] = '@';
      }

      return finalPosition;
   }

   private static Index HandleLeftExpanded(char[,] map, Index pos)
   {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);

      var (lastIndexToUpdate, canMove) = GetLastMovingIndex(Direction.Left, map, pos);
      var finalPosition = pos;

      if (canMove)
      {
         map[pos.Row, pos.Col] = '.';

         for (var i = lastIndexToUpdate.Col; i <= pos.Col - 2; i++)
            map[pos.Row, i] = map[pos.Row, i + 1];

         finalPosition = new Index(pos.Row, pos.Col - 1);

         map[finalPosition.Row, finalPosition.Col] = '@';
      }

      return finalPosition;
   }

   private static Index HandleRight(char[,] map, Index pos)
   {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);

      var (lastIndexToUpdate, canMove) = GetLastMovingIndex(Direction.Right, map, pos);
      var finalPosition = pos;

      if (canMove)
      {
         map[pos.Row, pos.Col] = '.';

         for (var i = pos.Col + 2; i <= lastIndexToUpdate.Col; i++)
            map[pos.Row, i] = 'O';

         finalPosition = new Index(pos.Row, pos.Col + 1);
         map[finalPosition.Row, finalPosition.Col] = '@';
      }

      return finalPosition;
   }

   private static Index HandleRightExpanded(char[,] map, Index pos)
   {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);

      var (lastIndexToUpdate, canMove) = GetLastMovingIndex(Direction.Right, map, pos);
      var finalPosition = pos;

      if (canMove)
      {
         map[pos.Row, pos.Col] = '.';

         for (var i = lastIndexToUpdate.Col; i >= pos.Col + 2; i--)
            map[pos.Row, i] = map[pos.Row, i - 1];

         finalPosition = new Index(pos.Row, pos.Col + 1);

         map[finalPosition.Row, finalPosition.Col] = '@';
      }

      return finalPosition;
   }

   private static void GetEnclosingObject(HashSet<Index> indexes, Direction direction, char[,] map, Index index)
   {
      char ValueAt(Index pos) => map[pos.Row, pos.Col];

      if (indexes.Contains(index))
         return;

      if (indexes.Count > 0)
      {
         var group = indexes.GroupBy(x => x.Row);

         var lastFrontier = direction == Direction.Top ?
            group.OrderBy(x => x.Key).First() :
            group.OrderByDescending(x => x.Key).First();

         if (lastFrontier.All(x => ValueAt(
            direction == Direction.Top ?
               new Index(x.Row - 1, x.Col) :
               new Index(x.Row + 1, x.Col)) == '.' || ValueAt(
            direction == Direction.Top ?
               new Index(x.Row - 1, x.Col) :
               new Index(x.Row + 1, x.Col)) == '#'))
            return;
      }

      var value = map[index.Row, index.Col];
      if (value == '[')
      {
         indexes.Add(index);
         indexes.Add(new Index(index.Row, index.Col + 1));
      }
      else if (value == ']')
      {
         indexes.Add(index);
         indexes.Add(new Index(index.Row, index.Col - 1));
      }

      if (direction == Direction.Top)
      {
         foreach (var i in indexes)
            GetEnclosingObject(indexes, direction, map, new Index(i.Row - 1, i.Col));
      }
      else if (direction == Direction.Bottom)
      {
         foreach (var i in indexes)
            GetEnclosingObject(indexes, direction, map, new Index(i.Row + 1, i.Col));
      }
   }

   private static Index HandleTopExpanded(char[,] map, Index pos)
   {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);

      var (lastIndexToUpdate, canMove) = GetLastMovingIndex(Direction.Top, map, pos);
      var finalPosition = pos;

      if (canMove)
      {
         map[pos.Row, pos.Col] = '.';

         var enclosingIndexes = new HashSet<Index>();
         GetEnclosingObject(enclosingIndexes, Direction.Top, map, new Index(pos.Row - 1, pos.Col));

         //for (var i = pos.Row - 2; i >= lastIndexToUpdate.Row; i--)
         //   map[i, pos.Col] = 'O';

         finalPosition = new Index(pos.Row - 1, pos.Col);
         map[finalPosition.Row, finalPosition.Col] = '@';
      }

      return finalPosition;
   }

   private static Index HandleDownExpanded(char[,] map, Index pos)
   {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);

      var (lastIndexToUpdate, canMove) = GetLastMovingIndex(Direction.Bottom, map, pos);
      var finalPosition = pos;

      if (canMove)
      {
         map[pos.Row, pos.Col] = '.';

         //for (var i = pos.Row - 2; i >= lastIndexToUpdate.Row; i--)
         //   map[i, pos.Col] = 'O';

         finalPosition = new Index(pos.Row + 1, pos.Col);
         map[finalPosition.Row, finalPosition.Col] = '@';
      }

      return finalPosition;
   }

   private static Index HandleTop(char[,] map, Index pos)
   {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);

      var (lastIndexToUpdate, canMove) = GetLastMovingIndex(Direction.Top, map, pos);
      var finalPosition = pos;

      if (canMove)
      {
         map[pos.Row, pos.Col] = '.';

         for (var i = pos.Row - 2; i >= lastIndexToUpdate.Row; i--)
            map[i, pos.Col] = 'O';

         finalPosition = new Index(pos.Row - 1, pos.Col);
         map[finalPosition.Row, finalPosition.Col] = '@';
      }

      return finalPosition;
   }

   private static Index HandleDown(char[,] map, Index pos)
   {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);

      var (lastIndexToUpdate, canMove) = GetLastMovingIndex(Direction.Bottom, map, pos);
      var finalPosition = pos;

      if (canMove)
      {
         map[pos.Row, pos.Col] = '.';

         for (var i = pos.Row + 2; i <= lastIndexToUpdate.Row; i++)
            map[i, pos.Col] = 'O';

         finalPosition = new Index(pos.Row + 1, pos.Col);
         map[finalPosition.Row, finalPosition.Col] = '@';
      }

      return finalPosition;
   }

   private static (Index LastIndex, bool CanMove) GetLastMovingIndex(Direction direction, char[,] map, Index pos)
   {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);

      var canMove = false;
      var last = 0;

      switch (direction)
      {
         case Direction.Left:

            {
               last = pos.Col;

               for (var i = pos.Col - 1; i >= 0; i--)
               {
                  var value = map[pos.Row, i];
                  if (value == '.' || value == '#')
                  {
                     if (value == '.')
                     {
                        last = i;
                        canMove = true;
                     }

                     break;
                  }

                  last = i;
               }

               break;
            }

         case Direction.Right:

            {
               last = pos.Col;

               for (var i = pos.Col + 1; i < cols; i++)
               {
                  var value = map[pos.Row, i];
                  if (value == '.' || value == '#')
                  {
                     if (value == '.')
                     {
                        last = i;
                        canMove = true;
                     }

                     break;
                  }

                  last = i;
               }

               break;
            }

         case Direction.Top:

            {
               last = pos.Row;

               for (var i = pos.Row - 1; i >= 0; i--)
               {
                  var value = map[i, pos.Col];
                  if (value == '.' || value == '#')
                  {
                     if (value == '.')
                     {
                        last = i;
                        canMove = true;
                     }

                     break;
                  }

                  last = i;
               }

               break;
            }

         case Direction.Bottom:

            {
               last = pos.Row;

               for (var i = pos.Row + 1; i < rows; i++)
               {
                  var value = map[i, pos.Col];
                  if (value == '.' || value == '#')
                  {
                     if (value == '.')
                     {
                        last = i;
                        canMove = true;
                     }

                     break;
                  }

                  last = i;
               }

               break;
            }
      }

      return direction switch
      {
         Direction.Top => (new Index(last, pos.Col), canMove),
         Direction.Left => (new Index(pos.Row, last), canMove),
         Direction.Right => (new Index(pos.Row, last), canMove),
         Direction.Bottom => (new Index(last, pos.Col), canMove),
      };
   }

   private enum Direction
   {
      Top,
      Left,
      Right,
      Bottom
   }

   private readonly record struct Index(int Row, int Col);

   private static (char[,] Map, List<char> Sequence, Index StartPosition) LoadData()
   {
      var lines = File.ReadAllLines(@"Day15\day15.txt");

      var map = new char[lines.Length, lines.Length];
      var lastRow = 0;

      Index startPosition = default;
      foreach (var (row, line) in lines.Index())
      {
         if (line.Length == 0)
         {
            lastRow = row;
            break;
         }

         foreach (var (col, @char) in line.Index())
         {
            map[row, col] = @char;
            if (@char == '@')
               startPosition = new Index(row, col);
         }
      }

      var sequence = new List<char>();
      for (var i = lastRow + 1; i < lines.Length; i++)
      {
         var line = lines[i];
         foreach (var @char in line)
            sequence.Add(@char);
      }

      return (map, sequence, startPosition);
   }
}
