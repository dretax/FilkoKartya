using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace FilkoKartya
{
    internal class IniParser
    {
        /// <summary>
        /// Ini útvonala
        /// </summary>
        private string iniFilePath;
        /// <summary>
        /// Hashtable, hogy minden típusnak megfeleljen
        /// </summary>
        private Hashtable keyPairs = new Hashtable();
        /// <summary>
        /// Ini neve
        /// </summary>
        internal string Name;
        /// <summary>
        /// Adatok
        /// </summary>
        private System.Collections.Generic.List<SectionPair> tmpList = new System.Collections.Generic.List<SectionPair>();

        /// <summary>
        /// Ini beolvasása szekcióktól, majd kulcsoktól és értékektől
        /// </summary>
        /// <param name="iniPath"></param>
        internal IniParser(string iniPath)
        {
            string str2 = null;
            this.iniFilePath = iniPath;
            this.Name = Path.GetFileNameWithoutExtension(iniPath);

            if (!File.Exists(iniPath)) throw new FileNotFoundException("Nem található a file! " + iniPath);

            try
            {
                using (TextReader reader = new StreamReader(iniPath))
                {
                    for (string str = reader.ReadLine(); str != null; str = reader.ReadLine())
                    {
                        str = str.Trim();
                        if (str == "") continue;

                        if (str.StartsWith("[") && str.EndsWith("]"))
                            str2 = str.Substring(1, str.Length - 2);
                        else
                        {
                            SectionPair pair;

                            if (str.StartsWith(";"))
                                str = str.Replace("=", "%eq%") + @"=%comment%";

                            string[] strArray = str.Split(new char[] { '=' }, 2);
                            string str3 = null;
                            if (str2 == null)
                            {
                                str2 = "ROOT";
                            }
                            pair.Section = str2;
                            pair.Key = strArray[0];
                            if (strArray.Length > 1)
                            {
                                str3 = strArray[1];
                            }
                            this.keyPairs.Add(pair, str3);
                            this.tmpList.Add(pair);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hiba az ini olvasás közben: " + ex);
            }
        }

        /// <summary>
        /// Adat hozzáadása
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="settingName"></param>
        internal void AddSetting(string sectionName, string settingName)
        {
            this.AddSetting(sectionName, settingName, string.Empty);
        }

        /// <summary>
        /// Adat hozzáadása értékkel
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="settingName"></param>
        /// <param name="settingValue"></param>
        internal void AddSetting(string sectionName, string settingName, string settingValue)
        {
            SectionPair pair;
            pair.Section = sectionName;
            pair.Key = settingName;
            if (settingValue == null)
                settingValue = string.Empty;

            if (this.keyPairs.ContainsKey(pair))
            {
                this.keyPairs.Remove(pair);
            }
            if (this.tmpList.Contains(pair))
            {
                this.tmpList.Remove(pair);
            }
            this.keyPairs.Add(pair, settingValue);
            this.tmpList.Add(pair);
        }

        /// <summary>
        /// Szekciók megszámlálása
        /// </summary>
        /// <returns></returns>
        internal int Count()
        {
            return this.Sections.Length;
        }

        /// <summary>
        /// Adat törlése
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="settingName"></param>
        internal void DeleteSetting(string sectionName, string settingName)
        {
            SectionPair pair;
            pair.Section = sectionName;
            pair.Key = settingName;
            if (this.keyPairs.ContainsKey(pair))
            {
                this.keyPairs.Remove(pair);
                this.tmpList.Remove(pair);
            }
        }

        /// <summary>
        /// Szekció kulcsainak lekérdezése
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        internal string[] EnumSection(string sectionName)
        {
            List<string> list = new List<string>();
            foreach (SectionPair pair in this.tmpList)
            {
                if (pair.Key.StartsWith(";"))
                    continue;

                if (pair.Section == sectionName)
                {
                    list.Add(pair.Key);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Szekciók
        /// </summary>
        internal string[] Sections
        {
            get
            {
                List<string> list = new List<string>();
                foreach (SectionPair pair in this.tmpList)
                {
                    if (!list.Contains(pair.Section))
                    {
                        list.Add(pair.Section);
                    }
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// Adat lekérdezése
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="settingName"></param>
        /// <returns></returns>
        internal string GetSetting(string sectionName, string settingName)
        {
            SectionPair pair;
            pair.Section = sectionName;
            pair.Key = settingName;
            return (string)this.keyPairs[pair];
        }

        /// <summary>
        /// Ini file mentése
        /// </summary>
        internal void Save()
        {
            SaveSettings(this.iniFilePath);
        }

        /// <summary>
        /// Mentést kezelő method
        /// </summary>
        /// <param name="newFilePath"></param>
        private void SaveSettings(string newFilePath)
        {
            ArrayList list = new ArrayList();
            string str = "";
            string str2 = "";
            foreach (SectionPair pair in this.tmpList)
            {
                if (!list.Contains(pair.Section))
                {
                    list.Add(pair.Section);
                }
            }
            foreach (string str3 in list)
            {
                str2 = str2 + "[" + str3 + "]\r\n";
                foreach (SectionPair pair2 in this.tmpList)
                {
                    if (pair2.Section == str3)
                    {
                        str = (string)this.keyPairs[pair2];
                        if (str != null)
                        {
                            if (str == "%comment%")
                            {
                                str = "";
                            }
                            else
                            {
                                str = "=" + str;
                            }
                        }
                        str2 = str2 + pair2.Key.Replace("%eq%", "=") + str + "\r\n";
                    }
                }
                str2 = str2 + "\r\n";
            }

            using (TextWriter writer = new StreamWriter(newFilePath))
                writer.Write(str2);
        }

        /// <summary>
        /// Már létező beállítás átírása
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="settingName"></param>
        /// <param name="value"></param>
        internal void SetSetting(string sectionName, string settingName, string value)
        {
            SectionPair pair;
            pair.Section = sectionName;
            pair.Key = settingName;
            if (string.IsNullOrEmpty(value))
                value = string.Empty;

            if (this.keyPairs.ContainsKey(pair))
            {
                this.keyPairs[pair] = value;
            }
        }

        /// <summary>
        /// A tárolt hashtableben struktúrát alkalmazok az egyszerűbb tárolás érdekében
        /// </summary>
        private struct SectionPair
        {
            public string Section;
            public string Key;
        }
    }
}
