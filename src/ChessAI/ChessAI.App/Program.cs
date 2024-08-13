using ChessAI.Game;

// Stopwatch stopwatch = Stopwatch.StartNew();
// Lookup lookup = new();
// stopwatch.Stop();
// Console.WriteLine("Lookup tables construction: " + stopwatch.ElapsedMilliseconds + " ms");
//
// stopwatch.Restart();
// var attacks = lookup.InitializeRookAttacks();
// stopwatch.Stop();
// Console.WriteLine("Lookup attacks: " + stopwatch.ElapsedMilliseconds + " ms");
//
// stopwatch.Restart();
// var bb = MagicNumbers.SetOccupancy(4095, attacks.CountBits(), attacks);
// Console.WriteLine(bb);
// stopwatch.Stop();
// Console.WriteLine("Print board: " + stopwatch.ElapsedMilliseconds + " ms");

// var mn = new MagicNumbersGenerator();
// mn.InitMagicNumbers();
// var emptyBitboard = Bitboard.EmptyBitboard();
// emptyBitboard.SetBit(Board.Square.c5);
// emptyBitboard.SetBit(Board.Square.f2);
// emptyBitboard.SetBit(Board.Square.g7);
// var lookup = new Lookup();
// var bishopAttacks = lookup.GetBishopAttacks((int) Board.Square.d4, emptyBitboard);
// Console.WriteLine(bishopAttacks.ToString());
// stopwatch.Stop();
// Console.WriteLine("Bishop attacks: " + stopwatch.ElapsedMilliseconds + " ms");

Game game = new Game();
Console.WriteLine(game.ToString());