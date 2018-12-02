﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Http;
using Google.Apis.Services;
using Delegate = Google.Apis.Gmail.v1.Data.Delegate;

namespace Lithnet.GoogleApps
{
    public class GmailServiceRequestFactory
    {
        private readonly X509Certificate2 x509Certificate;

        private readonly string serviceAccountID;

        private readonly string[] scopes;

        private readonly OrderedDictionary cache = new OrderedDictionary(100);

        public GmailServiceRequestFactory(string serviceAccountID, X509Certificate2 x509Certificate, string[] scopes)
        {
            this.x509Certificate = x509Certificate;
            this.serviceAccountID = serviceAccountID;
            this.scopes = scopes;
        }

        private GmailService GetService(string user)
        {
            if (!this.cache.Contains(user))
            {
                ServiceAccountCredential.Initializer initializerInstance = new ServiceAccountCredential.Initializer(this.serviceAccountID)
                {
                    User = user,
                    Scopes = this.scopes
                }.FromCertificate(this.x509Certificate);

                GmailService x = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = new ServiceAccountCredential(initializerInstance),
                    ApplicationName = "LithnetGoogleAppsLibrary",
                    GZipEnabled = !Settings.DisableGzip,
                    Serializer = new GoogleJsonSerializer(),
                    DefaultExponentialBackOffPolicy = ExponentialBackOffPolicy.None,
                });

                x.HttpClient.Timeout = Timeout.InfiniteTimeSpan;
                this.cache.Add(user, x);

                if (this.cache.Count > 100)
                {
                    this.cache.RemoveAt(0);
                }
            }

            return (GmailService)this.cache[user];
        }

        public IEnumerable<string> GetDelegates(string id)
        {
            id.ThrowIfNotEmailAddress();

            GmailService service = this.GetService(id);
            UsersResource.SettingsResource.DelegatesResource.ListRequest request = service.Users.Settings.Delegates.List(id);
            ListDelegatesResponse result = request.ExecuteWithBackoff();
            return result.Delegates?.Select(t => t.DelegateEmail) ?? new List<string>();
        }

        public IEnumerable<string> GetSendAsAddresses(string id)
        {
            id.ThrowIfNotEmailAddress();

            GmailService service = this.GetService(id);
            UsersResource.SettingsResource.SendAsResource.ListRequest request = service.Users.Settings.SendAs.List(id);
            ListSendAsResponse result = request.ExecuteWithBackoff();
            return result.SendAs?.Select(t => t.SendAsEmail) ?? new List<string>();
        }

        public IEnumerable<SendAs> GetSendAs(string id)
        {
            id.ThrowIfNotEmailAddress();

            GmailService service = this.GetService(id);
            UsersResource.SettingsResource.SendAsResource.ListRequest request = service.Users.Settings.SendAs.List(id);
            ListSendAsResponse result = request.ExecuteWithBackoff();
            return result.SendAs ?? new List<SendAs>();
        }

        public void RemoveDelegate(string id, string @delegate)
        {
            id.ThrowIfNotEmailAddress();

            GmailService service = this.GetService(id);
            UsersResource.SettingsResource.DelegatesResource.DeleteRequest request = service.Users.Settings.Delegates.Delete(id, @delegate);
            request.ExecuteWithBackoff(-1, 5);
        }

        public void RemoveSendAs(string id, string sendas)
        {
            id.ThrowIfNotEmailAddress();

            GmailService service = this.GetService(id);
            UsersResource.SettingsResource.SendAsResource.DeleteRequest request = service.Users.Settings.SendAs.Delete(id, sendas);
            request.ExecuteWithBackoff(-1, 5);
        }

        public void RemoveDelegate(string id)
        {
            id.ThrowIfNotEmailAddress();

            GmailService service = this.GetService(id);
            ListDelegatesResponse result = service.Users.Settings.Delegates.List(id).ExecuteWithBackoff();

            if (result.Delegates != null)
            {
                foreach (Delegate item in result.Delegates)
                {
                    service.Users.Settings.Delegates.Delete(id, item.DelegateEmail).ExecuteWithBackoff(-1, 5);
                }
            }
        }

        public void RemoveSendAs(string id)
        {
            id.ThrowIfNotEmailAddress();

            GmailService service = this.GetService(id);
            ListSendAsResponse result = service.Users.Settings.SendAs.List(id).ExecuteWithBackoff();

            if (result.SendAs != null)
            {
                foreach (SendAs item in result.SendAs.Where(t => !t.IsPrimary ?? false))
                {
                    service.Users.Settings.SendAs.Delete(id, item.SendAsEmail).ExecuteWithBackoff(-1, 5);
                }
            }
        }

        public void AddDelegate(string id, string @delegate)
        {
            id.ThrowIfNotEmailAddress();

            GmailService service = this.GetService(id);
            service.Users.Settings.Delegates.Create(new Delegate { DelegateEmail = @delegate }, id).ExecuteWithBackoff(-1, 100);
        }

        public void AddSendAs(string id, string sendAs)
        {
            id.ThrowIfNotEmailAddress();

            GmailService service = this.GetService(id);
            service.Users.Settings.SendAs.Create(new SendAs { SendAsEmail = sendAs }, id).ExecuteWithBackoff(-1, 100);
        }

        public void AddSendAs(string id, SendAs sendAs)
        {
            id.ThrowIfNotEmailAddress();

            GmailService service = this.GetService(id);
            service.Users.Settings.SendAs.Create(sendAs, id).ExecuteWithBackoff(-1, 100);
        }

        public void AddSendAs(string id, IEnumerable<string> sendAs)
        {
            id.ThrowIfNotEmailAddress();

            GmailService service = this.GetService(id);
            foreach (string item in sendAs)
            {
                service.Users.Settings.SendAs.Create(new SendAs { SendAsEmail = item }, id).ExecuteWithBackoff(-1, 100);
            }
        }

        public void AddSendAs(string id, IEnumerable<SendAs> sendAs)
        {
            id.ThrowIfNotEmailAddress();

            GmailService service = this.GetService(id);
            foreach (SendAs item in sendAs)
            {
                service.Users.Settings.SendAs.Create(item, id).ExecuteWithBackoff(-1, 100);
            }
        }

        public void AddDelegate(string id, IEnumerable<string> delegates)
        {
            id.ThrowIfNotEmailAddress();

            GmailService service = this.GetService(id);
            foreach (string item in delegates)
            {
                service.Users.Settings.Delegates.Create(new Delegate { DelegateEmail = item }, id).ExecuteWithBackoff(-1, 100);
            }
        }
    }
}