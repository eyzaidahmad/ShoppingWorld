using Google.Apis.Auth.OAuth2;
using Google.Cloud.Speech.V1;
using Grpc.Auth;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace ZaidEy
{
    public partial class SavingRecordingSameTime : Form
    {

       
        private WaveIn recorder;
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        private BufferedWaveProvider bufferedWaveProvider;
        private SavingWaveProvider savingWaveProvider;
        private WaveOut player;
        SpeechClient speech = null;
        string searchTopics = "";
        string fileBasePath= @"C:\zaid\";
        string fileName = "zaid.flac";
        int flagb1 = 0;
        int flagb2 = 0;
        public SavingRecordingSameTime()
        {
            InitializeComponent();
        }

        private void SavingRecordingSameTime_Load(object sender, EventArgs e)
        {
           
            string credentialsFilePath = @"C:\zaid\Jarvi\ZaidEy\KeyFile\key.json";
            GoogleCredential googleCredential;
            using (Stream m = new FileStream(credentialsFilePath, FileMode.Open))
                googleCredential = GoogleCredential.FromStream(m);
            var channel = new Grpc.Core.Channel(SpeechClient.DefaultEndpoint.Host,
                googleCredential.ToChannelCredentials());
            speech = SpeechClient.Create(channel);
            //  DispatcherTimer dispatcherTimer = newDispatcherTimer();  
            dispatcherTimer.Tick -= new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0,0,10);
            StartVoiceRecognition();
        }

     
        private void RecorderOnDataAvailable(object sender, WaveInEventArgs waveInEventArgs)
        {
            bufferedWaveProvider.AddSamples(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // set up the recorder
            if (flagb1 == 0) {
                flagb1 = 1;
                recorder = new WaveIn();
            recorder.DataAvailable += RecorderOnDataAvailable;
            fileName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".flac";
            // set up our signal chain
            bufferedWaveProvider = new BufferedWaveProvider(recorder.WaveFormat);
            savingWaveProvider = new SavingWaveProvider(bufferedWaveProvider, fileBasePath + fileName);

            // set up playback
            player = new WaveOut();
            player.Init(savingWaveProvider);

            // begin playback & record
            //player.Play();
            //player.Volume = 0;
            recorder.StartRecording();
        }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (flagb2 == 0 )
            {
                flagb2 = 1;
                // stop recording
                recorder.StopRecording();
                // stop playback
                player.Stop();
                // finalise the WAV file
                savingWaveProvider.Dispose();
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
          
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 44100,
                
                LanguageCode = "en",
            }, RecognitionAudio.FromFile(@"C:\zaid\zaid.flac"));
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    searchTopics = alternative.Transcript;
                    //Console.WriteLine(alternative.Transcript);
                }
            }


            //var config = new RecognitionConfig
            //{
            //    Encoding = RecognitionConfig.Types.AudioEncoding.Flac,
            //    SampleRateHertz = 16000,
            //    LanguageCode = LanguageCodes.English.UnitedStates,
            //    EnableWordTimeOffsets = true
            //};
            //var audio = RecognitionAudio.FromStorageUri("gs://cloud-samples-tests/speech/zaid.wav");

            //var response = speech.Recognize(config, audio);

            //foreach (var result in response.Results)
            //{
            //    foreach (var alternative in result.Alternatives)
            //    {

            //        foreach (var item in alternative.Words)
            //        {
            //            Console.WriteLine($"  {item.Word}");

            //        }
            //    }
            //}
        }


        /// <summary>
        /// Streaming audio and print into text 
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>


        private void Button4_Click(object sender, EventArgs e)
        {



        }



        #region  Voice RecogniStion 
        WinAppRecorder winapp = new WinAppRecorder();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
       
        SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();



        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
          
            StopRecording();
            dispatcherTimer.Stop();
            detecttheSpeechText();
            searchTopics = searchTopics + "";
            //
            
        }

        void StartVoiceRecognition()
        {
            Choices command = new Choices();
            command.Add(new string[] { "search", "jarvis", "donkey","can you help me" });
            GrammarBuilder grBuilder = new GrammarBuilder();
            grBuilder.Append(command);
            Grammar gr = new Grammar(grBuilder);
            _recognizer.LoadGrammarAsync(gr);
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.SpeechRecognized -= GetSpeech;
            _recognizer.SpeechRecognized += GetSpeech;
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void GetSpeech(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)
            {
                case "search":
                    {
                        //StartVoiceRecognition();
                        //dispatcherTimer.Stop();
                        break;
                    }
                case "jarvis":
                    {
                       // RecognitionStop();
                        SpeakTest("What can I do for you Sir");
                        StartRecordingSpeech();
                        
                       // RecognitionStop();
                       // dispatcherTimer.Start();
                        break;
                    }
                case "donkey":
                    {
                        SpeakTest("Please do not call me donkey , Sir");
                        break;
                    }
                case "can you help me":
                    {
                        SpeakTest("Yes I can help you , Sir");
                        break;
                    }
                default:
                    {
                        SpeakTest("Sorry Command is not found");
                        break;
                    }
            }
        }


        void StartRecordingSpeech()
        {
            ///RecognitionStop();
            //recorder = new WaveIn();
            //recorder.DataAvailable += RecorderOnDataAvailable;

            //// set up our signal chain
            //bufferedWaveProvider = new BufferedWaveProvider(recorder.WaveFormat);
            //savingWaveProvider = new SavingWaveProvider(bufferedWaveProvider, "C:\\zaid\\zaid.flac");

            //// set up playback
            //player = new WaveOut();
            //player.Init(savingWaveProvider);

            //// begin playback & record
            ////player.Play();
            ////player.Volume = 0;
            //recorder.StartRecording();
            StartBtn.PerformClick();
            flagb2 = 0;
           RecognitionStop();
           dispatcherTimer.Start();

        }
        void StartRecordingSpeech1()
        {
            //using(WinAppRecorder winapp=new WinAppRecorder())
            //{
            RecognitionStop();
            dispatcherTimer.Start();
            winapp.startRecording();
            //}

        }
        void StopRecording1()
        {
           // using (WinAppRecorder winapp = new WinAppRecorder())
          //  {
                winapp.stopRecording();
            //}
        }
        void StopRecording()
        {
            //recorder.StopRecording();
            //// stop playback
            //player.Stop();
            //// finalise the WAV file
            //savingWaveProvider.Dispose();
            StopBtn.PerformClick();
            flagb1 = 0;
        }

        void SpeakTest(string readTheText)
        {
            synthesizer.Volume = 100;  // 0...100
            synthesizer.Rate = -2;     // -10...10
            // Asynchronous
            synthesizer.SpeakAsync(readTheText);
        }


        public string GetInformation(string search)
        {
            string returnStr = "Record is not found";
            try
            {
                string response = "";
                if (search != "")
                {
                    SpeakTest("I am searching for " + search);
                }
                else
                {
                    returnStr = "Google Api limit is over now";

                    SpeakTest("Google Api limit is over now");
                }
                searchingtextLabel.Text = "Jarvis is searching for "+search;

                // search = "C Sharp programming language";
                string url = @"https://www.googleapis.com/customsearch/v1?key=AIzaSyAB00sUNrWSOEjjfCqYSrbGbstk2_6NO8E&cx=017576662512468239146:omuauf_lfve&q=What&is&C&Shaprp";
                url = "https://en.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro=&explaintext=";
                url = "https://en.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro&explaintext&redirects=1&titles=";
                url += "&titles=" + search;
                url += "&rvprop=content";
                url += "&callback=?";
                var client = new WebClient();
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                response = client.DownloadString(url);
                string my_String = Regex.Replace(response, @"[^0-9a-zA-Z]+", " ");
                string[] multiArray = my_String.Split(new string[] { "extract" }, StringSplitOptions.RemoveEmptyEntries);
                if (multiArray.Length == 2)
                {
                    if (multiArray[1].Length > 50)
                        returnStr= multiArray[1].Substring(0, 50);
                }
                else
                {
                    returnStr = "No Record is found";
                }
            }
            catch
            {
                returnStr = "Record is not found";
            }
            searchingtextLabel.Text = returnStr;
            return returnStr;
        }

        void RecognitionStop()
        {
            _recognizer.RecognizeAsyncStop();
        }

        void detecttheSpeechText()
        {
           
            var response = speech.Recognize(new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                SampleRateHertz = 8000,
                LanguageCode = "en",
            }, RecognitionAudio.FromFile(fileBasePath + fileName));
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    searchTopics = alternative.Transcript;
                    Console.WriteLine(alternative.Transcript);
                }
            }

            string info=GetInformation(searchTopics);
            SpeakTest(info);
            StartVoiceRecognition();
        }

        #endregion

        #region New Logic For Wave




        #endregion
        public WaveIn waveSource = null;
        public WaveFileWriter waveFile = null;
        private void StartBtn_Click(object sender, EventArgs e)
        {
            StartBtn.Enabled = false;
            searchingtextLabel.Text = "Jarvis is litening you now";
            StopBtn.Enabled = true;

            waveSource = new WaveIn();
            waveSource.WaveFormat = new WaveFormat(8000, 1);

            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

            waveFile = new WaveFileWriter(@"C:\zaid\zaid.flac", waveSource.WaveFormat);

            waveSource.StartRecording();
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            StopBtn.Enabled = false;
            searchingtextLabel.Text = "Jarvis is going to search";

            waveSource.StopRecording();
            waveSource.Dispose();
            waveFile.Flush();
            waveFile.Dispose();
           
        }
        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveFile != null)
            {
                waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                waveFile.Flush();
            }
        }

        void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }

            if (waveFile != null)
            {
                waveFile.Dispose();
                waveFile = null;
            }

            StartBtn.Enabled = true;
        }
    }



}
