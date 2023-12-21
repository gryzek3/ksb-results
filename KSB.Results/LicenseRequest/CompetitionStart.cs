namespace KSB.Results.LicenseRequest
{

    public record CompetitionStart(string CompetionName, GunType GunType, Round Round);
    public record Round(string Name, string Dates);
}

