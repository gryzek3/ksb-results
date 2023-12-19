namespace KSB.Results
{

    public record CompetitionStart(string CompetionName, GunType GunType, Round Round);
    public record Round(string Name, string Dates);
}

