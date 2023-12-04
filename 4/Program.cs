using System.Text.RegularExpressions;

var input = args.Length > 0 ? args[0] : "example.txt";

if (!File.Exists(input))
{
    Console.WriteLine("Please provide an input file.");
    return;
}

var total = 0;
var cardCopies = new Dictionary<int, int>();
int cardsProcessed = 0;

foreach (var line in File.ReadLines(input))
{
    var match = Regex.Match(line, @"Card\s+(\d+): ([\d ]+)\| ([\d ]+)");

    int cardNumber = int.Parse(match.Groups[1].Value);
    List<int> myNumbers = match.Groups[2].Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse).ToList();
    List<int> winningNumbers = match.Groups[3].Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse).ToList();

    var copies = 1;
    if (cardCopies.ContainsKey(cardNumber))
        copies += cardCopies[cardNumber];

    if (copies == 1)
        Console.WriteLine($"Scratching card # {cardNumber}");
    else
        Console.WriteLine($"Scratching {copies} copies of card # {cardNumber}");

    var (points, cardsWon) = Scratch(cardNumber, myNumbers, winningNumbers);
    for (int i = 0; i < copies; i++)
    {
        cardsProcessed++;
        total += points;

        foreach(var card in cardsWon)
            if (cardCopies.ContainsKey(card))
                cardCopies[card]++;
            else
                cardCopies[card] = 1;
    }
    if (cardsWon.Any())
        Console.WriteLine($"> Scored {points} points, won {cardsWon.Count} cards [{string.Join(", ", cardsWon)}]");
    else
        Console.WriteLine($"> Scored {points} points, won {cardsWon.Count} cards");

    Console.WriteLine();
}

(int, List<int>) Scratch(int cardNumber, List<int> myNumbers, List<int> winningNumbers)
{
    var points = 0;
    var winnings = 0;

    foreach (var number in myNumbers)
    {
        if (winningNumbers.Contains(number) && points == 0)
        {
            points = 1;
            winnings++;
        }
        else if (winningNumbers.Contains(number))
        {
            points = points * 2;
            winnings++;
        }
    }

    List<int> cardsWon = new List<int>();
    for (int i = cardNumber; i < cardNumber + winnings; i++)
    {
        var cardWon = i + 1;
        cardsWon.Add(cardWon);
    }

    return (points, cardsWon);
}

Console.WriteLine();
Console.WriteLine($"Sum: {total}");
Console.WriteLine($"Cards: {cardsProcessed}");