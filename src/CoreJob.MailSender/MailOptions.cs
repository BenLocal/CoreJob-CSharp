using System;
using System.Collections.Generic;
using System.Text;

namespace CoreJob.MailSender
{
    public class MailOptions
    {
        /// <summary>
        /// mail server
        /// smtp.friends.com
        /// </summary>
        public string SmtpServer { get; set; }

        /// <summary>
        /// mail server port
        /// 587
        /// </summary>
        public string SmtpPort { get; set; }

        /// <summary>
        /// 发送人名称
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 发送人邮件
        /// </summary>
        public string SenderMail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Account { get; set; }

        public string Password { get; set; }
    }
}
