﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Concrete
{
    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings _emailSettings;

        public EmailOrderProcessor(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }
        public void ProcessOrder(Cart cart, ShippingDetails shippingInfo)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = _emailSettings.UseSsl;
                smtpClient.Host = _emailSettings.ServerName;
                smtpClient.Port = _emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);

                if (_emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = _emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body = new StringBuilder()
                    .AppendLine("A new order has been submitted")
                    .AppendLine("---")
                    .AppendLine("Items:");

                foreach (var line in cart.Lines)
                {
                    var subTotal = line.Quantity * line.Product.Price;
                    body.AppendFormat("{0} x {1} (subtotal: {2:c}", line.Quantity, line.Product.Name, subTotal);
                }

                body.AppendFormat("Total order value: {0:c}", cart.ComputeTotalValue())
                        .AppendLine("---")
                        .AppendLine("Ship to:")
                        .AppendLine(shippingInfo.Name)
                        .AppendLine(shippingInfo.Line1)
                        .AppendLine(shippingInfo.Line2 ?? "")
                        .AppendLine(shippingInfo.Line3 ?? "")
                        .AppendLine(shippingInfo.City)
                        .AppendLine(shippingInfo.State ?? "")
                        .AppendLine(shippingInfo.Country)
                        .AppendLine(shippingInfo.Zip)
                        .AppendLine("---")
                        .AppendFormat("Gift wrap: {0}",
                        shippingInfo.GiftWrap ? "Yes" : "No");

                MailMessage mailMessage = new MailMessage(
                        _emailSettings.MailFromAddress, // From
                        _emailSettings.MailToAddress, // To
                        "New order submitted!", // Subject
                        body.ToString()); // Body

                if (_emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                smtpClient.Send(mailMessage);
            }
        }
    }

    public class EmailSettings
    {
        public string MailToAddress = "orders@example.com";
        public string MailFromAddress = "sportsstore@example.com";
        public bool UseSsl = true;
        public string Username = "MySmtpUsername";
        public string Password = "MySmtpPassword";
        public string ServerName = "smtp.example.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"c:\sports_store_emails";
    }
}
