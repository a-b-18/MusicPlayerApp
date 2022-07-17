using NAudio.Wave;
using PianoMusicPlayer.Helpers;
using PianoMusicPlayer.Models;
using PianoMusicPlayer.Properties;
using System.Timers;

namespace PianoMusicPlayer
{
    public partial class MusicPlayerForm : Form
    {
        private System.Windows.Forms.Timer timer;
        private DateTime timeStarted;
        private TimeSpan timeElapsed;
        private const int defaultInterval = 500;

        public MusicPlayerForm()
        {
            InitializeComponent();
            StartTimer();
        }

        public readonly Dictionary<string, ScaleStep> Steps = new Dictionary<string, ScaleStep>
        {
            { "A1", new ScaleStep(1, new [] {Resources.A, Resources.B}) },
        };

        private async void Click_PlayAMajor(object sender, EventArgs e)
        {
            await Play_NoteAsync(Resources.A, defaultInterval);
            await Play_NoteAsync(Resources.B, defaultInterval);
            await Play_NoteAsync(Resources.C, defaultInterval);
            await Play_NoteAsync(Resources.D, defaultInterval);
            await Play_NoteAsync(Resources.E, defaultInterval);
            await Play_NoteAsync(Resources.F, defaultInterval);
            await Play_NoteAsync(Resources.G, defaultInterval);
            await Play_NoteAsync(Resources.A, defaultInterval);
        }

        private async void Click_PlayBMajor(object sender, EventArgs e)
        {
            await Play_NoteAsync(Resources.C, defaultInterval);
            await Play_NoteAsync(Resources.D, defaultInterval);
            await Play_NoteAsync(Resources.E, defaultInterval);
            await Play_NoteAsync(Resources.F, defaultInterval);
            await Play_NoteAsync(Resources.G, defaultInterval);
            await Play_NoteAsync(Resources.A, defaultInterval);
            await Play_NoteAsync(Resources.B, defaultInterval);
            await Play_NoteAsync(Resources.C, defaultInterval);
            await Play_NoteAsync(Resources.B, defaultInterval);
            await Play_NoteAsync(Resources.A, defaultInterval);
            await Play_NoteAsync(Resources.G, defaultInterval);
            await Play_NoteAsync(Resources.F, defaultInterval);
            await Play_NoteAsync(Resources.E, defaultInterval);
            await Play_NoteAsync(Resources.D, defaultInterval);
            await Play_NoteAsync(Resources.C, defaultInterval);
        }


        private async void Click_PlayCMajor(object sender, EventArgs e)
        {
            await Play_NoteAsync(Resources.C, defaultInterval);
            await Play_NoteAsync(Resources.D, defaultInterval);
            await Play_NoteAsync(Resources.E, defaultInterval);
            await Play_NoteAsync(Resources.F, defaultInterval);
            await Play_NoteAsync(Resources.G, defaultInterval);
            await Play_NoteAsync(Resources.A, defaultInterval);
            await Play_NoteAsync(Resources.B, defaultInterval);
            await Play_NoteAsync(Resources.C1, defaultInterval);
            await Play_NoteAsync(Resources.B, defaultInterval);
            await Play_NoteAsync(Resources.A, defaultInterval);
            await Play_NoteAsync(Resources.G, defaultInterval);
            await Play_NoteAsync(Resources.F, defaultInterval);
            await Play_NoteAsync(Resources.E, defaultInterval);
            await Play_NoteAsync(Resources.D, defaultInterval);
            await Play_NoteAsync(Resources.C, defaultInterval);
        }
        private async Task Play_NoteAsync(Stream stream, int interval_ms)
        {
            // Play audio sound
            using (var player = new System.Media.SoundPlayer(stream))
            {
                player.Play();

                // Wait until time has elapsed
                await Task.Delay(interval_ms);
                player.Stop();
            }
        }


        private void StartTimer()
        {
            // Set time started to now
            timeStarted = DateTime.Now;

            // Create timer which updates every 10 milliseconds
            timer = new System.Windows.Forms.Timer();
            timer.Interval = (10);
            timer.Tick += new EventHandler(UpdateTimer);

            // Enable timer
            timer.Enabled = true;
        }

        private void UpdateTimer(object sender, EventArgs e)
        {
            // Write time elapsed to label
            timeElapsed = DateTime.Now.Subtract(timeStarted);
            label_TimeElapsed.Text = timeElapsed.ToString("mm") + ":" + timeElapsed.ToString("ss") 
                + "." + timeElapsed.ToString("ff");

            // Render form changes
            Application.DoEvents();
        }
    }
}