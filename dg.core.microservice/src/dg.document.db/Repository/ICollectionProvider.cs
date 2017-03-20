using System;

using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace dg.document.db.Repository
{
    public interface ICollectionProvider
    {
        Task<DocumentCollection> CreateOrGetCollection();

        Task<string> GetCollectionDocumentsLink();
    }
}
