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
using SpeechRecognition.Properties;
using System.Xml;
using System.Diagnostics.Contracts;
using System.Xml.Linq;
using System.Runtime.Remoting.Channels;

namespace SpeechRecognition
{
    public class Parameters
    {
        public string name;
        public string value;
    }
    public class ThreadHelper
    {
        public SpeechRecognizedEventArgs e;
        private SynthesizerAbstract synth;
        private string[][] settings;
        private int number_of_commands;
        private ClickerAbstract au3;

        public ThreadHelper (SpeechRecognizedEventArgs eInp,
            SynthesizerAbstract synthInp, string[][] settingsInp, int number_of_commandsInp, ClickerAbstract au3Inp)
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
                    if (e.Result.Text == settings[0][i] )
                    {
                        au3.Click(settings[1][i]);
                        synth.Speak(settings[2][i]);
                        i = number_of_commands;
                    }
                }

            }
        }
    }

    public abstract class RecognitorAbstract
    {
        protected SynthesizerAbstract Synth;
        protected ClickerAbstract Clicker;

        public void SetSynthesizer(SynthesizerAbstract Synth)
        {
            this.Synth = Synth;
        }
        public void SetClicker(ClickerAbstract Clicker)
        {
            this.Clicker = Clicker;
        }
        public abstract void RecognizeStart();
        public abstract void SetCommandSetForRecognition(string[][] settings); //set commands
    }
    public class RecognitorMS:RecognitorAbstract
    {
        private string[][] settings;
        private readonly SpeechRecognitionEngine sre;
        private readonly System.Globalization.CultureInfo ci;
        void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            ThreadHelper ThreadRecognizedHelper = new ThreadHelper(e, this.Synth, settings, settings.GetLength(0), this.Clicker);
            Thread ThreadRecognized = new Thread(new ThreadStart(ThreadRecognizedHelper.DoRecognitionActions));
            ThreadRecognized.Start();
        }
        public RecognitorMS(string[][] settings)
        {
            this.settings = settings;
            ci = new System.Globalization.CultureInfo("ru-ru"); //create recogn engine
            sre = new SpeechRecognitionEngine(ci);

            bool okcode;
            okcode = false;
            while (okcode == false)
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

            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognized); //event register
            sre.LoadGrammar(GrammarForLoad(AddCommandsToChoices(settings)));

        }
        private Grammar GrammarForLoad(Choices commands_cho)
        {
            GrammarBuilder gb = new GrammarBuilder();   //Grammar of choices
            gb.Culture = ci;
            gb.Append(commands_cho);
            Grammar g = new Grammar(gb);

            return g;
        }
        private Choices AddCommandsToChoices(string[][] settings)
        {
            string[] commands = settings[0];
            Choices commands_cho = new Choices(); //add commads to class MS recogn
            commands_cho.Add(commands);

            return commands_cho;
        }
        public override void RecognizeStart()
        {
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }
        public override void SetCommandSetForRecognition(string[][] settings)
        {
            sre.RequestRecognizerUpdate();
            sre.UnloadAllGrammars();
            sre.LoadGrammar(GrammarForLoad(AddCommandsToChoices(settings)));
        }

    }
    public abstract class SynthesizerAbstract
    {
        public abstract void Speak(string promt);
        public abstract void GenerateAufioFiles(string[] toGenerate);
    }
    public class SynthesizerMS : SynthesizerAbstract
    {
        private SpeechSynthesizer synth;

        public SynthesizerMS()
        {
            synth = new SpeechSynthesizer();
            bool okcode;
            okcode = false;
            System.Globalization.CultureInfo ci;
            ci = new System.Globalization.CultureInfo("ru-ru"); //create recogn engine
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
        }
        public override void Speak(string promt)
        {
            synth.SpeakAsync(promt);
        }
        public override void GenerateAufioFiles(string[] toGenerate)
        {

        }
    }
    public class SynthesizerSilero : SynthesizerAbstract
    {
        //private string[] responses;
        public SynthesizerSilero()
        {
            //string[][] settings = Settings_utils.ReadSettings();
            //responses = settings[2];
        }
        public override void Speak(string promt)
        {
            //int filename = Array.IndexOf(responses, promt);
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\SileroTTS" + promt + ".wav");
            if (player == null)
            {
                player.Play();
            }
        }
        public override void GenerateAufioFiles(string[] toGenerate)
        {
            //int i = 0;
            foreach ( string promt in toGenerate )
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.StandardInput.WriteLine("cd " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\SpeechRecognitionEnv\Scripts");
                process.StandardInput.WriteLine("activate.bat");
                //process.StandardInput.WriteLine("activate virtualenvName");
                process.StandardInput.WriteLine("cd " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\SileroTTS");
                process.StandardInput.WriteLine("python SileroTTS.py \"  " + promt + "  \" \"" + promt + "\"");
                process.StandardInput.Flush();
                process.StandardInput.Close();
                //i++;
            }
        }
    }
    public abstract class ClickerAbstract
    {
        public abstract void Click(string promt);
    }
    public class ClickerAutoIt:ClickerAbstract
    {
        private readonly AutoItX3Class au3;
        public ClickerAutoIt()
        {
            au3 = new AutoItX3Class();
            au3.AutoItSetOption("SendKeyDelay", 0);
        }
        public override void Click(string promt)
        {
            au3.Send(promt);
        }
    }

    public class Model
    {
        private SynthesizerAbstract synth;
        private string[][] settings;
        private RecognitorAbstract sre;
        private int number_of_commands;
        private ClickerAbstract clicker;
        private Parameters[] parameters;

        public Model()
        {
            SetSettings(Settings_utils.ReadSettings());
            ConstructMain();
        }

        public void SetParameters(Parameters[] i_parameters)
        {
            parameters = i_parameters;
        }
        public  Parameters[] GetParameters()
        {
            return parameters;
        }

        public void RecognizeStart()
        {
            sre.RecognizeStart();
        }
        private void SetCommandSetForRecognition(string[][] settings)
        {
            sre.SetCommandSetForRecognition(settings);
        }

        public string[][] GetSettings()
        {
            return settings;
        }

        private void SetSettings(string[][] i_settings)
        {
            settings = i_settings;
            string[] commands = i_settings[0];
            number_of_commands = commands.GetLength(0);
        }

        private void SetRecognitor()
        {
            string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            //sre = (RecognitorAbstract)Activator.CreateInstance(AssemblyName, AssemblyName + "." + parameters[0].value)).Unwrap();
            Type type = Type.GetType(AssemblyName + "." + parameters[0].value);
            sre = (RecognitorAbstract)Activator.CreateInstance(type, new object[] { settings });
        }
        private void SetSynthesizer()
        {
            string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            //sre = (RecognitorAbstract)Activator.CreateInstance(AssemblyName, AssemblyName + "." + parameters[0].value)).Unwrap();
            Type type = Type.GetType(AssemblyName + "." + parameters[1].value);
            synth = (SynthesizerAbstract)Activator.CreateInstance(type, new object[] { });
        }
        private void SetClicker()
        {
            string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            //sre = (RecognitorAbstract)Activator.CreateInstance(AssemblyName, AssemblyName + "." + parameters[0].value)).Unwrap();
            Type type = Type.GetType(AssemblyName + "." + parameters[2].value);
            clicker = (ClickerAbstract)Activator.CreateInstance(type, new object[] { });
        }

        public void SetSynthesizerParam(string synthesizer)
        {
            parameters[1].value = synthesizer;
            Settings_utils.WriteParameters(parameters);
            SetSynthesizer();
            synth.GenerateAufioFiles(settings[2]);
            sre.SetSynthesizer(synth);
        }

        private void AddSetting(string command, string button, string response)
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

        private void ConstructMain()
        {
            SetParameters(Settings_utils.ReadParameters());
            SetRecognitor();
            SetSynthesizer();
            SetClicker();
            sre.SetSynthesizer(synth);
            sre.SetClicker(clicker);
        }

        public void SaveSettings(string[][] settings)
        {
            SetSettings(settings);
            SetCommandSetForRecognition(settings);
            Settings_utils.WriteSettings(settings);
            synth.GenerateAufioFiles(settings[2]);
        }
    } 

    class Settings_utils
    {
        static public string[][] ReadSettingsOld()
        {
            string[][] i_settings = new string[3][];
            i_settings = File.ReadAllLines(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\settings.txt")
                   .Select(l => l.Split('\t').Select(i => i).ToArray())
                   .ToArray();
            return i_settings;
        }
        static public string[][] ReadSettings()
        {
            string[][] i_settings = new string[3][];
            List<string> commands = new List<string>();
            List<string> buttons = new List<string>();
            List<string> responses = new List<string>();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\settings.xml");
            XmlElement xRoot = xDoc.DocumentElement;
            if (xRoot != null)
            {
                // обход всех узлов в корневом элементе
                foreach (XmlElement xnode in xRoot)
                {

                    if (xnode.Name == "settings")
                    {
                        foreach (XmlNode setting in xnode.ChildNodes)
                        {
                            foreach (XmlNode childnode in setting.ChildNodes)
                            {
                                if (childnode.Name == "command")
                                {
                                    commands.Add(childnode.InnerText);
                                }
                                if (childnode.Name == "button")
                                {
                                    buttons.Add(childnode.InnerText);
                                }
                                if (childnode.Name == "response")
                                {
                                    responses.Add(childnode.InnerText);
                                }
                            }
                        }
                    }
                }
            }

            i_settings[0] = commands.ToArray();
            i_settings[1] = buttons.ToArray();
            i_settings[2] = responses.ToArray();
            return i_settings;
        }

        static public void WriteSettingsOld(string[][] i_settings)
        {
            File.WriteAllLines(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\settings.txt", i_settings
            //   .ToJagged()
            .Select(line => String.Join("\t", line)));
        }
        static public void WriteSettings(string[][] i_settings)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\settings.xml");
            XmlElement xRoot = xDoc.DocumentElement;
            foreach (XmlNode node in xRoot.ChildNodes)
            {
                if (node.Name == "settings")
                {
                    xRoot.RemoveChild(node);
                }
            }

            XmlElement settingsElem = xDoc.CreateElement("settings");

            for (int i = 0; i < i_settings[0].GetLength(0); i++)
            {
                XmlElement settingElem = xDoc.CreateElement("setting");

                XmlElement commandElem = xDoc.CreateElement("command");
                XmlElement buttonElem = xDoc.CreateElement("button");
                XmlElement responseElem = xDoc.CreateElement("response");

                XmlText commandText = xDoc.CreateTextNode(i_settings[0][i]);
                XmlText buttonText = xDoc.CreateTextNode(i_settings[1][i]);
                XmlText responseText = xDoc.CreateTextNode(i_settings[2][i]);

                commandElem.AppendChild(commandText);
                buttonElem.AppendChild(buttonText);
                responseElem.AppendChild(responseText);

                settingElem.AppendChild(commandElem);
                settingElem.AppendChild(buttonElem);
                settingElem.AppendChild(responseElem);

                settingsElem.AppendChild(settingElem);
            }
            xRoot.AppendChild(settingsElem);

            xDoc.Save(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\settings.xml");
        }
        static public Parameters[] ReadParameters()
        {
            List<Parameters> parameters = new List<Parameters>();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\settings.xml");
            XmlElement xRoot = xDoc.DocumentElement;
            if (xRoot != null)
            {
                // обход всех узлов в корневом элементе
                foreach (XmlElement xnode in xRoot)
                {
                    if (xnode.Name == "parameters")
                    {
                        foreach (XmlElement param in xnode)
                        {
                            Parameters parameter = new Parameters();
                            XmlNode attr = param.Attributes.GetNamedItem("name");
                            if (attr != null)
                            {
                                parameter.name = attr.InnerText;
                            }

                            attr = param.Attributes.GetNamedItem("value");
                            if (attr != null)
                            {
                                parameter.value = attr.InnerText;
                            }
                            parameters.Add(parameter);
                        }
                    }
                }
            }
            return parameters.ToArray();
        }
        static public void WriteParameters(Parameters[] parameters)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\settings.xml");
            XmlElement xRoot = xDoc.DocumentElement;
            foreach (XmlNode node in xRoot.ChildNodes)
            {
                if (node.Name == "parameters")
                {
                    xRoot.RemoveChild(node);
                }
            }

            XmlElement parametersElem = xDoc.CreateElement("parameters");

            for (int i = 0; i < parameters.GetLength(0); i++)
            {
                XmlElement parameterElem = xDoc.CreateElement("parameter");
                XmlAttribute nameAttr = xDoc.CreateAttribute("name");
                XmlText nameText = xDoc.CreateTextNode(parameters[i].name);
                nameAttr.AppendChild(nameText);
                parameterElem.Attributes.Append(nameAttr);
                nameAttr = xDoc.CreateAttribute("value");
                nameText = xDoc.CreateTextNode(parameters[i].value);
                nameAttr.AppendChild(nameText);
                parameterElem.Attributes.Append(nameAttr);

                parametersElem.AppendChild(parameterElem);
            }
            xRoot.AppendChild(parametersElem);

            xDoc.Save(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\settings.xml");

        }

        static public string ConvertEncoding(string promt)
        {
            // Create two different encodings.
            Encoding batch = Encoding.GetEncoding("cp866");
            Encoding dflt = Encoding.Default;

            // Convert the string into a byte array.
            byte[] dfltBytes = dflt.GetBytes(promt);

            // Perform the conversion from one encoding to the other.
            byte[] batchBytes = Encoding.Convert(dflt, batch, dfltBytes);

            // Convert the new byte[] into a char[] and then into a string.
            char[] batchChars = new char[batch.GetCharCount(batchBytes, 0, batchBytes.Length)];
            batch.GetChars(batchBytes, 0, batchBytes.Length, batchChars, 0);
            string batchString = new string(batchChars);

            return batchString;
        }
    }

    public class Controller
    {
        private Model recognitor_process;
        private MainForm MainForm;
        public void ChangeView(DataGridView grid)
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
        public void Start()
        {
            recognitor_process = new Model();
            recognitor_process.RecognizeStart();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm = new MainForm(this);
            Application.Run(MainForm);
        }

        public void Save(DataGridView grid)
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
            recognitor_process.SaveSettings(settings);
        }

        public void ChangeParam(string synthesizer)
        {
            recognitor_process.SetSynthesizerParam(synthesizer);
        }
        public Parameters[] GetParameters()
        {
            return recognitor_process.GetParameters();
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
            Controller controller = new Controller();
            controller.Start();
        }
    }
}
