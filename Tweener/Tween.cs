using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace Tweener
{
    public delegate float Easing(float t, float b, float c, float d);

    public sealed class Tween // Sealed means it can't be inherited from.
    {
        //private static volatile Tweener _singleton; // For multithreading. Volatile to ensure threads don't try to access the singleton's methods before it has been initialized.
        //private static object _syncRoot = new Object(); // Blank object used to lock the singleton during instantiation to ensure multiple threads don't try to initialize it at once.
        private static Stack<TweenObject> _inactiveTweenPool;
        private static LinkedList<TweenObject> _activeTweenPool;
        private static Stack<TweenObject> _tweensToDispose;

        private static int _activeTweenCount;
        private static int _inactiveTweenCount;

        public static CultureInfo ParserCI;
        private static int m_poolSize = 0;

        public static void Initialize(int poolSize)
        {
            _inactiveTweenPool = new Stack<TweenObject>();
            _activeTweenPool = new LinkedList<TweenObject>();
            _tweensToDispose = new Stack<TweenObject>();

            for (int i = 0; i < poolSize; i++)
            {
                _inactiveTweenPool.Push(new TweenObject());
            }

            _inactiveTweenCount = poolSize;
            m_poolSize = poolSize;
            // flibit didn't like this!
            // ParserCI = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            // ParserCI.NumberFormat.CurrencyDecimalSeparator = ".";
            ParserCI = CultureInfo.InvariantCulture;
        }

        public static TweenObject By(object tweenObject, float duration, Easing ease, params string[] properties)
        {
            if (properties.Length % 2 != 0)
                throw new Exception("TweenTo parameters must be submitted as follows - <property name>, <property value>");

            TweenObject checkedOutTween = CheckOutTween();
            checkedOutTween.SetValues(tweenObject, duration, ease, false, properties);
            return checkedOutTween;
        }

        public static TweenObject To(object tweenObject, float duration, Easing ease, params string[] properties)
        {
            if (properties.Length % 2 != 0)
                throw new Exception("TweenTo parameters must be submitted as follows - <property name>, <property value>");

            TweenObject checkedOutTween = CheckOutTween();
            checkedOutTween.SetValues(tweenObject, duration, ease, true, properties);
            return checkedOutTween;
        }

        /// <summary>
        /// A convenience function to add an endhandler to the last tween made
        /// so that you do not have to hold a reference to the tween every time.
        /// </summary>
        public static void AddEndHandlerToLastTween(object methodObject, string functionName, params object[] args)
        {
            TweenObject lastTween = _activeTweenPool.Last.Value;
            lastTween.EndHandler(methodObject, functionName, args);
        }

        /// <summary>
        /// A convenience function to add an endhandler to the last tween made
        /// so that you do not have to hold a reference to the tween every time.
        /// </summary>
        public static void AddEndHandlerToLastTween(Type methodType, string functionName, params object[] args)
        {
            TweenObject lastTween = _activeTweenPool.Last.Value;
            lastTween.EndHandler(methodType, functionName, args);
        }

        public static TweenObject RunFunction(float delay, object methodObject, string functionName, params object[] args)
        {
            TweenObject toReturn = Tween.To(methodObject, delay, Tweener.Ease.Linear.EaseNone);
            Tween.AddEndHandlerToLastTween(methodObject, functionName, args);
            return toReturn;
        }

        public static TweenObject RunFunction(float delay, Type methodType, string functionName, params object[] args)
        {
            TweenObject toReturn = Tween.To(methodType, delay, Tweener.Ease.Linear.EaseNone);
            Tween.AddEndHandlerToLastTween(methodType, functionName, args);
            return toReturn;
        }

        /// <summary>
        /// If an active tween is found in the _activeTweenPool it is updated.
        /// If an inactive tween is found in the _activeTweenPool it is disposed (cleaned up and sent back into the _inactiveTweenPool).
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            int numOfDisposableTweens = 0;
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (TweenObject tween in _activeTweenPool)
            {
                if (tween.Active == true)
                    tween.Update(elapsedTime);
                else
                {
                    numOfDisposableTweens++;
                    _tweensToDispose.Push(tween);
                }
            }

            if (numOfDisposableTweens > 0)
            {
                foreach (TweenObject tween in _tweensToDispose)
                {
                    Dispose(tween);
                }
                _tweensToDispose.Clear();
            }
        }

        public static TweenObject Contains(object obj)
        {
            foreach (TweenObject tweenObj in _activeTweenPool)
            {
                if (obj == tweenObj.TweenedObject)
                    return tweenObj;
            }
            return null;
        }

        public static void StopAllContaining(object obj, bool runEndHandler)
        {
            foreach (TweenObject tweenObj in _activeTweenPool)
            {
                if (obj == tweenObj.TweenedObject)
                    tweenObj.StopTween(runEndHandler);
            }
        }

        public static void StopAllContaining(Type type, bool runEndHandler)
        {
            foreach (TweenObject tweenObj in _activeTweenPool)
            {
                if (tweenObj.TweenedObject != null && tweenObj.TweenedObject.GetType() == type)
                    tweenObj.StopTween(runEndHandler);
            }
        }

        public static void StopAll(bool runEndHandler)
        {
            foreach (TweenObject tween in _activeTweenPool)
            {
                tween.StopTween(runEndHandler);
            }
        }

        public static void PauseAll()
        {
            foreach (TweenObject tween in _activeTweenPool)
            {
                tween.PauseTween();
            }
        }

        public static void ResumeAll()
        {
            foreach (TweenObject tween in _activeTweenPool)
            {
                tween.ResumeTween();
            }
        }

        public static void PauseAllContaining(object obj)
        {
            foreach (TweenObject tween in _activeTweenPool)
            {
                if (tween.TweenedObject == obj)
                    tween.PauseTween();
            }
        }

        public static void ResumeAllContaining(object obj)
        {
            foreach (TweenObject tween in _activeTweenPool)
            {
                if (tween.TweenedObject == obj)
                    tween.ResumeTween();
            }
        }

        public static void PauseAllContaining(Type type)
        {
            foreach (TweenObject tween in _activeTweenPool)
            {
                if (tween.TweenedObject != null && tween.TweenedObject.GetType() == type)
                    tween.PauseTween();
            }
        }

        public static void ResumeAllContaining(Type type)
        {
            foreach (TweenObject tween in _activeTweenPool)
            {
                if (tween.TweenedObject != null && tween.TweenedObject.GetType() == type)
                    tween.ResumeTween();
            }
        }

        private static TweenObject CheckOutTween()
        {
            if (_inactiveTweenCount < 1)
                throw new Exception("TweenManager resource pool ran out of tweens.");

            TweenObject tweenToReturn = _inactiveTweenPool.Pop();
            _activeTweenPool.AddLast(tweenToReturn);

            if (tweenToReturn.CheckedOut == true)
                throw new Exception("Checked out tween already checked out. There is an error in your Tween CheckIn()/CheckOut() code.");

            tweenToReturn.CheckedOut = true;
            _activeTweenCount++;
            _inactiveTweenCount--;
            return tweenToReturn;
        }

        private static void CheckInTween(TweenObject tween)
        {
            if (tween.CheckedOut == true)
            {
                tween.CheckedOut = false;
                _inactiveTweenPool.Push(tween);
                _activeTweenPool.Remove(tween);

                _activeTweenCount--;
                _inactiveTweenCount++;
            }
            else
                throw new Exception("Checking in an already checked in tween. There is an error in your Tween CheckIn()/CheckOut() code.");
        }

        public static void Dispose(TweenObject tween)
        {
            //Tweens endhandler is called here so that it isn't called during the Update method, effectively modifying the collection in Update().
            tween.RunEndHandler();
            tween.Dispose();
            CheckInTween(tween);
        }

        public static void DisposeAll()
        {
            foreach (TweenObject tween in _activeTweenPool)
            {
                tween.Dispose();
            }
            _activeTweenPool.Clear();
            _inactiveTweenPool.Clear();
           // _singleton = null;
        }

        public static int PoolSize
        {
            get { return m_poolSize;}
        }

        public static int ActiveTweens
        {
            get { return _activeTweenCount; }
        }

        public static int AvailableTweens
        {
            get { return _inactiveTweenCount; }
        }

        /*public static Tweener Instance
        {
            get
            {
                if (_singleton == null)
                {
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new Tweener();
                    }
                }
                
                return _singleton;
            }
        }*/

        public static float EaseNone(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
    }

}
