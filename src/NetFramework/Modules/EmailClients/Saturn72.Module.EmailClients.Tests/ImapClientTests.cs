#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Module.EmailClients.Domain.Filters;
using Saturn72.Module.EmailClients.ImapXCommander;

#endregion

namespace Saturn72.Module.EmailClients.Tests
{
   // [Ignore("For develop only")]
    public class ImapClientTests
    {
        [Test]
     //   [Ignore("For develop only")]
        public void Login_Success()
        {
            var mapCommander = new ImapxImapCommander(); //TODO: replace with mock
            var gmailMapClient = new GmailImapClient(mapCommander);
            var username = "saturn72test@gmail.com";
            var password = "!Q@W#E$R%";

            gmailMapClient.Login(username, password).ShouldBeTrue();
        }

        [Test]
        public void Login_FailedOnRandonUsernameAndPAssword()
        {
            var mapCommander = new ImapxImapCommander(); //TODO: replace with mock
            var gmailMapClient = new GmailImapClient(mapCommander);
            var username = Guid.NewGuid().ToString();
            var password = "qwertyuoi";

            gmailMapClient.Login(username, password).ShouldBeFalse();
        }

        [Test]
        public void BrowseFolders()
        {
            var mapCommander = new ImapxImapCommander(); //TODO: replace with mock
            var gmailMapClient = new GmailImapClient(mapCommander);
            var username = "saturn72test@gmail.com";
            var password = "!Q@W#E$R%";

            gmailMapClient.Login(username, password).ShouldBeTrue();

            //var filter = new EmailMessageFilter
            //{
            //    SinceDateTime = DateTime.UtcNow.Subtract(TimeSpan.FromDays(400))
            //};


            var folder = gmailMapClient.GetFolders();

            (folder.Any()).ShouldBeTrue();
        }

        [Test]
      //  [Ignore("For develop only")]
        public void GetIncomingMessages()
        {
            var mapCommander = new ImapxImapCommander(); //TODO: replace with mock
            var gmailMapClient = new GmailImapClient(mapCommander);
            var username = "saturn72test@gmail.com";
            var password = "!Q@W#E$R%";

            gmailMapClient.Login(username, password).ShouldBeTrue();

            var filter = new EmailMessageFilter
            {
                SinceDateTime = DateTime.UtcNow.Subtract(TimeSpan.FromDays(40))
            };
            var messages = gmailMapClient.GetAllIncomingMessages(filter);

            (messages.Any()).ShouldBeTrue();
        }


        [Test]
      //  [Ignore("For develop only")]
        public void Connect_Success()
        {
            var mapCommander = new ImapxImapCommander(); //TODO: replace with mock
            var gmailMapClient = new GmailImapClient(mapCommander);
            var username = "saturn72test@gmail.com";
            var password = "!Q@W#E$R%";

            gmailMapClient.Login(username, password).ShouldBeTrue();
        }
    }

    public class ImapTestConfig : IConfigMap
    {
        private readonly IDictionary<string, object> _smtpConfig = new Dictionary<string, object>
        {
            {"Host", "imap.gmail.com"},
            {"Port", "993"},
            {"SslProtocol", "Ssl3"},
            {"ValidateServerCertificate", "true"},
            {"Username", "saturn72test@gmail.com"},
            {"Password", "!Q@W#E$R%"},
        };

        public IDictionary<string, object> AllConfigRecords
        {
            get { return _smtpConfig; }
        }

        public object GetValue(string key)
        {
            return _smtpConfig[key];
        }
    }
}