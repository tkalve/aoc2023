using System.Text.RegularExpressions;

var input = args.Length > 0 ? args[0] : "input.txt";
if (!File.Exists(input))
    Console.WriteLine("Please provide an input file.");

var numbers = new List<Number>();
var symbols = new List<Symbol>();
var partNumbers = new List<Number>();
var other = new List<Number>();
var ratioSum = 0;

var lineNumber = 0;
foreach (var line in File.ReadLines(input))
{
    lineNumber++;

    foreach (Match match in Regex.Matches(line, @"\d+"))
        numbers.Add(new Number(lineNumber, match.Index, match.Length, int.Parse(match.Value)));

    foreach (Match match in Regex.Matches(line, @"[^0-9\.]"))
        symbols.Add(new Symbol(lineNumber, match.Index, match.Value));
}

foreach (var number in numbers)
{
    if (symbols.Any(s => s.Line >= number.Line - 1
                           && s.Line <= number.Line + 1
                           && s.Column >= number.Column - 1
                           && s.Column <= number.Column + number.Length))
        partNumbers.Add(number);
}

foreach (var gear in symbols.Where(s => s.Value == "*"))
{
    var adjacentNumbers = numbers.Where(n => n.Line >= gear.Line - 1
                                             && n.Line <= gear.Line + 1
                                             && gear.Column >= n.Column - 1
                                             && gear.Column <= n.Column + n.Length);
    if (adjacentNumbers.Count() == 2)
    {
        var gearNumbers= adjacentNumbers.Select(n => n.Value).ToArray();
        var ratio = gearNumbers[0] * gearNumbers[1];
        ratioSum += ratio;
    }
}

Console.WriteLine($"Sum of part numbers: {partNumbers.Sum(n => n.Value)}");
Console.WriteLine($"Sum of gear ratios: {ratioSum}");

record Number(int Line, int Column, int Length, int Value);
record Symbol(int Line, int Column, string Value);