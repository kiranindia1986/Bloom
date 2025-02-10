using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Bloom.Models; // Ensure this matches the namespace where your models are defined


namespace Bloom.Services
{
    public class ScriptManager
    {
        /// <summary>
        /// Load a script (Level object) from an XML file.
        /// </summary>
        public static Level LoadScript(string filePath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Level));
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    var level = (Level)serializer.Deserialize(fs);
                    if (level?.TextEvents != null)
                    {
                        Debug.WriteLine($"Loaded {level.TextEvents.Count} events.");
                    }
                    return level;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading script: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Save a Level object to an XML file.
        /// </summary>
        public static void SaveScript(string filePath, Level level)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Level));

            // Ensure default values are set for text events
            foreach (var textEvent in level.TextEvents)
            {
                if (textEvent.SpeechType == Bloom.Models.TextType.text)
                {
                    textEvent.Face = FaceType.neutral;       // Set default face
                    textEvent.Character = CharacterType.hero; // Set default character
                }
            }

            // Save the serialized XML
            using (XmlWriter writer = XmlWriter.Create(filePath, new XmlWriterSettings { Indent = true }))
            {
                serializer.Serialize(writer, level);
            }
        }

        /// <summary>
        /// Load all scripts from a directory and return them as a collection.
        /// </summary>
        public static ObservableCollection<Level> LoadAllScripts(string directoryPath)
        {
            ObservableCollection<Level> levels = new ObservableCollection<Level>();

            if (Directory.Exists(directoryPath))
            {
                foreach (string filePath in Directory.GetFiles(directoryPath, "*.xml"))
                {
                    try
                    {
                        Level level = LoadScript(filePath);
                        levels.Add(level);
                    }
                    catch
                    {
                        // Handle invalid or corrupted XML files gracefully
                    }
                }
            }

            return levels;
        }
    }
}
