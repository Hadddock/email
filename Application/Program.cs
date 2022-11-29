using System.Net;
using System.Net.Mail;

namespace Examples.SmtpExamples.Async
{
    public class SimpleAsynchronousExample
    {
        public const int MAX_ATTEMPTS = 3;
        public static void Main(string[] args)
        {
            string messageRecipientAddress = args[0];
            string messageSubject = args[1];
            string messageBody = args[2];
            int attemptNumber = 0;

            if(args.Length == 4) {
                attemptNumber = Int32.Parse(args[3]);
            }

            //Client info
            SmtpClient client = new SmtpClient("smtp-relay.sendinblue.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential("text.mail.ex@gmail.com","OZ1aBEbPHjkLD5G6"),
            };

            //Message info
            MailAddress from = new MailAddress("text.mail.ex@gmail.com", "Do not reply");
            MailAddress to = new MailAddress(messageRecipientAddress);
            MailMessage message = new MailMessage(from, to)
            {
                Body = messageBody,
                Subject = messageSubject
            };
           
            Console.WriteLine("Sending message...");
            try {
                client.Send(message);
                Console.WriteLine("Message delivered");
            }

            catch(Exception e){
                Console.WriteLine(e.ToString());
                Console.WriteLine("Message failed to send");
                if(attemptNumber < (MAX_ATTEMPTS - 1)){
                    Console.WriteLine("Retrying");
                    Main(new string[] { messageRecipientAddress, messageBody, messageSubject, (attemptNumber + 1).ToString() });
                }
                else {
                    Console.WriteLine("Attempted to send " + MAX_ATTEMPTS +" times, aborting");
                }
            }
            message.Dispose();
        }
    }
}