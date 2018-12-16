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

        

        static void Test()
        {
            string testString = "надо купить сигареты";
            string[] str = testString.Split();

            List<int> W = new List<int>();

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
            MessageBox.Show(probSpam.ToString());
            foreach (int w in W)
            {
                probSpam += Math.Log((1 + w) / (double)(8 + SpamDict.Count()));
            }
            MessageBox.Show(probSpam.ToString());
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
