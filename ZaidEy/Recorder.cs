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
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZaidEy
{
    public partial class Recorder : Form
    {


       
        /// </summary>

        WaveIn sourceStream;
        WaveFileWriter waveWriter;
         String FilePath= @"D:\project\zaid28April2019";
         String FileName="";
        readonly int InputDeviceIndex;
        static string speechString = "";

        public Recorder()
        { InitializeComponent(); }
            public Recorder(int inputDeviceIndex, String filePath, String fileName)
        {
             this.InputDeviceIndex = inputDeviceIndex;
            this.FileName = fileName;
            this.FilePath = filePath;
        }

        private void Recorder_Load(object sender, EventArgs e)
        {
            //var input = "Love";//English
            //var languagePair = "de";//Deutsch
            //string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
            //WebClient webClient = new WebClient();
            //webClient.Encoding = System.Text.Encoding.Default;
            //string result = webClient.DownloadString(url);
            //result = result.Substring(result.IndexOf("TRANSLATED_TEXT"));
            //result = result.Substring(result.IndexOf("'") + 1);
            //result = result.Substring(0, result.IndexOf("'"));
            /// return result;
        }


        public void StartRecording(object sender, EventArgs e)
        {
           
        }

        public void SourceStreamDataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveWriter == null) return;
            waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            waveWriter.Flush();
        }

        private void RecordEnd(object sender, EventArgs e)
        {
           
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var Test=StreamingMicRecognizeAsync(10);
            textBox1.Text= speechString.ToString();
            //sourceStream = new WaveIn
            //{
            //    DeviceNumber = this.InputDeviceIndex,
            //    WaveFormat =
            //       new WaveFormat(44100, WaveIn.GetCapabilities(this.InputDeviceIndex).Channels)
            //};

            //sourceStream.DataAvailable += this.SourceStreamDataAvailable;

            //if (!Directory.Exists(FilePath))
            //{
            //    Directory.CreateDirectory(FilePath);
            //}

            //waveWriter = new WaveFileWriter(FilePath + "zaid", sourceStream.WaveFormat);
            //sourceStream.StartRecording();

        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            if (sourceStream != null)
            {
                sourceStream.StopRecording();
                sourceStream.Dispose();
                sourceStream = null;
            }
            if (this.waveWriter == null)
            {
                return;
            }
            this.waveWriter.Dispose();
            this.waveWriter = null;
            recordEndButton.Enabled = false;
            Application.Exit();
            Environment.Exit(0);
        }


        static async Task<int> StreamingMicRecognizeAsync(int seconds)
        {
            try
            {
                if (NAudio.Wave.WaveIn.DeviceCount < 1)
                {
                    Console.WriteLine("No microphone!");
                    return 0;
                }
                // var speech = SpeechClient.Create();
                string credentialsFilePath = @"D:\project\EY\testApp26April2019\khanzaid2490 (3)\key.json";
                GoogleCredential googleCredential;
                using (Stream m = new FileStream(credentialsFilePath, FileMode.Open))
                    googleCredential = GoogleCredential.FromStream(m);
                var channel = new Grpc.Core.Channel(SpeechClient.DefaultEndpoint.Host,
                    googleCredential.ToChannelCredentials());
                var speech = SpeechClient.Create(channel);
                //
                var streamingCall = speech.StreamingRecognize();
                // Write the initial request with the config.
                await streamingCall.WriteAsync(
                    new StreamingRecognizeRequest()
                    {
                        StreamingConfig = new StreamingRecognitionConfig()
                        {
                            Config = new RecognitionConfig()
                            {
                                Encoding =
                                RecognitionConfig.Types.AudioEncoding.Linear16,
                                SampleRateHertz = 16000,
                                LanguageCode = "en",
                            },
                            InterimResults = true,
                        }
                    });
                // Print responses as they arrive.
                Task printResponses = Task.Run(async () =>
                {
                    while (await streamingCall.ResponseStream.MoveNext(
                        default(CancellationToken)))
                    {
                        foreach (var result in streamingCall.ResponseStream
                            .Current.Results)
                        {
                            foreach (var alternative in result.Alternatives)
                            {
                                speechString = speechString + " " + alternative.Transcript;
                            }
                        }
                    }
                });
                // Read from the microphone and stream to API.
                object writeLock = new object();
                bool writeMore = true;
                var waveIn = new NAudio.Wave.WaveInEvent();
                waveIn.DeviceNumber = 0;
                waveIn.WaveFormat = new NAudio.Wave.WaveFormat(16000, 1);
                waveIn.DataAvailable +=
                    (object sender, NAudio.Wave.WaveInEventArgs args) =>
                    {
                        lock (writeLock)
                        {
                            if (!writeMore) return;
                            streamingCall.WriteAsync(
                                new StreamingRecognizeRequest()
                                {
                                    AudioContent = Google.Protobuf.ByteString
                                        .CopyFrom(args.Buffer, 0, args.BytesRecorded)
                                }).Wait();
                        }
                    };
                waveIn.StartRecording();
                Console.WriteLine("Speak now.");
                //button1
                await Task.Delay(TimeSpan.FromSeconds(seconds));
                // Stop recording and shut down.
                waveIn.StopRecording();
                lock (writeLock) writeMore = false;
                await streamingCall.WriteCompleteAsync();
                await printResponses;
            }catch(Exception )
            {
                return 0;
            }
            return 1;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
           
        }
    }
}