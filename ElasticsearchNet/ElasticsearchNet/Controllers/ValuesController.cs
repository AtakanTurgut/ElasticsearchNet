using Elasticsearch.Net;
using ElasticsearchNet.Context;
using ElasticsearchNet.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;

namespace ElasticsearchNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class ValuesController : ControllerBase
    {
        public AppDbContext _context = new();

        [HttpGet("[action]")]
        public async Task<IActionResult> CreateData(CancellationToken cancellationToken)
        {
            List<Travel> travels = new List<Travel>();

            string CreateWithAlphabet()
            {
                var random = new Random();

                var words = new string(Enumerable
                    .Repeat("abcdefghijklmnoprstuvyz", 5)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                return words;
            }

            for (var i = 0; i < 1000; i++)
            {
                var title = CreateWithAlphabet();

                var words = new List<string>();

                for (int j = 0; j < 500; j++)
                {
                    words.Add(CreateWithAlphabet());
                }

                var descriptions = string.Join(" ", words);
                var travel = new Travel()
                {
                    Title = title,
                    Description = descriptions
                };

                travels.Add(travel);
            }

            await _context.Set<Travel>().AddRangeAsync(travels);
            await _context.SaveChangesAsync(cancellationToken);

            return Ok();
        }

        [HttpGet("[action]/{value}")]
        public async Task<IActionResult> GetDataListWithEF(string value)
        {
            List<Travel> travels = await _context
                .Set<Travel>()
                .Where(t => t.Description.Contains(value))
                .AsNoTracking()
                .ToListAsync();

            return Ok(travels.Take(10));
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SyncToElastic()
        {
            var settings = new ConnectionConfiguration(new Uri("http://localhost:9200"));
            var client = new ElasticLowLevelClient(settings);

            List<Travel> travels = await _context.Travels.ToListAsync();

            var tasks = new List<Task>();
            foreach (var travel in travels)
            {
                tasks.Add(client.IndexAsync<StringResponse>("travels", travel.Id.ToString(),
                    PostData.Serializable(new
                    {
                        travel.Id,
                        travel.Title,
                        travel.Description,
                    })));
            }

            await Task.WhenAll(tasks);

            return Ok();
        }

        [HttpGet("[action]/{description}")]
        public async Task<IActionResult> GetDataListWithElasticsearch(string value)
        {
            var settings = new ConnectionConfiguration(new Uri("http://localhost:9200"));
            var client = new ElasticLowLevelClient(settings);

            var response = await client.SearchAsync<StringResponse>("travels",
                PostData.Serializable(new
                {
                    query = new
                    {
                        wildcard = new
                        {
                            Description = new { value = $"*{value}*" }
                        }
                    }
                }));

            var results = JObject.Parse(response.Body);

            var hits = results["hits"]["hits"].ToObject<List<JObject>>();

            List<Travel> travels = new();

            foreach (var hit in hits)
            {
                travels.Add(hit["_source"].ToObject<Travel>());
            }

            return Ok(travels.Take(10));
        }
    }
}
