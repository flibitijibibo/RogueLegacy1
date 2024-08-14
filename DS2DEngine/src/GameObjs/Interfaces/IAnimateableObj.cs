using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DS2DEngine
{
    public interface IAnimateableObj
    {
        int AnimationSpeed { get; set; }
        bool IsAnimating { get; }
        bool IsPaused { get; }
        bool IsLooping { get; }
        int TotalFrames { get; }
        int CurrentFrame { get; }
        float AnimationDelay { get; set; }
        string SpriteName { get; set; }

        void ChangeSprite(string spriteName);
        void PlayAnimation(bool loopAnimation = true);
        void PlayAnimation(int startIndex, int endIndex, bool loopAnimation);
        void PauseAnimation();
        void ResumeAnimation();
        void StopAnimation();
        void GoToNextFrame();
        void GoToFrame(int frameIndex);
        int FindLabelIndex(string label);

    }
}
