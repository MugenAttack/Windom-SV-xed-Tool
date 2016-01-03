using System.Xml;

namespace WindomSVXedTool
{
    public static class Extensions
    {
        public static void WriteAttributeString<T>(this XmlWriter writer, string name, T obj) => writer.WriteAttributeString(name, obj.ToString());

        public static void WriteVector(this XmlWriter writer, float x, float y, float z)
        {
            writer.WriteAttributeString("x", x);
            writer.WriteAttributeString("y", y);
            writer.WriteAttributeString("z", z);
        }

        public static void WriteVector(this XmlWriter writer, float x, float y, float z, float w)
        {
            writer.WriteAttributeString("x", x);
            writer.WriteAttributeString("y", y);
            writer.WriteAttributeString("z", z);
            writer.WriteAttributeString("w", w);
        }
    }
}
