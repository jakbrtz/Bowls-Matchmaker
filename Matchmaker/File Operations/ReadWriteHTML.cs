using Matchmaker.UserInterface;
using Newtonsoft.Json;
using System.IO;

namespace Matchmaker.FileOperations
{
    public static class ReadWriteHTML
    {
        public static void ReloadFormat(string directory)
        {
            if (File.Exists(directory))
            {
                using StreamReader streamReader = new StreamReader(directory);
                HTMLdocument.format = streamReader.ReadToEnd();
            }
            else
            {
                HTMLdocument.format = Properties.Resources.table;
            }
        }

        public static void ReloadElements(string directory)
        {
            HTMLdocument.elements = JsonConvert.DeserializeObject<HTMLelements>(Properties.Resources.elements);
            if (File.Exists(directory))
            {
                using StreamReader streamReader = new StreamReader(directory);
                JsonConvert.PopulateObject(streamReader.ReadToEnd(), HTMLdocument.elements);
                // todo: error handling
            }
        }
    }
}
