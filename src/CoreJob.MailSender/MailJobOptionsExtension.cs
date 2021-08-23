using CoreJob.Framework;
using CoreJob.Framework.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using System;

namespace CoreJob.MailSender
{
    public class MailJobOptionsExtension : IJobOptionsExtension
    {
        private readonly IConfiguration _configuration;

        public MailJobOptionsExtension(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void AddServices(IServiceCollection services)
        {
            var options = new MailOptions();
            services.AddOptionConfiguration(_configuration, options);
            services.AddSingleton(options);

            services.AddMailKit(optionBuilder =>
            {
                optionBuilder.UseMailKit(new MailKitOptions()
                {
                    //get options from sercets.json
                    Server = options.SmtpServer,
                    Port = Convert.ToInt32(options.SmtpPort),
                    SenderName = options.SenderName,
                    SenderEmail = options.SenderMail,

                    //can be optional with no authentication 
                    Account = options.Account,
                    Password = options.Password
                });
            });
        }
    }
}
