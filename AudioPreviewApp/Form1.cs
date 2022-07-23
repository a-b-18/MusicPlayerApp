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
using System.Diagnostics;

namespace AudioPreviewApp
{
    public partial class Form1 : Form
    {
        // Form fields
        private const int sampleRate = 16000;
        private const string audioPathOut = @"C:/Temp/demo.wav";
        private WaveIn audioWaveIn;
        private WaveFileWriter audioFileWriter;
        private BufferedWaveProvider audioBuffer;
        private ISampleProvider audioSamples;
        private int readCounter = 0;

        public Form1()
        {
            InitializeComponent();
            StartRecording();
        }

        private void DataAvailable_AudioWaveIn(object sender, WaveInEventArgs e)
        {
            // Event is called 10 times per second, reduce this down to 2 with counter
            readCounter++;
            if (audioFileWriter == null || readCounter % 5 != 0) return;

            // Prepare buffer
            audioBuffer.AddSamples(e.Buffer, 0, e.BytesRecorded);
            audioBuffer.DiscardOnBufferOverflow = true;
            var bufferedBytesPerSample = audioWaveIn.WaveFormat.BitsPerSample / 8
                              * audioWaveIn.WaveFormat.Channels;
            var bufferedSamplesPerCh = audioBuffer.BufferedBytes / bufferedBytesPerSample;
            var timeSamplesCh1 = new float[bufferedSamplesPerCh];
            audioSamples.Read(timeSamplesCh1, 0, bufferedSamplesPerCh);
            var timeSamplesCh2 = new float[bufferedSamplesPerCh];
            audioSamples.Read(timeSamplesCh2, 0, bufferedSamplesPerCh);

            // Plot buffer on time graph
            Chart_Time.Series["Channel 1"].Points.Clear();
            Chart_Time.Series["Channel 2"].Points.Clear();

            // Plot channel 1
            var freqSamplesCh1 = new Complex[bufferedSamplesPerCh];
            foreach (var frame in timeSamplesCh1.Select((_data, _index) => (_data, _index)))
            {
                freqSamplesCh1[frame._index] = new Complex(frame._data, 0);
                Chart_Time.Series["Channel 1"].Points.AddXY(frame._index, frame._data);
            }

            // Plot channel 2
            var freqSamplesCh2 = new Complex[bufferedSamplesPerCh];
            foreach (var frame in timeSamplesCh2.Select((_data, _index) => (_data, _index)))
            {
                freqSamplesCh2[frame._index] = new Complex(frame._data, 0);
                Chart_Time.Series["Channel 2"].Points.AddXY(frame._index, frame._data);
            }

            // Compute fourier transforms
            Fourier.Forward(freqSamplesCh1, FourierOptions.NoScaling);
            Fourier.Forward(freqSamplesCh2, FourierOptions.NoScaling);

            // Prepare frequency graph channel 1
            Chart_Frequency.Series["Channel 1"].Points.Clear();
            var freqMagList = new Dictionary<int, double>();

            // Plot frequency graph channel 1
            foreach (var freqSampleCh1 in freqSamplesCh1.Select((_value, _index) => (_value, _index)))
            {
                var mag = (2.0 / bufferedSamplesPerCh) * (freqSampleCh1._value.Magnitude);
                Chart_Frequency.Series["Channel 1"].Points.AddXY(freqSampleCh1._index * 2, mag);

                // Create frequency list
                if (mag > 0.002)
                    freqMagList.Add(freqSampleCh1._index * 2, mag);
            }

            // Display 5 highest-magnitude of frequency of channel 1
            ListBox_GreatestFrequency.Items.Clear();
            var freqList = freqMagList.OrderByDescending(freqMag => freqMag.Value).Take(5).Select(freqMag => freqMag.Key).ToList();
            freqList.Sort();
            freqList.ForEach(freq => ListBox_GreatestFrequency.Items.Add(freq));

            // Prepare frequency graph channel 2
            Chart_Frequency.Series["Channel 2"].Points.Clear();

            // Plot frequency graph channel 2
            foreach (var freqSampleCh2 in freqSamplesCh2.Select((_value, _index) => (_value, _index)))
            {
                var mag = (2.0 / bufferedSamplesPerCh) * (freqSampleCh2._value.Magnitude);
                Chart_Frequency.Series["Channel 2"].Points.AddXY(freqSampleCh2._index * 2, mag);
            }

            // Write and reset buffer
            audioFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
            audioFileWriter.Flush();
        }

        private void StartRecording()
        {
            // Initialize audio WaveIn
            audioWaveIn = new WaveIn();
            audioWaveIn.DeviceNumber = 0;
            audioWaveIn.WaveFormat = new WaveFormat(sampleRate, WaveIn.GetCapabilities(audioWaveIn.DeviceNumber).Channels);
            audioWaveIn.DataAvailable += new EventHandler<WaveInEventArgs>(DataAvailable_AudioWaveIn);
            audioWaveIn.StartRecording();

            // Initialize audio WaveWriter for continuous writing from buffer
            audioFileWriter = new WaveFileWriter(audioPathOut, audioWaveIn.WaveFormat);

            // Initialize providers for handling display data
            audioBuffer = new BufferedWaveProvider(audioWaveIn.WaveFormat);
            audioSamples = audioBuffer.ToSampleProvider();
        }

        private void Click_StopRecording(object sender, EventArgs e)
        {
            // Stop recording and dispose wave in and file writer
            if (audioWaveIn != null)
            {
                audioWaveIn.StopRecording();
                audioWaveIn.Dispose();
                audioWaveIn = null;
            }
            if (audioFileWriter != null)
            {
                audioFileWriter.Dispose();
                audioFileWriter = null;
            }
        }
    }
}
