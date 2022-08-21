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
using System.IO;

namespace AudioPreviewApp
{
    public partial class Form1 : Form
    {
        // Form fields
        private const int sampleRate = 44100;
        private const string audioPathOut = @"C:/Temp/demo.wav";
        private WaveIn audioWaveIn;
        private WaveFileWriter audioFileWriter;
        private BufferedWaveProvider audioBuffer;
        private ISampleProvider audioSamples;
        private int readCounter = 0;
        private double prevSelectFreq = 500;
        private float prevMaxTimeMag = 0;
        private double prevMaxFreqMag = 0;
        private PianoKey prevPianoKey;
        private bool writeFile = false;
        private readonly Dictionary<double, string> noteFrequency = new Dictionary<double, string> 
        {
            {16.35, "C0"},
            {17.32, "C#0/Db0"},
            {18.35, "D0"},
            {19.45, "D#0/Eb0"},
            {20.6, "E0"},
            {21.83, "F0"},
            {23.12, "F#0/Gb0"},
            {24.5, "G0"},
            {25.96, "G#0/Ab0"},
            {27.5, "A0"},
            {29.14, "A#0/Bb0"},
            {30.87, "B0"},
            {32.7, "C1"},
            {34.65, "C#1/Db1"},
            {36.71, "D1"},
            {38.89, "D#1/Eb1"},
            {41.2, "E1"},
            {43.65, "F1"},
            {46.25, "F#1/Gb1"},
            {49, "G1"},
            {51.91, "G#1/Ab1"},
            {55, "A1"},
            {58.27, "A#1/Bb1"},
            {61.74, "B1"},
            {65.41, "C2"},
            {69.3, "C#2/Db2"},
            {73.42, "D2"},
            {77.78, "D#2/Eb2"},
            {82.41, "E2"},
            {87.31, "F2"},
            {92.5, "F#2/Gb2"},
            {98, "G2"},
            {103.83, "G#2/Ab2"},
            {110, "A2"},
            {116.54, "A#2/Bb2"},
            {123.47, "B2"},
            {130.81, "C3"},
            {138.59, "C#3/Db3"},
            {146.83, "D3"},
            {155.56, "D#3/Eb3"},
            {164.81, "E3"},
            {174.61, "F3"},
            {185, "F#3/Gb3"},
            {196, "G3"},
            {207.65, "G#3/Ab3"},
            {220, "A3"},
            {233.08, "A#3/Bb3"},
            {246.94, "B3"},
            {261.63, "C4"},
            {277.18, "C#4/Db4"},
            {293.66, "D4"},
            {311.13, "D#4/Eb4"},
            {329.63, "E4"},
            {349.23, "F4"},
            {369.99, "F#4/Gb4"},
            {392, "G4"},
            {415.3, "G#4/Ab4"},
            {440, "A4"},
            {466.16, "A#4/Bb4"},
            {493.88, "B4"},
            {523.25, "C5"},
            {554.37, "C#5/Db5"},
            {587.33, "D5"},
            {622.25, "D#5/Eb5"},
            {659.25, "E5"},
            {698.46, "F5"},
            {739.99, "F#5/Gb5"},
            {783.99, "G5"},
            {830.61, "G#5/Ab5"},
            {880, "A5"},
            {932.33, "A#5/Bb5"},
            {987.77, "B5"},
            {1046.5, "C6"},
            {1108.73, "C#6/Db6"},
            {1174.66, "D6"},
            {1244.51, "D#6/Eb6"},
            {1318.51, "E6"},
            {1396.91, "F6"},
            {1479.98, "F#6/Gb6"},
            {1567.98, "G6"},
            {1661.22, "G#6/Ab6"},
            {1760, "A6"},
            {1864.66, "A#6/Bb6"},
            {1975.53, "B6"},
            {2093, "C7"},
            {2217.46, "C#7/Db7"},
            {2349.32, "D7"},
            {2489.02, "D#7/Eb7"},
            {2637.02, "E7"},
            {2793.83, "F7"},
            {2959.96, "F#7/Gb7"},
            {3135.96, "G7"},
            {3322.44, "G#7/Ab7"},
            {3520, "A7"},
            {3729.31, "A#7/Bb7"},
            {3951.07, "B7"},
            {4186.01, "C8"},
            {4434.92, "C#8/Db8"},
            {4698.63, "D8"},
            {4978.03, "D#8/Eb8"},
            {5274.04, "E8"},
            {5587.65, "F8"},
            {5919.91, "F#8/Gb8"},
            {6271.93, "G8"},
            {6644.88, "G#8/Ab8"},
            {7040, "A8"},
            {7458.62, "A#8/Bb8"},
            {7902.13, "B8"},
        };
        private readonly Dictionary<string, PianoKey> keyString = new Dictionary<string, PianoKey>
        {
            {"C0", PianoKey.C0},
            {"C#0/Db0", PianoKey.Cs0orDb0},
            {"D0", PianoKey.D0},
            {"D#0/Eb0", PianoKey.Ds0orEb0},
            {"E0", PianoKey.E0},
            {"F0", PianoKey.F0},
            {"F#0/Gb0", PianoKey.Fs0orGb0},
            {"G0", PianoKey.G0},
            {"G#0/Ab0", PianoKey.Gs0orAb0},
            {"A0", PianoKey.A0},
            {"A#0/Bb0", PianoKey.As0orBb0},
            {"B0", PianoKey.B0},
            {"C1", PianoKey.C1},
            {"C#1/Db1", PianoKey.Cs1orDb1},
            {"D1", PianoKey.D1},
            {"D#1/Eb1", PianoKey.Ds1orEb1},
            {"E1", PianoKey.E1},
            {"F1", PianoKey.F1},
            {"F#1/Gb1", PianoKey.Fs1orGb1},
            {"G1", PianoKey.G1},
            {"G#1/Ab1", PianoKey.Gs1orAb1},
            {"A1", PianoKey.A1},
            {"A#1/Bb1", PianoKey.As1orBb1},
            {"B1", PianoKey.B1},
            {"C2", PianoKey.C2},
            {"C#2/Db2", PianoKey.Cs2orDb2},
            {"D2", PianoKey.D2},
            {"D#2/Eb2", PianoKey.Ds2orEb2},
            {"E2", PianoKey.E2},
            {"F2", PianoKey.F2},
            {"F#2/Gb2", PianoKey.Fs2orGb2},
            {"G2", PianoKey.G2},
            {"G#2/Ab2", PianoKey.Gs2orAb2},
            {"A2", PianoKey.A2},
            {"A#2/Bb2", PianoKey.As2orBb2},
            {"B2", PianoKey.B2},
            {"C3", PianoKey.C3},
            {"C#3/Db3", PianoKey.Cs3orDb3},
            {"D3", PianoKey.D3},
            {"D#3/Eb3", PianoKey.Ds3orEb3},
            {"E3", PianoKey.E3},
            {"F3", PianoKey.F3},
            {"F#3/Gb3", PianoKey.Fs3orGb3},
            {"G3", PianoKey.G3},
            {"G#3/Ab3", PianoKey.Gs3orAb3},
            {"A3", PianoKey.A3},
            {"A#3/Bb3", PianoKey.As3orBb3},
            {"B3", PianoKey.B3},
            {"C4", PianoKey.C4},
            {"C#4/Db4", PianoKey.Cs4orDb4},
            {"D4", PianoKey.D4},
            {"D#4/Eb4", PianoKey.Ds4orEb4},
            {"E4", PianoKey.E4},
            {"F4", PianoKey.F4},
            {"F#4/Gb4", PianoKey.Fs4orGb4},
            {"G4", PianoKey.G4},
            {"G#4/Ab4", PianoKey.Gs4orAb4},
            {"A4", PianoKey.A4},
            {"A#4/Bb4", PianoKey.As4orBb4},
            {"B4", PianoKey.B4},
            {"C5", PianoKey.C5},
            {"C#5/Db5", PianoKey.Cs5orDb5},
            {"D5", PianoKey.D5},
            {"D#5/Eb5", PianoKey.Ds5orEb5},
            {"E5", PianoKey.E5},
            {"F5", PianoKey.F5},
            {"F#5/Gb5", PianoKey.Fs5orGb5},
            {"G5", PianoKey.G5},
            {"G#5/Ab5", PianoKey.Gs5orAb5},
            {"A5", PianoKey.A5},
            {"A#5/Bb5", PianoKey.As5orBb5},
            {"B5", PianoKey.B5},
            {"C6", PianoKey.C6},
            {"C#6/Db6", PianoKey.Cs6orDb6},
            {"D6", PianoKey.D6},
            {"D#6/Eb6", PianoKey.Ds6orEb6},
            {"E6", PianoKey.E6},
            {"F6", PianoKey.F6},
            {"F#6/Gb6", PianoKey.Fs6orGb6},
            {"G6", PianoKey.G6},
            {"G#6/Ab6", PianoKey.Gs6orAb6},
            {"A6", PianoKey.A6},
            {"A#6/Bb6", PianoKey.As6orBb6},
            {"B6", PianoKey.B6},
            {"C7", PianoKey.C7},
            {"C#7/Db7", PianoKey.Cs7orDb7},
            {"D7", PianoKey.D7},
            {"D#7/Eb7", PianoKey.Ds7orEb7},
            {"E7", PianoKey.E7},
            {"F7", PianoKey.F7},
            {"F#7/Gb7", PianoKey.Fs7orGb7},
            {"G7", PianoKey.G7},
            {"G#7/Ab7", PianoKey.Gs7orAb7},
            {"A7", PianoKey.A7},
            {"A#7/Bb7", PianoKey.As7orBb7},
            {"B7", PianoKey.B7},
            {"C8", PianoKey.C8},
            {"C#8/Db8", PianoKey.Cs8orDb8},
            {"D8", PianoKey.D8},
            {"D#8/Eb8", PianoKey.Ds8orEb8},
            {"E8", PianoKey.E8},
            {"F8", PianoKey.F8},
            {"F#8/Gb8", PianoKey.Fs8orGb8},
            {"G8", PianoKey.G8},
            {"G#8/Ab8", PianoKey.Gs8orAb8},
            {"A8", PianoKey.A8},
            {"A#8/Bb8", PianoKey.As8orBb8},
            {"B8", PianoKey.B8},
        };

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
            var hzPerSample = sampleRate / samplesPerBuffer;

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
            float maxVal = 0;
            double timeStartCursor = -1;
            double timeEndCursor = -1;
            foreach (var sample in timeSamplesCh1.Select((_data, _index) => (_data, _index)))
            {
                // Add sample to complex array for Fourier transform
                freqSamplesCh1[sample._index] = new Complex(sample._data, 0);

                // Add to summ of samples
                if (sample._data > maxVal)
                    maxVal = sample._data;

                // Start plotting time domain when data is zero and increasing
                if ((int)(sample._data * 1e2) == 0 && sample._data > lastVal && sample._index < indexPlotStart + samplesPerBuffer / (16 * prevSelectFreq / 500))
                {
                    // Begin plotting
                    if (indexPlotStart == -1)
                    {
                        indexPlotStart = sample._index;
                    }

                    // Create time cursors for meauring approx. frequency
                    var currentTime = ((double)(sample._index - indexPlotStart) * msPerSample);
                    if (timeStartCursor == -1)
                    {
                        timeStartCursor = currentTime;
                        Chart_Time.Series["Start Cursor"].Points.Clear();
                        Chart_Time.Series["Start Cursor"].Points.AddXY(currentTime, prevMaxTimeMag);
                        Chart_Time.Series["Start Cursor"].Points.AddXY(currentTime, -prevMaxTimeMag);
                    }
                    else if (timeEndCursor == -1 && currentTime > timeStartCursor + 0.5)
                    {
                        timeEndCursor = currentTime;
                        Chart_Time.Series["End Cursor"].Points.Clear();
                        Chart_Time.Series["End Cursor"].Points.AddXY(currentTime, prevMaxTimeMag);
                        Chart_Time.Series["End Cursor"].Points.AddXY(currentTime, -prevMaxTimeMag);
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
                        ListBox_Statistics.Items.Add("Delta Time: " + (Math.Abs(((double)timeEndCursor - timeStartCursor))).ToString("0.00") + " ms");
                        ListBox_Statistics.Items.Add("Approx frequency: " + (1 / Math.Abs(((double)timeEndCursor - timeStartCursor) / 1000)).ToString("0.00") + " Hz");
                    }
                }
            }

