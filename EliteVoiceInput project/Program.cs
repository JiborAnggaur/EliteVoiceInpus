using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Runtime.InteropServices;
using System.Diagnostics;
using AutoItX3Lib;
using System.IO;
using System.Reflection;
using System.Threading;

namespace SpeechRecognition
{
    public class ThreadHelper
    {
        public SpeechRecognizedEventArgs e;
        private SpeechSynthesizer synth;
        private string[][] settings;
        private int number_of_commands;
        private AutoItX3Class au3;

        public ThreadHelper (SpeechRecognizedEventArgs eInp,
            SpeechSynthesizer synthInp, string[][] settingsInp, int number_of_commandsInp, AutoItX3Class au3Inp)
            {
                e = eInp;
                synth = synthInp;
                settings = settingsInp;
                number_of_commands = number_of_commandsInp;
                au3 = au3Inp;
            }

        public void DoRecognitionActions()
        {
            if (e.Result.Confidence > 0.8)
            {
                for (int i = 0; i < number_of_commands; i++)
                {
                    if (e.Result.Text == settings[0][i])
                    {
                        au3.Send(settings[1][i]);
                        synth.SpeakAsync(settings[2][i]);
                        i = number_of_commands;
                    }
                }

            }
        }
    }
        public class recognitor
    {
        private SpeechSynthesizer synth;
        private string[][] settings;
        private SpeechRecognitionEngine sre;
        private int number_of_commands;
        private AutoItX3Class au3;
        private System.Globalization.CultureInfo ci;


        public recognitor (string[][] i_settings)
        {
            settings = new string[3][];
            SetSettings(i_settings);
            constructMain();
        }

        public recognitor()
        {
            SetSettings(settings_utils.read_settings());
            constructMain();
        }

        public void recognize_start()
        {
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
              ThreadHelper ThreadRecognizedHelper = new ThreadHelper(e, synth, settings, number_of_commands, au3);
              Thread ThreadRecognized = new Thread(new ThreadStart(ThreadRecognizedHelper.DoRecognitionActions));
              ThreadRecognized.Start();
        }

        public string[][] GetSettings()
        {
            return settings;
        }

        public void SetSettings(string[][] i_settings)
        {
            settings = i_settings;
            string[] commands = i_settings[0];
            number_of_commands = commands.GetLength(0);
        }

        public void AddSetting(string command, string button, string response)
        {
            string[] commands = new string[number_of_commands+1];
            string[] buttons = new string[number_of_commands+1];
            string[] responses = new string[number_of_commands+1];
            for (int i = 0; i < number_of_commands; i++)
            {
                commands[i] = settings[0][i];
                buttons[i] = settings[1][i];
                responses[i] = settings[2][i];
            }
            commands[number_of_commands] = command;
            buttons[number_of_commands] = button;
            responses[number_of_commands] = response;
            string[][] settings_new = new string[3][];
            settings_new[0] = commands;
            settings_new[1] = buttons;
            settings_new[2] = responses;

            SetSettings(settings_new);
        }

        public void ChangeGrammar()
        {
            sre.RequestRecognizerUpdate();
            sre.UnloadAllGrammars(); 
            sre.LoadGrammar(GrammarForLoad(AddCommandsToChoices()));
        }

        private Choices AddCommandsToChoices()
        {
            string[] commands = settings[0];
            Choices commands_cho = new Choices(); //add commads to class MS recogn
            commands_cho.Add(commands);

            return commands_cho;
        }

        private Grammar GrammarForLoad(Choices commands_cho)
        {
            GrammarBuilder gb = new GrammarBuilder();   //Grammar of choices
            gb.Culture = ci;
            gb.Append(commands_cho);
            Grammar g = new Grammar(gb);

            return g;
        }

        private void constructMain()
        {
            synth = new SpeechSynthesizer();    //create new voice synth

            ci = new System.Globalization.CultureInfo("ru-ru"); //create recogn engine
            sre = new SpeechRecognitionEngine(ci);
            bool okcode;
            okcode = false;
            while ( okcode == false )
            { 
                try
                {
                    sre.SetInputToDefaultAudioDevice();
                    okcode = true;
                }
                catch
                {
                    var result = MessageBox.Show("Input device is not connected. Please connect it and press 'Yes' or press 'No' for exit", 
                                   "Error",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question);

                    // If the no button was pressed ...
                    if (result == DialogResult.No)
                    {
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                    }
                }
            }

            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized); //event register
            sre.LoadGrammar(GrammarForLoad(AddCommandsToChoices()));

            foreach (InstalledVoice voice in synth.GetInstalledVoices(ci))
            {
                string VoiceName = voice.VoiceInfo.Name;
                synth.SelectVoice(VoiceName);
            }
            // Configure the audio output. 
            okcode = false;
            while (okcode == false)
            {
                try
                {
                    synth.SetOutputToDefaultAudioDevice();
                    okcode = true;
                }
                catch
                {
                    var result = MessageBox.Show("Output device is not connected. Please connect it and press 'Yes' or press 'No' for exit",
                                   "Error",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question);

                    // If the no button was pressed ...
                    if (result == DialogResult.No)
                    {
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                    }
                }
            }
            // Set the volume of the SpeechSynthesizer's ouput.
            synth.Volume = 100;

            au3 = new AutoItX3Class();
            au3.AutoItSetOption("SendKeyDelay", 0);
        }
    } 

    class settings_utils
    {
        static public string[][] read_settings()
        {
            string[][] i_settings = new string[3][];
            i_settings = File.ReadAllLines(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\settings.txt")
                   .Select(l => l.Split('\t').Select(i => i).ToArray())
                   .ToArray();
            return i_settings;
        }

        static public void write_settings(string[][] i_settings)
        {
            File.WriteAllLines(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\settings.txt", i_settings
            //   .ToJagged()
            .Select(line => String.Join("\t", line)));
        }
    }

    class controller
    {
        private static recognitor recognitor_process;
        private static Form1 MainForm;
        static public void ChangeView(DataGridView grid)
        {
            grid.Rows.Clear();
            string[][] settings = recognitor_process.GetSettings();
            int number_of_commands = settings[0].GetLength(0);
            for (int i = 0; i < number_of_commands; i++)
            {
                string[] row = new string[] { settings[0][i], settings[1][i], settings[2][i] };
                grid.Rows.Add(row);
            }
        }
        public static void start()
        {
            recognitor_process = new recognitor();
            recognitor_process.recognize_start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm = new Form1();
            Application.Run(MainForm);
        }

        public static void save(DataGridView grid)
        {
            string[][] settings = new string[3][];
            settings[0] = new string[grid.Rows.Count-1];
            settings[1] = new string[grid.Rows.Count-1];
            settings[2] = new string[grid.Rows.Count-1];
            foreach (DataGridViewRow i in grid.Rows)
            {
                if (i.IsNewRow) continue;
                foreach (DataGridViewCell j in i.Cells)
                {
                    try
                    {
                        if ((j.ColumnIndex == 0 && j.Value == null)||((j.ColumnIndex == 1 && j.Value == null)))
                        {
                            MessageBox.Show("Command or Button can't have empty value");
                            return;
                        }
                        settings[j.ColumnIndex][j.RowIndex] = j.Value.ToString();
                    }
                    catch
                    {
                    }
                }
            }
            recognitor_process.SetSettings(settings);
            recognitor_process.ChangeGrammar();
            settings_utils.write_settings(settings);
        }
    }

    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            controller.start();
        }
    }
}
