using FluentAssertions;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Services;
using Xunit;


namespace TicTacToe.Tests
{
    public class ScoreboardServiceTests
    {
        [Fact]
        public void RecordResult_IncrementsXWins_ForXVictory()
        {
            var service = new ScoreboardService();
            service.RecordResult(GameStatus.Won, Player.X);

            service.GetScoreboard().XWins.Should().Be(1);
        }

        [Fact]
        public void RecordResult_IncrementsDraws_ForDraw()
        {
            var service = new ScoreboardService();
            service.RecordResult(GameStatus.Draw, null);

            service.GetScoreboard().Draws.Should().Be(1);
        }

        [Fact]
        public void ResetScoreboard_ZeroesAllCounters()
        {
            var service = new ScoreboardService();
            service.RecordResult(GameStatus.Won, Player.X);
            service.RecordResult(GameStatus.Won, Player.O);
            service.RecordResult(GameStatus.Draw, null);

            service.ResetScoreboard();

            var board = service.GetScoreboard();
            board.XWins.Should().Be(0);
            board.OWins.Should().Be(0);
            board.Draws.Should().Be(0);
        }
    }
}