            // Store max for calibrating magnitude
            prevMaxTimeMag = maxVal;

            // Compute fourier transforms
            Fourier.Forward(freqSamplesCh1, FourierOptions.NoScaling);

            // Prepare frequency graph channel 1
            Chart_Frequency.Series["Channel 1"].Points.Clear();
            var freqMagList = new Dictionary<double, double>();
            double maxFreqMag = 0;

            // Plot frequency graph channel 1.
            // Only first half of samples need to be plotted because fourier transform is bidirectional.
            foreach (var sample in freqSamplesCh1.Take(samplesPerBuffer / 2).Select((_value, _index) => (_value, _index)))
            {
                // Plot magnitude and frequency.
                var mag = sample._value.Magnitude;
                var freq = 2 * hzPerSample * sample._index;
                Chart_Frequency.Series["Channel 1"].Points.AddXY(freq, mag);

                // Create frequency list
                if (sample._value.Magnitude > prevMaxFreqMag * 0.6)
                    freqMagList.Add(freq, mag);

                // Assign max frequency
                if (sample._value.Magnitude > maxFreqMag)
                    maxFreqMag = sample._value.Magnitude;
            }

            // Draw note on note history
            DrawNoteHistory(freqMagList.Where(freqMag => freqMag.Value > 3).Select(freq => (int)freq.Key).ToList());
            DrawNoteHistoryLabel(noteFrequency.OrderBy(note => Math.Abs(note.Key - (int)(freqMagList.Where(freqMag => freqMag.Value > 3).OrderByDescending(freq => freq.Value).FirstOrDefault().Key))).FirstOrDefault().Value);

