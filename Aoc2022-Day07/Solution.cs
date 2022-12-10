using System.Diagnostics;

namespace Aoc2022_Day07;

internal class Solution
{
    public string Title => "Day 7: No Space Left On Device";

    public object PartOne()
    {
        var root = ReadFileSystemFromInput();
        var result = EnumerateDirectories(root).Where(d => d.CalculateSize() <= 100000)
                                               .Sum(d => d.CalculateSize());
        return result;
    }

    public object PartTwo()
    {
        var root = ReadFileSystemFromInput();
        var spaceAvailable = 70000000 - root.CalculateSize();
        var shortfall = 30000000 - spaceAvailable;
        var result = EnumerateDirectories(root).Where(d => d.CalculateSize() >= shortfall)
                                               .Min(d => d.CalculateSize());
        return result;
    }

    private static Directory ReadFileSystemFromInput(string? fileName = null)
    {
        var root = new Directory { Name = "/" };
        var directory = root;
        foreach (var line in InputFile.ReadAllLines(fileName))
        {
            // If it's a `cd` command, change to that directory.
            if (line.StartsWith("$ cd "))
            {
                var arg = line[5..];
                directory = arg switch
                      {
                          "/"  => root,
                          ".." => directory.Parent ?? throw new UnreachableException("Attempt to navigate up from root."),
                          _    => directory.Directories.Single(d => d.Name == arg)
                      };
                continue;
            }

            // If it's an ls, skip over (any non-command lines are assumed to be `ls` command output).
            if (line == "$ ls")
            {
                continue;
            }

            // Assume this is an output line from an `ls` command.
            var split = line.Split(" ", 2);
            var (size, name) = (split[0], split[1]);
            if (size == "dir")
                directory.AddDirectory(name);
            else
                directory.AddFile(name, Convert.ToInt32(size));
        }

        return root;
    }

    private static IEnumerable<Directory> EnumerateDirectories(Directory directory)
    {
        yield return directory;
        foreach (var descendant in directory.Directories.SelectMany(EnumerateDirectories))
        {
            yield return descendant;
        }
    }

    // ReSharper disable MemberCanBePrivate.Local
    private record Directory
    {
        public required string Name { get; init; }
        public Directory? Parent { get; init; }
        public List<Directory> Directories { get; } = new();
        public List<File> Files { get; } = new();

        public void AddDirectory(string name)
        {
            if (Directories.Any(d => d.Name == name)) return;
            Directories.Add(new Directory { Parent = this, Name = name });
        }

        public void AddFile(string name, int size)
        {
            var existing = Files.SingleOrDefault(f => f.Name == name);
            if (existing is not null && existing.Size == size) return;
            if (existing is not null) throw new UnreachableException("File repeated with a different size.");
            Files.Add(new File { Name = name, Size = size });
        }

        public int CalculateSize() => Directories.Sum(d => d.CalculateSize())
                                      + Files.Sum(f => f.Size);
    }

    private record File
    {
        public required string Name { get; init; }
        public required int Size { get; init; }
    }
    // ReSharper restore MemberCanBePrivate.Local
}
