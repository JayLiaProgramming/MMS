using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmsEngine.NowPlaying.Support
{
    [Flags]
    public enum PlayerCommands
    {
        None = 0,
        BrowseNowPlaying = 1,
        Play = 2,
        Pause = 4,
        PlayPause = 8,
        Stop = 16,
        Seek = 32,
        Repeat = 64,
        Shuffle = 128,
        SkipPrev = 256,
        SkipNext = 512,
    }
}