            // Store max frequency for magnitude calibration
            prevMaxFreqMag = maxFreqMag;

            // Display 5 highest-magnitude of frequency of channel 1
            ListBox_GreatestFrequency.Items.Clear();
            double avgFreq = -1;
            double prevFreq = -1;
            int groupCount = 1;
            string noteString = "";
            var selectFreqList = freqMagList.OrderByDescending(freqMag => freqMag.Value).Select(freqMag => freqMag.Key).ToList();

            // Set prev freq
            if (selectFreqList.Count != 0)
                prevSelectFreq = selectFreqList.First();

            foreach (var selectFreq in selectFreqList)
            {
                // If freq sample doesn't belong in same group as prev freq (more than 10 steps ahead)
                if (prevFreq == -1 || selectFreq > prevFreq + (2 * hzPerSample))
                {
                    avgFreq = selectFreq;
                    prevFreq = selectFreq;
                    groupCount = 1;
                    if (prevFreq != -1)
                    {
                        // Return closest note to selected frequency
                        noteString = noteFrequency.OrderBy(note => Math.Abs(note.Key - avgFreq)).First().Value;
                        // Add details to list
                        ListBox_GreatestFrequency.Items.Add("Freq: " + (int)avgFreq + ". Closest Note: " + noteString + ".");
                    }
                }
                else
                {
                    // Average the frequency between members of group
                    groupCount++;
                    avgFreq = (selectFreq + prevFreq) / groupCount;
                }
            }

