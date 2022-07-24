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
        private const int sampleRate = 32000;
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
            var samplesPerBuffer = audioBuffer.BufferedBytes / bufferedBytesPerSample;
            var msPerSample = ((double) audioWaveIn.BufferMilliseconds / samplesPerBuffer);

            // Read buffer into channel
            var timeSamplesCh1 = new float[samplesPerBuffer];
            audioSamples.Read(timeSamplesCh1, 0, samplesPerBuffer);
            //var timeSamplesCh2 = new float[samplesPerBuffer];
            //audioSamples.Read(timeSamplesCh2, 0, samplesPerBuffer);

            // Plot buffer on time graph
            Chart_Time.Series["Channel 1"].Points.Clear();
            //Chart_Time.Series["Channel 2"].Points.Clear();

            // Plot channel 1
            var freqSamplesCh1 = new Complex[samplesPerBuffer];
            var indexPlotStart = -1;
            float lastVal = 0;
            double timeStartCursor = -1;
            double timeEndCursor = -1;
            foreach (var sample in timeSamplesCh1.Select((_data, _index) => (_data, _index)))
            {
                freqSamplesCh1[sample._index] = new Complex(sample._data, 0);

                // Start plotting time domain when data is zero and increasing
                if ((int)(sample._data * 1e2) == 0 && sample._data > lastVal && sample._index < indexPlotStart + samplesPerBuffer / 16)
                {
                    // Begin plotting
                    if (indexPlotStart == -1)
                    {
                        indexPlotStart = sample._index;
                    }

                    // Create time cursors for meauring frequency
                    var currentTime = ((double)(sample._index - indexPlotStart) * msPerSample);
                    if (timeStartCursor == -1)
                    {
                        timeStartCursor = currentTime;
                        Chart_Time.Series["Start Cursor"].Points.Clear();
                        Chart_Time.Series["Start Cursor"].Points.AddXY(currentTime, 0.2);
                        Chart_Time.Series["Start Cursor"].Points.AddXY(currentTime, -0.2);
                    }
                    else if (timeEndCursor == -1 && currentTime > timeStartCursor + 0.5)
                    {
                        timeEndCursor = currentTime;
                        Chart_Time.Series["End Cursor"].Points.Clear();
                        Chart_Time.Series["End Cursor"].Points.AddXY(currentTime, 0.2);
                        Chart_Time.Series["End Cursor"].Points.AddXY(currentTime, -0.2);
                    }
                }

                // Update last val (to determine if increasing) every 20 smaples
                if (sample._index % 20 == 0)
                    lastVal = sample._data;

                // Zoom time graph along x axis to a 16th of sample size
                if (indexPlotStart != -1 && sample._index < indexPlotStart + samplesPerBuffer / 16)
                {
                    Chart_Time.Series["Channel 1"].Points.AddXY((((double)sample._index - indexPlotStart) * msPerSample), sample._data);

                    if (sample._data > 0.01)
                    {
                        ListBox_Statistics.Items.Clear();
                        ListBox_Statistics.Items.Add("Delta Time: \n" + (Math.Abs(((double)timeEndCursor - timeStartCursor))).ToString("0.00") + " ms");
                        ListBox_Statistics.Items.Add("Approx frequency: \n" + (1 / Math.Abs(((double)timeEndCursor - timeStartCursor) / 1000)).ToString("0.00") + " Hz");
                    }
                }
            }

            // Compute fourier transforms
            Fourier.Forward(freqSamplesCh1, FourierOptions.NoScaling);

            // Prepare frequency graph channel 1
            Chart_Frequency.Series["Channel 1"].Points.Clear();
            var freqMagList = new Dictionary<double, double>();

            // Plot frequency graph channel 1
            foreach (var sample in freqSamplesCh1.Select((_value, _index) => (_value, _index)))
            {
                var mag = sample._value.Magnitude;
                var freq = /*secondsPerBuffer*/ 2 * sample._index;
                Chart_Frequency.Series["Channel 1"].Points
                    .AddXY(freq, mag);

                // Create frequency list
                if (sample._value.Magnitude > 40)
                    freqMagList.Add(freq, mag);
            }

            // Display 5 highest-magnitude of frequency of channel 1
            ListBox_GreatestFrequency.Items.Clear();
            var freqList = freqMagList.OrderByDescending(freqMag => freqMag.Value).Take(5).Select(freqMag => freqMag.Key).ToList();
            freqList.Sort();
            freqList.ForEach(freq => ListBox_GreatestFrequency.Items.Add(freq));

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
