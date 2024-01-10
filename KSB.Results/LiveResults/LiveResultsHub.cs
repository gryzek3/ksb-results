using KSB.Results.Db;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
        public LiveResultsHub(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        private readonly DataContext _dataContext;

        public async Task SendResult(PlayerRunResult result)
        {
            await _dataContext.AddAsync(new StartResult
            {
                Player = result.playerName,
                Course = result.courseName,
                Factor = result.result.Factor,
                Points = result.result.Points,
                TensCount = result.result.TensCount,
                TimeStamp = DateTime.UtcNow,
                Time = result.result.Time,

            });
            await _dataContext.SaveChangesAsync();
            var newResults = await _dataContext.StartResults.OrderByDescending(x => x.TimeStamp)
                .Where(x => x.TimeStamp >= DateTime.UtcNow.AddHours(-1))
                .Select(x => new PlayerRunResult
                 (x.Player,
                    x.Course,
                     new SingleResult
                    (
                        x.Points, x.TensCount, x.Factor, x.Time)
                )).ToArrayAsync();
            await Clients.All.SendAsync("ResultReceived", newResults);
        }
    }
}
