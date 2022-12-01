﻿using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Message
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 3) {
                throw new ArgumentException("Main function requires the message recipient " +
                    "address, the message subject, and the message body arguments respectively.");
            }
            var host = CreateDefaultBuilder().Build();
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            var workerInstance = provider.GetRequiredService<MessageWorker>();
            workerInstance.CreateMessage(args);
        }

        static IHostBuilder CreateDefaultBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(app =>
                {
                    app.AddJsonFile("appsettings.json");
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<MessageWorker>();
                });
        }

        internal class MessageWorker
        {
            private const int MAX_ATTEMPTS = 3;
            private readonly IConfiguration configuration;
            private readonly string? smtpClientServer;
            private readonly int smtpClientPort;
            private readonly string? smtpClientLogin;
            private readonly string? smtpClientPassword;

            public MessageWorker(IConfiguration configuration)
            {
                this.configuration = configuration;
                smtpClientServer = configuration["smtpClientServer"];
                smtpClientLogin = configuration["smtpClientLogin"];
                smtpClientPassword = configuration["smtpClientPassword"];
                string? smtpClientPortString = configuration["smtpClientPort"];
                if (smtpClientPortString is not null)
                {
                    smtpClientPort = Int32.Parse(smtpClientPortString);
                }
            }

            internal void CreateMessage(string[] args)
            {
                string messageRecipientAddress = args[0];
                string messageSubject = args[1];
                string messageBody = args[2];

                SmtpClient client = new SmtpClient(smtpClientServer, smtpClientPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpClientLogin, smtpClientPassword),
                };

                if (smtpClientLogin is null)
                {
                    Console.WriteLine("Client login information is invalid");
                    return;
                }

                MailAddress from = new MailAddress(smtpClientLogin, "Do not reply");
                MailAddress to = new MailAddress(messageRecipientAddress);
                MailMessage message = new MailMessage(from, to)
                {
                    Body = messageBody,
                    Subject = messageSubject
                };

                SendMessage(client, message);
                
                void SendMessage(SmtpClient client, MailMessage message, int attemptNumber = 0)
                {
                    if (message.From is null)
                    {
                        Console.WriteLine("MailAddress From is null. Aborting");
                        return;
                    }

                    Console.WriteLine("Attempting to send message...");
                    try
                    {
                        client.Send(message);
                        Console.WriteLine("Message delivered");
                        using (StreamWriter w = File.AppendText("log.csv"))
                        {
                            w.WriteLine(string.Join(",", new string[] 
                            { 
                                "Sent", 
                                message.From.Address.ToString(), 
                                message.To.ToString(), message.Subject, 
                                message.Body, DateTime.Today.ToString("dd/MM/yyyy") 
                            }));
                        }
                    }

                    catch (Exception e)
                    {
                        using (StreamWriter w = File.AppendText("log.csv"))
                        {
                            w.WriteLine(string.Join(",", new string[] 
                            { 
                                "Not Sent", 
                                message.From.Address.ToString(), 
                                message.To.ToString(), message.Subject, 
                                message.Body, DateTime.Today.ToString("dd/MM/yyyy") 
                            }));
                        }
                        Console.WriteLine(e.ToString());
                        Console.WriteLine("Message failed to send");
                        if (attemptNumber < (MAX_ATTEMPTS - 1))
                        {
                            Console.WriteLine("Retrying");
                            SendMessage(client, message, attemptNumber + 1);
                        }

                        else
                        {
                            Console.WriteLine("Attempted to send " + MAX_ATTEMPTS + 
                                " times. Aborting");
                        }
                    }
                }
                
            }
        }
    }
}