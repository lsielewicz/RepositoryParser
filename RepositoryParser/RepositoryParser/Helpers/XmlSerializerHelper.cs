using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RepositoryParser.Helpers
{
    public static class XmlSerializeHelper<T>
    {
        public static void Serialize(T source, string fileName, bool emptyNamespace = false)
        {
            using (Stream outputStream = new FileStream(fileName, FileMode.Create))
            {
                if (emptyNamespace)
                {
                    var nameSpace = new XmlSerializerNamespaces();
                    nameSpace.Add(string.Empty, string.Empty);

                    new XmlSerializer(typeof(T)).Serialize(outputStream, source, nameSpace);
                }
                else
                {
                    new XmlSerializer(typeof(T)).Serialize(outputStream, source);
                }
            }
        }

        public static void Serialize(T source, Stream stream)
        {
            new XmlSerializer(typeof(T)).Serialize(stream, source);
        }

        public static T Deserialize(string fileName)
        {
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var inputStream = new XmlTextReader(stream))
                {
                    var deserializer = new XmlSerializer(typeof(T));
                    return (T)deserializer.Deserialize(inputStream);
                }
            }
        }

        public static T Deserialize(Stream stream)
        {
            return (T)new XmlSerializer(typeof(T)).Deserialize(stream);
        }

        public static void InitEmptyProperties(object objectToCheck)
        {
            Type type = objectToCheck.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo pi in properties)
            {
                object val = pi.GetValue(objectToCheck, null);
                if (val != null)
                {
                    continue;
                }

                if (pi.PropertyType.Name == typeof(string).Name)
                {
                    pi.SetValue(objectToCheck, string.Empty, null);
                    continue;
                }

                object newVal = Activator.CreateInstance(pi.PropertyType);
                pi.SetValue(objectToCheck, newVal, null);
            }
        }

    }
}
