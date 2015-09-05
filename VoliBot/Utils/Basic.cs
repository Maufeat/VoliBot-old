using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VoliBot.Utils;

namespace VoliBot.Utils
{
    class Basic
    {
        private static Random randomizer = new Random();

        private static string _vowels = "aeiouy";
        private static string _consonants = "bcdfghjklmnpqrstvwxz";

        public static int Rand(int min, int max)
        {
            return randomizer.Next(min, max + 1);
        }

        public static string GetVowel
        {
            get
            {
                return _vowels.Substring((Rand(0, _vowels.Length - 1)), 1);
            }
        }

        public static string GetConsonant
        {
            get
            {
                return _consonants.Substring((Rand(0, _consonants.Length - 1)), 1);
            }
        }

        public static string RandomName()
        {
            var name = GetConsonant.ToUpper();
            name += GetVowel;

            if (Rand(0, 1) == 0)
                name += GetVowel;

            name += GetConsonant;

            if (Rand(0, 1) == 0)
                name += name[name.Length - 1].ToString();

            name += GetVowel;

            if (Rand(0, 1) == 0)
            {
                name += GetConsonant;
                name += GetVowel;
            }

            if (Rand(0, 1) == 0)
            {
                name += Rand(0, 8);
                if (Rand(0, 1) == 0)
                {
                    name += Rand(0, 8);
                }
            }

            return name;
        }
        public static Image returnIcon(int id)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string specificFolder = Path.Combine(folder, "VoliBot");
            string url = "http://ddragon.leagueoflegends.com/cdn/5.15.1/img/profileicon/" + id.ToString() + ".png";
            string path = specificFolder + "\\assets\\icons";
            string file = path + "\\" + id + ".png";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!File.Exists(file))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(url, file);
                }
            }
            return Image.FromFile(file);
        }

        public static string UppercaseFirst(string value)
        {
            value = value.ToLower();
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }
        public static void ReplaceGameConfig(string path)
        {
            try
            {
                path = path + @"Config\game.cfg";
                FileInfo fileInfo = new FileInfo(path);
                fileInfo.IsReadOnly = false;
                fileInfo.Refresh();
                string str = "[General]\nGameMouseSpeed=9\nEnableAudio=0\nUserSetResolution=1\nBindSysKeys=0\nSnapCameraOnRespawn=1\nOSXMouseAcceleration=1\nAutoAcquireTarget=1\nEnableLightFx=0\nWindowMode=1\nShowTurretRangeIndicators=0\nPredictMovement=0\nWaitForVerticalSync=0\nColors=16\nHeight=200\nWidth=300\nSystemMouseSpeed=0\nCfgVersion=4.13.265\n\n[HUD]\nShowNeutralCamps=0\nDrawHealthBars=0\nAutoDisplayTarget=0\nMinimapMoveSelf=0\nItemShopPrevY=19\nItemShopPrevX=117\nShowAllChannelChat=0\nShowTimestamps=0\nObjectTooltips=0\nFlashScreenWhenDamaged=0\nNameTagDisplay=1\nShowChampionIndicator=0\nShowSummonerNames=0\nScrollSmoothingEnabled=0\nMiddleMouseScrollSpeed=0.5000\nMapScrollSpeed=0.5000\nShowAttackRadius=0\nNumericCooldownFormat=3\nSmartCastOnKeyRelease=0\nEnableLineMissileVis=0\nFlipMiniMap=0\nItemShopResizeHeight=47\nItemShopResizeWidth=455\nItemShopPrevResizeHeight=200\nItemShopPrevResizeWidth=300\nItemShopItemDisplayMode=1\nItemShopStartPane=1\n\n[Performance]\nShadowsEnabled=0\nEnableHUDAnimations=0\nPerPixelPointLighting=0\nEnableParticleOptimizations=0\nBudgetOverdrawAverage=10\nBudgetSkinnedVertexCount=10\nBudgetSkinnedDrawCallCount=10\nBudgetTextureUsage=10\nBudgetVertexCount=10\nBudgetTriangleCount=10\nBudgetDrawCallCount=1000\nEnableGrassSwaying=0\nEnableFXAA=0\nAdvancedShader=0\nFrameCapType=3\nGammaEnabled=1\nFull3DModeEnabled=0\nAutoPerformanceSettings=0\n=0\nEnvironmentQuality=0\nEffectsQuality=0\nShadowQuality=0\nGraphicsSlider=0\n\n[Volume]\nMasterVolume=1\nMusicMute=0\n\n[LossOfControl]\nShowSlows=0\n\n[ColorPalette]\nColorPalette=0\n\n[FloatingText]\nCountdown_Enabled=0\nEnemyTrueDamage_Enabled=0\nEnemyMagicalDamage_Enabled=0\nEnemyPhysicalDamage_Enabled=0\nTrueDamage_Enabled=0\nMagicalDamage_Enabled=0\nPhysicalDamage_Enabled=0\nScore_Enabled=0\nDisable_Enabled=0\nLevel_Enabled=0\nGold_Enabled=0\nDodge_Enabled=0\nHeal_Enabled=0\nSpecial_Enabled=0\nInvulnerable_Enabled=0\nDebug_Enabled=1\nAbsorbed_Enabled=1\nOMW_Enabled=1\nEnemyCritical_Enabled=0\nQuestComplete_Enabled=0\nQuestReceived_Enabled=0\nMagicCritical_Enabled=0\nCritical_Enabled=1\n\n[Replay]\nEnableHelpTip=0";
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(str);
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(builder.ToString());
                }
                fileInfo.IsReadOnly = true;
                fileInfo.Refresh();
            }
            catch (Exception){}
        }
        public static void DeleteGameConfig(string path)
        {
            try
            {
                path = path + @"Config\game.cfg";
                File.Delete(path);
            }
            catch (Exception) { }
        }
    }

}
