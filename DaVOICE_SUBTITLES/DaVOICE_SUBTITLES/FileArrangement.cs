using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using static DaVOICE_SUBTITLES.Log;
using static DaVOICE_SUBTITLES.Characters;

namespace DaVOICE_SUBTITLES
{
    class FileArrangement
    {
        public static string GetFileArrangementType(string FolderPath)
        {
            string FileArrangementType = "multi";
            int SpeakersCounter = 0;

            List<string> FileNameList = Get_FilePathList(FolderPath);
            try
            {
                for (int i = 0; i < FileNameList.Count; i++)
                {
                    for (int j = 0; j < CharacterName.Count; j++)
                    {
                        string Contents = ReadFile(FileNameList[i]);
                        if (Contents.StartsWith(CharacterName[j] + "＞"))
                        {
                            SpeakersCounter++;
                        }
                    }
                }

                if (SpeakersCounter == 0)
                {
                    FileArrangementType = "mono";
                }

            }
            catch (Exception e)
            {
                OutputLog(e.Message);
            }


            return FileArrangementType;

        }

        public static void StartFileArrangement(string SpeakerName, string FolderPath)
        {
            List<string> FileNameList = Get_FileNameList(FolderPath);
            try
            {
                for (int i = 0; i < FileNameList.Count; i++)
                {
                    //テキストを読み込んでリネーム
                    string TextFilePath = Path.Combine(FolderPath, FileNameList[i] + ".txt");
                    string VoiceFilePath = Path.Combine(FolderPath, FileNameList[i] + ".wav");
                    string Contents = ReadFile(TextFilePath);
                    string NewFileName = Contents.Replace(Environment.NewLine, string.Empty);

                    File.Move(TextFilePath, Path.Combine(FolderPath, GenerateNewFileName(SpeakerName, Contents, i) + ".txt"));
                    File.Move(VoiceFilePath, Path.Combine(FolderPath, GenerateNewFileName(SpeakerName, Contents, i) + ".wav"));

                    MoveFile(SpeakerName, FolderPath, GenerateNewFileName(SpeakerName, Contents, i));
                }
            }
            catch (Exception e)
            {
                OutputLog(e.Message);
            }
        }

        public static void StartFileArrangement_MultiSpeakers(string FolderPath)
        {
            List<string> FileNameList = Get_FileNameList(FolderPath);
            try
            {
                for (int i = 0; i < FileNameList.Count; i++)
                {
                    //テキストを読み込んで話者に合わせてリネーム
                    string TextFilePath = Path.Combine(FolderPath, FileNameList[i] + ".txt");
                    string VoiceFilePath = Path.Combine(FolderPath, FileNameList[i] + ".wav");
                    string Contents = ReadFile(TextFilePath);
                    string Speaker = WhoSpeaks(Contents);

                    File.Move(TextFilePath, Path.Combine(FolderPath, GenerateNewFileName(Speaker, Contents, i) + ".txt"));
                    File.Move(VoiceFilePath, Path.Combine(FolderPath, GenerateNewFileName(Speaker, Contents, i) + ".wav"));

                    MoveFile(Speaker, FolderPath, GenerateNewFileName(Speaker, Contents, i));
                }
            }
            catch (Exception e)
            {
                OutputLog(e.Message);
            }

        }

        public static List<string> Get_FileNameList(string FolderPath)
        {
            List<string> FilePathList = new List<string>();
            List<string> FileNameList = new List<string>();

            try
            {
                FilePathList = Directory.GetFiles(FolderPath, "*.txt", SearchOption.TopDirectoryOnly)
                    .OrderBy(FilePath => File.GetCreationTime(FilePath).Date)
                    .ThenBy(FilePath => File.GetCreationTime(FilePath).TimeOfDay)
                    .ThenBy(FilePath => File.GetCreationTime(FilePath).Millisecond)
                    .ToList();

                foreach (string filePath in FilePathList)
                {
                    FileNameList.Add(Path.GetFileNameWithoutExtension(filePath));
                }

                return FileNameList;
            }
            catch (Exception e)
            {
                OutputLog(e.Message);
                return FileNameList;
            }

        }

