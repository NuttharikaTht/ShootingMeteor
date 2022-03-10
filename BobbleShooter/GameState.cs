using System;
namespace BobbleShooter
{
    public enum GameState
    {
        play, // play #initial state
        lose, // lose #if bubble near limit
        win, // win #if no bubble 
    }
}
