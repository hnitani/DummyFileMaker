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

namespace DummyFileMaker
{
    public partial class Form1 : Form
    {
        private string filename;
        
        public Form1()
        {
            InitializeComponent();
            radioButton1.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            filename = sfd.FileName;

            textBox1.Text = filename;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(filename == "")
            {
                MessageBox.Show("ファイル名を先に設定してください", "エラー");
                return;
            }
            
            long filesize = (long)numericUpDown1.Value;

            if(radioButton1.Checked)
            {
                filesize *= 1000000000;
            }
            else if(radioButton2.Checked)
            {
                filesize *= 1000000;
            }

            MessageBox.Show($"作成ファイル名 {filename}\r\n作成サイズ {filesize:#,0}バイト", "確認");

            makefile(filename, filesize);

            MessageBox.Show("終了しました", "完了");
        }

        private void makefile(string _filename, long _filesize)
        {
            FileStream fs = new FileStream(_filename, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);

            Random rnd = new Random();

            byte[] temp = { 0x00 };
            for (int i = 0; i < _filesize; i++)
            {
                rnd.NextBytes(temp);
                //fs.Write(temp, i, temp.Length);
                bw.Write(temp);
            }
            bw.Close();
            fs.Close();
        }
    }
}
