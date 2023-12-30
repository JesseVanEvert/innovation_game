using System;
using Newtonsoft.Json;

namespace CardDeck.Testing.Service
{
    public class EnvironmentVariables
    {
        private Dictionary<string, string> environmentVariables;
        public EnvironmentVariables()
        {// File contains json with required EnvironmentVariables
            using (var file = File.OpenText("../../../Service/launchRequirements.json"))
            {
                var json = file.ReadToEnd();
                // Convert json to dictionary
                environmentVariables = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
        }

        public void SetMockVariables()
        {
            Environment.SetEnvironmentVariable("AzureWebJobsStorage", environmentVariables["AzureWebJobsStorage"]);
            Environment.SetEnvironmentVariable("QueueName", environmentVariables["QueueName"]);
        }
    }
}

