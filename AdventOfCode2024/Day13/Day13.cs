namespace AdventOfCode2024.Day13;

public static class Day13
{
   public static long CountTokens() => LoadData().Sum(x => SolveMachineWithArray(x) ?? 0);

   public static long CountTokensShifted() => LoadData().Sum(x => SolveMachineSingleLoop(x) ?? 0);

   private static long? SolveMachineSingleLoop(ClawMachineData machine)
   {
      var aButton = machine.AButton;
      var bButton = machine.BButton;

      //machine = machine with
      //{
      //   PrizeX = machine.PrizeX + 10000000000000,
      //   PrizeY = machine.PrizeY + 10000000000000,
      //};

      long? minCost = long.MaxValue;

      var i = 0;
      while (true)
      {
         var dpState = new DPState(
            i * bButton.XOffset,
            i * bButton.YOffset,
            i * bButton.Cost);

         if (dpState.X == machine.PrizeX && dpState.Y == machine.PrizeY && dpState.Cost < minCost)
            minCost = dpState.Cost;

         var xRemaining = machine.PrizeX - dpState.X;
         var yRemaining = machine.PrizeY - dpState.Y;

         if (xRemaining % aButton.XOffset == 0)
         {
            var n = xRemaining / aButton.XOffset;
            if (n * aButton.YOffset == yRemaining)
            {
               var cost = (n * aButton.Cost) + dpState.Cost;
               if (cost < minCost)
                  minCost = cost;
            }
         }

         if (dpState.X > machine.PrizeX & dpState.Y > machine.PrizeY)
            break;

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

      for (i = 1; i < length; i++)
      {
         dpTable[i, 0] = new DPState(
           currentIState.X + aButton.XOffset,
           currentIState.Y + aButton.YOffset,
           currentIState.Cost + aButton.Cost);

         currentIState = dpTable[i, 0]!.Value;
      }

      for (j = 1; j < length; j++)
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