            // Handle final group
            if (prevFreq != -1)
            {
                // Return closest note to selected frequency
                noteString = noteFrequency.OrderBy(note => Math.Abs(note.Key - avgFreq)).First().Value;
                // Add details to list
                ListBox_GreatestFrequency.Items.Add("Freq: " + (int)avgFreq + ". Closest Note: " + noteString + ".");
            }

            // Write to recorded notes if enabled
            if (noteString != "" && writeFile)
            {
                using (var fs = new FileStream("RecordedNotes.txt", FileMode.Append))
                {
                    using (var sr = new StreamWriter(fs))
                    {
                        sr.WriteLine(noteString);
                    }
                }
            }

            // Highlight the note being played
            if (noteString != "" && Panel_Keyboard.Visible)
                SwitchToKey(noteString);

            // Write and reset buffer
            audioFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
            audioFileWriter.Flush();
        }

        const int y_chunk = 10;
        private void DrawNoteHistoryLabel(string notePlayed)
        {
            if (notePlayed != "")
            {
                notePlayed = notePlayed.Substring(0, 2);
                if (notePlayed.Last() != '#')
                    notePlayed = notePlayed.Substring(0, 1);
            }
            var labelWidth = (PictureBox_NoteHistory.Image.Width - 1) / 12;
            var labelList = new List<string>
            {
                "C",
                "C#",
                "D",
                "D#",
                "E",
                "F",
                "F#",
                "G",
                "G#",
                "A",
                "A#",
                "B",
            };

            // Draw label
            foreach (var offset_x in Enumerable.Range(1, 12))
            {
                using (var graphics = Graphics.FromImage(PictureBox_NoteHistory.Image))
                {
                    graphics.DrawString(labelList[offset_x - 1], new Font("Arial", 6), new SolidBrush(Color.CadetBlue), offset_x * labelWidth -(labelWidth/2), PictureBox_NoteHistory.Image.Height - 1 - y_chunk);

                    if (notePlayed == labelList[offset_x - 1])
                    {
                        foreach (var range_x in Enumerable.Range(-10, 20))
                        {
                            foreach (var range_y in Enumerable.Range(-y_chunk, y_chunk))
                            {

                                ((Bitmap)PictureBox_NoteHistory.Image).SetPixel(range_x + offset_x * labelWidth - (labelWidth / 2),
                                    PictureBox_NoteHistory.Image.Height - 1 - y_chunk + range_y, Color.Black);
                            }
                        }
                    }
                }

            }

            // Draw grid
            foreach (var offset_x in Enumerable.Range(0, 13))
            {
                foreach (var pixel_y in Enumerable.Range(1, PictureBox_NoteHistory.Image.Height - 2))
                {
                    ((Bitmap)PictureBox_NoteHistory.Image).SetPixel(offset_x * labelWidth + 1, pixel_y, Color.LightGray);
                }
            }
        }

