using System.Text.RegularExpressions;

if (args.Length == 0)
{
    Console.WriteLine("Please provide an input file.");
    return;
}

int total = 0;
var numbers = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

string? FromWord(string? input)
{
    var index = Array.IndexOf(numbers, input);
    return index >= 0 ? (index + 1).ToString() : input;
}

foreach (var line in File.ReadLines(args[0]))
{
    var matches = Regex.Matches(line, @"(?=(" + string.Join('|', numbers) + @"|\d))")
                        .Cast<Match>()
                        .Where(m => m.Groups[1].Success)
                        .ToList();

    var first = FromWord(matches.FirstOrDefault()?.Groups[1].Value);
    var last = FromWord(matches.LastOrDefault()?.Groups[1].Value);

    if (first != null && last != null)
        total += int.Parse($"{first}{last}");
}

Console.WriteLine("Total: " + total);