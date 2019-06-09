using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZaidEy
{
    public partial class Form2 : Form
    {
        SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();
        public Form2()
        {
            InitializeComponent();
            button1.Text = "Talk";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
           
            Grammar dictationGrammar = new DictationGrammar();
            _recognizer.LoadGrammarAsync(dictationGrammar);
            try
            {
                button1.Text = "Speak Now";
                //Choices command = new Choices();
                //command.Add(new string[] { "search", "jarvis", "donkey", "can you help me" });
                //GrammarBuilder grBuilder = new GrammarBuilder();
                //grBuilder.Append(command);
                //Grammar gr = new Grammar(grBuilder);
               // _recognizer.LoadGrammarAsync(gr);
                _recognizer.SetInputToDefaultAudioDevice();
                _recognizer.SpeechRecognized += GetSpeech;
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (InvalidOperationException exception)
            {
                button1.Text = String.Format("Could not recognize input from default aduio device. Is a microphone or sound card available?\r\n{0} - {1}.", exception.Source, exception.Message);
            }
            finally
            {
               /// _recognizer.UnloadAllGrammars();
            }
        }

        private void GetSpeech(object sender, SpeechRecognizedEventArgs e)
        {
            textBox1.Text = e.Result.Text;
        }
    }
}