        private void DrawNoteHistory(List<int> frequencyList)
        {
            var resultImage = new Bitmap(600, 308);

            // Draw current frequency list
            //foreach (var frequency in frequencyList)
            //{
            //    var pixel_x = ((double)frequency / sampleRate) * resultImage.Width;

            //    foreach (var offset_x in Enumerable.Range(-10,20))
            //    {
            //        if (pixel_x + offset_x > 0 && pixel_x + offset_x < resultImage.Width)
            //        {
            //            foreach (var pixel_y in Enumerable.Range(resultImage.Height - 1 - 2 * y_chunk, y_chunk))
            //            {
            //                resultImage.SetPixel((int)pixel_x + offset_x, pixel_y, Color.Black);
            //            }
            //        }
            //    }
            //}

            // Move the pixels up
            if (PictureBox_NoteHistory.Image != null)
            {
                for (var row = +1 + 2*y_chunk; row < resultImage.Height; row++)
                {
                    for (var col = 1; col < resultImage.Width; col++)
                    {
                        if (((Bitmap)PictureBox_NoteHistory.Image).GetPixel(col, row).Name == "ff000000")
                        {
                            resultImage.SetPixel(col, row - y_chunk, Color.Black);
                        }
                    }
                }
            }

            PictureBox_NoteHistory.Image = resultImage;
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

        private void Click_ClearSession(object sender, EventArgs e)
        {

            using (var fs = new FileStream("RecordedNotes.txt", FileMode.Create))
            {
                using (var sr = new StreamWriter(fs))
                {
                    sr.Write("");
                }
            }
        }

        private void Click_WriteSession(object sender, EventArgs e)
        {
            writeFile = true;
            Button_WriteSession.Text = "Pause Session";
            Button_WriteSession.Click -= Click_WriteSession;
            Button_WriteSession.Click += Click_PauseSession;
        }
        private void Click_PauseSession(object sender, EventArgs e)
        {
            writeFile = false;
            Button_WriteSession.Text = "Write Session";
            Button_WriteSession.Click -= Click_PauseSession;
            Button_WriteSession.Click += Click_WriteSession;
        }

        private void SwitchToKey(string noteString) 
        {
            // Set old key invisible
            SetKeyVisibility(prevPianoKey, false);

            // Set new key to visible
            var newKey = keyString[noteString];
            SetKeyVisibility(newKey, true);

            // Update previous key
            prevPianoKey = newKey;
        }

        private void SetKeyVisibility(PianoKey pianoKey, bool isVisible)
        {
            // Set key to visiblity
            switch (pianoKey)
            {
                case PianoKey.C0:
                    PictureBox_C0.Visible = isVisible;
                    break;
                case PianoKey.Cs0orDb0:
                    PictureBox_C0s.Visible = isVisible;
                    break;
                case PianoKey.D0:
                    PictureBox_D0.Visible = isVisible;
                    break;
                case PianoKey.Ds0orEb0:
                    PictureBox_D0s.Visible = isVisible;
                    break;
                case PianoKey.E0:
                    PictureBox_E0.Visible = isVisible;
                    break;
                case PianoKey.F0:
                    PictureBox_F0.Visible = isVisible;
                    break;
                case PianoKey.Fs0orGb0:
                    PictureBox_F0s.Visible = isVisible;
                    break;
                case PianoKey.G0:
                    PictureBox_G0.Visible = isVisible;
                    break;
                case PianoKey.Gs0orAb0:
                    PictureBox_G0s.Visible = isVisible;
                    break;
                case PianoKey.A0:
                    PictureBox_A0.Visible = isVisible;
                    break;
                case PianoKey.As0orBb0:
                    PictureBox_A0s.Visible = isVisible;
                    break;
                case PianoKey.B0:
                    PictureBox_B0.Visible = isVisible;
                    break;
                case PianoKey.C1:
                    PictureBox_C1.Visible = isVisible;
                    break;
                case PianoKey.Cs1orDb1:
                    PictureBox_C1s.Visible = isVisible;
                    break;
                case PianoKey.D1:
                    PictureBox_D1.Visible = isVisible;
                    break;
                case PianoKey.Ds1orEb1:
                    PictureBox_D1s.Visible = isVisible;
                    break;
                case PianoKey.E1:
                    PictureBox_E1.Visible = isVisible;
                    break;
                case PianoKey.F1:
                    PictureBox_F1.Visible = isVisible;
                    break;
                case PianoKey.Fs1orGb1:
                    PictureBox_F1s.Visible = isVisible;
                    break;
                case PianoKey.G1:
                    PictureBox_G1.Visible = isVisible;
                    break;
                case PianoKey.Gs1orAb1:
                    PictureBox_G1s.Visible = isVisible;
                    break;
                case PianoKey.A1:
                    PictureBox_A1.Visible = isVisible;
                    break;
                case PianoKey.As1orBb1:
                    PictureBox_A1s.Visible = isVisible;
                    break;
                case PianoKey.B1:
                    PictureBox_B1.Visible = isVisible;
                    break;
                case PianoKey.C2:
                    PictureBox_C2.Visible = isVisible;
                    break;
                case PianoKey.Cs2orDb2:
                    PictureBox_C2s.Visible = isVisible;
                    break;
                case PianoKey.D2:
                    PictureBox_D2.Visible = isVisible;
                    break;
                case PianoKey.Ds2orEb2:
                    PictureBox_D2s.Visible = isVisible;
                    break;
                case PianoKey.E2:
                    PictureBox_E2.Visible = isVisible;
                    break;
                case PianoKey.F2:
                    PictureBox_F2.Visible = isVisible;
                    break;
                case PianoKey.Fs2orGb2:
                    PictureBox_F2s.Visible = isVisible;
                    break;
                case PianoKey.G2:
                    PictureBox_G2.Visible = isVisible;
                    break;
                case PianoKey.Gs2orAb2:
                    PictureBox_G2s.Visible = isVisible;
                    break;
                case PianoKey.A2:
                    PictureBox_A2.Visible = isVisible;
                    break;
                case PianoKey.As2orBb2:
                    PictureBox_A2s.Visible = isVisible;
                    break;
                case PianoKey.B2:
                    PictureBox_B2.Visible = isVisible;
                    break;
                case PianoKey.C3:
                    PictureBox_C3.Visible = isVisible;
                    break;
                case PianoKey.Cs3orDb3:
                    PictureBox_C3s.Visible = isVisible;
                    break;
                case PianoKey.D3:
                    PictureBox_D3.Visible = isVisible;
                    break;
                case PianoKey.Ds3orEb3:
                    PictureBox_D3s.Visible = isVisible;
                    break;
                case PianoKey.E3:
                    PictureBox_E3.Visible = isVisible;
                    break;
                case PianoKey.F3:
                    PictureBox_F3.Visible = isVisible;
                    break;
                case PianoKey.Fs3orGb3:
                    PictureBox_F3s.Visible = isVisible;
                    break;
                case PianoKey.G3:
                    PictureBox_G3.Visible = isVisible;
                    break;
                case PianoKey.Gs3orAb3:
                    PictureBox_G3s.Visible = isVisible;
                    break;
                case PianoKey.A3:
                    PictureBox_A3.Visible = isVisible;
                    break;
                case PianoKey.As3orBb3:
                    PictureBox_A3s.Visible = isVisible;
                    break;
                case PianoKey.B3:
                    PictureBox_B3.Visible = isVisible;
                    break;
                case PianoKey.C4:
                    PictureBox_C4.Visible = isVisible;
                    break;
                case PianoKey.Cs4orDb4:
                    PictureBox_C4s.Visible = isVisible;
                    break;
                case PianoKey.D4:
                    PictureBox_D4.Visible = isVisible;
                    break;
                case PianoKey.Ds4orEb4:
                    PictureBox_D4s.Visible = isVisible;
                    break;
                case PianoKey.E4:
                    PictureBox_E4.Visible = isVisible;
                    break;
                case PianoKey.F4:
                    PictureBox_F4.Visible = isVisible;
                    break;
                case PianoKey.Fs4orGb4:
                    PictureBox_F4s.Visible = isVisible;
                    break;
                case PianoKey.G4:
                    PictureBox_G4.Visible = isVisible;
                    break;
                case PianoKey.Gs4orAb4:
                    PictureBox_G4s.Visible = isVisible;
                    break;
                case PianoKey.A4:
                    PictureBox_A4.Visible = isVisible;
                    break;
                case PianoKey.As4orBb4:
                    PictureBox_A4s.Visible = isVisible;
                    break;
                case PianoKey.B4:
                    PictureBox_B4.Visible = isVisible;
                    break;
                case PianoKey.C5:
                    PictureBox_C5.Visible = isVisible;
                    break;
                case PianoKey.Cs5orDb5:
                    PictureBox_C5s.Visible = isVisible;
                    break;
                case PianoKey.D5:
                    PictureBox_D5.Visible = isVisible;
                    break;
                case PianoKey.Ds5orEb5:
                    PictureBox_D5s.Visible = isVisible;
                    break;
                case PianoKey.E5:
                    PictureBox_E5.Visible = isVisible;
                    break;
                case PianoKey.F5:
                    PictureBox_F5.Visible = isVisible;
                    break;
                case PianoKey.Fs5orGb5:
                    PictureBox_F5s.Visible = isVisible;
                    break;
                case PianoKey.G5:
                    PictureBox_G5.Visible = isVisible;
                    break;
                case PianoKey.Gs5orAb5:
                    PictureBox_G5s.Visible = isVisible;
                    break;
                case PianoKey.A5:
                    PictureBox_A5.Visible = isVisible;
                    break;
                case PianoKey.As5orBb5:
                    PictureBox_A5s.Visible = isVisible;
                    break;
                case PianoKey.B5:
                    PictureBox_B5.Visible = isVisible;
                    break;
                case PianoKey.C6:
                    PictureBox_C6.Visible = isVisible;
                    break;
                case PianoKey.Cs6orDb6:
                    PictureBox_C6s.Visible = isVisible;
                    break;
                case PianoKey.D6:
                    PictureBox_D6.Visible = isVisible;
                    break;
                case PianoKey.Ds6orEb6:
                    PictureBox_D6s.Visible = isVisible;
                    break;
                case PianoKey.E6:
                    PictureBox_E6.Visible = isVisible;
                    break;
                case PianoKey.F6:
                    PictureBox_F6.Visible = isVisible;
                    break;
                case PianoKey.Fs6orGb6:
                    PictureBox_F6s.Visible = isVisible;
                    break;
                case PianoKey.G6:
                    PictureBox_G6.Visible = isVisible;
                    break;
                case PianoKey.Gs6orAb6:
                    PictureBox_G6s.Visible = isVisible;
                    break;
                case PianoKey.A6:
                    PictureBox_A6.Visible = isVisible;
                    break;
                case PianoKey.As6orBb6:
                    PictureBox_A6s.Visible = isVisible;
                    break;
                case PianoKey.B6:
                    PictureBox_B6.Visible = isVisible;
                    break;
                case PianoKey.C7:
                    PictureBox_C7.Visible = isVisible;
                    break;
                case PianoKey.Cs7orDb7:
                    PictureBox_C7s.Visible = isVisible;
                    break;
                case PianoKey.D7:
                    PictureBox_D7.Visible = isVisible;
                    break;
                case PianoKey.Ds7orEb7:
                    PictureBox_D7s.Visible = isVisible;
                    break;
                case PianoKey.E7:
                    PictureBox_E7.Visible = isVisible;
                    break;
                case PianoKey.F7:
                    PictureBox_F7.Visible = isVisible;
                    break;
                case PianoKey.Fs7orGb7:
                    PictureBox_F7s.Visible = isVisible;
                    break;
                case PianoKey.G7:
                    PictureBox_G7.Visible = isVisible;
                    break;
                case PianoKey.Gs7orAb7:
                    PictureBox_G7s.Visible = isVisible;
                    break;
                case PianoKey.A7:
                    PictureBox_A7.Visible = isVisible;
                    break;
                case PianoKey.As7orBb7:
                    PictureBox_A7s.Visible = isVisible;
                    break;
                case PianoKey.B7:
                    PictureBox_B7.Visible = isVisible;
                    break;
                case PianoKey.C8:
                    PictureBox_C8.Visible = isVisible;
                    break;
                case PianoKey.Cs8orDb8:
                    PictureBox_C8s.Visible = isVisible;
                    break;
                case PianoKey.D8:
                    PictureBox_D8.Visible = isVisible;
                    break;
                case PianoKey.Ds8orEb8:
                    PictureBox_D8s.Visible = isVisible;
                    break;
                case PianoKey.E8:
                    PictureBox_E8.Visible = isVisible;
                    break;
                case PianoKey.F8:
                    PictureBox_F8.Visible = isVisible;
                    break;
                case PianoKey.Fs8orGb8:
                    PictureBox_F8s.Visible = isVisible;
                    break;
                case PianoKey.G8:
                    PictureBox_G8.Visible = isVisible;
                    break;
                case PianoKey.Gs8orAb8:
                    PictureBox_G8s.Visible = isVisible;
                    break;
                case PianoKey.A8:
                    PictureBox_A8.Visible = isVisible;
                    break;
                case PianoKey.As8orBb8:
                    PictureBox_A8s.Visible = isVisible;
                    break;
                case PianoKey.B8:
                    PictureBox_B8.Visible = isVisible;
                    break;
            }
        }

        private enum PianoKey
        {
            C0,
            Cs0orDb0,
            D0,
            Ds0orEb0,
            E0,
            F0,
            Fs0orGb0,
            G0,
            Gs0orAb0,
            A0,
            As0orBb0,
            B0,
            C1,
            Cs1orDb1,
            D1,
            Ds1orEb1,
            E1,
            F1,
            Fs1orGb1,
            G1,
            Gs1orAb1,
            A1,
            As1orBb1,
            B1,
            C2,
            Cs2orDb2,
            D2,
            Ds2orEb2,
            E2,
            F2,
            Fs2orGb2,
            G2,
            Gs2orAb2,
            A2,
            As2orBb2,
            B2,
            C3,
            Cs3orDb3,
            D3,
            Ds3orEb3,
            E3,
            F3,
            Fs3orGb3,
            G3,
            Gs3orAb3,
            A3,
            As3orBb3,
            B3,
            C4,
            Cs4orDb4,
            D4,
            Ds4orEb4,
            E4,
            F4,
            Fs4orGb4,
            G4,
            Gs4orAb4,
            A4,
            As4orBb4,
            B4,
            C5,
            Cs5orDb5,
            D5,
            Ds5orEb5,
            E5,
            F5,
            Fs5orGb5,
            G5,
            Gs5orAb5,
            A5,
            As5orBb5,
            B5,
            C6,
            Cs6orDb6,
            D6,
            Ds6orEb6,
            E6,
            F6,
            Fs6orGb6,
            G6,
            Gs6orAb6,
            A6,
            As6orBb6,
            B6,
            C7,
            Cs7orDb7,
            D7,
            Ds7orEb7,
            E7,
            F7,
            Fs7orGb7,
            G7,
            Gs7orAb7,
            A7,
            As7orBb7,
            B7,
            C8,
            Cs8orDb8,
            D8,
            Ds8orEb8,
            E8,
            F8,
            Fs8orGb8,
            G8,
            Gs8orAb8,
            A8,
            As8orBb8,
            B8,
        }

        private void Click_TimeGraphToggle(object sender, EventArgs e)
        {
            switch (TS_TimeGraphToggle.Text)
            {
                case "Time Graph (Hide)":
                    Panel_TimeGraph.Visible = false;
                    TS_TimeGraphToggle.Text = "Time Graph (Unhide)";
                    break;
                case "Time Graph (Unhide)":
                    Panel_TimeGraph.Visible = true;
                    TS_TimeGraphToggle.Text = "Time Graph (Hide)";
                    break;
            }
        }

        private void Click_FreqGraphToggle(object sender, EventArgs e)
        {
            switch (TS_FreqGraphToggle.Text)
            {
                case "Freq Graph (Hide)":
                    Panel_FreqGraph.Visible = false;
                    TS_FreqGraphToggle.Text = "Freq Graph (Unhide)";
                    break;
                case "Freq Graph (Unhide)":
                    Panel_FreqGraph.Visible = true;
                    TS_FreqGraphToggle.Text = "Freq Graph (Hide)";
                    break;
            }
        }

        private void Click_KeyboardToggle(object sender, EventArgs e)
        {
            switch (TS_KeyboardToggle.Text)
            {
                case "Keyboard (Hide)":
                    Panel_Keyboard.Visible = false;
                    TS_KeyboardToggle.Text = "Keyboard (Unhide)";
                    break;
                case "Keyboard (Unhide)":
                    Panel_Keyboard.Visible = true;
                    TS_KeyboardToggle.Text = "Keyboard (Hide)";
                    break;
            }
        }


    }
}