        public static List<string> Get_FilePathList(string FolderPath)
        {
            List<string> FilePathList = new List<string>();

            try
            {
                FilePathList = Directory.GetFiles(FolderPath, "*.txt", SearchOption.TopDirectoryOnly)
                    .OrderBy(FilePath => File.GetCreationTime(FilePath).Date)
                    .ThenBy(FilePath => File.GetCreationTime(FilePath).TimeOfDay)
                    .ThenBy(FilePath => File.GetCreationTime(FilePath).Millisecond)
                    .ToList();

                return FilePathList;
            }
            catch (Exception e)
            {
                OutputLog(e.Message);
                return FilePathList;
            }
        }

        public static string ReadFile(string FilePath)
        {
            string data;
            data = string.Empty;  //読み込んだデータ

            try
            {

                using (StreamReader sr = new StreamReader(FilePath, Encoding.Default))
                {
                    data = sr.ReadToEnd();
                }

                return data;
            }
            catch (Exception e)
            {
                OutputLog(e.Message);
                return data;
            }
        }

        public static string WhoSpeaks(string Contents)
        {
            string Speaker = "unknown";

            for (int i = 0; i < CharacterName.Count; i++)
            {
                if (Contents.StartsWith(CharacterName[i] + "＞"))
                {
                    Speaker = CharacterName[i];
                    break;

                }

            }

            return Speaker;

        }

        private static string GenerateNewFileName(string Speaker, string Contents, int FileNum)
        {
            try
            {
                string NewName = string.Empty;
                Contents = Contents.Replace(Speaker + "＞", string.Empty);
                Contents = Contents.Replace(System.Environment.NewLine, string.Empty);

                NewName = String.Format("{0:0000}", FileNum) + "_" + Speaker + "_" + Contents;

                //ファイル名が248文字以上になっていないか確認
                if (NewName.Length > 248)
                {
                    NewName = NewName.Substring(0, 248);
                }

                return NewName;
            }
            catch (Exception e)
            {
                OutputLog(e.Message);
                return string.Empty;
            }
        }

        private static void MoveFile(string Speaker, string OldFolderPath, string FileName)
        {
            try
            {
                string SpeakerDirectory = Path.Combine(OldFolderPath, Speaker);
                //話者フォルダが存在するかチェック。なかったら作る
                if (Directory.Exists(SpeakerDirectory) == false)
                {
                    Directory.CreateDirectory(SpeakerDirectory);
                }

                //ファイルを移動
                if (FileName.Contains(Speaker) == true)
                {
                    if (File.Exists(Path.Combine(SpeakerDirectory, FileName + ".txt")) == false)
                    {
                        File.Move(Path.Combine(OldFolderPath, FileName + ".txt"), Path.Combine(SpeakerDirectory, FileName + ".txt"));
                        File.Move(Path.Combine(OldFolderPath, FileName + ".wav"), Path.Combine(SpeakerDirectory, FileName + ".wav"));
                    }
                    else
                    {
                        File.Delete(Path.Combine(OldFolderPath, FileName + ".txt"));
                        File.Delete(Path.Combine(OldFolderPath, FileName + ".wav"));
                    }


                }

            }
            catch (Exception e)
            {
                OutputLog(e.Message);
            }


        }

        public static List<string> Get_SpeakersFolderPath_From_Folder(string FolderPath)
        {
            List<string> SpeakersFolderPath = new List<string>();

            //フォルダ内のキャラ名フォルダを検索する
            string[] dirs = Directory.GetDirectories(FolderPath, "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in dirs)
            {
                string DirName = Path.GetFileName(dir);
                if (CharacterName.Contains(DirName))
                {
                    SpeakersFolderPath.Add(dir);
                }

            }

            return SpeakersFolderPath;

        }
    }
}
