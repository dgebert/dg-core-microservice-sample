using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dg.document.db
{
    /*
     * Strongly typed configuration for DocumentDb
     * 
     *  services.Configure<MySetDocumentDbConfigtings>(Configuration.GetSection("DocumentDbSettings"));
     * 
     */
    public class DocumentDbSettings
    {
        public string DatabaseId;
        public string CollectionId;
        public string EndPointUrl;
        public string AuthorizationKey;
    }
}
