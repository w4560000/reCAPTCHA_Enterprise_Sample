using Google.Api.Gax.ResourceNames;
using Google.Cloud.RecaptchaEnterprise.V1;

namespace reCAPTCHA_Enterprise_Sample.Service
{
    public class CreateAssessmentSample
    {
        public RecaptchaEnterpriseServiceClient GetRecaptchaEnterpriseServiceClient_SetEnv()
        {
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"D:\Lab\reCAPTCHA_Enterprise_Sample\service-account-file.json");
            return RecaptchaEnterpriseServiceClient.Create();
        }

        public RecaptchaEnterpriseServiceClient GetRecaptchaEnterpriseServiceClient_SetCredentialsPath()
        {
            return new RecaptchaEnterpriseServiceClientBuilder()
            {
                CredentialsPath = @"D:\Lab\reCAPTCHA_Enterprise_Sample\service-account-file.json"
            }.Build();
        }

        // Create an assessment to analyze the risk of an UI action.
        // projectID: GCloud Project ID.
        // recaptchaSiteKey: Site key obtained by registering a domain/app to use recaptcha.
        // token: The token obtained from the client on passing the recaptchaSiteKey.
        // recaptchaAction: Action name corresponding to the token.
        public decimal createAssessment(string projectID = "project-id", string recaptchaSiteKey = "recaptcha-site-key",
            string token = "action-token", string recaptchaAction = "action-name")
        {
            // Create the client.
            // TODO: To avoid memory issues, move this client generation outside
            // of this example, and cache it (recommended) or call client.close()
            // before exiting this method.

            // 若在GCP 機器環境的服務帳戶 含有 recaptchaenterprise.assessments.create 權限 則可以直接呼叫 不需憑證
            RecaptchaEnterpriseServiceClient client = RecaptchaEnterpriseServiceClient.Create();

            // 若在地端環境 或 非 GCP 機器環境，則可以將憑證下載至機器上，呼叫時在設定憑證路徑
            //RecaptchaEnterpriseServiceClient client = GetRecaptchaEnterpriseServiceClient_SetEnv(); // 透過環境變數
            //RecaptchaEnterpriseServiceClient client = GetRecaptchaEnterpriseServiceClient_SetCredentialsPath(); // 設定 CredentialsPath

            ProjectName projectName = new ProjectName(projectID);

            // Build the assessment request.
            CreateAssessmentRequest createAssessmentRequest = new CreateAssessmentRequest()
            {
                Assessment = new Assessment()
                {
                    // Set the properties of the event to be tracked.
                    Event = new Event()
                    {
                        SiteKey = recaptchaSiteKey,
                        Token = token,
                        ExpectedAction = recaptchaAction
                    },
                },
                ParentAsProjectName = projectName
            };

            Assessment response = client.CreateAssessment(createAssessmentRequest);

            // Check if the token is valid.
            if (response.TokenProperties.Valid == false)
            {
                System.Console.WriteLine("The CreateAssessment call failed because the token was: " +
                    response.TokenProperties.InvalidReason.ToString());
                return 0;
            }

            // Check if the expected action was executed.
            if (response.TokenProperties.Action != recaptchaAction)
            {
                System.Console.WriteLine("The action attribute in reCAPTCHA tag is: " +
                    response.TokenProperties.Action.ToString());
                System.Console.WriteLine("The action attribute in the reCAPTCHA tag does not " +
                    "match the action you are expecting to score");
                return 0;
            }

            // Get the risk score and the reason(s).
            // For more information on interpreting the assessment,
            // see: https://cloud.google.com/recaptcha-enterprise/docs/interpret-assessment
            System.Console.WriteLine("The reCAPTCHA score is: " + ((decimal)response.RiskAnalysis.Score));

            foreach (RiskAnalysis.Types.ClassificationReason reason in response.RiskAnalysis.Reasons)
            {
                System.Console.WriteLine(reason.ToString());
            }

            return (decimal)response.RiskAnalysis.Score;
        }
    }
}