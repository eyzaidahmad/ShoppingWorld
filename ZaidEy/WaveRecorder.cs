using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZaidEy
{
    public  class WaveRecorder
    {
        /*
         *  Zaid Ahmad Khan 
         *  
         */

        public WaveIn waveSource = null;
        public WaveFileWriter waveFile = null;
        public WaveRecorder()
        {
          
        }
        public void startRecording()
        {
            waveSource = new WaveIn();
            waveSource.WaveFormat = new WaveFormat(8000, 1);

            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

            waveFile = new WaveFileWriter(@"C:\zaid\zaid.flac", waveSource.WaveFormat);

            waveSource.StartRecording();
        }
        public void stopRecording()
        {
            waveSource.StopRecording();
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

        }
    }
}
