using System;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Tetris.Game
{
    public abstract class BaseConfiguration
    {
        public abstract string Filename { get; }

        public void Update()
        {
            if (File.Exists(Filename)) File.Delete(Filename);
            using var fs = File.Create(Filename);
            XmlWriter xmlWriter = XmlWriter.Create(fs);
            XmlSerializer serializer = new(this.GetType());
            serializer.Serialize(xmlWriter, this);
        }

        public static TConfig Load<TConfig>() where TConfig : BaseConfiguration
        {
            TConfig config = Activator.CreateInstance<TConfig>();
            XmlSerializer serializer = new(typeof(TConfig));

            if (File.Exists(config.Filename))
            {
                using (var stream = File.OpenRead(config.Filename))
                {
                    XmlReader xmlReader = XmlReader.Create(stream);
                    config = serializer.Deserialize(xmlReader) as TConfig;
                }
            }
            else
            {
                using (var stream = File.Create(config.Filename))
                {
                    XmlWriter xmlWriter = XmlWriter.Create(stream);
                    serializer.Serialize(xmlWriter, config);
                }
            }

            return config;
        }
    }
}
