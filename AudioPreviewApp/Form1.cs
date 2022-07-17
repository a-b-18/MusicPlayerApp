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

namespace AudioPreviewApp
{
    public partial class Form1 : Form
    {
        Complex[] samples = new Complex[1000];
        int sampleRate = 2000;
        int magnitudeSecond;
        int magnitudeThird;
        double phaseSecond;
        double phaseThird;

        public Form1()
        {
            InitializeComponent(); 
            PlotWaveform();
        }

        public void PlotWaveform()
        {
            var numSamples = samples.Length;

            chart1.Series["Waveform"].Points.Clear();
            chart1.Series["First Harmonic"].Points.Clear();
            chart1.Series["Second Harmonic"].Points.Clear();
            chart1.Series["Third Harmonic"].Points.Clear();

            // Generate fundamental, 2nd & 3rd harmonic waveforms using MathNet libraries
            var first = Generate.Sinusoidal(numSamples, sampleRate, 60, 10.0, 0, 0);
            var second = Generate.Sinusoidal(numSamples, sampleRate, 120, 5, 0, 0);
            var third = Generate.Sinusoidal(numSamples, sampleRate, 180, 2, 0, 0);

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

        public void PlotAudio()
        {
            var numSamples = samples.Length;

            chart1.Series["Waveform"].Points.Clear();
            chart1.Series["First Harmonic"].Points.Clear();
            chart1.Series["Second Harmonic"].Points.Clear();
            chart1.Series["Third Harmonic"].Points.Clear();

            // Generate fundamental, 2nd & 3rd harmonic waveforms using MathNet libraries
            var first = Generate.Sinusoidal(numSamples, sampleRate, 60, 10.0, 0, 0);
            var second = Generate.Sinusoidal(numSamples, sampleRate, 120, 5, 0, 0);
            var third = Generate.Sinusoidal(numSamples, sampleRate, 180, 2, 0, 0);

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

    }
}
