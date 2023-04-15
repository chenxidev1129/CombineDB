using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace CombineDB
{
    public partial class Main : Form
    {
        private List<TextBox> urlTextBoxList;
        public Main()
        {
            InitializeComponent();

            urlTextBoxList = new List<TextBox>();
            urlTextBoxList.Add(urlTextBox0);
            urlTextBoxList.Add(urlTextBox1);
            urlTextBoxList.Add(urlTextBox2);
            urlTextBoxList.Add(urlTextBox3);
            urlTextBoxList.Add(urlTextBox4);
            urlTextBoxList.Add(urlTextBox5);
            urlTextBoxList.Add(urlTextBox6);
            urlTextBoxList.Add(urlTextBox7);
        }

        private void GetDBFile(int index)
        {
            OpenFileDialog res = new OpenFileDialog();

            res.Filter = "Image Files|*.csv;...";

            //When the user select the file
            if (res.ShowDialog() == DialogResult.OK)
            {
                //Get the file's path
                var filePath = res.FileName;
                //Do something
                urlTextBoxList[index].Text = filePath;
            }
        }

        private Tuple<List<string>, List<List<string>>> GetDBFromFile()
        {
            var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = false
            };

            bool isFullHeaderList = false;
            List<string> headerList = new List<string>();
            List<List<string>> bodyList = new List<List<string>>();

            try
            {
                for (int i = 0; i < 8; i++)
                {
                    if (urlTextBoxList[i].Text != "")
                    {
                        int recordIndex = 0;
                        string path = urlTextBoxList[i].Text;
                        List<List<string>> recordList = new List<List<string>>();

                        using (var streamReader = File.OpenText(path))
                        using (var csvReader = new CsvReader(streamReader, csvConfig))
                        {
                            string value;

                            while (csvReader.Read())
                            {
                                bool existData = false;
                                List<string> recordInfo = new List<string>();

                                for (int j = 0; csvReader.TryGetField<string>(j, out value); j++)
                                {
                                    if (recordIndex == 0)
                                    {
                                        if (!isFullHeaderList)
                                        {
                                            headerList.Add(value);
                                        }
                                    } else
                                    {
                                        if (value != "")
                                        {
                                            existData = true;
                                        }
                                        recordInfo.Add(value);
                                    }
                                }

                                if (isFullHeaderList)
                                {
                                    if (existData)
                                    {
                                        recordList.Add(recordInfo);
                                    }
                                }
                                else
                                {
                                    isFullHeaderList = true;
                                }

                                recordIndex++;
                            }
                        }

                        bodyList.AddRange(recordList);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return new Tuple<List<string>, List<List<string>>>(headerList, bodyList);
        }

        private void WriteDBFile(Tuple<List<string>, List<List<string>>> data)
        {
            List<string> headerList = data.Item1;
            List<List<string>> bodyList = data.Item2;

            DateTime date = fileDateTimePicker.Value;

            string filePath = string.Format("D://APL_CombinedALLFiles_{0}.CSV", date.ToString("ddMMyyyy"));

            try
            {
                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
                {
                    // Write the header row
                    foreach (var header in headerList)
                    {
                        csv.WriteField(header);
                    }
                    csv.NextRecord();

                    // Write each row of the body
                    foreach (var row in bodyList)
                    {
                        foreach (var value in row)
                        {
                            csv.WriteField(value);
                        }
                        csv.NextRecord();
                    }
                }
            } catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void CombineDB()
        {
            Tuple<List<string>, List<List<string>>> data = GetDBFromFile();
            WriteDBFile(data);

            MessageBox.Show("DB is combined successful!");
        }

        private void FileButton_Click(object sender, EventArgs e)
        {
            Button actionButton = (Button)sender;

            for (int i = 0; i < 8; i++)
            {
                if (actionButton.Name.Equals("fileButton" + i))
                {
                    GetDBFile(i);
                }
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            Button actionButton = (Button)sender;

            for (int i = 0; i < 8; i++)
            {
                if (actionButton.Name.Equals("resetButton" + i))
                {
                    urlTextBoxList[i].Clear();
                }
            }
        }

        private void ResetAllButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                urlTextBoxList[i].Clear();
            }
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            CombineDB();
        }
    }
}
