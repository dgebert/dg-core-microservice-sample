

using System;
using System.Security;

namespace dg.document.db.Repository
{
    public class DocumentDbSettings
    {
        internal readonly string CollectionName;

        public Uri DatabaseUri { get; set; }
        public SecureString DatabaseKey { get; set; }
        public string DatabaseName { get; internal set; }
    }
}
