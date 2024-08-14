using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tweener;
using Tweener.Ease;

namespace DS2DEngine
{
    public class PlayAnimationLogicAction : LogicAction
    {
        private int m_startFrame;
        private int m_endFrame;
        private bool m_loop;

        private string m_startLabel = "";
        private string m_endLabel = "";
        private bool m_useLabel = false;

        //private TweenObject m_tweenDelay;
        //private BlankObj m_blankObj;
        private IAnimateableObj m_animateableObj;
        
        public PlayAnimationLogicAction(int startingFrame, int endFrame, bool loopAnimation = false)
        {
            m_startFrame = startingFrame;
            m_endFrame = endFrame;
            m_loop = loopAnimation;
            //m_blankObj = new BlankObj(1, 1);
        }

        public PlayAnimationLogicAction(string startLabel, string endLabel, bool loopAnimation = false)
        {
            m_startLabel = startLabel;
            m_endLabel = endLabel;
            m_loop = loopAnimation;
            m_useLabel = true;
            //m_blankObj = new BlankObj(1, 1);
        }

        public PlayAnimationLogicAction(bool loopAnimation = true)
        {
            m_loop = loopAnimation;
            m_startFrame = 1;
            m_endFrame = 1000;
            //m_blankObj = new BlankObj(1, 1);
        }

        public override void Execute()
        {
            if (ParentLogicSet != null && ParentLogicSet.IsActive == true)
            {
                //IAnimateableObj animateableObj = this.ParentLogicSet.ParentGameObj as IAnimateableObj;
                m_animateableObj = this.ParentLogicSet.ParentGameObj as IAnimateableObj;

                if (m_animateableObj != null)
                {
                    if (m_useLabel == true)
                    {
                        m_startFrame = m_animateableObj.FindLabelIndex(m_startLabel);
                        m_endFrame = m_animateableObj.FindLabelIndex(m_endLabel);
                        if (m_startFrame == -1)
                            throw new Exception("Could not find starting label " + m_startLabel);
                        else if (m_endFrame == -1)
                            throw new Exception("Could not find ending label " + m_endLabel);
                    }

                    m_animateableObj.PlayAnimation(m_startFrame, m_endFrame, m_loop);
                    //if (SequenceType == Types.Sequence.Serial && animateableObj.IsLooping == false)
                    //{
                    //    m_startFrame = (m_startFrame - 1) * animateableObj.AnimationSpeed + 1;
                    //    m_endFrame = m_endFrame * animateableObj.AnimationSpeed;
                    //    if (m_endFrame > animateableObj.TotalFrames)
                    //        m_endFrame = animateableObj.TotalFrames;

                    //    m_tweenDelay = Tween.To(m_blankObj, m_endFrame - m_startFrame, Linear.EaseNone, "X", "1");
                    //    m_tweenDelay.UseTicks = true;
                    //}
                }

                base.Execute();
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            this.ExecuteNext();
            base.Update(gameTime);
        }

        public override void ExecuteNext()
        {
            if (m_loop == true || m_loop == true || (m_animateableObj != null && SequenceType == Types.Sequence.Serial && m_animateableObj.IsLooping == false && m_animateableObj.IsAnimating == false))
            {
                base.ExecuteNext();
            }
        }


        //public override void ExecuteNext()
        //{
        //    if (m_tweenDelay != null)
        //    {
        //        if (NextLogicAction != null)
        //            m_tweenDelay.EndHandler(NextLogicAction, "Execute");
        //        else
        //            m_tweenDelay.EndHandler(ParentLogicSet, "ExecuteComplete");
        //    }
        //    else
        //        base.ExecuteNext();
        //}

        public override void Stop()
        {
            // Okay. Big problem with delay tweens.  Because logic actions are never disposed in-level (due to garbage collection concerns) the reference to the delay tween in this logic action will exist until it is 
            // disposed, EVEN if the tween completes.  If the tween completes, it goes back into the pool, and then something else will call it, but this logic action's delay tween reference will still be pointing to it.
            // This becomes a HUGE problem if this logic action's Stop() method is called, because it will then stop the tween that this tween reference is pointing to. 
            // The solution is to comment out the code below. This means that this tween reference will always call it's endhandler, which in this case is either Execute() or ExecuteComplete().  This turns out
            // to not be a problem, because when a logic set is called to stop, its IsActive flag is set to false, and all Execute() methods in logic actions have to have their parent logic set's IsActive flag to true
            // in order to run.  Therefore, when the tween calls Execute() or ExecuteComplete() nothing will happen.
            // - This bug kept you confused for almost 5 hours.  DO NOT FORGET IT. That is what this long explanation is for.
            // TL;DR - DO NOT UNCOMMENT THE CODE BELOW OR LOGIC SETS BREAK.

            //if (m_tweenDelay != null)
            //    m_tweenDelay.StopTween(false);

            IAnimateableObj obj = this.ParentLogicSet.ParentGameObj as IAnimateableObj;
            if (obj != null)
                obj.StopAnimation();
            base.Stop();
        }

        public override object Clone()
        {
            if (m_useLabel == true)
                return new PlayAnimationLogicAction(m_startLabel, m_endLabel, m_loop);
            else
                return new PlayAnimationLogicAction(m_startFrame, m_endFrame, m_loop);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // See above for explanation.
                //if (m_tweenDelay != null)
                //    m_tweenDelay.StopTween(false);

                //m_tweenDelay = null;
                //m_blankObj.Dispose();
                //m_blankObj = null;
                m_animateableObj = null;
                base.Dispose();
            }
        }
    }
}
