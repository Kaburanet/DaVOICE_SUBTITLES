using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DaVOICE_SUBTITLES
{
    class Characters
    {
        public static List<string> CharacterName = new List<string>()
        {
            "紲星あかり",
            "結月ゆかり",
            "音街ウナ",
            "桜乃そら",
            "琴葉 茜",
            "琴葉 葵",
            "伊織弓鶴",
            "ついな",
            "東北イタコ",
        };

        public static Dictionary<string, string> CharacterNameAlphabetDic = new Dictionary<string, string>()
        {
            { "紲星あかり","akari" },
            {"結月ゆかり","yukari" },
            {"音街ウナ","una" },
            {"桜乃そら","sora" },
            {"琴葉 茜","akane" },
            {"琴葉 葵","aoi" },
            {"伊織弓鶴","yuduru" },
            {"ついな","tusina" },
            {"東北イタコ","itako" }

        };

        public static List<string> GetInstalledSpeakersNameList(string VoiceroidAppFolderPath)
        {
            return GetInstalledSpeakerNameList(VoiceroidAppFolderPath);
        }



        private static List<string> GetInstalledSpeakerNameList(string VoiceroidAppFolder)
        {
            List<string> SpeakerNameList = new List<string>();

            if (Directory.Exists(VoiceroidAppFolder) == false) return SpeakerNameList;

            //全フォルダを取得
            string[] folders = Directory.GetDirectories(VoiceroidAppFolder);

            //フォルダ内を探索して話者を探す
            for (int i = 0; i < folders.Length; i++)
            {
                for (int j = 0; j < CharacterNameAlphabetDic.Count; j++)
                {
                    if (folders[i].Contains(CharacterNameAlphabetDic[CharacterName[j]]) == true)
                    {
                        SpeakerNameList.Add(CharacterName[j]);
                    }
                }

            }

            return SpeakerNameList;
        }
    }
}
