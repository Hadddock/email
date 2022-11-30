using System.Net;
using System.Net.Mail;
using System.Reflection;

public class Program
{
    public const int MAX_ATTEMPTS = 3;
    private static string smtpClientServer = "";
    private static int smtpClientPort = -1;
    private static string smtpClientLogin = "";
    private static string smtpClientPassword = "";

    public static void Main(string[] args)
    {

        string messageRecipientAddress = args[0];
        string messageSubject = args[1];
        string messageBody = args[2];
        int attemptNumber = 0;



        if (args.Length == 4)
        {
            attemptNumber = Int32.Parse(args[3]);
        }

        if (smtpClientServer == "") {
            string appsettingsPath = System.Reflection.Assembly.GetCallingAssembly().CodeBase; 
            Console.WriteLine(appsettingsPath);
            //string jsonString = File.ReadAllText();
            return;
        }

    //Client info
    SmtpClient client = new SmtpClient(smtpClientServer, smtpClientPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(smtpClientLogin, smtpClientPassword),
        };

        //Message info
        MailAddress from = new MailAddress(smtpClientLogin, "Do not reply");
        MailAddress to = new MailAddress(messageRecipientAddress);
        MailMessage message = new MailMessage(from, to)
        {
            Body = messageBody,
            Subject = messageSubject
        };

        Console.WriteLine("Sending message...");

        try
        {
            client.Send(message);
            Console.WriteLine("Message delivered");
        }

        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            Console.WriteLine("Message failed to send");
            if (attemptNumber < (MAX_ATTEMPTS - 1))
            {
                Console.WriteLine("Retrying");
                Main(new string[] { messageRecipientAddress, messageBody, messageSubject, (attemptNumber + 1).ToString() });
            }
            else
            {
                Console.WriteLine("Attempted to send " + MAX_ATTEMPTS + " times, aborting");
            }
        }
    }

}

