using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace SPTC.WebSockets.JsonHelper
{
    public enum Action
    {
        CONNECT,
        CREATE,
        DELETE,
        UPDATE,
        MOVE,
        DISCONNECT
    }

    public class JsonHelper
    {
        private readonly string _handlerName;

        public JsonHelper(string handlerName)
        {
            _handlerName = handlerName;
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                settings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
                return settings;
            };
        }

        public object CreateResponse(Action action, string message)
        {
            return new { sender = "broadcast", handler = _handlerName, action = Enum.GetName(typeof(Action), action), data = message };
        }

        public object CreateResponse(Action action, object message)
        {
            return new { sender = "broadcast", handler = _handlerName, action = Enum.GetName(typeof(Action), action), data = message };
        }

        public object CreateResponse(Action action, string message, string senderID)
        {
            return new { sender = senderID, handler = _handlerName, action = Enum.GetName(typeof(Action), action), data = message };
        }

        public object CreateResponse(Action action, object message, string senderID)
        {
            return new { sender = senderID, handler = _handlerName, action = Enum.GetName(typeof(Action), action), data = message };
        }

        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public object Deserialize(string value)
        {
            return JsonConvert.DeserializeObject(value);
        }

    }
}
