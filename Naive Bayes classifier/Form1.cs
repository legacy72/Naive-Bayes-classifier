using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Naive_Bayes_classifier
{
    public partial class Form1 : Form
    {
        static string path = @"C:\Users\vladr\Desktop\test.txt";
        static Dictionary<string, int> SpamDict = new Dictionary<string, int>();
        static Dictionary<string, int> HamDict = new Dictionary<string, int>();
        static double SpamFrequencies = 0;
        static double HamFrequencies = 0;
        public Form1()
        {
            InitializeComponent();

            ReadFromFile();
            Test();
            this.Close();
        }

        static void ReadFromFile()
        {
            string contents = File.ReadAllText(path, Encoding.UTF8);

            string[] str = contents.Split('\n');

            
            foreach (string s in str)
            {
                SplitRow(s);
            }

            //PrintDict(SpamDict);
            //MessageBox.Show(SpamFrequencies.ToString());
            //PrintDict(HamDict);
            //MessageBox.Show(HamFrequencies.ToString());
        }

        static void SplitRow(string s)
        {
            string[] text = new string[2];
            text = s.Split(':');

           
            FillDictionary(text[0], text[1]);
        }

        static void FillDictionary(string s, string filter)
        {
            string[] words = s.Split();

            if (filter == "spam\r" || filter == "spam")
            {
                SpamFrequencies++;
                foreach (string word in words)
                {
                    if (SpamDict.ContainsKey(word))
                    {
                        SpamDict[word] += 1;
                    }
                    else
                    {
                        SpamDict.Add(word, 1);
                    }
                }
            }
            else if (filter == "ham\r" || filter == "ham")
            {
                HamFrequencies++;
                foreach (string word in words)
                {
                    if (HamDict.ContainsKey(word))
                    {
                        HamDict[word] += 1;
                    }
                    else
                    {
                        HamDict.Add(word, 1);
                    }
                }
            }
            else
            {
                MessageBox.Show("неверный формат данных в файле");
            }
        }

        

    

        static double GetProbabilitySpam(string testString, int countUniqueKeys)
        {
            string[] str = testString.Split();

            List<int> W = new List<int>(); // сколько раз слово встречалось в классе спам

            foreach (string s in str)
            {
                if (SpamDict.ContainsKey(s))
                {
                    W.Add(SpamDict[s]);
                }
                else
                {
                    W.Add(0);
                }
            }

            double probSpam = Math.Log(SpamFrequencies / (SpamFrequencies + HamFrequencies));
            foreach (int w in W)
            {
                probSpam += Math.Log((1 + w) / (double)(countUniqueKeys + SpamDict.Count()));
            }
          
            return probSpam;
        }

        static double GetProbabilityHam(string testString, int countUniqueKeys)
        {
            string[] str = testString.Split();

            List<int> W = new List<int>(); // сколько раз слово встречалось в классе не спам

            foreach (string s in str)
            {
                if (HamDict.ContainsKey(s))
                {
                    W.Add(HamDict[s]);
                }
                else
                {
                    W.Add(0);
                }
            }

            double probHam = Math.Log(HamFrequencies / (SpamFrequencies + HamFrequencies));
            foreach (int w in W)
            {
                probHam += Math.Log((1 + w) / (double)(countUniqueKeys + HamDict.Count()));
            }

            return probHam;
        }

        static int GetCountUniqueKeys()
        {
            List<string> keyListSpam = new List<string>(SpamDict.Keys);
            List<string> keyListHam = new List<string>(HamDict.Keys);
            keyListSpam.AddRange(keyListHam);

            keyListSpam = keyListSpam.Distinct().ToList();

            return keyListSpam.Count();
        }

        static double FormationOfProbabilisticSpace(double a, double b)
        {
            return Math.Exp(a) / (Math.Exp(a) + Math.Exp(b));
        }

        static void Test()
        {
            // строка проверки на спам
            string testString = "надо купить сигареты";
            // количество уникальных слов в обеих выборках
            int countUniqueKeys = GetCountUniqueKeys();
            
            // подсчет шанса что спам (не вероятностное пространство)
            double resSpam = GetProbabilitySpam(testString, countUniqueKeys);

            // подсчет шанса что не спам (не вероятностное пространство)
            double resHam = GetProbabilityHam(testString, countUniqueKeys);

            double probSpam = FormationOfProbabilisticSpace(resSpam, resHam);
            double probHam = FormationOfProbabilisticSpace(resHam, resSpam); // ну либо 1 - probSpam (как угодно)
            MessageBox.Show("Вероятность, что сообщение спам: " + probSpam.ToString());
            MessageBox.Show("Вероятность, что сообщение не спам: " + probHam.ToString());
        }

        static void PrintDict(Dictionary<string, int> dictionary)
        {
            string s = "";
            foreach (KeyValuePair<string, int> kvp in dictionary)
            {
                s += string.Format("{0}: {1}", kvp.Key, kvp.Value) + "\n";
            }
            MessageBox.Show(s);
        }
    }
}
