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
    public partial class Main : Form
    {
        private string filename = "";

        private volatile bool flg_stop = false;

        public Main()
        {
            InitializeComponent();
            radioButton1.Checked = true;
            radioButton3.Checked = true;
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

        private async void button2_Click(object sender, EventArgs e)
        {
            if(filename == "")
            {
                MessageBox.Show("ファイル名を先に設定してください", "エラー");
                return;
            }

            long filesize = (long)numericUpDown1.Value;
            long unitsize = (long)numericUpDown2.Value;

            if (radioButton1.Checked)
            {
                filesize *= (long)Math.Pow(1024, 3);
            }
            else if (radioButton2.Checked)
            {
                filesize *= (long)Math.Pow(1024, 2);
            }
            
            if(
            MessageBox.Show($"作成ファイル名 {filename}\r\n作成サイズ {filesize:#,0}バイト\r\n処理サイズ {unitsize:#,0}バイト", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) 
                == DialogResult.Cancel)
            {
                return;
            }

            button2.Enabled = false;

            DateTime starttime = DateTime.Now;

            Progress<int> progress = new Progress<int>(onProgressChanged);
            await Task.Run(() => makefile(filename, filesize, unitsize,progress));

            DateTime endtime = DateTime.Now;

            button2.Enabled = true;

            MessageBox.Show($"終了しました\r\nStart {starttime:yyyy-MM-dd HH:mm:ss}\r\nEnd {endtime:yyyy-MM-dd HH:mm:ss}", "完了");
        }

        private void makefile(string _filename, long _filesize, long _unitsize, IProgress<int> _progress)
        {
            flg_stop = false;
            
            using(FileStream fs = new FileStream(_filename, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    Random rnd = new Random();

                    long unitsize = _unitsize;
                    long num = (int)(_filesize / unitsize);
                    long mod = _filesize % unitsize;

                    byte[] temp = new byte[unitsize];
                    for (long i = 0; i < num; i++)
                    {
                        rnd.NextBytes(temp);
                        bw.Write(temp);

                        _progress.Report((int)(100 * (i + 1) / num));

                        if (flg_stop)
                        {
                            break;
                        }
                    }
            
                    temp = new byte[mod];
                    rnd.NextBytes(temp);
                    bw.Write(temp);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            flg_stop = true;
        }

        private void onProgressChanged(int percent)
        {
            if(percent < progressBar1.Maximum)
            {
                progressBar1.Value = percent + 1; 
                progressBar1.Value = percent;
            }
            else
            {
                progressBar1.Maximum += 1;
                progressBar1.Value = percent + 1;
                progressBar1.Value = percent;
                progressBar1.Maximum -= 1;
            }

            label3.Text = $"{percent}%";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
