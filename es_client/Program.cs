using System;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;


namespace es_client
{

    class Program
    {

        // public static async Task SearchTestIndexWithTimeoutAsync(ElasticsearchClient client, TimeSpan requestTimeout)
        // {
        //     var request = new SearchRequest<object>
        //     {
        //         IndexName = "test_index",
        //         Query = new MatchAllQuery(),
        //         RequestConfiguration = new Elastic.Transport.TransportRequestOptions
        //         {
        //             RequestTimeout = requestTimeout
        //         }
        //     };

        //     var response = await client.SearchAsync<object>(request);
        //     Console.WriteLine($"[test_index] Found {response.Hits.Count} documents:");
        //     foreach (var hit in response.Hits)
        //     {
        //         Console.WriteLine(hit.Source);
        //     }
        // }

        public static void SearchTestIndexWithTimeout(ElasticsearchClient client, TimeSpan requestTimeout)
        {
            try
            {
                var response = client.SearchAsync<object>(s => s
                    .Index("test_index")
                    .Query(q => q.MatchAll(_ => { }))
                    .RequestConfiguration(rc => rc.RequestTimeout(requestTimeout))
                ).Result;

                Console.WriteLine($"[test_index] Found {response.Hits.Count} documents:");
                foreach (var hit in response.Hits)
                {
                    Console.WriteLine(hit.Source);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during search: {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                }
            }
        }

        // Mimics the custom SearchRequest usage for EsRestaurant
        public static void SearchEsRestaurantWithCustomRequest(ElasticsearchClient client, object queryDescriptor, dynamic builder, dynamic criteria, double requestTimeout)
        {
            // Placeholder for ToQuery and builder.BuildSourceQuery()
            var query = ToQuery(queryDescriptor); // You should implement ToQuery
            var source = builder.BuildSourceQuery(); // You should implement BuildSourceQuery
            bool canBeSlow = CanBeSlow(criteria); // You should implement CanBeSlow

            var request = new SearchRequest<EsRestaurant>
            {
                Query = query,
                Source = source,
                From = criteria.From,
                Size = criteria.Size,
                TrackTotalHits = new Elastic.Clients.Elasticsearch.Core.Search.TrackHits(true),
                RequestConfiguration = new Elastic.Transport.RequestConfiguration
                {
                    RequestTimeout = TimeSpan.FromSeconds(canBeSlow ? requestTimeout * 2.5 : requestTimeout)
                }
            };

            try
            {
                var response = client.SearchAsync<EsRestaurant>(request).Result;
                Console.WriteLine($"[esrestaurant] Found {response.Hits.Count} documents:");
                foreach (var hit in response.Hits)
                {
                    Console.WriteLine(hit.Source);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during EsRestaurant search: {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                }
            }
        }

        // Placeholder types and methods for compilation
        public class EsRestaurant { }
        public static Query ToQuery(object descriptor) => new MatchAllQuery();
        public static bool CanBeSlow(dynamic criteria) => false;

        public static void Main(string[] args)
        {
            Console.WriteLine("hello");

            var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("test_index");

            var client = new ElasticsearchClient(settings);

            // Example: Search test_index with custom timeout (e.g., 10 seconds)
            // SearchTestIndexWithTimeout(client, TimeSpan.FromSeconds(10));

            // Example: Call SearchEsRestaurantWithCustomRequest
            // Dummy builder and criteria for demonstration
            var builder = new DummyBuilder();
            dynamic criteria = new System.Dynamic.ExpandoObject();
            criteria.From = 0;
            criteria.Size = 10;
            double requestTimeout = 10;
            object queryDescriptor = null; // Replace with your actual descriptor if needed
            SearchEsRestaurantWithCustomRequest(client, queryDescriptor, builder, criteria, requestTimeout);
        }

        // DummyBuilder for demonstration
        public class DummyBuilder
        {
            public object BuildSourceQuery() => null;
        }
    }
}
