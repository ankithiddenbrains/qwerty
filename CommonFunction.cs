using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tracer.API.Helper.AppSetting;

namespace Tracer.API.Helper
{
    public class CommonFunction
    {

        #region Encrypt
        /// <summary>
        /// Encrypts the specified plain text.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns>Encrypt string</returns>
        public string Encrypt(string plainText)
        {
            MemoryStream mStream = new MemoryStream();
            try
            {
                string key = "jdsg432387#";
                byte[] EncryptKey = { };
                byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
                EncryptKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByte = Encoding.UTF8.GetBytes(plainText);

                CryptoStream cStream = new CryptoStream(mStream, des.CreateEncryptor(EncryptKey, IV), CryptoStreamMode.Write);
                cStream.Write(inputByte, 0, inputByte.Length);
                cStream.FlushFinalBlock();
            }
            catch (Exception Ex)
            {
            }
            string Encrypt = Convert.ToBase64String(mStream.ToArray()).Replace("/", "");
            Encrypt = Encrypt.Replace("+", "");
            return Encrypt;
        }
        #endregion

        #region GetAngularPageLinkForCrDrInvoice
        /// <summary>
        /// Gets the angular page link for cr dr invoice.
        /// </summary>
        /// <param name="RouteName">Name of the route.</param>
        /// <param name="Salesid">The salesid.</param>
        /// <param name="InvoiceNo">The invoice no.</param>
        /// <param name="Opt">The opt.</param>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="StatusId">The status identifier.</param>
        /// <param name="Email">The email.</param>
        /// <param name="TypeId">The type identifier.</param>
        /// <returns>AngularPageLink</returns>
        public string GetAngularPageLinkForCrDrInvoice(string RouteName, string Salesid, string InvoiceNo, string Opt, string UserId, string StatusId, string Email, string TypeId)
        {
            string url = string.Empty;
            string EncodedSalesId = ToBase64(Salesid);
            string EncodedInvoiceNo = ToBase64(InvoiceNo);
            string EncodedOpt = ToBase64(Opt);
            string EncodedUserId = ToBase64(UserId);
            string EncodedStatusId = ToBase64(StatusId);
            string EncodedEmail = ToBase64(Email);
            string EncodedTypeId = ToBase64(TypeId);
            string EncodedParam = string.Empty;

            if (InvoiceNo != "")
            {
                EncodedParam = EncodedSalesId + "/" + EncodedInvoiceNo + "/" + EncodedOpt + "/" + EncodedUserId + "/" + EncodedStatusId + "/" + EncodedEmail + "/" + EncodedTypeId;
            }
            else
            {
                EncodedParam = EncodedSalesId + "/" + EncodedOpt + "/" + EncodedUserId + "/" + EncodedStatusId + "/" + EncodedEmail + "/" + EncodedTypeId;
            }

            EncodedParam = ToBase64(EncodedParam);
            url = SettingsConfig.AppSetting("ExternalUrl:TracerProjectIP") + SettingsConfig.AppSetting("ExternalUrl:AngularPath") + "/" + RouteName + "/";
            url = url + EncodedParam;

            return url;

            //if (InvoiceNo != "")
            //{
            //    url = SettingsConfig.AppSetting("ExternalUrl:TracerProjectIP") + "TracerAngular/" + RouteName + "/" + Salesid + "/" + InvoiceNo + "/" + Opt + "/" + UserId + "/" + StatusId + "/" + Email + "/" + TypeId;
            //}
            //else
            //{
            //    url = SettingsConfig.AppSetting("ExternalUrl:TracerProjectIP") + "TracerAngular/" + RouteName + "/" + Salesid + "/" + Opt + "/" + UserId + "/" + StatusId + "/" + Email + "/" + TypeId;
            //}
            //return url;
        }
        #endregion

        #region random
        public async Task<string> random(int passwordLength)
        {
            string allowedChars =  "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] chars = new char[passwordLength];
            Random rd = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            string pwd = new string(chars);
            return pwd;
        }

        public string ToBase64(string value)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(value);
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        #endregion
    }
}
