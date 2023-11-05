using DataAccess.Repositories.QueueRepo;
using DataModels.API;
using Newtonsoft.Json;
using ProducerWorker.Utilities;

namespace ProducerWorker.API
{
    public class PullEvents : IPullEvents
    {
        private readonly string _apiUrlFormat;
        private readonly IQueueRepository _queueRepository;
        private readonly IJsonParser _jsonParser;

        public PullEvents(IConfiguration configuration,
                          IQueueRepository queueRepository,
                          IJsonParser jsonParser)
        {
            _apiUrlFormat = configuration["apiUrlFormat"];
            _queueRepository = queueRepository;
            _jsonParser = jsonParser;
        }

        public async void Pull()
        {
            // apiUrl will be something like "https://localhost:7019/v1/api/Scans/scanevents?fromEventId=1&limit=100"
            var cursor = _queueRepository.GetCursor();
            string apiUrl = string.Format(_apiUrlFormat, cursor);

            using (HttpClient httpClient = new())
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // Ideally I shouldn't use the JsonConvert class instead I should use object defined on app level and DI injected instead.
                    EventCollection eventCollection = JsonConvert.DeserializeObject<EventCollection>(responseContent);

                    if (eventCollection == null ||
                        eventCollection.ScanEvents.Length == 0)
                        return;

                    _queueRepository.PushEvents(eventCollection);
                    _queueRepository.SetCursor(_jsonParser.GetLastId(eventCollection));

                    Console.WriteLine("Response:");
                    Console.WriteLine(responseContent);
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                }
            }
        }
    }
}
