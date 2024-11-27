using System.IO;
using Code.Infrastructure.Serialization;

namespace Code.Progress
{
    public static class ProgressExtensions
    {
        public static void WriteFile(string json, string path)
        {
            File.WriteAllText(path, json);
        }

        public static T TryReadFile<T>(string path) where T : class
        {
            if (!File.Exists(path))
                return null;

            string progress = File.ReadAllText(path);
            return progress.FromJson<T>();
        }

        public static void DeleteProgress(string path)
        {
            if (!File.Exists(path))
                return;

            File.Delete(path);
        }
    }
}