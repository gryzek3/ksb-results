using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Text.Json;

namespace KSB.Results
{
    public class LicenseRequestsGenerator
    {
        private readonly IGoogleAuthProvider _auth;
        private readonly StartsFromSpreadSheetLoader _resultsLoader;
        private readonly DocumentEditor _documentEditor;
        private readonly AppJsonSerializerContext _appJsonSerializerContext;

        public LicenseRequestsGenerator(IGoogleAuthProvider auth, StartsFromSpreadSheetLoader resultsLoader,
            DocumentEditor documentEditor, AppJsonSerializerContext appJsonSerializerContext)
        {
            _auth = auth;
            _resultsLoader = resultsLoader;
            _documentEditor = documentEditor;
            _appJsonSerializerContext = appJsonSerializerContext;
        }
        private async Task<FilesResource> Init()
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
            var client = new DriveService(initializer);

            return new FilesResource(client);
        }
        [GoogleScopedAuthorize(DriveService.ScopeConstants.DriveReadonly)]
        public async Task<PlayerStartsResult[]> Run()
        {
            var filePath = "results.json";
            PlayerStartsResult[] finalResults = null;
            if (File.Exists(filePath))
            {
                var results = JsonSerializer.Deserialize(File.ReadAllText(filePath), typeof(PlayerStartsResult[]), _appJsonSerializerContext);
                if (results is null)
                {
                    throw new Exception();
                }
                finalResults = (PlayerStartsResult[])results;
            }

            finalResults = await ReadResultsFromGoogleDriveFiles(filePath);

            foreach (var playerStart in finalResults)
            {
                _documentEditor.CreateLicenseRequest(playerStart);
            }
            return finalResults.ToArray();
        }

        private async Task<PlayerStartsResult[]> ReadResultsFromGoogleDriveFiles(string filePath)
        {
            var filesResource = await Init();
            var id = "1z2HvT3G1JzWodRJkAdNkxp-2zocJf6AX";
            var currentYear = DateTime.Now.Year.ToString();
            var years = await ListDirectoryFiles(filesResource, id);
            var currentYearId = years.Single(x => x.Name == currentYear).Id;
            var months = (await ListDirectoryFiles(filesResource, currentYearId)).OrderBy(x => x.Name);
            var results = new List<Player>();
            var roundsDescriptions = new List<Round>(8);
            try
            {
                foreach (var month in months)
                {
                    results = await ProcesMonthResults(filesResource, month, results);
                }

                var finalResults = ProcessPlayersStarts(results.OrderBy(x => x.Name)).ToArray();
                File.WriteAllText(filePath, JsonSerializer.Serialize(finalResults, typeof(PlayerStartsResult[]), _appJsonSerializerContext));
                return finalResults;

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private PlayerStartsResult[] ProcessPlayersStarts(IEnumerable<Player> results)
        {
            return results.Select(x =>
            {
                var mainGunStarts = new PlayerStart[4];
                var secondGunStarts = new PlayerStart[2];
                var thirdGunStarts = new PlayerStart[2];
                var startsByGunType = x.Starts.GroupBy(start => start.GunType).OrderByDescending(starts => starts.Count()).ToArray();
                if (startsByGunType.Length == 0)
                {
                    throw new Exception();
                }
                var mainStarts = startsByGunType.First().ToArray();
                for (var i = 0; i < mainStarts.Length && i < 4; i++)
                {
                    var currentStart = mainStarts[i];
                    mainGunStarts[i] = new PlayerStart($"{currentStart.Round.Name} {currentStart.CompetionName}", currentStart.Round.Dates, currentStart.GunType);
                }
                if (startsByGunType.Length > 1)
                {
                    var secondStarts = startsByGunType.Skip(1).First().ToArray();
                    for (var i = 0; i < secondStarts.Length && i < 2; i++)
                    {
                        var currentStart = secondStarts[i];
                        secondGunStarts[i] = new PlayerStart($"{currentStart.Round.Name} {currentStart.CompetionName}", currentStart.Round.Dates, currentStart.GunType);
                    }
                }
                if (startsByGunType.Length > 2)
                {
                    var thirdStarts = startsByGunType.Skip(2).First().ToArray();
                    for (var i = 0; i < thirdStarts.Length && i < 2; i++)
                    {
                        var currentStart = thirdStarts[i];
                        thirdGunStarts[i] = new PlayerStart($"{currentStart.Round.Name} {currentStart.CompetionName}", currentStart.Round.Dates, currentStart.GunType);
                    }
                }

                return new PlayerStartsResult(x.Name, x.License, mainGunStarts, secondGunStarts, thirdGunStarts);
            }).ToArray();
        }

        private Round LoadRoundData(string monthName)
        {
            var cupName = "PUCHAR PREZESA KSB RUNDA ";
            switch (monthName)
            {
                case "01 Styczeń":
                    return new Round($"{cupName} 1", "09.01.2023 - 13.01.2023");
                case "02 Luty":
                    return new Round($"{cupName} 2", "06.02.2023 - 10.02.2023");
                case "03 Marzec":
                    return new Round($"{cupName} 3", "13.03.2023 - 17.03.2023");
                case "04 Kwiecień":
                    return new Round($"{cupName} 4", "03.04.2023 - 07.04.2023");
                case "09 Wrzesień":
                    return new Round($"{cupName} 5", "11.09.2023 - 15.09.2023");
                case "10 Październik":
                    return new Round($"{cupName} 6", "09.10.2023 - 13.10.2023");
                case "11 Listopad":
                    return new Round($"{cupName} 7", "13.11.2023 - 17.11.2023");
                case "12 Grudzień":
                    return new Round($"{cupName} 8", "11.12.2023 - 15.12.2023");
                default:
                    throw new InvalidOperationException();
            }

        }

        private async Task<List<Player>> ProcesMonthResults(FilesResource filesResource, FileDescription month, List<Player> results)
        {
            var resultsFileId = (await ListDirectoryFiles(filesResource, month.Id)).Single(x => x.Name.Contains("Wyniki"));
            var roundDetails = LoadRoundData(month.Name);
            var loadedResults = await _resultsLoader.ReadDataFromSpreadSheet(resultsFileId.Id, roundDetails);
            foreach (var loadedResult in loadedResults)
            {
                var playerResult = results.SingleOrDefault(x => x.License == loadedResult.License);
                if (playerResult == null)
                {
                    results.Add(loadedResult);
                    continue;
                }
                playerResult.Starts.AddRange(loadedResult.Starts);
            }
            return results;
        }

        private static async Task<IReadOnlyCollection<FileDescription>> ListDirectoryFiles(FilesResource filesResource, string id)
        {
            var listQuery = filesResource.List();
            listQuery.Q = $"'{id}' in parents";
            listQuery.Fields = "files(id, name)";
            listQuery.PageSize = 100;
            try
            {

                var result = await listQuery.ExecuteAsync();
                var names = result.Files.Select(x => new FileDescription(x.Id, x.Name)).ToArray();
                return names;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private record FileDescription(string Id, string Name);
    }

}

