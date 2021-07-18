using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Reflection;
using static DaVOICE_SUBTITLES.Log;
using static DaVOICE_SUBTITLES.GenerateSubtitle;
using static DaVOICE_SUBTITLES.FileArrangement;
using static DaVOICE_SUBTITLES.Characters;

namespace DaVOICE_SUBTITLES
{
    class Program
    {
        static void Main(string[] args)
        {
            //起動時にこのアプリケーションのあるフォルダを取得してその中にあるファイルをリネームする
            string CurrentFolder = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();

            bool flag = true;
            while (flag == true)
            {
                int TaskNumber = 4;
                if (args.Length == 1)
                {
                    if (args[0].ToUpper() == "M")
                    {
                        Show_Menu();
                        Console.Write(">>>");
                        TaskNumber = int.Parse(Console.ReadLine());
                        flag = true;
                    }
                }
                else
                {
                    flag = false;
                }

                List<string> SpeakerList;

                switch (TaskNumber)
                {
                    case 1:
                        //どちらのパターンか、判定
                        if (GetFileArrangementType(CurrentFolder) == "multi")
                        {
                            StartFileArrangement_MultiSpeakers(CurrentFolder);
                        }
                        else
                        {
                            Console.WriteLine(Environment.NewLine);
                            Console.WriteLine("キャラクター名を入力してください。");
                            StartFileArrangement(Console.ReadLine(), CurrentFolder);
                        }
                        break;

                    case 2:
                        SpeakerList = Get_SpeakersFolderPath_From_Folder(CurrentFolder);   //フォルダ内の話者フォルダ一覧を取得
                        for (int i = 0; i < SpeakerList.Count; i++)
                        {
                            List<string> SpeakerFiles = Get_FilePathList(SpeakerList[i]);
                            string SpeakerName = Path.GetFileName(SpeakerList[i]);
                            CreateSubtitleFile(SpeakerName, SpeakerFiles, SpeakerList[i] + "\\" + SpeakerName + "字幕.srt");
                        }
                        break;


                    case 3:
                        SpeakerList = Get_SpeakersFolderPath_From_Folder(CurrentFolder);   //フォルダ内の話者フォルダ一覧を取得
                        for (int i = 0; i < SpeakerList.Count; i++)
                        {
                            List<string> SpeakerFiles = Get_FilePathList(SpeakerList[i]);
                            string SpeakerName = Path.GetFileName(SpeakerList[i]);
                            CreateFCPXML(SpeakerList[i] + "\\" + SpeakerName + "字幕.srt");
                        }
                        break;

                    case 4:
                        //どちらのパターンか、判定
                        if (GetFileArrangementType(CurrentFolder) == "multi")
                        {
                            StartFileArrangement_MultiSpeakers(CurrentFolder);
                        }
                        else
                        {
                            Console.WriteLine(Environment.NewLine);
                            Console.WriteLine("キャラクター名を入力してください。");
                            StartFileArrangement(Console.ReadLine(), CurrentFolder);
                        }
                        SpeakerList = Get_SpeakersFolderPath_From_Folder(CurrentFolder);   //フォルダ内の話者フォルダ一覧を取得
                        for (int i = 0; i < SpeakerList.Count; i++)
                        {
                            List<string> SpeakerFiles = Get_FilePathList(SpeakerList[i]);
                            string SpeakerName = Path.GetFileName(SpeakerList[i]);
                            CreateSubtitleFile(SpeakerName, SpeakerFiles, SpeakerList[i] + "\\" + SpeakerName + "字幕.srt");
                            CreateFCPXML(SpeakerList[i] + "\\" + SpeakerName + "字幕.srt");
                        }
                        break;

                    case 5:
                        Environment.Exit(0);
                        break;
                }

                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("処理が完了しました。");
                Console.WriteLine(Environment.NewLine);
            }

        }

        private static void Show_Menu()
        {
            Console.WriteLine("処理を選択してください。");
            Console.WriteLine("\t1 - リネーム,フォルダ分け");
            Console.WriteLine("\t2 - 字幕ファイル(*.srt)生成");
            Console.WriteLine("\t3 - タイムライン(*.fcpxml)生成");
            Console.WriteLine("\t4 - リネーム,フォルダ分け,字幕ファイル(*.srt)生成,タイムライン形式に変換");
            Console.WriteLine("\t5 - 終了");

        }

    }
}
