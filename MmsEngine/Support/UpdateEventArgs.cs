using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmsEngine.Support
{
    public class UpdateEventArgs
    {
        [JsonProperty("guid")] public string Guid { get; set; }
        [JsonProperty("type")] public UpdateEventType Type { get; set; }
        [JsonProperty("propertyName")] public string PropertyName { get; set; }
        [JsonProperty("params")] public object[] Params { get; set; }
    }
}
