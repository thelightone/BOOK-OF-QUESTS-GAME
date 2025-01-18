//using Google.Apis.Auth.OAuth2;
////using Google.Apis.Services;
//using Google.Apis.Sheets.v4;
//using Google.Apis.Sheets.v4.Data;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading;

//public class GoogleSheetsHelper
//{
//    private static string[] Scopes = { SheetsService.Scope.Spreadsheets };
//    private static string ApplicationName = "SQLite to Google Sheets";
//    private SheetsService service;

//    public GoogleSheetsHelper(string credentialsFilePath)
//    {
//        GoogleCredential credential;
//        using (var stream = new FileStream(credentialsFilePath, FileMode.Open, FileAccess.Read))
//        {
//            credential = GoogleCredential.FromStream(stream)
//                .CreateScoped(Scopes);
//        }

//       // service = new SheetsService(new BaseClientService.Initializer()
//        {
//            HttpClientInitializer = credential,
//            ApplicationName = ApplicationName,
//        });
//    }

//    public void UpdateSheet(string spreadsheetId, string range, List<IList<object>> values)
//    {
//        var valueRange = new ValueRange { Values = values };
//        var request = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
//        request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
//        request.Execute();
//    }
//}
