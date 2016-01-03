using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Text;
using static WindomSVXedTool.Helper;

namespace WindomSVXedTool
{
    class XedDecrypt
    {
        XmlWriter xw;
        XmlWriterSettings xws = new XmlWriterSettings();
        BinaryReader br;
        List<string> filelist;
        string xText = "";
        string section = "";
        int AnimeCount = 0;
        int PhysicsCount = 0;
        string lastbone = "";
        bool endread = false;
        string Folder;

        public void Decrypt(string path, string folderName)
        {
            filelist = new List<string>();
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            Folder = folderName;

            using (var stream = File.OpenRead(path))
            using (var buffStream = new BufferedStream(stream, 0x100000))
            using (br = new BinaryReader(buffStream))
            {
                long length = br.BaseStream.Length;
                br.BaseStream.Seek(3, SeekOrigin.Begin);
                do
                {
                    xText = ReadXedNodeTxt();
                    //Console.WriteLine(xText);
                    isSectionChange(xText);

                    switch (section)
                    {
                        case "MeshData":
                            WriteXof();
                            break;
                        case "BoneProperty":
                            ReadBoneProperty(xText);
                            break;
                        case "AnimeName":
                            ReadAnimation(xText);
                            break;
                        case "Physics":
                            ReadPhysics(xText);
                            break;
                    }

                } while (buffStream.Position < length);
            }

            File.WriteAllLines(Path.Combine(Folder, "filelist.txt"), filelist);
        }

        void isSectionChange(string ptext)
        {
            switch (ptext)
            {
                case "MeshData":
                    section = "MeshData";
                    break;
                case "BoneProperty":
                    section = "BoneProperty";
                    break;
                case "AnimeName":
                    if (section == "BoneProperty")
                        xw.WriteEndElement();
                    xw.Close();
                    lastbone = "";
                    section = "AnimeName";
                    break;
                case "Physics":
                    if (section == "AnimeName")
                        xw.Close();
                    section = "Physics";
                    break;
                case "End":
                    break;
            }
        }

        void ReadBoneProperty(string ptext)
        {
            switch (ptext)
            {
                case "BoneProperty":
                    xw = XmlWriter.Create(Path.Combine(Folder, "BoneProperty.xml"), xws);
                    filelist.Add("BoneProperty.xml");
                    xw.WriteStartDocument();
                    xw.WriteStartElement("BoneProperty");
                    xw.WriteAttributeString("Count", br.ReadInt32());
                    xw.WriteStartElement(ReadText());
                    break;
                case "Level":
                    xw.WriteStartElement("Level");
                    xw.WriteAttributeString("Value", br.ReadInt32());
                    xw.WriteEndElement();
                    break;
                case "ParentBoneIdx":
                    xw.WriteStartElement("ParentBoneIdx");
                    xw.WriteAttributeString("Value", br.ReadInt32());
                    xw.WriteEndElement();
                    break;
                case "TransMat":
                    xw.WriteStartElement("TransMat");
                    for (int i = 0; i < 16; i++)
                        xw.WriteString(br.ReadSingle() + " ");
                    xw.WriteEndElement();
                    break;
                case "OffsetMat":
                    xw.WriteStartElement("OffsetMat");
                    for (int i = 0; i < 16; i++)
                        xw.WriteString(br.ReadSingle() + " ");
                    xw.WriteEndElement();
                    break;
                case "EulerMode":
                    xw.WriteStartElement("EulerMode");
                    xw.WriteAttributeString("Value", br.ReadByte());
                    xw.WriteEndElement();
                    break;
                case "BoneLayers":
                    xw.WriteStartElement("BoneLayers");
                    xw.WriteAttributeString("Value", br.ReadInt32());
                    xw.WriteEndElement();
                    break;
                case "BoneFlag":
                    xw.WriteStartElement("BoneFlag");
                    xw.WriteAttributeString("Value", br.ReadInt16());
                    xw.WriteAttributeString("Value2", br.ReadInt16());
                    xw.WriteEndElement();
                    break;
                case "LimitAng":
                    xw.WriteStartElement("LimitAng");
                    for (int i = 0; i < 6; i++)
                        xw.WriteString(br.ReadSingle() + " ");
                    xw.WriteEndElement();
                    break;
                case "Windom_FileName":
                    xw.WriteStartElement("Windom_FileName");
                    xw.WriteAttributeString("Text", ReadText());
                    xw.WriteEndElement();
                    break;
                case "Windom_Hide":
                    xw.WriteStartElement("Windom_Hide");
                    xw.WriteAttributeString("Value", br.ReadByte());
                    xw.WriteEndElement();
                    break;
                case "End":
                    xw.WriteEndElement();
                    if (!isNode())
                        xw.WriteStartElement(ReadText());
                    break;
            }
        }

