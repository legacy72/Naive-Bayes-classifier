﻿using System;
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
        string path = @"C:\Users\vladr\Desktop\test.txt"; // тут укажи свой путь к файлу для тренировки
        Dictionary<string, int> SpamDict = new Dictionary<string, int>(); // словарь спама
        Dictionary<string, int> HamDict = new Dictionary<string, int>(); // словарь неспама
        double SpamFrequencies = 0; // количество документов в обучающей выборке принадлежащих классу спам;
        double HamFrequencies = 0; // количество документов в обучающей выборке принадлежащих классу неспам;

        public Form1()
        {
            InitializeComponent();

            ReadFromFile();
            Test();
        }

        // чтение данных из файла
        void ReadFromFile()
        {
            string contents = File.ReadAllText(path, Encoding.UTF8);
            string[] str = contents.Split('\n');

            foreach (string s in str)
            {
                SplitRow(s);
            }
        }

        void SplitRow(string s)
        {
            // расплитили текст из файла на 
            // text[0] - все слова сообщения
            // text[1] - категория (спам/неспам)
            string[] text = new string[2];
            text = s.Split(':');

            FillDictionary(text[0], text[1]);
        }

        // заполнение словарей словами, которые принадлежат к конкретной категории
        void FillDictionary(string s, string filter)
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

        // Вычисление вероятности, что сообщение спам
        double GetProbabilitySpam(string testString, int countUniqueKeys)
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

        // Вычисление вероятности, что сообщение неспам
        double GetProbabilityHam(string testString, int countUniqueKeys)
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

        // Подсчет кол-ва уникальных слов во всей выборке
        int GetCountUniqueKeys()
        {
            List<string> keyListSpam = new List<string>(SpamDict.Keys);
            List<string> keyListHam = new List<string>(HamDict.Keys);
            keyListSpam.AddRange(keyListHam);

            keyListSpam = keyListSpam.Distinct().ToList();

            return keyListSpam.Count();
        }

        // Формирование вероятностного пространства
        double FormationOfProbabilisticSpace(double a, double b)
        {
            return Math.Exp(a) / (Math.Exp(a) + Math.Exp(b));
        }

        // сама проверка на спам
        void Test()
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
            
            //MessageBox.Show("Вероятность, что сообщение спам: " + probSpam.ToString());
            //MessageBox.Show("Вероятность, что сообщение не спам: " + probHam.ToString());

            ShowResultGraph(probSpam, probHam);
        }

        //Вывод результата на диаграмму
        void ShowResultGraph(double probSpam, double probHam)
        {
            chart1.Series["spamProb"].Points.AddXY("спам", probSpam * 100 + 60);
            chart1.Series["spamProb"].Points.AddXY("не спам", probHam * 100);
        }

        // Вывод словаря для тестирования (можно удалить)
        void PrintDict(Dictionary<string, int> dictionary)
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
