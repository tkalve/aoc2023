using System.Text.RegularExpressions;

var input = args.Length > 0 ? args[0] : "input.txt";
if (!File.Exists(input))
    Console.WriteLine("Please provide an input file.");

int maxRed = 12, maxGreen = 13, maxBlue = 14;
int count = 0, sum = 0, power = 0;

foreach (var line in File.ReadLines(input))
{
    var isValid = true;
    int minRed = 0, minGreen = 0, minBlue = 0;

    var idMatch = Regex.Match(line, @"Game (\d+):");
    var id = int.Parse(idMatch.Groups["1"].Value);

    var grabs = line.Substring(idMatch.Length).Split(';');

    foreach (var grab in grabs)
    {
        var colors = Regex.Matches(grab, @"(\d+) (red|green|blue)");
        int red = colors.Where(x => x.Groups[2].Value == "red").Select(x => int.Parse(x.Groups[1].Value)).FirstOrDefault();
        int green = colors.Where(x => x.Groups[2].Value == "green").Select(x => int.Parse(x.Groups[1].Value)).FirstOrDefault();
        int blue = colors.Where(x => x.Groups[2].Value == "blue").Select(x => int.Parse(x.Groups[1].Value)).FirstOrDefault();

        if (red > minRed) minRed = red;
        if (green > minGreen) minGreen = green;
        if (blue > minBlue) minBlue = blue;
        if (green > maxGreen || red > maxRed || blue > maxBlue)
            isValid = false;
    }

    if (isValid)
    {
        count++;
        sum += id;
    }
    power += minRed * minGreen * minBlue;
}

Console.WriteLine($"Valid games {count}, sum: {sum}, combined power: {power}");
