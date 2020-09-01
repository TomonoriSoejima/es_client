using System;
using System.Threading.Tasks;
using Nest;

namespace es_client
{

    class Program
    {


        public static void Main(string[] args)
        {

            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
.DefaultIndex("people");

            var client = new ElasticClient(settings);

            watcher_sample2(client);
            //Do_it(client);
            BulkAll(client);
            //Task task = AsyncMethod(client);
            //task.Wait();
        }


        async static Task<string> AsyncMethod(ElasticClient client)
        {

     
            int max = 2147483519;
            for (int i = max - 1; i >= 0; i--)
            {
                var p = new Person(i)
                {
                    Id = i,
                    FirstName = "tomo1",
                    LastName = "haruto1"
                };
                var asyncIndexResponse = await client.IndexDocumentAsync(p);
            }

            var person = new Person
            {
                Id = 9,
                FirstName = "tomo1",
                LastName = "haruto1"
            };

            //var asyncIndexResponse = await client.IndexDocumentAsync(person);
            return "done";

        }



        public static void watcher_sample(ElasticClient client)
        {
            var watcher_id = "status-check";

    
            ExecuteWatchRequest request = new ExecuteWatchRequest(watcher_id);
            request.IgnoreCondition = true;
            request.RecordExecution = true;

       
            ExecuteWatchResponse response = client.Watcher.Execute(request);
          
            Console.WriteLine(response.WatchRecord.Result.Actions);
            
    
        }

        public static void watcher_sample2(ElasticClient client)
        {


            // action
            var action = new LoggingAction("mylog")
            {
                Text = "some node is down",
                Level = LogLevel.Info
            };


            var my_http = new HttpInputRequest
            {
                Host = "localhost",
                Port = 9200,
                Path = "_cat/nodes",
                Method = HttpInputMethod.Get
            };

            var input_container = new HttpInput
            {
                Request = my_http
            };


        
            var schedule = new HourlySchedule();

            int[] intNumbers = new int[] { 30 };
            schedule.Minute = intNumbers;

            var schedule1 = new ScheduleContainer
            {
                Hourly = schedule
            };

            ConditionContainer cd = new AlwaysCondition();

            var id = "akito";
            PutWatchRequest watchRequest = new PutWatchRequest(id);
            watchRequest.Actions = action;
            watchRequest.Condition = cd;
            watchRequest.Input = input_container;
            watchRequest.Trigger = schedule1;


            PutWatchResponse response = client.Watcher.Put(watchRequest);

            //ExecuteWatchResponse response = client.Watcher.Execute(request);


            Console.WriteLine("hello");
          


        }



        public static void Do_it(ElasticClient client)
        {
            var people = new[]
            {
                new Person
                {
                    Id = 1,
                    FirstName = "Martijn",
                    LastName = "Laarman"
                },
                new Person
                {
                    Id = 2,
                    FirstName = "Stuart",
                    LastName = "Cam"
                },
                new Person
                {
                    Id = 3,
                    FirstName = "Russ",
                    LastName = "Cam"
                }
            };

            var bulkIndexResponse = client.Bulk(b => b
                .Index("people")
                .IndexMany(people));
        }


        public static void BulkAll(ElasticClient client)
        {
            var people = new[]
            {
                new Person
                {
                    Id = 1,
                    FirstName = "Martijn",
                    LastName = "Laarman"
                },
                new Person
                {
                    Id = 2,
                    FirstName = "Stuart",
                    LastName = "Cam"
                },
                new Person
                {
                    Id = 3,
                    FirstName = "Russ",
                    LastName = "Cam"
                }
            };

            var bulkAllObservable = client.BulkAll(people, b => b
           .Index("people")
           .BackOffTime("30s")
           .BackOffRetries(2)
           .RefreshOnCompleted()
           .MaxDegreeOfParallelism(Environment.ProcessorCount)
           .Size(3)
           )
           .Wait(TimeSpan.FromMinutes(15), next =>
           {
                // do something e.g. write number of pages to console
            });
        }

    }
}
