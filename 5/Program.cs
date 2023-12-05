using System.Diagnostics;

Stopwatch stopWatch = new();
stopWatch.Start();

if (args.Length == 0 || !File.Exists(args[0]))
    throw new Exception("Could not read input file");

var input = File.ReadAllText(args[0]);

// Part 1
var seeds = new List<long>();
var mappings = new List<MapDefinition>();

var sections = input.Split("\r\n\r\n");
foreach (var section in sections)
{
    var lines = section.Split("\r\n");
    var firstLine = lines[0].Trim();

    if (firstLine.StartsWith("seeds:"))
        seeds.AddRange(firstLine[6..].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse));
    else
    {
        var mapName = firstLine.Split(" ")[0];
        var mapNamePart = mapName.Split("-");
        var mapDef = new MapDefinition(mapNamePart[0], mapNamePart[2], new List<Map>());
        foreach (var line in lines.Skip(1))
        {
            var values = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
            mapDef.Maps.Add(new Map(values[0], values[1], values[2]));
        }

        mappings.Add(mapDef);
    }
}

long? lowest = null;

foreach (var seed in seeds)
{
    var location = Traverse(seed);

    if (lowest == null || lowest > location)
        lowest = location;
}
stopWatch.Stop();

Console.WriteLine($"Part 1 | lowest location: {lowest} [{stopWatch.ElapsedMilliseconds} ms]");


// Part 2
stopWatch.Restart();

var seedGroups = Chunk(seeds, 2).Select(x => new SeedGroup(x.First(), x.First() + x.Last() - 1)).ToList();

var highestLocation = mappings
    .FirstOrDefault(x => x.Destination == "location")?.Maps
    .Select(x => x.Destination + x.Range)
    .OrderByDescending(x => x)
    .FirstOrDefault();

long start = 0;
long leap = 10000;
if (args[0] == "example.txt") leap = 1; 

lowest = null;

var checkLocation = start;
while (lowest == null)
{
    var seed = Reverse(checkLocation);
    if (IsSeed(seed))
        lowest = checkLocation;

    checkLocation += leap;

    if (highestLocation != null && highestLocation < checkLocation)
        throw new Exception("Oops");
}

if (lowest == null)
    throw new Exception("Did not find seed for any locations");

start = (long)lowest - leap;
lowest = null;

checkLocation = start;
while (lowest == null)
{
    var seed = Reverse(checkLocation);
    if (IsSeed(seed))
        lowest = checkLocation;

    checkLocation++;

    if (highestLocation != null && highestLocation < checkLocation)
        throw new Exception("Oops");
}

stopWatch.Stop();

Console.WriteLine($"Part 2 | lowest location: {lowest} [{stopWatch.ElapsedMilliseconds} ms]");

long Traverse(long seed)
{
    var source = "seed";

    while (source != "location")
    {
        var mapDef = mappings.First(x => x.Source == source);
        var map = mapDef.Maps.Where(x => seed >= x.Source && seed <= x.Source + x.Range).FirstOrDefault();
        if (map != null)
            seed += map.Destination - map.Source;
        source = mapDef.Destination;
    }

    return seed;
}

long Reverse(long location)
    {
        var destination = "location";

        while (destination != "seed")
        {
            var mapDef = mappings!.First(x => x.Destination == destination);
            var map = mapDef.Maps.Where(x => location >= x.Destination && location <= x.Destination + x.Range).FirstOrDefault();
            if (map != null)
                location -= map.Destination - map.Source;
            destination = mapDef.Source;
        }

        return location;
    }

    List<List<long>> Chunk(IEnumerable<long> data, int size)
    {
        return data
          .Select((x, i) => new { Index = i, Value = x })
          .GroupBy(x => x.Index / size)
          .Select(x => x.Select(v => v.Value).ToList())
          .ToList();
    }

    bool IsSeed(long seed)
    {
        foreach (var seedGroup in seedGroups)
            if (seed >= seedGroup.First && seed <= seedGroup.Last)
                return true;

        return false;
    }

record Map(long Destination, long Source, long Range);
record MapDefinition(string Source, string Destination, List<Map> Maps);
record SeedGroup(long First, long Last);