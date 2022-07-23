using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System.Numerics;
using System.Windows.Forms.DataVisualization.Charting;
using NAudio.Wave;

namespace AudioPreviewApp
{
    public partial class Form1 : Form
    {
        Complex[] samples = new Complex[1000];
        int sampleRateOld = 2000;
        int magnitudeSecond;
        int magnitudeThird;
        double phaseSecond;
        double phaseThird;

        public Form1()
        {
            InitializeComponent();
            PlotAudioWaveform();
        }

        public void PlotTestWaveform()
        {
            var numSamples = samples.Length;

            chart1.Series["Waveform"].Points.Clear();
            chart1.Series["First Harmonic"].Points.Clear();
            chart1.Series["Second Harmonic"].Points.Clear();
            chart1.Series["Third Harmonic"].Points.Clear();

            // Generate fundamental, 2nd & 3rd harmonic waveforms using MathNet libraries
            var first = Generate.Sinusoidal(numSamples, sampleRateOld, 60, 10.0, 0, 0);
            var second = Generate.Sinusoidal(numSamples, sampleRateOld, 120, 5, 0, 0);
            var third = Generate.Sinusoidal(numSamples, sampleRateOld, 180, 2, 0, 0);

            // Add waveforms together to get composite waveforms
            for (int i = 0; i < numSamples; i++)
            {
                samples[i] = new Complex(first[i] + second[i] + third[i], 0);

                if (i > numSamples / 10)
                    continue;

                chart1.Series["Waveform"].Points.AddXY(i, samples[i].Real);
                chart1.Series["First Harmonic"].Points.AddXY(i, first[i]);
                chart1.Series["Second Harmonic"].Points.AddXY(i, second[i]);
                chart1.Series["Third Harmonic"].Points.AddXY(i, third[i]);
            }

            // Compute fourier transform
            Complex[] fourierSeries = new Complex[1000];
            samples.CopyTo(fourierSeries, 0);
            Fourier.Forward(fourierSeries, FourierOptions.NoScaling);

            // New list
            var frequencyList = new List<int>();

            // Add waveforms together to get composite waveforms
            for (int i = 0; i < numSamples / 10; i++)
            {
                var mag = (2.0 / numSamples) * (fourierSeries[i].Magnitude);
                chart2.Series["Waveform"].Points.AddXY(i * 2, mag);

                // Create frequency list
                if (mag > 0.2)
                    frequencyList.Add(i * 2);
            }
        }

        public void PlotAudioWaveform()
        {
            double[] audio_samples;
            var filePath = "C:\\Users\\alexb\\source\\repos\\MusicPlayerApp\\AudioPreviewApp\\Static\\C_synth.wav";
            (audio_samples, sampleRateOld) = ReadWav(filePath);
            samples = new Complex[audio_samples.Length];
            var numSamples = audio_samples.Length;

            chart1.Series["Waveform"].Points.Clear();

            // Add waveforms together to get composite waveforms
            for (int i = 0; i < numSamples; i++)
            {
                samples[i] = new Complex(audio_samples[i], 0);
                if (i % 100 == 0)
                    chart1.Series["Waveform"].Points.AddXY(i, samples[i].Real);
            }

            // Compute fourier transform
            Complex[] fourierSeries = new Complex[audio_samples.Length];
            samples.CopyTo(fourierSeries, 0);
            Fourier.Forward(fourierSeries, FourierOptions.NoScaling);

            // New list
            var frequencyList = new List<int>();

            // Add waveforms together to get composite waveforms
            for (int i = 0; i < numSamples / 40; i++)
            {
                var mag = (2.0 / numSamples) * (fourierSeries[i].Magnitude);
                chart2.Series["Waveform"].Points.AddXY(i * 2, mag);

                // Create frequency list
                if (mag > 0.2)
                    frequencyList.Add(i * 2);
            }

            // Add
            frequencyList.Sort();
            frequencyList.Reverse();
            frequencyList.Take(5).ToList().ForEach(freq => ListBox_GreatestFrequency.Items.Add(freq));
            ListBox_GreatestFrequency.Refresh();
        }
        static (double[] audio, int sampleRate) ReadWav(string filePath)
        {
            using (var afr = new NAudio.Wave.AudioFileReader(filePath))
            {
                int sampleRate = afr.WaveFormat.SampleRate;
                int sampleCount = (int)(afr.Length / afr.WaveFormat.BitsPerSample / 8);
                int channelCount = afr.WaveFormat.Channels;
                var audio = new List<double>(sampleCount);
                var buffer = new float[sampleRate * channelCount];
                int samplesRead = 0;
                while ((samplesRead = afr.Read(buffer, 0, buffer.Length)) > 0)
                    audio.AddRange(buffer.Take(samplesRead).Select(x => (double)x));
                return (audio.Take(10000).ToArray(), sampleRate);
            }
        }



        WaveIn waveIn;
        WaveFileWriter waveWriter;
        private const int sampleRate = 44100;
        public NAudio.Wave.BufferedWaveProvider waveBuffer = null; // clears buffer 
        ISampleProvider samplesProvider;
        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveWriter == null) return;

            // Prepare buffer
            waveBuffer.AddSamples(e.Buffer, 0, e.BytesRecorded);
            waveBuffer.DiscardOnBufferOverflow = true;
            var bytesPerFrame = waveIn.WaveFormat.BitsPerSample / 8
                              * waveIn.WaveFormat.Channels;
            var bufferedFrames = waveBuffer.BufferedBytes / bytesPerFrame;
            var frames = new float[bufferedFrames];
            samplesProvider.Read(frames, 0, bufferedFrames);

            // Plot buffer on graph
            chart1.Series["Waveform"].Points.Clear();
            foreach (var frame in frames.Select((_data, _index) => (_data = _data, _index = _index))
                .Where(frame => frame.Item2 % 100 == 0))
            {
                chart1.Series["Waveform"].Points.AddXY(frame.Item2, frame.Item1);
            }

            waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            waveWriter.Flush();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            waveIn = new WaveIn();
            waveIn.DeviceNumber = 0;
            string outputFilename = @"C:/Temp/demo.wav";
            waveIn.WaveFormat = new WaveFormat(sampleRate, WaveIn.GetCapabilities(waveIn.DeviceNumber).Channels);

            waveBuffer = new NAudio.Wave.BufferedWaveProvider(waveIn.WaveFormat); // initializes buffer
            samplesProvider = waveBuffer.ToSampleProvider();


            waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(waveIn_DataAvailable);
            waveWriter = new WaveFileWriter(outputFilename, waveIn.WaveFormat);
            waveIn.StartRecording();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;
            }
            if (waveWriter != null)
            {
                waveWriter.Dispose();
                waveWriter = null;
            }
        }
    }
}