        void ReadAnimation(string ptext)
        {
            switch (ptext)
            {
                case "AnimeName":
                    string fileName = $"Anime_{AnimeCount}.xml";
                    xw = XmlWriter.Create(Path.Combine(Folder, fileName), xws);
                    filelist.Add(fileName);
                    AnimeCount++;
                    xw.WriteStartDocument();
                    xw.WriteStartElement("AnimeName");
                    xw.WriteAttributeString("Name", ReadText());
                    xw.WriteAttributeString("ID", br.ReadInt32());
                    break;
                case "Windom_TopScript":
                    xw.WriteStartElement("Windom_TopScript");
                    xw.WriteString(ReadText());
                    xw.WriteEndElement();
                    break;
                case "ScriptKey":
                    xw.WriteStartElement("ScriptKey");
                    xw.WriteAttributeString("Count", br.ReadInt32());
                    xw.WriteEndElement();
                    break;
                case "Time":
                    xw.WriteStartElement("Time");
                    xw.WriteAttributeString("Value", br.ReadInt32());
                    break;
                case "ScriptText":
                    xw.WriteStartElement("ScriptText");
                    xw.WriteString(ReadText());
                    xw.WriteEndElement();
                    break;
                case "End":
                    xw.WriteEndElement();
                    break;
                case "BoneData":
                    xw.WriteStartElement("BoneData");
                    break;
                case "BoneName":
                    if (lastbone != "")
                        xw.WriteEndElement();
                    xw.WriteStartElement("BoneName");
                    lastbone = ReadText();
                    xw.WriteAttributeString("Text", lastbone);
                    break;
                case "CalcType":
                    xw.WriteStartElement("CalcType");
                    xw.WriteAttributeString("Value", br.ReadByte());
                    xw.WriteEndElement();
                    break;
                case "PowVal":
                    xw.WriteStartElement("PowVal");
                    xw.WriteAttributeString("Value", br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "Pos":
                    xw.WriteStartElement("Pos");
                    xw.WriteVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "PosKey":
                    xw.WriteStartElement("PosKey");
                    xw.WriteAttributeString("Count", br.ReadInt32());
                    xw.WriteEndElement();
                    break;
                case "Rota":
                    xw.WriteStartElement("Rota");
                    xw.WriteVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "RotateKey":
                    xw.WriteStartElement("RotateKey");
                    xw.WriteAttributeString("Count", br.ReadInt32());
                    xw.WriteEndElement();
                    break;
                case "Scale":
                    xw.WriteStartElement("Scale");
                    xw.WriteVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "ScaleKey":
                    xw.WriteStartElement("ScaleKey");
                    xw.WriteAttributeString("Count", br.ReadInt32());
                    xw.WriteEndElement();
                    break;
            }
        }

        void ReadPhysics(string pText)
        {
            switch (pText)
            {
                case "Physics":
                    string fileName = $"Physics_{PhysicsCount}.xml";
                    xw = XmlWriter.Create(Path.Combine(Folder, fileName), xws);
                    filelist.Add(fileName);
                    PhysicsCount++;
                    xw.WriteStartDocument();
                    xw.WriteStartElement("Physics");
                    break;
                case "SizeRatio":
                    xw.WriteStartElement("SizeRatio");
                    xw.WriteAttributeString("Value", br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "Gravity":
                    xw.WriteStartElement("Gravity");
                    xw.WriteAttributeString("Value", br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "RigidBody":
                    xw.WriteStartElement("RigidBody");
                    xw.WriteAttributeString("Count", br.ReadInt32());
                    xw.WriteEndElement();
                    break;
                case "Name":
                    xw.WriteStartElement("Name");
                    xw.WriteAttributeString("Text", ReadText());
                    break;
                case "BoneIdx":
                    xw.WriteStartElement("BoneIdx");
                    xw.WriteAttributeString("Value", br.ReadInt32());
                    xw.WriteEndElement();
                    break;
                case "GroupNo":
                    xw.WriteStartElement("GroupNo");
                    xw.WriteAttributeString("Value", br.ReadByte());
                    xw.WriteEndElement();
                    break;
                case "UnCollisionGroup":
                    xw.WriteStartElement("UnCollisionGroup");
                    xw.WriteAttributeString("Value", br.ReadInt16());
                    xw.WriteEndElement();
                    break;
                case "Shape":
                    xw.WriteStartElement("Shape");
                    xw.WriteAttributeString("Value", br.ReadByte());
                    xw.WriteEndElement();
                    break;
                case "Size":
                    xw.WriteStartElement("Size");
                    xw.WriteVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "Pos":
                    xw.WriteStartElement("Pos");
                    xw.WriteVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "Rota":
                    xw.WriteStartElement("Rota");
                    xw.WriteVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "Mass":
                    xw.WriteStartElement("Mass");
                    xw.WriteAttributeString("Value", br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "MoveAtte":
                    xw.WriteStartElement("MoveAtte");
                    xw.WriteAttributeString("Value", br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "RotaAtte":
                    xw.WriteStartElement("RotaAtte");
                    xw.WriteAttributeString("Value", br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "Repulsion":
                    xw.WriteStartElement("Repulsion");
                    xw.WriteAttributeString("Value", br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "Fric":
                    xw.WriteStartElement("Fric");
                    xw.WriteAttributeString("Value", br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "CalcType":
                    xw.WriteStartElement("CalcType");
                    xw.WriteAttributeString("Value", br.ReadByte());
                    xw.WriteEndElement();
                    break;
                case "EndRigid":
                    xw.WriteEndElement();
                    break;
                case "Joint":
                    xw.WriteStartElement("Joint");
                    xw.WriteAttributeString("Count", br.ReadInt32());
                    xw.WriteEndElement();
                    break;
                case "Type":
                    xw.WriteStartElement("Type");
                    xw.WriteAttributeString("Value", br.ReadByte());
                    xw.WriteEndElement();
                    break;
                case "CFM":
                    xw.WriteStartElement("CFM");
                    xw.WriteAttributeString("Value", br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "ERP":
                    xw.WriteStartElement("ERP");
                    xw.WriteAttributeString("Value", br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "RigidAIdx":
                    xw.WriteStartElement("RigidAIdx");
                    xw.WriteAttributeString("Value", br.ReadInt32());
                    xw.WriteEndElement();
                    break;
                case "RigidBIdx":
                    xw.WriteStartElement("RigidBIdx");
                    xw.WriteAttributeString("Value", br.ReadInt32());
                    xw.WriteEndElement();
                    break;
                case "MoveLimMin":
                    xw.WriteStartElement("MoveLimMin");
                    xw.WriteVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "MoveLimMax":
                    xw.WriteStartElement("MoveLimMax");
                    xw.WriteVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "RotaLimMin":
                    xw.WriteStartElement("RotaLimMin");
                    xw.WriteVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "RotaLimMax":
                    xw.WriteStartElement("RotaLimMax");
                    xw.WriteVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "SpringMoveVal":
                    xw.WriteStartElement("SpringMoveVal");
                    xw.WriteVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "SpringRotaVal":
                    xw.WriteStartElement("SpringRotaVal");
                    xw.WriteVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "SpringMoveDamp":
                    xw.WriteStartElement("SpringMoveDamp");
                    xw.WriteVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "SpringRotaDamp":
                    xw.WriteStartElement("SpringRotaDamp");
                    xw.WriteVector(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    xw.WriteEndElement();
                    break;
                case "EndJoint":
                    xw.WriteEndElement();
                    break;
                case "EndPhysics":
                    xw.WriteEndElement();
                    xw.WriteEndDocument();
                    xw.Close();
                    break;
            }
        }

        void WriteXof()
        {
            int binarylength = br.ReadInt32();
            using (var stream = File.Create(Path.Combine(Folder, "MeshData.xof")))
            using (BinaryWriter bw = new BinaryWriter(stream))
                bw.Write(br.ReadBytes(binarylength));
            filelist.Add("MeshData.xof");
        }

        void BoneProperty(string ptext)
        {

        }

        string ReadText()
        {
            int length = br.ReadInt32();
            byte[] bt = br.ReadBytes(length);
            return ShiftJis.GetString(bt);
        }

        string ReadXedNodeTxt()
        {
            // Reading is faster than seeking with Buffered streams, it ends up saving about a second on load time
            int txtCount = br.ReadByte();
            br.ReadByte();
            byte[] bTxt = br.ReadBytes(txtCount);
            br.ReadInt16();
            return ShiftJis.GetString(bTxt);
        }

        string getNextNode()
        {
            string next = ReadXedNodeTxt();
            br.BaseStream.Seek(br.BaseStream.Position - (next.Length + 2), SeekOrigin.Begin);
            return next;
        }

        bool isNode()
        {
            br.BaseStream.Seek(1, SeekOrigin.Current);
            byte Marker = br.ReadByte();
            br.BaseStream.Seek(br.BaseStream.Position - 2, SeekOrigin.Begin);
            return Marker == 0x8C;
        }
    }
}
