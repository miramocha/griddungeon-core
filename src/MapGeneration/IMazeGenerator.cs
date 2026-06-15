// Maze generator contract inspired by mazelib (MIT) — https://github.com/john-science/mazelib

namespace GridDungeon.Core.MapGeneration
{
    public interface IMazeGenerator
    {
        string AlgorithmId { get; }

        MazeGrid Generate(MazeGenerationParams parameters);
    }
}
