using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace WindomSVXedTool
{
    class XedDecrypt
    {
     
        XmlWriter xw;
        XmlWriterSettings xws;
        BinaryReader br;
        List<string> filelist;
        string xText = "";
        string section = "";
        int AnimeCount = 0;
        int PhysicsCount = 0;
        string lastbone = "";
        bool endread = false;
        Encoding encode;
        string Folder;
        public void Decrypt(string path,string folderName)
        {
            filelist = new List<string>();
            xws = new XmlWriterSettings();

            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            Folder = folderName;

            encode = Encoding.GetEncoding("shift-jis");
            br = new BinaryReader(File.Open(path, FileMode.Open));

            br.BaseStream.Seek(3, SeekOrigin.Begin);


            do
            {
                xText = ReadXedNodeTxt();
                Console.WriteLine(xText);
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

            } while (br.BaseStream.Length > br.BaseStream.Position);



            br.Close();

            StreamWriter sw = new StreamWriter(Folder + "\\filelist.txt");
            for (int i = 0; i < filelist.Count; i++)
                sw.WriteLine(filelist[i]);

            sw.Close();


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
                    //MessageBox.Show("BoneProperty");

                    break;
                case "AnimeName":
                    //MessageBox.Show("AnimeName");
                    if (section == "BoneProperty")
                        xw.WriteEndElement();

                    xw.Close();
                    lastbone = "";
                    section = "AnimeName";

                    break;
                case "Physics":

                    if (section == "AnimeName")
                    {
                        xw.Close();

                    }


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
                    xw = XmlWriter.Create(Folder + "\\BoneProperty.xml", xws);
                    filelist.Add("BoneProperty.xml");
                    xw.WriteStartDocument();
                    xw.WriteStartElement("BoneProperty");
                    xw.WriteAttributeString("Count", br.ReadInt32().ToString());
                    xw.WriteStartElement(ReadText());
                    break;
                case "Level":
                    xw.WriteStartElement("Level");
                    xw.WriteAttributeString("Value", br.ReadInt32().ToString());
                    xw.WriteEndElement();
                    break;
                case "ParentBoneIdx":
                    xw.WriteStartElement("ParentBoneIdx");
                    xw.WriteAttributeString("Value", br.ReadInt32().ToString());
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
                        xw.WriteString(br.ReadSingle().ToString() + " ");

                    xw.WriteEndElement();
                    break;
                case "EulerMode":
                    xw.WriteStartElement("EulerMode");
                    xw.WriteAttributeString("Value", br.ReadByte().ToString());
                    xw.WriteEndElement();
                    break;
                case "BoneLayers":
                    xw.WriteStartElement("BoneLayers");
                    xw.WriteAttributeString("Value", br.ReadInt32().ToString());
                    xw.WriteEndElement();
                    break;
                case "BoneFlag":
                    xw.WriteStartElement("BoneFlag");
                    xw.WriteAttributeString("Value", br.ReadInt16().ToString());
                    xw.WriteAttributeString("Value2", br.ReadInt16().ToString());
                    xw.WriteEndElement();
                    break;
                case "LimitAng":
                    xw.WriteStartElement("LimitAng");
                    for (int i = 0; i < 6; i++)
                        xw.WriteString(br.ReadSingle().ToString() + " ");
                    xw.WriteEndElement();
                    break;
                case "Windom_FileName":
                    xw.WriteStartElement("Windom_FileName");
                    xw.WriteAttributeString("Text", ReadText());
                    xw.WriteEndElement();
                    break;
                case "Windom_Hide":
                    xw.WriteStartElement("Windom_Hide");
                    xw.WriteAttributeString("Value", br.ReadByte().ToString());
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
                    xw = XmlWriter.Create(Folder + "\\Anime_" + AnimeCount.ToString() + ".xml", xws);
                    filelist.Add("Anime_" + AnimeCount.ToString() + ".xml");
                    AnimeCount++;
                    xw.WriteStartDocument();
                    xw.WriteStartElement("AnimeName");
                    xw.WriteAttributeString("Name", ReadText());
                    xw.WriteAttributeString("ID", br.ReadInt32().ToString());
                    break;
                case "Windom_TopScript":
                    xw.WriteStartElement("Windom_TopScript");
                    xw.WriteString(ReadText());
                    xw.WriteEndElement();
                    break;
                case "ScriptKey":
                    xw.WriteStartElement("ScriptKey");
                    xw.WriteAttributeString("Count", br.ReadInt32().ToString());
                    xw.WriteEndElement();
                    break;
                case "Time":
                    xw.WriteStartElement("Time");
                    xw.WriteAttributeString("Value", br.ReadInt32().ToString());

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
                    xw.WriteAttributeString("Value", br.ReadByte().ToString());
                    xw.WriteEndElement();
                    break;
                case "PowVal":
                    xw.WriteStartElement("PowVal");
                    xw.WriteAttributeString("Value", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "Pos":
                    xw.WriteStartElement("Pos");
                    xw.WriteAttributeString("x", br.ReadSingle().ToString());
                    xw.WriteAttributeString("y", br.ReadSingle().ToString());
                    xw.WriteAttributeString("z", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "PosKey":
                    xw.WriteStartElement("PosKey");
                    xw.WriteAttributeString("Count", br.ReadInt32().ToString());
                    xw.WriteEndElement();
                    break;
                case "Rota":
                    xw.WriteStartElement("Rota");
                    xw.WriteAttributeString("x", br.ReadSingle().ToString());
                    xw.WriteAttributeString("y", br.ReadSingle().ToString());
                    xw.WriteAttributeString("z", br.ReadSingle().ToString());
                    xw.WriteAttributeString("w", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "RotateKey":
                    xw.WriteStartElement("RotateKey");
                    xw.WriteAttributeString("Count", br.ReadInt32().ToString());
                    xw.WriteEndElement();
                    break;
                case "Scale":
                    xw.WriteStartElement("Scale");
                    xw.WriteAttributeString("x", br.ReadSingle().ToString());
                    xw.WriteAttributeString("y", br.ReadSingle().ToString());
                    xw.WriteAttributeString("z", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "ScaleKey":
                    xw.WriteStartElement("ScaleKey");
                    xw.WriteAttributeString("Count", br.ReadInt32().ToString());
                    xw.WriteEndElement();
                    break;

            }


        }

        void ReadPhysics(string pText)
        {
            switch (pText)
            {
                case "Physics":
                    xw = XmlWriter.Create(Folder + "\\Physics_" + PhysicsCount + ".xml", xws);
                    PhysicsCount++;
                    filelist.Add("Physics_" + PhysicsCount + ".xml");
                    xw.WriteStartDocument();
                    xw.WriteStartElement("Physics");
                    break;
                case "SizeRatio":
                    xw.WriteStartElement("SizeRatio");
                    xw.WriteAttributeString("Value", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "Gravity":
                    xw.WriteStartElement("Gravity");
                    xw.WriteAttributeString("Value", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "RigidBody":
                    xw.WriteStartElement("RigidBody");
                    xw.WriteAttributeString("Count", br.ReadInt32().ToString());
                    xw.WriteEndElement();
                    break;
                case "Name":
                    xw.WriteStartElement("Name");
                    xw.WriteAttributeString("text", ReadText());

                    break;
                case "BoneIdx":
                    xw.WriteStartElement("BoneIdx");
                    xw.WriteAttributeString("Value", br.ReadInt32().ToString());
                    xw.WriteEndElement();
                    break;
                case "GroupNo":
                    xw.WriteStartElement("GroupNo");
                    xw.WriteAttributeString("Value", br.ReadByte().ToString());
                    xw.WriteEndElement();
                    break;
                case "UnCollisionGroup":
                    xw.WriteStartElement("UnCollisionGroup");
                    xw.WriteAttributeString("Value", br.ReadInt16().ToString());
                    xw.WriteEndElement();
                    break;
                case "Shape":
                    xw.WriteStartElement("Shape");
                    xw.WriteAttributeString("Value", br.ReadByte().ToString());
                    xw.WriteEndElement();
                    break;
                case "Size":
                    xw.WriteStartElement("Size");
                    xw.WriteAttributeString("x", br.ReadSingle().ToString());
                    xw.WriteAttributeString("y", br.ReadSingle().ToString());
                    xw.WriteAttributeString("z", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "Pos":
                    xw.WriteStartElement("Pos");
                    xw.WriteAttributeString("x", br.ReadSingle().ToString());
                    xw.WriteAttributeString("y", br.ReadSingle().ToString());
                    xw.WriteAttributeString("z", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "Rota":
                    xw.WriteStartElement("Rota");
                    xw.WriteAttributeString("x", br.ReadSingle().ToString());
                    xw.WriteAttributeString("y", br.ReadSingle().ToString());
                    xw.WriteAttributeString("z", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "Mass":
                    xw.WriteStartElement("Mass");
                    xw.WriteAttributeString("Value", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "MoveAtte":
                    xw.WriteStartElement("MoveAtte");
                    xw.WriteAttributeString("Value", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "RotaAtte":
                    xw.WriteStartElement("RotaAtte");
                    xw.WriteAttributeString("Value", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "Repulsion":
                    xw.WriteStartElement("Repulsion");
                    xw.WriteAttributeString("Value", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "Fric":
                    xw.WriteStartElement("Fric");
                    xw.WriteAttributeString("Value", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "CalcType":
                    xw.WriteStartElement("CalcType");
                    xw.WriteAttributeString("Value", br.ReadByte().ToString());
                    xw.WriteEndElement();
                    break;
                case "EndRigid":
                    xw.WriteEndElement();
                    break;
                case "Joint":
                    xw.WriteStartElement("Joint");
                    xw.WriteAttributeString("Count", br.ReadInt32().ToString());
                    xw.WriteEndElement();
                    break;
                case "Type":
                    xw.WriteStartElement("Type");
                    xw.WriteAttributeString("Value", br.ReadByte().ToString());
                    xw.WriteEndElement();
                    break;
                case "CFM":
                    xw.WriteStartElement("CFM");
                    xw.WriteAttributeString("Value", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "ERP":
                    xw.WriteStartElement("ERP");
                    xw.WriteAttributeString("Value", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "RigidAIdx":
                    xw.WriteStartElement("RigidAIdx");
                    xw.WriteAttributeString("Value", br.ReadInt32().ToString());
                    xw.WriteEndElement();
                    break;
                case "RigidBIdx":
                    xw.WriteStartElement("RigidBIdx");
                    xw.WriteAttributeString("Value", br.ReadInt32().ToString());
                    xw.WriteEndElement();
                    break;
                case "MoveLimMin":
                    xw.WriteStartElement("MoveLimMin");
                    xw.WriteAttributeString("x", br.ReadSingle().ToString());
                    xw.WriteAttributeString("y", br.ReadSingle().ToString());
                    xw.WriteAttributeString("z", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "MoveLimMax":
                    xw.WriteStartElement("MoveLimMax");
                    xw.WriteAttributeString("x", br.ReadSingle().ToString());
                    xw.WriteAttributeString("y", br.ReadSingle().ToString());
                    xw.WriteAttributeString("z", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "RotaLimMin":
                    xw.WriteStartElement("RotaLimMin");
                    xw.WriteAttributeString("x", br.ReadSingle().ToString());
                    xw.WriteAttributeString("y", br.ReadSingle().ToString());
                    xw.WriteAttributeString("z", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "RotaLimMax":
                    xw.WriteStartElement("RotaLimMax");
                    xw.WriteAttributeString("x", br.ReadSingle().ToString());
                    xw.WriteAttributeString("y", br.ReadSingle().ToString());
                    xw.WriteAttributeString("z", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "SpringMoveVal":
                    xw.WriteStartElement("SpringMoveVal");
                    xw.WriteAttributeString("x", br.ReadSingle().ToString());
                    xw.WriteAttributeString("y", br.ReadSingle().ToString());
                    xw.WriteAttributeString("z", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "SpringRotaVal":
                    xw.WriteStartElement("SpringRotaVal");
                    xw.WriteAttributeString("x", br.ReadSingle().ToString());
                    xw.WriteAttributeString("y", br.ReadSingle().ToString());
                    xw.WriteAttributeString("z", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "SpringMoveDamp":
                    xw.WriteStartElement("SpringMoveDamp");
                    xw.WriteAttributeString("x", br.ReadSingle().ToString());
                    xw.WriteAttributeString("y", br.ReadSingle().ToString());
                    xw.WriteAttributeString("z", br.ReadSingle().ToString());
                    xw.WriteEndElement();
                    break;
                case "SpringRotaDamp":
                    xw.WriteStartElement("SpringRotaDamp");
                    xw.WriteAttributeString("x", br.ReadSingle().ToString());
                    xw.WriteAttributeString("y", br.ReadSingle().ToString());
                    xw.WriteAttributeString("z", br.ReadSingle().ToString());
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
            BinaryWriter bw = new BinaryWriter(File.Create(Folder + "\\MeshData.xof"));
            bw.Write(br.ReadBytes(binarylength));
            bw.Close();
            filelist.Add("MeshData.xof");
        }

        void BoneProperty(string ptext)
        {

        }

        string ReadText()
        {
            int length = br.ReadInt32();
            byte[] bt = br.ReadBytes(length);
            return encode.GetString(bt);
        }

        string ReadXedNodeTxt()
        {
            int txtCount = br.ReadByte();
            br.BaseStream.Seek(1, SeekOrigin.Current);
            byte[] bTxt = br.ReadBytes(txtCount);
            br.BaseStream.Seek(2, SeekOrigin.Current);
            return System.Text.Encoding.ASCII.GetString(bTxt);
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
