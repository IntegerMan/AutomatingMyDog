using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MattEland.AutomatingMyDog;

public class ConfigurationManager
{ 
    public ConfigData LoadConfigData()
    {
        using StreamReader file = File.OpenText("credentials.json");
        using JsonTextReader reader = new(file);

        JObject jObj = (JObject)JToken.ReadFrom(reader);

        return new ConfigData(jObj["key"].Value<string>(), 
            jObj["endpoint"].Value<string>(), 
            jObj["region"].Value<string>(),
            jObj["appId"].Value<string>(),
            jObj["slotId"].Value<string>());
    }
}