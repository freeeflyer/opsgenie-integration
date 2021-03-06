public class OpsGenieAlertTrigger : ActiveTrigger<Force__Case>
{
	public override void Execute(TransactionContext<Force__Case> trigger)
	{
	    const string OG_API_KEY = "<YOUR OPSGENIE MAGENTRIX INTEGRATION API KEY>";
        const string OG_URL = "https://api.opsgenie.com/v1/json/magentrix?apiKey=" + OG_API_KEY;

	    if (trigger.IsAfter)
        {
            foreach (var rec in trigger.Records)
            {
                var caseId = rec.New.Id;
                var caseDescription = rec.New.Case_Description__c;
                var severity = rec.New.Case_Severity__c;
                var caseNumber = rec.New.CaseNumber;
                var description = rec.New.Description;
                var priority = rec.New.Priority;
                var priorityValue = "";

                if (priority != null)
                {
                    priorityValue = priority.ToString();
                }

                var subject = rec.New.Subject;
                var caseType = rec.New.Type;
                var ownerEmail = "";

                if (!string.IsNullOrEmpty(rec.New.Magentrix_Owner__c)) {
                    var ownerIds = trigger.Records.Where(a => a.New != null).Select(a => a.New.Magentrix_Owner__c).ToList();
                    var owners = Database.Query<Force__User>(a => ownerIds.Contains(a.Id)).Select(a => new { a.Id, a.Name, a.Email }).ToListAsAdmin();
                    var owner = owners.FirstOrDefault(a => a.Id == rec.New.Magentrix_Owner__c);
    
                    if (owner != null)
                    {
                        ownerEmail = owner.Email;
                    }
                }
                
                var caseStatus = rec.New.Status;
                var caseStatusValue = "";
                
                if (caseStatus != null)
                {
                    caseStatusValue = caseStatus.ToString();
                }

                var accountName = "";

                if ((rec.New.Account != null) && (!string.IsNullOrEmpty(rec.New.Account.Name))) {
                    accountName = rec.New.Account.Name;
                }
                
                var assetName = "";
                
                if((rec.New.Asset != null) && (!string.IsNullOrEmpty(rec.New.Asset.Name))) {
                    assetName = rec.New.Asset.Name;
                }

                System.Net.WebClient client = new System.Net.WebClient();
                client.Headers.Add("Content-Type","application/json");

                var postData = "{";
                postData += "\"caseId\": \"" + sanitizePayload(caseId) + "\",";
                postData += "\"caseDescription\": \"" + sanitizePayload(caseDescription) + "\",";
                postData += "\"severity\": \"" + sanitizePayload(severity) + "\",";
                postData += "\"caseNumber\": \"" + sanitizePayload(caseNumber) + "\",";
                postData += "\"description\": \"" + sanitizePayload(description) + "\",";
                postData += "\"priority\": \"" + sanitizePayload(priority) + "\",";
                postData += "\"subject\": \"" + sanitizePayload(subject) + "\",";
                postData += "\"caseType\": \"" + sanitizePayload(caseType) + "\",";
                postData += "\"ownerEmail\": \"" + sanitizePayload(ownerEmail) + "\",";
                postData += "\"caseStatus\": \"" + sanitizePayload(caseStatusValue) + "\",";
                postData += "\"accountName\": \"" + sanitizePayload(accountName) + "\",";
                postData += "\"assetName\": \"" + sanitizePayload(assetName) + "\"";
                postData += "}";

                client.UploadString(OG_URL, postData);
            }
        }
	}
	
    private String sanitizePayload(String str)
    {
        if (str == null)
        {
            return null;
        }

        str = str.Replace("\"", "\\\"");
        str = str.Replace("\r\n", " ");
        str = str.Replace("\n", " ");
        
        return str;
    }
}
