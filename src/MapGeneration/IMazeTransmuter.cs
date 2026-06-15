// Maze transmuter contract inspired by mazelib (MIT) — https://github.com/john-science/mazelib

namespace GridDungeon.Core.MapGeneration
{
    public interface IMazeTransmuter
    {
        string TransmuterId { get; }

        void Transmute(MazeGrid grid, MazeTransmutationParams parameters);
    }
}
