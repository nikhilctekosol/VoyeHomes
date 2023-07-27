using System;
using System.Data;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using VTravel.CustomerWeb.Models;
using Microsoft.AspNetCore.Identity;
using System.Timers;
using Dapper;

namespace VTravel.CustomerWeb
{
    public class RedisDataWorkerService : BackgroundService
    {
        private IDatabase _db;
        private readonly string DefaultConnectionString;
        ConnectionMultiplexer connectionMultiplexer;
        ConfigurationOptions redisConfiguration;
        string redisConnectionString;
        private System.Timers.Timer _timer;
        private bool _isRunning = false;
       

        public RedisDataWorkerService(IConfiguration configuration)
        {
            try 
            {
                var useRedis = bool.Parse(configuration["UseRedis"]);
                if(!useRedis) return;
                // Get Redis connection string
                redisConnectionString = configuration.GetConnectionString("RedisConnectionString");

                redisConfiguration = ConfigurationOptions.Parse(redisConnectionString);
                redisConfiguration.CommandMap = CommandMap.Create(new HashSet<string>
                { // EXCLUDE a few commands
                    "INFO", "CONFIG", "CLUSTER",
                    "PING", "ECHO", "CLIENT"
                }, available: false);

                //var redisConnectionParts = redisConnectionString.Split(new[] { "://", "@", ":" }, StringSplitOptions.RemoveEmptyEntries);
                //var redisUser = redisConnectionParts[1];
                //var redisPassword = redisConnectionParts[2];
                //var redisHost = redisConnectionParts[3];
                //var redisPort = Convert.ToInt32(redisConnectionParts[4]);

                //// Connect to Redis
                //redisConfiguration = new ConfigurationOptions
                //{
                //    EndPoints = { $"{redisHost}:{redisPort}" },
                //    Password = redisPassword,
                //    User = redisUser
                //};




                // Get MySQL connection string
                DefaultConnectionString = configuration.GetConnectionString("DefaultConnectionString");
            }
            catch (Exception ex)
            {
                General.LogException(ex);
               
            }
        }       

        private async void DoWork(object sender, ElapsedEventArgs e)
        {
            var redis_sync_now = General.GetSettingsValue("redis_sync_now");
            //redis_sync_now = "1";//temp
            if (redis_sync_now == "1")
            {
                using (var defaultConnection = new MySqlConnection(DefaultConnectionString))
                {
                    await defaultConnection.ExecuteAsync("UPDATE system_settings set settings_value='0' WHERE settings_name='redis_sync_now'");
                }
            }
            var currentTime = DateTime.Now;
            if ((redis_sync_now == "1" || currentTime.Hour == 6 && currentTime.Minute == 0 ||
                currentTime.Hour == 18 && currentTime.Minute == 0) && !_isRunning)
            {
                _isRunning = true; // This flag will ensure we don't run the task more than once per specified time

               // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    SyncRedis();
                  

                    _isRunning = false; // Reset the flag when the job is done
                }
                catch (Exception ex)
                {
                    General.LogException(ex);
                }
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new System.Timers.Timer();
            _timer.Elapsed += DoWork;
            _timer.Interval = 60000; // Check every minute
            _timer.Start();
            return Task.CompletedTask;
        }
        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Stop();
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _timer?.Dispose();
        }

        async void SyncRedis()
        {
            try
            {
                //
                //using (connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString))
                connectionMultiplexer = ConnectionMultiplexer.Connect(redisConfiguration);
                if (connectionMultiplexer != null)
                {
                    try
                    {
                        _db = connectionMultiplexer.GetDatabase();

                        using (var connection = new MySqlConnection(DefaultConnectionString))
                        {
                            var query = string.Format(General.HOME_TAG_LIST_QUERY);

                            var command = new MySqlCommand(query, connection);

                            var dataTable = new DataTable();
                            new MySqlDataAdapter(command).Fill(dataTable);

                            var jsonArray = new List<string>();

                            foreach (DataRow row in dataTable.Rows)
                            {
                                jsonArray.Add(row[0].ToString());
                            }

                            // Convert the list of JSON objects to a single JSON array and store it in Redis
                            var json = JsonConvert.SerializeObject(jsonArray);
                            if (_db != null)
                                await _db.StringSetAsync("HomeTagList", json);


                        }
                    }
                    catch (Exception ex)
                    {
                        General.LogException(ex);

                    }
                 
                }
            }
            catch (Exception ex)
            {
                General.LogException(ex);

            }
        }
    }
}
