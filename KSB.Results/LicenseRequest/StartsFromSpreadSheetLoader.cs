using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace KSB.Results.LicenseRequest
{
    public class StartsFromSpreadSheetLoader
    {
        private readonly IGoogleAuthProvider _auth;

        public StartsFromSpreadSheetLoader(IGoogleAuthProvider auth)
        {
            _auth = auth;
        }

        [GoogleScopedAuthorize(SheetsService.ScopeConstants.SpreadsheetsReadonly)]
        public async Task<IReadOnlyCollection<Player>> ReadDataFromSpreadSheet(string fileId, Round roundDetails)
        {
            var data = await Load(fileId, "A1:AD206");
            var competitions = ReadCompetitions(data[0]);
            var starts = ReadStarts(competitions, data, roundDetails);
            return starts.ToArray();
        }

        private IReadOnlyCollection<Player> ReadStarts(IReadOnlyCollection<Competition> competitions, IList<IList<object>> data, Round roundDetails)
        {
            var players = new List<Player>(data.Count / 2);
            for (int rowIndex = 3; rowIndex + 1 < data.Count; rowIndex += 2)
            {
                var firstRow = data[rowIndex];
                var secondRow = data[rowIndex + 1];
                if (string.IsNullOrWhiteSpace(firstRow[2].ToString()))
                {
                    return players;
                }
                var playerName = $"{firstRow[2]}, {firstRow[3]}";

                var player = new Player(playerName, firstRow[5].ToString()!.ToUpper());
                for (int columnIndex = 6; columnIndex + 1 < secondRow.Count; columnIndex += 2)
                {

                    var start = secondRow[columnIndex + 1].ToString();
                    if (string.IsNullOrWhiteSpace(start))
                    {
                        continue;
                    }
                    var competition = competitions.Single(x => x.ColumnIndex == columnIndex);
                    player.Starts.Add(new CompetitionStart(competition.Name, competition.GunType, roundDetails));
                }
                if (player.Starts.Count == 0)
                {
                    continue;
                }
                players.Add(player);
            }
            return players;
        }

        private IReadOnlyCollection<Competition> ReadCompetitions(IList<object> data)
        {
            var result = new List<Competition>(12);
            var index = 6;
            for (int i = 6; i < data.Count; i += 2)
            {
                var name = data[i].ToString();
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }
                var gunType = GetGunType(name);
                result.Add(new Competition(name, gunType, i));
            }
            return result;

        }

        private GunType GetGunType(string name)
        {
            if (name.StartsWith("Pistolet"))
                return GunType.Pistol;
            if (name.StartsWith("Karabin"))
                return GunType.Rifle;
            if (name.StartsWith("Strzelba"))
                return GunType.Shotgun;
            throw new NotImplementedException();
        }

        [GoogleScopedAuthorize(SheetsService.ScopeConstants.SpreadsheetsReadonly)]
        private async Task<IList<IList<object>>> Load(string fileId, string range)
        {
            var initializer = new BaseClientService.Initializer()
            {
                HttpClientInitializer = await _auth.GetCredentialAsync(),
                ApplicationName = "KSB.Results",
            };
            if (initializer == null)
            {
                throw new InvalidOperationException();
            }
            var service = new SheetsService(initializer);
            var getRequest = service.Spreadsheets.Values.Get(fileId, range);
            var result = await getRequest.ExecuteAsync();
            return result.Values;
        }

    }

}

