using System.Diagnostics;
using ChessAI.Board;

Stopwatch stopwatch = Stopwatch.StartNew();
Lookup lookup = new();
stopwatch.Stop();
Console.WriteLine("Lookup tables construction: " + stopwatch.ElapsedMilliseconds + " ms");

stopwatch.Restart();
var attacks = lookup.kingAttacks[(int)Board.Square.h1];
stopwatch.Stop();
Console.WriteLine("Lookup attacks: " + stopwatch.ElapsedMilliseconds + " ms");

stopwatch.Restart();
Console.WriteLine(attacks);
stopwatch.Stop();
Console.WriteLine("Print board: " + stopwatch.ElapsedMilliseconds + " ms");