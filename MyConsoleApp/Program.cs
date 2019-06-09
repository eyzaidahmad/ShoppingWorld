using Google.Cloud.Speech.V1;
using System;
using Grpc.Auth;
using Google.Apis.Auth.OAuth2;
using System.IO;

namespace MyConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Google Auth
            string credentialsFilePath = @"D:\project\EY\testApp26April2019\khanzaid2490 (3)\key.json";
            GoogleCredential googleCredential;
            using (Stream m = new FileStream(credentialsFilePath, FileMode.Open))
                googleCredential = GoogleCredential.FromStream(m);
            var channel = new Grpc.Core.Channel(SpeechClient.DefaultEndpoint.Host,
                googleCredential.ToChannelCredentials());
            var speech = SpeechClient.Create(channel);
            //Google Auth
            //var speech = SpeechClient.Create();
            var config = new RecognitionConfig
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Flac,
                SampleRateHertz = 16000,
                LanguageCode = LanguageCodes.English.UnitedStates
            };
            var audio = RecognitionAudio.FromStorageUri("gs://cloud-samples-tests/speech/brooklyn.flac");

            var response = speech.Recognize(config, audio);

            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine(alternative.Transcript);
                }
            }
        }
    }
}