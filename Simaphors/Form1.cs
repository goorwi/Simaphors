using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simaphors
{
    public partial class Form1 : Form
    {
        private Semaphore semaphore;
        private bool isEven;
        private Thread threadCalculateAndWrite;
        private Thread threadCheckIsEven;

        public Form1()
        {
            InitializeComponent();

            semaphore = new Semaphore(1, 1);
            dataGridView1.Columns.Add("Time", "Time");
            dataGridView1.Columns.Add("Amount", "Amount");
            dataGridView1.Columns.Add("IsEven", "IsEven");
            isEven = false;
        }

        private void CalculateAndWrite()
        {
            while (true)
            {
                Console.WriteLine("CalculateAndWrite ожидает");
                semaphore.WaitOne();
                Console.WriteLine("CalculateAndWrite выполняется");
                DateTime currentTime = DateTime.Now;
                dataGridView1.Invoke(new Action(() =>
                {
                    dataGridView1.Rows.Add(currentTime.ToString("HH:mm:ss"));
                }));

                if (dataGridView1.Rows.Count > 0)
                {
                    int sum = currentTime.Hour + currentTime.Minute + currentTime.Second;

                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["Amount"].Value = sum.ToString();
                }

                semaphore.Release();
                Console.WriteLine("CalculateAndWrite выполнился");

                Thread.Sleep(1000);
            }
        }

        private void CheckIsEven()
        {
            while (true)
            {
                Console.WriteLine("IsEven ожидает");
                semaphore.WaitOne();
                Console.WriteLine("IsEven выполняется");

                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["IsEven"].Value = (int.Parse(dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["Amount"].Value.ToString()) % 2 == 0);
                }

                semaphore.Release();
                Console.WriteLine("IsEven выполнился");

                Thread.Sleep(1000);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            threadCalculateAndWrite = new Thread(new ThreadStart(CalculateAndWrite));
            threadCheckIsEven = new Thread(new ThreadStart(CheckIsEven));

            threadCalculateAndWrite.Start();
            threadCheckIsEven.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Завершение потоков при закрытии формы
            threadCalculateAndWrite.Abort();
            threadCheckIsEven.Abort();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (!threadCalculateAndWrite.IsAlive && !threadCheckIsEven.IsAlive)
            {
                threadCalculateAndWrite = new Thread(new ThreadStart(CalculateAndWrite));
                threadCheckIsEven = new Thread(new ThreadStart(CheckIsEven));

                threadCalculateAndWrite.Start();
                threadCheckIsEven.Start();
            }
        }

        private void breakButton_Click(object sender, EventArgs e)
        {
            if (threadCalculateAndWrite.IsAlive && threadCheckIsEven.IsAlive)
            {
                threadCalculateAndWrite.Abort();
                threadCheckIsEven.Abort();
            }
        }
    }
}
