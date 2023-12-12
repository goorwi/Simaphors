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
        private int currentTime = 0;
        private int threadSleepingDate = 1000;
        private int threadSleepingEven = 1000;

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
                DateTime time = DateTime.Now;
                dataGridView1.Invoke(new Action(() =>
                {
                    dataGridView1.Rows.Add(time.ToString("HH:mm:ss"));
                }));

                if (dataGridView1.Rows.Count > 0)
                {
                    int sum = time.Hour + time.Minute + time.Second;

                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["Amount"].Value = sum.ToString();
                    if (currentTime == 0) currentTime = dataGridView1.Rows.Count - 1;
                }

                semaphore.Release();
                Console.WriteLine("CalculateAndWrite выполнился");

                Thread.Sleep(threadSleepingDate);
            }
        }

        private void CheckIsEven()
        {
            while (true)
            {
                Console.WriteLine("IsEven ожидает");
                semaphore.WaitOne();
                Console.WriteLine("IsEven выполняется");

                if (dataGridView1.Rows.Count > 0 && dataGridView1.Rows.Count > currentTime)
                {
                    dataGridView1.Rows[currentTime].Cells["IsEven"].Value = (int.Parse(dataGridView1.Rows[currentTime].Cells["Amount"].Value.ToString()) % 2 == 0);
                    currentTime++;
                }

                semaphore.Release();
                Console.WriteLine("IsEven выполнился");

                Thread.Sleep(threadSleepingEven);
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

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            switch (trackBar2.Value) {
                case 0:
                    {
                        threadSleepingEven = 1800;
                        break;
                    }
                case 1:
                    {
                        threadSleepingEven = 1400;
                        break;
                    }
                case 2:
                    {
                        threadSleepingEven = 1000;
                        break;
                    }
                case 3:
                    {
                        threadSleepingEven = 600;
                        break;
                    }
                case 4:
                    {
                        threadSleepingEven = 200;
                        break;
                    }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            switch (trackBar1.Value)
            {
                case 0:
                    {
                        threadSleepingDate = 1800;
                        break;
                    }
                case 1:
                    {
                        threadSleepingDate = 1400;
                        break;
                    }
                case 2:
                    {
                        threadSleepingDate = 1000;
                        break;
                    }
                case 3:
                    {
                        threadSleepingDate = 600;
                        break;
                    }
                case 4:
                    {
                        threadSleepingDate = 200;
                        break;
                    }
            }
        }
    }
}
