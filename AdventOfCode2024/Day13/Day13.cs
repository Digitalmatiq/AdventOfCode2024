namespace AdventOfCode2024.Day13;

public static class Day13
{
   public static long CountTokens() => LoadData().Sum(x => SolveMachineWithArray(x) ?? 0);

   public static long CountTokensShifted() => LoadData().Sum(x => SolveMachineWithoutArray(x) ?? 0);

   private static long? SolveMachineWithoutArray(ClawMachineData machine)
   {
      var aButton = machine.AButton;
      var bButton = machine.BButton;

      machine = machine with
      {
         PrizeX = machine.PrizeX + 10000000000000,
         PrizeY = machine.PrizeY + 10000000000000,
      };

      long? minCost = long.MaxValue;
      var minXIncrement = Math.Min(aButton.XOffset, bButton.XOffset);
      var minYIncrement = Math.Min(aButton.YOffset, bButton.YOffset);
      var leastIncrement = Math.Max(machine.PrizeX / minXIncrement, machine.PrizeY / minYIncrement);

      var stateAbove = new DPState(0, 0, 0);

      var i = 0;
      while (stateAbove.X <= machine.PrizeX && stateAbove.Y <= machine.PrizeY)
      {
         stateAbove = new DPState(
           stateAbove.X + (i * bButton.XOffset),
           stateAbove.Y + (i * bButton.YOffset),
           stateAbove.Cost + (i * bButton.Cost));

         if (stateAbove.X == machine.PrizeX && stateAbove.Y == machine.PrizeY && stateAbove.Cost < minCost)
            minCost = stateAbove.Cost;

         var j = 1;
         var stateRight = stateAbove;

         while (stateRight.X <= machine.PrizeX || stateRight.Y <= machine.PrizeY)
         {
            stateRight = new DPState(
              stateAbove.X + (j * aButton.XOffset),
              stateAbove.Y + (j * aButton.YOffset),
              stateAbove.Cost + (j * aButton.Cost));

            if (stateRight.X == machine.PrizeX && stateRight.Y == machine.PrizeY && stateRight.Cost < minCost)
               minCost = stateRight.Cost;

            j++;
         }

         i++;
      }

      return minCost == long.MaxValue ? null : minCost;
   }

   private static long? SolveMachineWithArray(ClawMachineData machine)
   {
      var aButton = machine.AButton;
      var bButton = machine.BButton;

      var minAX = (machine.PrizeX / aButton.XOffset) + 1;
      var minAY = (machine.PrizeY / aButton.YOffset) + 1;
      var minBX = (machine.PrizeX / bButton.XOffset) + 1;
      var minBY = (machine.PrizeY / bButton.YOffset) + 1;

      var length = Math.Max(minAX, Math.Max(minAY, Math.Max(minBX, minBY)));
      var dpTable = new DPState?[length, length];

      var currentState = new DPState(0, 0, 0);
      dpTable[0, 0] = currentState;

      var i = 0;
      var j = 0;

      var currentIState = dpTable[i++, 0]!.Value;
      var currentJState = dpTable[0, j++]!.Value;

      for (i = 0; i < length; i++)
      {
         dpTable[i, 0] = new DPState(
           currentIState.X + aButton.XOffset,
           currentIState.Y + aButton.YOffset,
           currentIState.Cost + aButton.Cost);

         currentIState = dpTable[i, 0]!.Value;
      }

      for (j = 0; j < length; j++)
      {
         dpTable[0, j] = new DPState(
           currentJState.X + bButton.XOffset,
           currentJState.Y + bButton.YOffset,
           currentJState.Cost + bButton.Cost);

         currentJState = dpTable[0, j]!.Value;
      }

      long? minCost = long.MaxValue;

      for (i = 1; i < length; i++)
      {
         for (j = 1; j < length; j++)
         {
            var lastAbove = dpTable[i - 1, j]!.Value;

            dpTable[i, j] = new DPState(
               lastAbove.X + aButton.XOffset,
               lastAbove.Y + aButton.YOffset,
               lastAbove.Cost + aButton.Cost);

            currentState = dpTable[i, j]!.Value;

            if (currentState.X == machine.PrizeX && currentState.Y == machine.PrizeY)
            {
               if (currentState.Cost < minCost)
                  minCost = currentState.Cost;
            }
         }
      }

      return minCost == long.MaxValue ? null : minCost;
   }

   private readonly record struct DPState(long X, long Y, long Cost);

   private static List<ClawMachineData> LoadData()
   {
      var data = File.ReadAllText(@"Day13\day13.txt");
      var machines = new List<ClawMachineData>();

      static void LoadDataInternal(string data, List<ClawMachineData> machines)
      {
         if (string.IsNullOrEmpty(data))
            return;

         var region = data.Split("\r\n")
            .TakeWhile(x => x != string.Empty)
            .ToList();

         var aOffsets = region[0].Split('+');
         var aButton = new Button('A', 3,
            int.Parse(new string(aOffsets[1].TakeWhile(char.IsDigit).ToArray())),
            int.Parse(new string(aOffsets[2].TakeWhile(char.IsDigit).ToArray())));

         var bOffsets = region[1].Split('+');
         var bButton = new Button('B', 1,
            int.Parse(new string(bOffsets[1].TakeWhile(char.IsDigit).ToArray())),
            int.Parse(new string(bOffsets[2].TakeWhile(char.IsDigit).ToArray())));

         var prizeOffsets = region[2].Split('=');
         var clawMachineData = new ClawMachineData(
            int.Parse(new string(prizeOffsets[1].TakeWhile(char.IsDigit).ToArray())),
            int.Parse(new string(prizeOffsets[2].TakeWhile(char.IsDigit).ToArray())),
            aButton,
            bButton);

         machines.Add(clawMachineData);

         var remaining = data
            .Substring(string.Join("\r\n", region).Length)
            .TrimStart("\r\n".ToCharArray());

         LoadDataInternal(remaining, machines);
      }

      LoadDataInternal(data, machines);

      return machines;
   }

   private readonly record struct ClawMachineData(long PrizeX, long PrizeY, Button AButton, Button BButton);

   private readonly record struct Button(char Label, int Cost, int XOffset, int YOffset);
}
