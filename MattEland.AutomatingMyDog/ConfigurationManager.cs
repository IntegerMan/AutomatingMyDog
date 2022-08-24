using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MattEland.AutomatingMyDog;

public class ConfigurationManager
{ 
    public AutomatingMyDogConfig LoadConfigData()
    {
        /* The credentials.json file should look something like this:
            {
              "key": "YourCognitiveServicesKey",
              "endpoint": "https://YourUrlHere.cognitiveservices.azure.com/",
              "region": "northcentralus",
              "appId": "YourAppId",
              "slotId": "Production"
            }        
         */

        using StreamReader file = File.OpenText("credentials.json");
        using JsonTextReader reader = new(file);

        JObject jObj = (JObject)JToken.ReadFrom(reader);

        return new AutomatingMyDogConfig(jObj["key"].Value<string>(), 
            jObj["endpoint"].Value<string>(), 
            jObj["region"].Value<string>(),
            jObj["appId"].Value<string>(),
            jObj["slotId"].Value<string>());
    }
}