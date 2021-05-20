using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Astor.Background.Management.Protocol;
using Astor.JsonHttp;
using Astor.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Background.ElasticLogs.Tests
{
    [TestClass]
    public class Save_Should
    {
        public static string[] HeroActions = new[]
        {
            "IronMan_Fight",
            "CaptainAmerica_Fight",
            "BlackWidow_Fight",
            "Thor_Fight",
            "CaptainMarvel_Fight",
            "Hulk_Fight",
            "SpiderMan_Fight",
        };

        public static string[] Enemy = new[]
        {
            "Altron",
            "Thanos",
            "Loki",
        };

        public static string[] FightOutcome = new[]
        {
            "Draw",
            "World Saved",
            "World Saved With Damages",
            "Lost"
        };
        
        public static int BeforeRangeMilliseconds = 15 * 60 * 1000;
        
        [TestMethod]
        public async Task InsertLogRecord()
        {
            var client = new WebApplicationFactory().CreateClient();
            await this.sendAsync(client, new ActionResultCandidate
            {
                ActionId = "Reporter_SendReport",
                IsSuccessful = true,
                AttemptIndex = 2,
                StartTime = DateTime.Now.Subtract(TimeSpan.FromSeconds(30)),
                EndTime = DateTime.Now,
                SourceExchange = "my-exchange",
                Event = new
                {
                    Id = 66,
                    Name = "Somebody"
                },
                Result = new[]
                {
                    1, 2, 3
                }
            });
        }

        [TestMethod]
        public async Task CreateDashboard()
        {
            var client = new WebApplicationFactory().CreateClient();
            var response = await client.PostJsonAsync("ElasticLogs_CreateDashboard", null);

            response.EnsureSuccessStatusCode();
        }
        
        [TestMethod]
        public async Task FillWithTestData()
        {
            var count = 200_000;
            var before = TimeSpan.FromMinutes(2);
            var generated = this.Generate(count, before).ToArray();
            
            var client = new WebApplicationFactory().CreateClient();

            var watch = Stopwatch.StartNew();
            
            foreach (var slice in generated.Slices(100))
            {
                await this.SendInParallelAsync(slice, client);
            }
            
            Console.WriteLine($"took: {watch.Elapsed}");
        }

        private async Task SendInParallelAsync(IEnumerable<ActionResultCandidate> generated, HttpClient client)
        {
            var sends = new List<Task>();
            
            foreach (var resultCandidate in generated)
            {
                sends.Add(this.sendAsync(client, resultCandidate));
            }
            
            await Task.WhenAll(sends);
        }

        [TestMethod]
        public async Task FillWithMostlySuccessfulForIronMan()
        {
            await this.FillWithMostlySuccessfulAsync(HeroActions[0]);
        }
        
        [TestMethod]
        public async Task FillWithMostlySuccessfulForCaptainAmerica()
        {
            await this.FillWithMostlySuccessfulAsync(HeroActions[1]);
        }

        [TestMethod]
        public async Task FillWithAllSuccessesForBlackWidow()
        {
            await this.FillWithAllSuccessesAsync(HeroActions[2]);
        }

        private async Task FillWithAllSuccessesAsync(string hero)
        {
            var client = new WebApplicationFactory().CreateClient();
            var random = new Random();
            for (var i = 0; i < 1000; i++)
            {
                var (startTime, endTime) = this.randomStartTimeInRange(random, TimeSpan.FromMinutes(15));

                await this.sendAsync(client, new ActionResultCandidate
                {
                    ActionId = hero,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsSuccessful = true
                });
            }
        }
        
        private async Task FillWithMostlySuccessfulAsync(string hero)
        {
            var client = new WebApplicationFactory().CreateClient();
            var random = new Random();
            for (var i = 0; i < 1000; i++)
            {
                var (startTime, endTime) = this.randomStartTimeInRange(random, TimeSpan.FromMinutes(15));

                await this.sendAsync(client, new ActionResultCandidate
                {
                    ActionId = hero,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsSuccessful = this.mostlySuccessful(random),
                });
            }
        }

        private Task sendUnvalidated(HttpClient client, ActionResultCandidate candidate)
        {
            return client.PostJsonAsync("ElasticLogs_Save", candidate);
        }
        
        private async Task sendAsync(HttpClient client, ActionResultCandidate candidate)
        {
            var response = await client.PostJsonAsync("ElasticLogs_Save", candidate);
            
            response.EnsureSuccessStatusCode();
        }

        private IEnumerable<ActionResultCandidate> Generate(int count, TimeSpan beforeRange)
        {
            var random = new Random();

            for (var i = 0; i < count; i++)
            {
                yield return this.Generate(random, beforeRange);
            }
        }

        private ActionResultCandidate Generate(Random random, TimeSpan beforeRange)
        {
            var (startTime, endTime) = this.randomStartTimeInRange(random, beforeRange);

            return new ActionResultCandidate
            {
                ActionId = this.random(HeroActions, random),
                IsSuccessful = Convert.ToBoolean(random.Next(0, 2)),
                AttemptIndex = 1,
                StartTime = startTime,
                EndTime = endTime,
                Event = new
                {
                    enemy = this.random(Enemy, random)
                },
                Result = this.random(FightOutcome, random)
            };
        }

        private (DateTime startTime, DateTime endTime) randomStartTimeInRange(Random random, TimeSpan range)
        {
            var before = random.Next(0, (int)range.TotalMilliseconds);
            var elapsed = random.Next(0, before);

            var startTime = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(before));
            var endTime = startTime.Add(TimeSpan.FromMilliseconds(elapsed));

            return (startTime, endTime);
        }   

        private bool mostlySuccessful(Random random)
        {
            var result = random.Next(0, 5);
            return result < 4;
        }
        
        private T random<T>(T[] source, Random random) => source[random.Next(0, source.Length)];
    }
}