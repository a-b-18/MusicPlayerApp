using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PianoMusicPlayer.Models
{
    public class ScaleStep
    {
        public double StepSize;
        public Stream[] Notes;

        public ScaleStep(double step, Stream[] notes)
        {
            StepSize = step;
            Notes = notes;
        }
    }

}
