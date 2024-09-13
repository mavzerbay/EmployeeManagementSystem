using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientLibrary.Helper
{
    public class Serializations
    {
        public static string SerializeObj<T>(T obj) => JsonSerializer.Serialize(obj);
        public static T? DeserializeObj<T>(string json) => JsonSerializer.Deserialize<T>(json);
    }
}