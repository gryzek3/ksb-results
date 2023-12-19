using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace KSB.Results
{
    public class DocumentEditor
    {
        public void CreateLicenseRequest(PlayerStartsResult result)
        {
            try
            {
                var fileName = $"{result.License}.docx";
                File.Copy("wniosek-o-przedluzenie-licencji-zawodniczej.docx", fileName, true);
                using WordprocessingDocument docPackage = WordprocessingDocument.Open(fileName, true);
                var allGunTypes = result.MainGunStarts.Where(x => x is not null).Select(x => x.GunType).Distinct().ToList();
                allGunTypes.AddRange(result.SecondGunStarts.Where(x => x is not null).Select(x => x.GunType).Distinct());
                allGunTypes.AddRange(result.ThirdGunStarts.Where(x => x is not null).Select(x => x.GunType).Distinct());

                var mainPart = docPackage.MainDocumentPart;
                var gunsElement = mainPart.Document.Body.ChildElements.First(x => x.InnerText.Contains("XP"));
                var pistolElement = gunsElement.ChildElements.First(x => x.InnerText.Contains("XP"));
                ReplaceGunTyp(allGunTypes, pistolElement, GunType.Pistol, "XP");
                var rifleElement = gunsElement.ChildElements.First(x => x.InnerText.Contains("XK"));
                ReplaceGunTyp(allGunTypes, rifleElement, GunType.Rifle, "XK");
                var shoutGunElement = gunsElement.ChildElements.First(x => x.InnerText.Contains("XS"));
                ReplaceGunTyp(allGunTypes, shoutGunElement, GunType.Shotgun, "XS");

                var table = mainPart.Document.Body.ChildElements.First(x => x.InnerText.Contains("Z1"));
                for (int i = 0; i < result.MainGunStarts.Length; i++)
                {
                    PlayerStart? start = result.MainGunStarts[i];
                    FillStartInRequest(table, i, start);

                }
                for (int i = 0; i < result.SecondGunStarts.Length; i++)
                {
                    PlayerStart? start = result.SecondGunStarts[i];
                    FillStartInRequest(table, i + 4, start);

                }
                for (int i = 0; i < result.ThirdGunStarts.Length; i++)
                {
                    PlayerStart? start = result.ThirdGunStarts[i];
                    FillStartInRequest(table, i + 6, start);

                }

                docPackage.Save();
            }
            catch (Exception e)
            {

                throw e;
            }


        }

        private void FillStartInRequest(OpenXmlElement table, int i, PlayerStart? start)
        {
            if (start == null)
            {
                FindChildElementWithText(table, $"Z{i + 1}", "");
                FindChildElementWithText(table, $"D{i + 1}", "");
                FindChildElementWithText(table, $"M{i + 1}", "");

                FindChildElementWithText(table, $"P{i + 1}", "");
                FindChildElementWithText(table, $"K{i + 1}", "");
                FindChildElementWithText(table, $"S{i + 1}", "");
                FindChildElementWithText(table, $"C{i + 1}", "");
                return;
            }
            FindChildElementWithText(table, $"Z{i + 1}", start.CompetitionName);
            FindChildElementWithText(table, $"D{i + 1}", start.CompetitionDate);
            FindChildElementWithText(table, $"M{i + 1}", "Paczółtowice");

            FindChildElementWithText(table, $"P{i + 1}", start.GunType == GunType.Pistol ? "X" : "");
            FindChildElementWithText(table, $"K{i + 1}", start.GunType == GunType.Rifle ? "X" : "");
            FindChildElementWithText(table, $"S{i + 1}", start.GunType == GunType.Shotgun ? "X" : "");
            FindChildElementWithText(table, $"C{i + 1}", "WZSS");
        }

        private void FindChildElementWithText(OpenXmlElement element, string lookForText, string replaceWith)
        {
            var found = element.ChildElements.FirstOrDefault(x => x.InnerText.Contains(lookForText));
            if (found is Text && found != null)
            {
                ((Text)found).Text = replaceWith;
                return;
            }
            if (found is null)
            {
                throw new Exception("Found is null");
            }
            FindChildElementWithText(found, lookForText, replaceWith);
        }

        private static void ReplaceGunTyp(List<GunType> allGunTypes, DocumentFormat.OpenXml.OpenXmlElement pistolElement, GunType gunType, string textToBeReplaced)
        {
            var textElement = (Text)pistolElement.ChildElements.First(x => x.GetType() == typeof(Text));
            if (allGunTypes.Contains(gunType))
            {
                textElement.Text = textElement.Text.Replace(textToBeReplaced, "X");
            }
            else
            {
                textElement.Text = textElement.Text.Replace(textToBeReplaced, "");
            }
        }
    }

}

