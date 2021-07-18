using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using static DaVOICE_SUBTITLES.SoundInfo;
using static DaVOICE_SUBTITLES.FileArrangement;
using static DaVOICE_SUBTITLES.Log;

namespace DaVOICE_SUBTITLES
{
    class GenerateSubtitle
    {
        public static void CreateSubtitleFile(string Speaker, List<string> TalkTextFilePath, string SavePath)
        {

            try
            {
                string Subtitle = string.Empty; //主力するsrtファイルの中身

                TimeSpan InsertTime = new TimeSpan(0, 0, 0, 0, 0);
                TimeSpan EndTime = new TimeSpan(0, 0, 0, 0, 0);
                for (int i = 0; i < TalkTextFilePath.Count; i++)
                {
                    string VoiceFilePath = Path.Combine(Directory.GetParent(TalkTextFilePath[i]).FullName, Path.GetFileNameWithoutExtension(TalkTextFilePath[i]) + ".wav");

                    TimeSpan VoiceLength = new TimeSpan(0, 0, 0, 0, GetSoundLength(VoiceFilePath));

                    string TalkText = ReadFile(TalkTextFilePath[i]);
                    TalkText = TalkText.Replace(Speaker + "＞", string.Empty).Replace(Environment.NewLine, string.Empty);

                    EndTime = InsertTime + VoiceLength;

                    Subtitle += i + 1;
                    Subtitle += Environment.NewLine;
                    Subtitle += InsertTime.ToString("hh':'mm':'ss','fff") + " --> " + EndTime.ToString("hh':'mm':'ss','fff");
                    Subtitle += Environment.NewLine;
                    Subtitle += TalkText;
                    Subtitle += Environment.NewLine;
                    Subtitle += Environment.NewLine;

                    InsertTime = EndTime;

                }

                using (StreamWriter sw = new StreamWriter(SavePath, false, Encoding.Default))
                {
                    sw.Write(Subtitle);
                }
            }
            catch (Exception e)
            {
                OutputLog(e.Message);
            }


        }


