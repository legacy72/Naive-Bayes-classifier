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
        string path = @"C:\Users\vladr\source\repos\Naive Bayes classifier\data\training data.txt"; // тут укажи свой путь к файлу для тренировки
        Dictionary<string, int> SpamDict = new Dictionary<string, int>(); // словарь спама
        Dictionary<string, int> HamDict = new Dictionary<string, int>(); // словарь неспама
        double SpamFrequencies = 0; // количество документов в обучающей выборке принадлежащих классу спам;
        double HamFrequencies = 0; // количество документов в обучающей выборке принадлежащих классу неспам;

        public Form1()
        {
            InitializeComponent();

            TrainingData();
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

        // чтение данных из файла
        void TrainingData()
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
        void Test(string testString)
        {
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
            chart1.Series[0].Points.Clear();
            chart1.Series["spamProb"].Points.AddXY("спам", probSpam * 100);
            chart1.Series["spamProb"].Points.AddXY("не спам", probHam * 100);
        }



        // опен файл диалог (выбираем текстовик на проверку)
        // ВАЖНО!!!
        // 1) файл должен быть сохранен в кодировке UTF-8 без BOM
        // 2) чтобы была точность писать без знаков препинания
        // Можно конечно настроить преобразование все к lowercase, удалить знаки и тд,
        // но к алгоритму это никак не относится
        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            richTextBox1.Clear();

            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "Рабочий стол:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();

                        LoadText(fileContent);

                     

                    }
                }
            }
        }

        private void LoadText(string fileContent)
        {
            richTextBox1.Text = fileContent.ToLower();

            richTextBox1.SelectionStart = 0;
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectionColor = Color.Black;

            DistinguishWords(richTextBox1.Text);
        }

        // Выделение слов цветом
        private void DistinguishWords(string text)
        {
            string[] words = text.Split(' ');

            var intersectedWordsIEnum = words.Intersect(SpamDict.Keys, StringComparer.OrdinalIgnoreCase);
            string[] intersectedWords = String.Join(" ", intersectedWordsIEnum).Split(' ');


            foreach (string word in intersectedWords)
            {
                HighlightText(word, Color.Red); // выделение слов цветом
            }



            var anotherWordsIEnum = words.Except(intersectedWords);
            string[] anotherWords = String.Join(" ", anotherWordsIEnum).Split(' ');

            foreach (string word in anotherWords)
            {
                listBox1.Items.Add(word);
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            Test(richTextBox1.Text.ToLower());
        }



        private void HighlightText(string word, Color color)
        {
            if (word == string.Empty)
                return;

            int s_start = richTextBox1.SelectionStart, startIndex = 0, index;

            while ((index = richTextBox1.Text.IndexOf(word, startIndex)) != -1)
            {
                richTextBox1.Select(index, word.Length);
                richTextBox1.SelectionColor = color;

                startIndex = index + word.Length;
            }

            richTextBox1.SelectionStart = s_start;
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectionColor = Color.Black;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                textBoxWord.Text = listBox1.SelectedItem.ToString();
            }
            catch
            {

            }
        }

        private void buttonAddToDict_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                if (textBoxWord.Text != string.Empty)
                {
                    sw.Write("\n" + textBoxWord.Text + ":spam");
                    listBox1.Items.Remove(textBoxWord.Text);
                    textBoxWord.Text = "";
                }
            }

            // обновляем все глобальные переменные перед переобучением
            SpamDict = new Dictionary<string, int>(); // словарь спама
            HamDict = new Dictionary<string, int>(); // словарь неспама
            SpamFrequencies = 0; // количество документов в обучающей выборке принадлежащих классу спам;
            HamFrequencies = 0; // количество документов в обучающей выборке принадлежащих классу неспам;

            TrainingData();
        }

    }
}
