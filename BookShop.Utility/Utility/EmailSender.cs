using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace BookShop.Utility.Utility;

public class EmailSender : IEmailSender
{
    public MailJetOptions mailJetOptions { get; set; }

    public EmailSender(IOptions<MailJetOptions> mailJetOptions)
    {
        this.mailJetOptions = mailJetOptions.Value;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Execute(email, subject, htmlMessage);
    }

    public async Task Execute(string email, string subject, string body)
    {
        MailjetClient client = new MailjetClient(mailJetOptions.ApiKey, mailJetOptions.SecretKey)
        {
          /*  Version = ApiVersion.V3_1,*/
        };
        MailjetRequest request = new MailjetRequest
        {
            Resource = Send.Resource,
        }
         .Property(Send.Messages, new JArray {
              new JObject {
              {
               "From",
               new JObject {
                {"Email", "bogdanskafa@gmail.com"},
                {"Name", "Bohdan"}
               }
              }, {
               "To",
               new JArray {
                new JObject {
                 {
                  "Email",
                  "bogdanskafa@gmail.com"
                 }, {
                  "Name",
                  "Bohdan"
                 }
                }
               }
              }, {
               "Subject",
               "Greetings from Mailjet."
              }, {
               "TextPart",
               "My first Mailjet email"
              }, {
               "HTMLPart",
               "<h3>Dear passenger 1, welcome to <a href='https://www.mailjet.com/'>Mailjet</a>!</h3><br />May the delivery force be with you!"
              }, {
               "CustomID",
               "AppGettingStartedTest"
              }
             }
            });
        MailjetResponse response = await client.PostAsync(request);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
            Console.WriteLine(response.GetData());
        }
        else
        {
            Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
            Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
            Console.WriteLine(response.GetData());
            Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
        }
    }
}