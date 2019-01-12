using System;

namespace MediaProcess.Model
{
    public class PlaySettingModel
    {
        public TimeSpan PlayTime { get; set; }
        public int CircleTimes { get; set; }

        public PlaySettingModel(int circleTimes = 1,TimeSpan playTime = default(TimeSpan))
        {
            CircleTimes = circleTimes;
            PlayTime = playTime;
        }
    }
}
