using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Communication.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Communication.Services
{
    public static class DeviceService
    {
        public static void SaveDevicesToFile(IEnumerable<Device> devices,string savePath)
        {
            JArray jArray = new JArray();
            foreach (var device in devices)
            {
                JObject jObject = new JObject
                {
                    {"Name", device.Name}, {"IPAddress", device.IpAddress}, {"Port", device.Port}
                };
                jArray.Add(jObject);
            }
            
            using (StreamWriter sw = new StreamWriter(savePath))
            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jArray.WriteTo(jsonWriter);
                sw.Close();
                jsonWriter.Close();
            }
        }

        public static ObservableCollection<Device> LoadDevicesFromFile(string url)
        {
            ObservableCollection<Device> devices;
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sr = new StreamReader(url))
            using (JsonReader jsonReader = new JsonTextReader(sr))
            {
                devices = serializer.Deserialize<ObservableCollection<Device>>(jsonReader);
            }
            return devices;
        }
    }
}
