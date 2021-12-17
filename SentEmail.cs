using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Tracer.API.Domain;
using Tracer.API.Helper.AppSetting;

namespace Tracer.API.Helper
{
    public class SentEmail
    {

        private readonly Email emailSetting;

        private Regex regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,6}|[0-9]{1,3})(\]?)$");

        /// <summary>
        /// Initializes a new instance of the <see cref="SentEmail"/> class.
        /// </summary>
        /// <param name="email">The email.</param>
        public SentEmail(IOptions<Email> email)
        {
            emailSetting = email.Value;
        }

        #region SentForgotPasswordMail
        /// <summary>
        /// Sents the forgot password mail.
        /// </summary>
        /// <param name="loginModel">The login model.</param>
        /// <returns></returns>
        public bool SentForgotPasswordMail(Login loginModel)
        {
            try
            {
                MailMessage mail = new MailMessage(SettingsConfig.AppSetting("SentEmail:FromAddress"), SettingsConfig.AppSetting("SentEmail:tomail"), "Login Password", "Hi,<br><br>Your EmailId is:&nbsp;" + loginModel.Email + "<br> Your Password is:&nbsp;" + loginModel.Password + "<br>Kindly Login here:&nbsp<a href='" + SettingsConfig.AppSetting("SentEmail:TracerProjectIP") + "tracer/emplogin.aspx'>Click Here</a> <br><br> With Regards,<br>Branch Admin");
                SmtpClient SmtpServer = new SmtpClient(SettingsConfig.AppSetting("SentEmail:Name"));
                mail.From = new MailAddress(SettingsConfig.AppSetting("SentEmail:FromAddress"));
                mail.IsBodyHtml = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Port = Convert.ToInt32(SettingsConfig.AppSetting("SentEmail:Port"));
                SmtpServer.Credentials = new System.Net.NetworkCredential(SettingsConfig.AppSetting("SentEmail:FromAddress"), SettingsConfig.AppSetting("SentEmail:Password"));
                SmtpServer.EnableSsl = Convert.ToBoolean(SettingsConfig.AppSetting("SentEmail:EnableSsl"));
                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region SendMail
        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="toList">To list.</param>
        /// <param name="from">From.</param>
        /// <param name="ccList">The cc list.</param>
        /// <param name="bccList">The BCC list.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="MailType">Type of the mail.</param>
        /// <returns></returns>
        public string SendMail(string toList, string from, string ccList, string bccList, string subject, string body, string MailType)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            string msg = string.Empty;
            // vcrGlobalIP = objtrcr.GetIPAddress();
            try
            {
                //  from = emailSetting.FromAddress;
                MailAddress fromAddress = new MailAddress(from);
                message.From = fromAddress;
                if (!string.IsNullOrEmpty(toList))
                {
                    char[] seperate = new char[] { ',' };
                    string[] strto = toList.Split(seperate);
                    int legto = strto.Length;
                    for (int a = 0; a < legto; a++)
                    {
                        Match match1 = regex.Match(strto[a].Trim());
                        if (match1.Success)
                        {
                            message.To.Add(strto[a]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(ccList))
                {
                    char[] seperate = new char[] { ',' };
                    string[] strcc = ccList.Split(seperate);
                    int legcc = strcc.Length;
                    for (int a = 0; a < legcc; a++)
                    {
                        Match match2 = regex.Match(strcc[a].Trim());
                        if (match2.Success)
                        {
                            message.CC.Add(strcc[a]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(bccList))
                {
                    char[] sep = new char[] { ',' };
                    string[] strbbc = bccList.Split(sep);
                    int legbcc = strbbc.Length;
                    for (int b = 0; b < legbcc; b++)
                    {
                        Match match3 = regex.Match(strbbc[b].Trim());
                        if (match3.Success)
                        {
                            message.Bcc.Add(strbbc[b]);
                        }
                    }
                }
                message.Bcc.Add(bccList);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;
                smtpClient.Host = SettingsConfig.AppSetting("SentEmail:Name");
                smtpClient.Port = Convert.ToInt32(SettingsConfig.AppSetting("SentEmail:Port"));
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                smtpClient.EnableSsl = Convert.ToBoolean(SettingsConfig.AppSetting("SentEmail:EnableSsl"));
                smtpClient.UseDefaultCredentials = false;
                if (MailType == "ImportEmail")
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(SettingsConfig.AppSetting("SentEmail:ImportEmail"), SettingsConfig.AppSetting("SentEmail:ImportPassword"));
                }
                else if (MailType == "DOEmail")
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(SettingsConfig.AppSetting("SentEmail:DOEmail"), SettingsConfig.AppSetting("SentEmail:DOPassword"));
                }
                else if (MailType == "NoReplyEmail")
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(SettingsConfig.AppSetting("SentEmail:NoReplyEmail"), SettingsConfig.AppSetting("SentEmail:NoReplyPassword"));
                }
                else if (MailType == "SalesEmail")
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(SettingsConfig.AppSetting("SentEmail:SalesEmail"), SettingsConfig.AppSetting("SentEmail:SalesPassword"));
                }
                else if (MailType == "BusinessEmail")
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(SettingsConfig.AppSetting("SentEmail:BusinessEmail"), SettingsConfig.AppSetting("SentEmail:BusinessPassword"));
                }
                else if (MailType == "ShipmentEmail")
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(SettingsConfig.AppSetting("SentEmail:ShipmentEmail"), SettingsConfig.AppSetting("SentEmail:ShipmentPassword"));
                }
                else if (MailType == "EconTracerEmail")
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(SettingsConfig.AppSetting("SentEmail:EconTracerEmail"), SettingsConfig.AppSetting("SentEmail:EconTracerPassword"));
                }
                else if (MailType == "TSEmail")
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(SettingsConfig.AppSetting("SentEmail:TSEmail"), SettingsConfig.AppSetting("SentEmail:TSPassword"));
                }
                else
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(SettingsConfig.AppSetting("SentEmail:NoReplyEmail"), SettingsConfig.AppSetting("SentEmail:NoReplyPassword"));
                }
                smtpClient.Send(message);
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
            }
            return msg;
        }

        #endregion

        #region SendMailWithAttachment
        /// <summary>
        /// Sends the mail with attachment.
        /// </summary>
        /// <param name="toList">To list.</param>
        /// <param name="from">From.</param>
        /// <param name="ccList">The cc list.</param>
        /// <param name="bccList">The BCC list.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="attachment">The attachment.</param>
        /// <param name="Filename">The filename.</param>
        /// <returns></returns>
        public MessageData SendMailWithAttachment(string toList, string from, string ccList, string bccList, string subject, string body, Stream attachment, string Filename)
        {
            MessageData messageData = new MessageData();
            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient(SettingsConfig.AppSetting("MailSetting:SMTPHost"), Convert.ToInt32(SettingsConfig.AppSetting("MailSetting:Port")));
            string msg = string.Empty;
            MailAddress fromAddress = new MailAddress(from);
            if (toList != null)
            {
                foreach (var address in toList.Split(','))
                {
                    if (address != "")
                    {
                        message.To.Add(address);
                    }
                }
            }
            if (ccList != null)
            {
                foreach (var address in ccList.Split(','))
                {
                    if (address != "")
                    {
                        message.CC.Add(address);
                    }
                }
            }
            if (bccList != null)
            {
                foreach (var address in bccList.Split(','))
                {
                    if (address != "")
                    {
                        message.Bcc.Add(address);
                    }
                }
            }
            message.From = fromAddress;
            message.Subject = subject;
            message.IsBodyHtml = Convert.ToBoolean(SettingsConfig.AppSetting("MailSetting:IsMessageBodyHtml"));
            message.Body = body;
            //Attachment att = new Attachment(attachment);
            //att.TransferEncoding = TransferEncoding.Base64;
            message.Attachments.Add(new Attachment(attachment, Filename + ".png"));
            smtpClient.EnableSsl = Convert.ToBoolean(SettingsConfig.AppSetting("MailSetting:SmtpEnableSsl"));
            smtpClient.UseDefaultCredentials = Convert.ToBoolean(SettingsConfig.AppSetting("MailSetting:SmtpUseDefaultCredentials"));
            message.SubjectEncoding = Encoding.UTF8;
            message.BodyEncoding = Encoding.UTF8;
            smtpClient.Credentials = new NetworkCredential(SettingsConfig.AppSetting("MailSetting:SmtpCredentialId"), SettingsConfig.AppSetting("MailSetting:SmtpCredentialPassword"));
            smtpClient.Send(message);
            messageData = new MessageData("alert-success", "Success!", "Sent mail successfully!!");
            return messageData;
        }
        #endregion

        #region SendMailModel
        /// <summary>
        /// SendMailModel
        /// </summary>
        public class SendMailModel
        {
            public string Attachment { get; set; }
            public string Refid { get; set; }
            public int Userid { get; set; }
            public string Toemailid { get; set; }
            public string BillNo { get; set; }
            public bool ManualMailIds { get; set; }
            public string InvoiceType { get; set; }
        }
        #endregion

        #region MessageData
        /// <summary>
        /// MessageData
        /// </summary>
        public class MessageData
        {
            public MessageData()
            {

            }

            public MessageData(string status, string msgType, string message)
            {
                this.Message = message;
                this.MsgType = msgType;
                this.Status = status;
            }

            public string Status { get; set; }
            public string MsgType { get; set; }
            public string Message { get; set; }
        }
        #endregion

        #region GetAttachmentFromStream
        /// <summary>
        /// Gets the attachment from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="BillNo">The bill no.</param>
        /// <returns></returns>
        public Attachment GetAttachmentFromStream(Stream stream, string BillNo)
        {
            Attachment attachment = new Attachment(stream, BillNo + ".pdf", "application/pdf");
            ContentDisposition disposition = attachment.ContentDisposition;
            disposition.CreationDate = System.DateTime.Now;
            disposition.ModificationDate = System.DateTime.Now;
            disposition.DispositionType = DispositionTypeNames.Attachment;
            return attachment;
        }
        #endregion

        #region PopulateEmailBody
        /// <summary>
        /// Populates the email body.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string PopulateEmailBody(string path)
        {
            string body = string.Empty;
            string serverPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            serverPath = serverPath.Replace("\\bin\\Debug\\net5.0", "");
            serverPath = serverPath + "\\wwwroot" + path;
            using (StreamReader reader = new StreamReader(serverPath))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }
        #endregion
    }

}
