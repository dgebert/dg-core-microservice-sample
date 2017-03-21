using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace dg.document.db
{
   
    public  class DocumentDbRepository<T> : IDocumentDbRepository<T> where T : class
    {
        private  DocumentClient _client;
        private readonly string _endPointUrl;
        private readonly string _authKey;
        private readonly string _databaseId;
        private readonly string _collectionId;
        private readonly DocumentDbSettings _settings;

        public DocumentDbRepository(DocumentDbSettings settings)
        {
            _settings = settings;
            _endPointUrl = settings.EndPointUrl;
            _authKey = settings.AuthorizationKey;
            _databaseId = settings.DatabaseId;
            _collectionId = settings.CollectionId;
        }

        public void Initialize()
        {
            var connectionPolicy = new ConnectionPolicy { EnableEndpointDiscovery = false };
            _client = new DocumentClient(new Uri(_endPointUrl), _authKey, connectionPolicy);
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        public  async Task<T> GetAsync(string id)
        {
            try
            {
                var documentUri = GetDocumentUri(id);
                Document document = await _client.ReadDocumentAsync(documentUri);
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }

        public  async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            var documentCollectionUri = GetDocumentCollectionUri();
            var feedOptions = new FeedOptions { MaxItemCount = -1 };

            IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(documentCollectionUri, feedOptions)
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }


        public  async Task<Document> CreateAsync(T item)
        {
            var documentCollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId);
            return await _client.CreateDocumentAsync(documentCollectionUri, item);
        }

        public  async Task<Document> UpdateAsync(string id, T item)
        {
            var documentUri = UriFactory.CreateDocumentUri(_databaseId, _collectionId, id);
            return await _client.ReplaceDocumentAsync(documentUri, item);
        }

        public  async Task DeleteAsync(string id)
        {
            var documentUri = UriFactory.CreateDocumentUri(_databaseId, _collectionId, id);
            await _client.DeleteDocumentAsync(documentUri);
        }

        #region private
        private Uri GetDocumentCollectionUri()
        {
            return UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId);
        }

        private Uri GetDocumentUri(string id)
        {
            return UriFactory.CreateDocumentUri(_databaseId, _collectionId, id);
        }

        private  async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await _client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _client.CreateDatabaseAsync(new Database { Id = _databaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private  async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var databaseUri = UriFactory.CreateDatabaseUri(_databaseId);
                    var documentCollection = new DocumentCollection { Id = _collectionId };
                    var requestOptions = new RequestOptions { OfferThroughput = 1000 };

                    await _client.CreateDocumentCollectionAsync(databaseUri, documentCollection, requestOptions);
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion
    }
}
