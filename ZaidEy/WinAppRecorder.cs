using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZaidEy
{
    public class WinAppRecorder 
    {
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int record(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);

        public void Dispose()
        {
            record("","",0,0);
        }
        ~WinAppRecorder() { }
       public  void startRecording()
        {
            record("open new Type waveaudio Alias recsound", "", 0, 0);
            record("record recsound", "", 0, 0);
            //return true;
        }
       public void stopRecording()
        {
            record("save recsound C:\\zaid\\zaid.flac", "", 0, 0);
            record("close recsound", "", 0, 0);
        }
    }
}
