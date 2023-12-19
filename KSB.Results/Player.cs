namespace KSB.Results
{
    public record Player(string Name, string License)
    {
        public List<CompetitionStart> Starts { get; } = new List<CompetitionStart>();
    }
    public record PlayerStartsResult(string Name, string License,
        PlayerStart[] MainGunStarts,
        PlayerStart[] SecondGunStarts,
        PlayerStart[] ThirdGunStarts)
    {

    }
    public record PlayerStart(string CompetitionName, string CompetitionDate, GunType GunType);
}