        public static void CreateFCPXML(string SRT_FilePath)
        {

            try
            {
                //字幕ファイルの親フォルダ
                string ParentFolder = Directory.GetParent(SRT_FilePath).ToString();
                //字幕ファイルの名前
                string FileName = Path.GetFileNameWithoutExtension(SRT_FilePath).ToString();

                //出力用のファイル名
                string SaveFileName = Path.Combine(ParentFolder, FileName + ".fcpxml");

                // 出力する場所を指定
                XmlWriter xw = XmlWriter.Create(SaveFileName);


                //必要な項目のインプット
                int audioRate = 48;
                int height = 1024;
                int width = 1980;
                int frameRate = 30;


                xw.WriteStartElement("fcpxml");
                xw.WriteAttributeString("version", "1.6");

                xw.WriteStartElement("resources");

                xw.WriteStartElement("format");
                xw.WriteAttributeString("height", $"{height}");  //要変更
                xw.WriteAttributeString("width", $"{width}");   //要変更
                xw.WriteAttributeString("frameDuration", $"1/{frameRate}s");  //要変更
                xw.WriteAttributeString("id", "r1");
                xw.WriteEndElement();   //formatの

                xw.WriteStartElement("effect");
                xw.WriteAttributeString("id", "r2");
                xw.WriteAttributeString("uid", ".../Titles.localized/Bumper:Opener.localized/Basic Title.localized/Basic Title.moti\" name = \"Basic Title\"");
                xw.WriteEndElement();   //effectの

                xw.WriteEndElement();   //resourcesの

                xw.WriteStartElement("library");
                xw.WriteAttributeString("location", "");

                xw.WriteStartElement("event");
                xw.WriteAttributeString("name", "Title");

                xw.WriteStartElement("project");
                xw.WriteAttributeString("name", "SUBTITLES");

                xw.WriteStartElement("sequence");
                xw.WriteAttributeString("duration", "12s");   //要変更
                xw.WriteAttributeString("format", "r1");
                xw.WriteAttributeString("tcStart", "0s");
                xw.WriteAttributeString("tcFormat", "NDF");
                xw.WriteAttributeString("audioLayout", "stereo");
                xw.WriteAttributeString("audioRate", $"{audioRate}k");    //要変更

                xw.WriteStartElement("spine");



                double duration = 0; //duration計算用
                double offset = 0; //duration計算用2
                //ここから字幕本文
                List<string> content = Read_SRT(SRT_FilePath);
                for (int i = 0; i < content.Count; i++)
                {
                    //最初にデータをばらす
                    string[] tmp = content[i].Split('|');
                    string startTime = tmp[0];
                    string endTime = tmp[1];
                    string body = tmp[2];

                    duration = frameRate * 100000 * Calculate_Duration(startTime, endTime).TotalSeconds;


                    xw.WriteStartElement("title");
                    xw.WriteAttributeString("name", "Basic Title: ");
                    xw.WriteAttributeString("lane", "1");
                    xw.WriteAttributeString("offset", $"{(long)offset}/{frameRate * 100000}s");
                    xw.WriteAttributeString("ref", "r2");
                    xw.WriteAttributeString("duration", $"{duration}/{frameRate * 100000}s");
                    xw.WriteAttributeString("start", $"{(long)offset}/{frameRate * 100000}s");

                    offset += duration;


                    xw.WriteStartElement("param");
                    xw.WriteAttributeString("name", "Position");
                    xw.WriteAttributeString("key", "9999/999166631/999166633/1/100/101");
                    xw.WriteAttributeString("value", "-1.67499 -470.934");
                    xw.WriteEndElement();


                    xw.WriteStartElement("text");

                    xw.WriteStartElement("text-style");
                    xw.WriteAttributeString("ref", $"ts{i + 1}-1");
                    xw.WriteValue(body);
                    xw.WriteEndElement();
                    xw.WriteEndElement();

                    xw.WriteStartElement("text-style-def");
                    xw.WriteAttributeString("id", $"ts{i + 1}-1");

                    xw.WriteStartElement("text-style");
                    xw.WriteAttributeString("font", "Lucida Sans");
                    xw.WriteAttributeString("fontSize", "36");
                    xw.WriteAttributeString("fontFace", "Regular");
                    xw.WriteAttributeString("fontColor", "0.960784 0.960784 0.960784 1");
                    xw.WriteAttributeString("baseline", "29");
                    xw.WriteAttributeString("shadowColor", "0 0 0 1");
                    xw.WriteAttributeString("shadowOffset", "5 315");
                    xw.WriteAttributeString("alignment", "center");
                    xw.WriteEndElement();

                    xw.WriteEndElement();   //textの
                    xw.WriteEndElement();   //titleの

                }


                xw.WriteEndElement();   //fcpxmlの

                // xmlに書き込む
                xw.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                throw;
            }




        }


        public static List<string> Read_SRT(string SRT_FilePath)
        {
            List<string> ret = new List<string>();
            string time = string.Empty;
            string content = string.Empty;

            try
            {
                //SRTファイルの読み込み
                string line = string.Empty;
                int counter = 1;
                StreamReader file = new StreamReader(SRT_FilePath, Encoding.Default);
                while ((line = file.ReadLine()) != null)
                {



                    if ((counter % 4) == 2) //時間
                    {
                        time = line;
                    }
                    else if ((counter % 4) == 3)    //しゃべってる内容
                    {
                        content = line;
                    }
                    else if ((counter % 4) == 0)
                    {
                        string[] t_div = Time_Divide(time);
                        ret.Add(t_div[0] + "|" + t_div[1] + "|" + content);
                    }

                    counter++;
                }

                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ret;
                throw;
            }
        }


        public static string[] Time_Divide(string time)
        {

            string[] del = { "-->" };
            string[] ret = time.Split(del, StringSplitOptions.None);
            ret[0].Trim();
            ret[1].Trim();

            return ret;
        }

        public static TimeSpan Calculate_Duration(string startTime, string endTime)
        {
            //timeには00:00:01,000のようにして記録されている。

            //時間、分、秒、ミリ秒にばらす
            startTime = startTime.Replace(",", ":");
            endTime = endTime.Replace(",", ":");

            string[] startTmp = startTime.Split(':');
            string[] endTmp = endTime.Split(':');

            TimeSpan sT = new TimeSpan(0, Int32.Parse(startTmp[0]), Int32.Parse(startTmp[1]), Int32.Parse(startTmp[2]), Int32.Parse(startTmp[3]));
            TimeSpan eT = new TimeSpan(0, Int32.Parse(endTmp[0]), Int32.Parse(endTmp[1]), Int32.Parse(endTmp[2]), Int32.Parse(endTmp[3]));


            return (eT - sT);
        }
    }

}
