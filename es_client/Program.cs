using System;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;


namespace es_client
{

    class Program
    {


        public static void Main(string[] args)
        {

                Console.WriteLine("hello");

            var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("people");

            var client = new ElasticsearchClient(settings);

            // Query example: Fetch all documents explicitly from the "people" index
            var response = client.SearchAsync<object>(s => s
                .Index("people") // Explicitly specify the index
                .Query(q => q
                    .MatchAll(_ => { })
                )
            ).Result;

            // Print the results
            Console.WriteLine($"Found {response.Hits.Count} documents:");
            foreach (var hit in response.Hits)
            {
                Console.WriteLine(hit.Source);
            }
        }
        
    }
    
}
