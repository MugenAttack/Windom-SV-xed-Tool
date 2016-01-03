using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace WindomSVXedTool
{
    class XedEncrypt
    {
        List<string> filelist;
        BinaryWriter bw;
        XmlDocument Doc = new XmlDocument();
        Encoding encode;
        public void Encrypt(string path,string folderpath, string filename)
        {
            bw = new BinaryWriter(File.Create(folderpath + "\\"+filename+".xed"));
            filelist = new List<string>();
            encode = Encoding.GetEncoding("shift-jis");
            StreamReader sr = new StreamReader(path);
            do
            {
                filelist.Add(sr.ReadLine());
            } while (!sr.EndOfStream);

            bw.Write(new byte[] { 0x58, 0x45, 0x44 });
            for (int i = 0; i < filelist.Count; i++)
            {
                if (filelist[i].Contains("MeshData"))
                    MeshData(folderpath + "\\" + filelist[i]);
                else if (filelist[i].Contains("BoneProperty"))
                    BoneProperty(folderpath + "\\" + filelist[i]);
                else if (filelist[i].Contains("Anime"))
                    Anime(folderpath + "\\" + filelist[i]);
                //else if (filelist[i].Contains("Physics"))
                 //   Physics(folderpath + "\\" + filelist[i]);
            }
            //WriteNode("End");
            bw.Close();
        }
        
        void MeshData(string path)
        {
            Console.WriteLine("MeshData Writing");
            WriteNode("MeshData");
            byte[] file = File.ReadAllBytes(path);
            bw.Write(file.Length);
            bw.Write(file);
        }

        void BoneProperty(string path)
        {
            Console.WriteLine("BoneProperty Writing");
            Doc.Load(path);
            XmlNode Bp = Doc.SelectSingleNode("BoneProperty");
            WriteNode(Bp.Name);
            bw.Write(Int32.Parse(Bp.Attributes["Count"].Value));
            XmlNodeList Bones = Bp.ChildNodes;
            foreach (XmlNode Bone in Bones)
            {
                WriteText(Bone.Name);
                foreach(XmlNode data in Bone.ChildNodes)
                {
                    switch(data.Name)
                    {
                        case "Level":
                            WriteNode("Level");
                            bw.Write(Int32.Parse(data.Attributes["Value"].Value));
                            break;
                        case "ParentBoneIdx":
                            WriteNode("ParentBoneIdx");
                            bw.Write(Int32.Parse(data.Attributes["Value"].Value));
                            break;
                        case "TransMat":
                            WriteNode("TransMat");
                            WriteMatrix(data.InnerText);
                            break;
                        case "OffsetMat":
                            WriteNode("OffsetMat");
                            WriteMatrix(data.InnerText);
                            break;
                        case "EulerMode":
                            WriteNode("EulerMode");
                            bw.Write(byte.Parse(data.Attributes["Value"].Value));
                            break;
                        case "BoneLayers":
                            WriteNode("BoneLayers");
                            bw.Write(Int32.Parse(data.Attributes["Value"].Value));
                            break;
                        case "BoneFlag":
                            WriteNode("BoneFlag");
                            bw.Write(Int16.Parse(data.Attributes["Value"].Value));
                            bw.Write(Int16.Parse(data.Attributes["Value2"].Value));
                            break;
                        case "LimitAng":
                            WriteNode("LimitAng");
                            string[] values = data.InnerText.Split(" ".ToCharArray());
                            for (int i = 0; i < 6; i++)
                                bw.Write(Single.Parse(values[i]));
                            break;
                        case "Windom_FileName":
                            WriteNode("Windom_FileName");
                            WriteText(data.Attributes["Text"].Value);
                            break;
                        case "Windom_Hide":
                            WriteNode("Windom_Hide");
                            bw.Write(byte.Parse(data.Attributes["Value"].Value));
                            break;
                    }
                }
                WriteNode("End");
            }
        }

        void Anime(string path)
        {
            Console.WriteLine(path);
            Doc = new XmlDocument();
            Doc.Load(path);
            XmlNode AN = Doc.SelectSingleNode("AnimeName");
            WriteNode("AnimeName");
            WriteText(AN.Attributes["Name"].Value);
            bw.Write(Int32.Parse(AN.Attributes["ID"].Value));
            foreach (XmlNode node in AN.ChildNodes)
            {
                switch (node.Name)
                {
                
                    case "Windom_TopScript":
                        WriteNode("Windom_TopScript");
                        WriteText(node.InnerText);
                        break;
                    case "ScriptKey":
                        WriteNode("ScriptKey");
                        bw.Write(Int32.Parse(node.Attributes["Count"].Value));
                        break;
                    case "Time":
                        WriteNode(node.Name);
                        bw.Write(Int32.Parse(node.Attributes["Value"].Value));
                       foreach (XmlNode time in node.ChildNodes)
                        {
                            if (time.Name == "ScriptText")
                            {
                                WriteNode("ScriptText");
                                WriteText(time.InnerText);
                            }
                        }
                        WriteNode("End");
                        break;
                  
                    case "BoneData":
                        WriteNode(node.Name);
                        foreach (XmlNode bone in node.ChildNodes)
                        {
                            switch (bone.Name)
                            {
                                case "BoneName":
                                    WriteNode(bone.Name);
                                    WriteText(bone.Attributes["Text"].Value);
                                    foreach(XmlNode data in bone.ChildNodes)
                                    {
                                        switch(data.Name)
                                        {
                                            case "PosKey":
                                                WriteNode(data.Name);
                                                bw.Write(Int32.Parse(data.Attributes["Count"].Value));
                                                break;
                                            case "RotateKey":
                                                WriteNode(data.Name);
                                                bw.Write(Int32.Parse(data.Attributes["Count"].Value));
                                                break;
                                            case "ScaleKey":
                                                WriteNode(data.Name);
                                                bw.Write(Int32.Parse(data.Attributes["Count"].Value));
                                                break;
                                            case "Time":
                                                WriteNode(data.Name);
                                                bw.Write(Int32.Parse(data.Attributes["Value"].Value));
                                                foreach (XmlNode time in data.ChildNodes)
                                                {
                                                    switch (time.Name)
                                                    {
                                                        case "CalcType":
                                                            WriteNode(time.Name);
                                                            bw.Write(byte.Parse(time.Attributes["Value"].Value));
                                                            break;
                                                        case "PowVal":
                                                            WriteNode(time.Name);
                                                            bw.Write(Single.Parse(time.Attributes["Value"].Value));
                                                            break;
                                                        case "Pos":
                                                            WriteNode(time.Name);
                                                            bw.Write(Single.Parse(time.Attributes["x"].Value));
                                                            bw.Write(Single.Parse(time.Attributes["y"].Value));
                                                            bw.Write(Single.Parse(time.Attributes["z"].Value));
                                                            break;
                                                        case "Rota":
                                                            WriteNode(time.Name);
                                                            bw.Write(Single.Parse(time.Attributes["x"].Value));
                                                            bw.Write(Single.Parse(time.Attributes["y"].Value));
                                                            bw.Write(Single.Parse(time.Attributes["z"].Value));
                                                            bw.Write(Single.Parse(time.Attributes["w"].Value));
                                                            break;
                                                        case "Scale":
                                                            WriteNode(time.Name);
                                                            bw.Write(Single.Parse(time.Attributes["x"].Value));
                                                            bw.Write(Single.Parse(time.Attributes["y"].Value));
                                                            bw.Write(Single.Parse(time.Attributes["z"].Value));
                                                            break;
                                                    }
                                                }
                                                WriteNode("End");
                                                break;
                                        }
                                    }
                                    break;
                               
                            }
                        }
                        WriteNode("End");
                        break;
                   
                }
            }
            WriteNode("End");
        }

        void Physics(string path)
        {
            Console.WriteLine("physics Writing");
            string writeEnd = "";
            Doc.Load(path);
            XmlNode p = Doc.SelectSingleNode("Physics");
            WriteNode("Physics");
            foreach (XmlNode node in p.ChildNodes)
            {
                switch (node.Name)
                {
                   
                    case "SizeRatio":
                        WriteNode(node.Name);
                        bw.Write(Single.Parse(node.Attributes["Value"].Value));
                        break;
                    case "Gravity":
                        WriteNode(node.Name);
                        bw.Write(Single.Parse(node.Attributes["Value"].Value));
                        break;
                    case "RigidBody":
                        WriteNode(node.Name);
                        bw.Write(Int32.Parse(node.Attributes["Count"].Value));
                        writeEnd = "EndRigid";
                        break;
                    case "Joint":
                        WriteNode(node.Name);
                        bw.Write(Int32.Parse(node.Attributes["Count"].Value));
                        writeEnd = "EndJoint";
                        break;
                    case "Name":
                        WriteNode(node.Name);
                        WriteText(node.Attributes["Text"].Value);
                        foreach (XmlNode data in node.ChildNodes)
                        {
                            switch(data.Name)
                            {
                                case "BoneIdx":
                                    WriteNode(data.Name);
                                    bw.Write(Int32.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "GroupNo":
                                    WriteNode(data.Name);
                                    bw.Write(byte.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "UnCollisionGroup":
                                    WriteNode(data.Name);
                                    bw.Write(Int16.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "Shape":
                                    WriteNode(data.Name);
                                    bw.Write(byte.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "Size":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["x"].Value));
                                    bw.Write(Single.Parse(data.Attributes["y"].Value));
                                    bw.Write(Single.Parse(data.Attributes["z"].Value));
                                    break;
                                case "Pos":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["x"].Value));
                                    bw.Write(Single.Parse(data.Attributes["y"].Value));
                                    bw.Write(Single.Parse(data.Attributes["z"].Value));
                                    break;
                                case "Rota":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["x"].Value));
                                    bw.Write(Single.Parse(data.Attributes["y"].Value));
                                    bw.Write(Single.Parse(data.Attributes["z"].Value));
                                    break;
                                case "Mass":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "MoveAtte":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "RotaAtte":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "Repulsion":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "Fric":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "CalcType":
                                    WriteNode(data.Name);
                                    bw.Write(byte.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "Type":
                                    WriteNode(data.Name);
                                    bw.Write(byte.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "CFM":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "ERP":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "RigidAIdx":
                                    WriteNode(data.Name);
                                    bw.Write(Int32.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "RigidBIdx":
                                    WriteNode(data.Name);
                                    bw.Write(Int32.Parse(data.Attributes["Value"].Value));
                                    break;
                                case "MoveLimMin":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["x"].Value));
                                    bw.Write(Single.Parse(data.Attributes["y"].Value));
                                    bw.Write(Single.Parse(data.Attributes["z"].Value));
                                    break;
                                case "MoveLimMax":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["x"].Value));
                                    bw.Write(Single.Parse(data.Attributes["y"].Value));
                                    bw.Write(Single.Parse(data.Attributes["z"].Value));
                                    break;
                                case "RotaLimMin":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["x"].Value));
                                    bw.Write(Single.Parse(data.Attributes["y"].Value));
                                    bw.Write(Single.Parse(data.Attributes["z"].Value));
                                    break;
                                case "RotaLimMax":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["x"].Value));
                                    bw.Write(Single.Parse(data.Attributes["y"].Value));
                                    bw.Write(Single.Parse(data.Attributes["z"].Value));
                                    break;
                                case "SpringMoveVal":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["x"].Value));
                                    bw.Write(Single.Parse(data.Attributes["y"].Value));
                                    bw.Write(Single.Parse(data.Attributes["z"].Value));
                                    break;
                                case "SpringRotaVal":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["x"].Value));
                                    bw.Write(Single.Parse(data.Attributes["y"].Value));
                                    bw.Write(Single.Parse(data.Attributes["z"].Value));
                                    break;
                                case "SpringMoveDamp":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["x"].Value));
                                    bw.Write(Single.Parse(data.Attributes["y"].Value));
                                    bw.Write(Single.Parse(data.Attributes["z"].Value));
                                    break;
                                case "SpringRotaDamp":
                                    WriteNode(data.Name);
                                    bw.Write(Single.Parse(data.Attributes["x"].Value));
                                    bw.Write(Single.Parse(data.Attributes["y"].Value));
                                    bw.Write(Single.Parse(data.Attributes["z"].Value));

                                    break;
                            }
                            
                        }
                        WriteNode(writeEnd);
                        break;
                    
                }
             }
            WriteNode("EndPhysics");
        }

        void WriteMatrix(string pText)
        {
            string[] values = pText.Split(" ".ToCharArray());
            for (int i = 0; i < 16 ; i++)
                bw.Write(Single.Parse(values[i]));      
        }

        void WriteText(string pText)
        {
            byte[] bt = encode.GetBytes(pText);
            bw.Write(bt.Length);
            bw.Write(bt);
        }

        void WriteNode(string pText)
        {
            bw.Write((byte)pText.Length);
            bw.Write((byte)0x8C);
            bw.Write(System.Text.Encoding.ASCII.GetBytes(pText));
            bw.Write((byte)0x60);
            bw.Write((byte)0x99);
        }
    }
}
