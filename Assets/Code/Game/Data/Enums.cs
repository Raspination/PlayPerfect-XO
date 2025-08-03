namespace PlayPerfect.XO.Data
{
    public class Enums
    {
        public enum CellState
        {
            Empty,
            X,
            O
        }

        public enum GameState
        {
            Playing,
            PlayerWin,
            ComputerWin,
            Draw
        }
    }
}