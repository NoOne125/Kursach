using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;

namespace API
{
    public static class Extension
    {
        public static T ToClass<T>(this Dictionary<string, AttributeValue> dict)
        {
            var type = typeof(T);
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                var property = type.GetProperty(kv.Key);
                if (property != null)
                {
                    if (!string.IsNullOrEmpty(kv.Value.S))
                    {
                        property.SetValue(obj, kv.Value.S);
                    }
                    else if (!string.IsNullOrEmpty(kv.Value.N))
                    {
                        property.SetValue(obj, double.Parse(kv.Value.N.Replace(".", ",")));
                    }
                    else if (kv.Value.SS.Count != 0)
                    {
                        property.SetValue(obj, kv.Value.SS);
                    }
                    else if (kv.Value.NS.Count != 0)
                    {
                        List<double> loc = new List<double>(){ };
                        for (int i = 0; i < kv.Value.NS.Count; i++)
                        {
                            loc.Add(double.Parse(kv.Value.NS[i].Replace(".", ",")));
                        }
                        property.SetValue(obj, loc);
                    }
                    else if(kv.Value.L.Count != 0)
                    {
                        List<List<string>> loc_0 = new List<List<string>>();
                        foreach(var loc in kv.Value.L)
                        {
                            loc_0.Add(loc.SS);
                        }
                        property.SetValue(obj, loc_0);
                    }
                }
            }

            return (T)obj;
        }
    }
}
