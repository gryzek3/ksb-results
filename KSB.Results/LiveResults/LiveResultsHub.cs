using Microsoft.AspNetCore.SignalR;
using Reinforced.Typings.Attributes;

namespace KSB.Results.LiveResults
{
    [TsInterface]
    public record SingleResult(int? Points,
        int? TensCount,
        double? Factor,
        double? Time);

    [TsInterface]
    public record PlayerRunResult(string playerName, string courseName, SingleResult result);
    public class LiveResultsHub : Hub
    {
        private static List<PlayerRunResult> _results = new List<PlayerRunResult>();
        public async Task SendResult(PlayerRunResult result)
        {
            if (_results.Count > 10)
            {
                _results.RemoveRange(10, _results.Count - 10);
            }
            _results.Add(result);
            await Clients.All.SendAsync("ResultReceived", _results);
        }
    }
}
