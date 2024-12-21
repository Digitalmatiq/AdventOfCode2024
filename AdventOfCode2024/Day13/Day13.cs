namespace AdventOfCode2024.Day13;

public static class Day13
{
   public static long CountTokens() => LoadData().Sum(x => SolveMachineSingleLoop(x) ?? 0);

   public static long CountTokensShifted() => LoadData().Sum(x => SolveWithMatrixDecomposition(x) ?? 0);

   private static long? SolveUsingAlgLib(ClawMachineData machine)
   {
      var aButton = machine.AButton;
      var bButton = machine.BButton;

      alglib.minlpcreate(2, out var state);
      alglib.minlpsetcost(state, [-aButton.Cost, -bButton.Cost]);
      alglib.minlpsetbc(state, [0, 0], [double.PositiveInfinity, double.PositiveInfinity]);

      var generalForm = new double[,]
      {
         { aButton.Cost, bButton.Cost },
         { aButton.XOffset, bButton.XOffset },
         { aButton.YOffset, bButton.YOffset },
      };

      var lowerBounds = new double[] { 0, machine.PrizeX, machine.PrizeY, };
      var upperBounds = new double[] { double.PositiveInfinity, machine.PrizeX, machine.PrizeY };

      alglib.minlpsetlc2dense(state, generalForm, lowerBounds, upperBounds, 3);
      alglib.minlpoptimize(state);
      alglib.minlpresults(state, out var results, out var rep);

      return (int)((results[0] * aButton.Cost) + (results[1] * bButton.Cost));
   }

   private static long? SolveWithMatrixDecomposition(ClawMachineData machine)
   {
      var aButton = machine.AButton;
      var bButton = machine.BButton;

      machine = machine with
      {
         PrizeX = machine.PrizeX + 10000000000000,
         PrizeY = machine.PrizeY + 10000000000000,
      };

      decimal n2Numerator = (aButton.XOffset * machine.PrizeY) - (aButton.YOffset * machine.PrizeX);
      decimal n2Denominator = (aButton.XOffset * bButton.YOffset) - (aButton.YOffset * bButton.XOffset);

      if (n2Numerator % n2Denominator != 0)
         return null;

      var n2 = n2Numerator / n2Denominator;

      var n1Numerator = machine.PrizeX - (n2 * bButton.XOffset);
      decimal n1Denominator = aButton.XOffset;

      if (n1Numerator % n1Denominator != 0)
         return null;

      var n1 = n1Numerator / n1Denominator;
      return (long)((n1 * aButton.Cost) + (n2 * bButton.Cost));
   }

   private static long? SolveMachineSingleLoop(ClawMachineData machine)
   {
      var aButton = machine.AButton;
      var bButton = machine.BButton;

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
