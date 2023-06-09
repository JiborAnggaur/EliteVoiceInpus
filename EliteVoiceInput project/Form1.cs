using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SpeechRecognition
{
    public partial class MainForm : Form
    {
        private Controller controller;
        public MainForm(Controller controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            
        }

        public void Form1_Load(object sender, EventArgs e)
        {
            controller.ChangeView(dataGridView1);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            controller.Save(dataGridView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ParametersWindow paramWindow = new ParametersWindow(controller.GetParameters(), controller);
            paramWindow.ShowDialog();
        }
    }
}
