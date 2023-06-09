using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpeechRecognition
{
    public partial class ParametersWindow : Form
    {
        private string synthesizer;
        private Controller controller;
        public ParametersWindow(Parameters[] parameters, Controller controller)
        {
            InitializeComponent();
            synthesizer = parameters[1].value;
            if (synthesizer == "SynthesizerMS")
            {
                SynthesizerMSOpt.Checked = true;
            }
            else
            {
                SynthesizerMSOpt.Checked = false;
            }
            if (synthesizer == "SynthesizerSilero")
            {
                SynthesizerSileroOpt.Checked = true;
            }
            else
            {
                SynthesizerSileroOpt.Checked = false;
            }
            this.controller = controller;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            controller.ChangeParam(synthesizer);
            this.Close();
        }

        private void SynthesizerMSOpt_CheckedChanged(object sender, EventArgs e)
        {
            synthesizer = "SynthesizerMS";
        }

        private void SynthesizerSileroOpt_CheckedChanged(object sender, EventArgs e)
        {
            synthesizer = "SynthesizerSilero";
        }
    }
}
