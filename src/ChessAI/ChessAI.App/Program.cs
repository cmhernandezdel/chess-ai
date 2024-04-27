using System.Diagnostics;
using ChessAI.Board;

Stopwatch stopwatch = Stopwatch.StartNew();
Lookup lookup = new();
stopwatch.Stop();
Console.WriteLine("Lookup tables construction: " + stopwatch.ElapsedMilliseconds + " ms");

stopwatch.Restart();
var attacks = lookup.InitializeRookAttacks();
stopwatch.Stop();
Console.WriteLine("Lookup attacks: " + stopwatch.ElapsedMilliseconds + " ms");

stopwatch.Restart();
var bb = MagicNumbers.SetOccupancy(4095, attacks.CountBits(), attacks);
Console.WriteLine(bb);
stopwatch.Stop();
Console.WriteLine("Print board: " + stopwatch.ElapsedMilliseconds + " ms");