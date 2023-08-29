using Discord;
using Discord.Commands;
using InnoTasker.Services;
using InnoTasker.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoTasker.Data.Databases
{
    public abstract class Database<TKey, TValue> : Dictionary<TKey, TValue>, IDatabase where TKey : notnull
    {
        private readonly ILogger _logger;
        public string Path { get; }

        public Database(IServiceProvider services, string path)
        {
            _logger = services.GetRequiredService<ILogger>();
            Path = path;

            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);

            _logger.LogAsync(LogSeverity.Info, this, $"Initializing database");

            foreach (string file in Directory.GetFiles(path, "*.json"))
            {
                string rawJson = File.ReadAllText(file);
                string fileName = file.Split("\\").Last().Split('.').First();
                try
                {
                    TKey key = (TKey)Convert.ChangeType(fileName, typeof(TKey));
                    TValue value = JsonConvert.DeserializeObject<TValue>(rawJson);
                    base.Add(key, value);
                }
                catch (Exception ex)
                {
                    _logger.LogAsync(LogSeverity.Error, this, $"Error loading file '{fileName}', file skipped", ex);
                }
            }

            _logger.LogAsync(LogSeverity.Info, this, $"Database initialized");
        }

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            Save(key);
            _logger.LogAsync(LogSeverity.Debug, this, $"Added entry '{key}'");
        }

        public new void Remove(TKey key)
        {
            try
            {
                base.Remove(key);
                File.Delete($"{Path}{key}.json");
                _logger.LogAsync(LogSeverity.Debug, this, $"Remove entry '{key}'");
            }
            catch (Exception ex)
            {
                _logger.LogAsync(LogSeverity.Error, this, $"Failed to remove entry '{key}'", ex);
            }
        }

        public void Save()
        {
            _logger.LogAsync(LogSeverity.Info, this, $"Starting database save");
            int failCount = 0;
            foreach (TKey key in Keys)
            {
                if (!Save(key)) failCount++;
            }
            _logger.LogAsync(LogSeverity.Info, this, $"Successfully saved {Keys.Count - failCount} entries with {failCount} failures");
        }

        public bool Save(TKey key)
        {
            try
            {
                TValue value = this[key];
                string path = $"{Path}{key}.json";
                string valueJson = JsonConvert.SerializeObject(value, Formatting.Indented);
                File.WriteAllText(path, valueJson);
            }
            catch (Exception ex)
            {
                _logger.LogAsync(LogSeverity.Error, this, $"Failed to save entry {key}", ex);
                return false;
            }
            return true;
        }
    }
}
