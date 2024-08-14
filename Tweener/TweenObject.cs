
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace Tweener
{
    public class TweenObject
    {
        private string[] _propertiesList;
        private object _tweenObject;
        private PropertyInfo _propertyInfo; // Create an property info instance so that a new one isn't constantly created.
        private object _methodObject;
        private object[] _methodArgs;
        private MethodInfo _methodInfo;
        private float _duration = 0;
        private float _elapsedTime = 0;
        private float[] _initialValues;
        private bool _tweenTo;
        private Easing _ease;
        private bool _runEndHandler;
        private bool _paused;
        private bool m_isType; // Flag to determine if the tweenedObject passed in is a Type or not. (For static classes)

        private bool _active;
        public bool CheckedOut;

        public bool UseTicks = false;
        private float m_delayTime = 0;
        private float m_elapsedDelayTime = 0;

        public TweenObject()
        {
            _runEndHandler = false;
            _paused = false;
        }

        public void SetValues(object tweenObject, float duration, Easing ease, bool tweenTo, string[] properties)
        {
            if (tweenObject is Type)
                m_isType = true;
            else
                m_isType = false;

            _tweenObject = tweenObject;
            _propertiesList = properties;
            _duration = duration;
            _ease = ease;
            _active = true;
            _tweenTo = tweenTo;
            _elapsedTime = 0;

            _initialValues = new float[_propertiesList.Length / 2];

            int counter = 0; // Counter is used to go up in the array by 1, since the for loop goes up by 2.
            for (int i = 0; i < _propertiesList.Length; i = i + 2)
            {
                if (properties[i] != "delay")
                {
                    if (m_isType == true)
                        _propertyInfo = (_tweenObject as Type).GetProperty(_propertiesList[i]);
                    else
                        _propertyInfo = _tweenObject.GetType().GetProperty(_propertiesList[i]);

                    if (_propertyInfo == null)
                        throw new Exception("Property " + _propertiesList[i] + " not found on object " + _tweenObject.GetType().ToString());
                    _initialValues[counter] = Convert.ToSingle(_propertyInfo.GetValue(_tweenObject, null));
                    //Console.WriteLine("test " + counter + " " + _initialValues[counter]);
                    counter++;
                }
                else
                {
                    m_delayTime = float.Parse(_propertiesList[i + 1], NumberStyles.Any, Tween.ParserCI);
                    m_elapsedDelayTime = 0;
                }
            }
        }

        //public void Update(GameTime gameTime)
        public void Update(float elapsedTime)
        {
            //float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_paused == false)
            {
                if (m_elapsedDelayTime > m_delayTime) // Adds a special "delay" keycode so that tweens can be delayed before running.
                {
                    if (_elapsedTime < _duration)  // Means tween is still running.
                    {
                        int counter = 0;
                        for (int i = 0; i < _propertiesList.Length - 1; i = i + 2)
                        {
                            if (_propertiesList[i] != "delay")
                            {
                                if (m_isType == true)
                                    _propertyInfo = (_tweenObject as Type).GetProperty(_propertiesList[i]);
                                else
                                    _propertyInfo = _tweenObject.GetType().GetProperty(_propertiesList[i]);

                                float value;
                                if (_tweenTo == true)
                                    value = _ease(_elapsedTime, _initialValues[counter], float.Parse(_propertiesList[i + 1], NumberStyles.Any, Tween.ParserCI) - _initialValues[counter], _duration);
                                else
                                    value = _ease(_elapsedTime, _initialValues[counter], float.Parse(_propertiesList[i + 1], NumberStyles.Any, Tween.ParserCI), _duration);
                                if (_propertyInfo.PropertyType == typeof(int))
                                    _propertyInfo.SetValue(_tweenObject, (int)value, null);
                                else
                                    _propertyInfo.SetValue(_tweenObject, value, null);
                                counter++;
                            }
                            //Console.WriteLine(_elapsedTime);
                        }
                        if (UseTicks == true)
                            _elapsedTime++;
                        else
                            _elapsedTime += elapsedTime;
                    }
                    else
                    {
                        this.Complete();
                    }
                }
                else
                    m_elapsedDelayTime += elapsedTime;
            }
        }

        /// <summary>
        /// Sets the endhandler for the tween.
        /// The method MUST be public, otherwise it will not be invoked at the end of the tween.
        /// </summary>
        public void EndHandler(object methodObject, string functionName, params object[] args)
        {
            if (methodObject == null)
                throw new Exception("methodObject cannot be null");

            // Type array used to find endhandler that is overloaded.
            Type[] typeArray = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                Type argType = args[i] as Type;
                if (argType != null)
                {
                    typeArray[i] = argType;
                    args[i] = null;
                }
                else
                    typeArray[i] = args[i].GetType();
            }

            _methodInfo = methodObject.GetType().GetMethod(functionName, typeArray);

            if (_methodInfo == null)
                throw new Exception("Function " + functionName + " not found in class " + methodObject.GetType().ToString());

            _methodObject = methodObject;
            _methodArgs = args;
            _runEndHandler = true;
        }

        // Used for static methods.
        public void EndHandler(Type objectType, string functionName, params object[] args)
        {
            // Type array used to find endhandler that is overloaded.
            Type[] typeArray = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                Type argType = args[i] as Type;
                if (argType != null)
                {
                    typeArray[i] = argType;
                    args[i] = null;
                }
                else
                    typeArray[i] = args[i].GetType();
            }

            _methodInfo = objectType.GetMethod(functionName, typeArray);

            if (_methodInfo == null)
                throw new Exception("Function " + functionName + " not found in class " + objectType.GetType().ToString());

            _methodObject = null;
            _methodArgs = args;
            _runEndHandler = true;
        }

        public void Complete()
        {
            int counter = 0;
            for (int i = 0; i < _propertiesList.Length - 1; i = i + 2)
            {
                if (_propertiesList[i] != "delay")
                {
                    if (m_isType == true)
                        _propertyInfo = (_tweenObject as Type).GetProperty(_propertiesList[i]);
                    else
                        _propertyInfo = _tweenObject.GetType().GetProperty(_propertiesList[i]);

                    float value;
                    if (_tweenTo == true)
                        value = float.Parse(_propertiesList[i + 1], NumberStyles.Any, Tween.ParserCI);
                    else
                        value = _initialValues[counter] + float.Parse(_propertiesList[i + 1], NumberStyles.Any, Tween.ParserCI);
                    _elapsedTime = _duration;

                    if (_propertyInfo.PropertyType == typeof(int))
                        _propertyInfo.SetValue(_tweenObject, (int)value, null);
                    else
                    _propertyInfo.SetValue(_tweenObject, value, null);
                    counter++;
                }
            }

            //Console.WriteLine("Tween complete");
            _active = false;
        }

        public void RunEndHandler()
        {
            // Run the tween's end handler.
            if (_runEndHandler == true)
            {
                if (_methodInfo != null)
                    _methodInfo.Invoke(_methodObject, _methodArgs);
            }
        }

        public void StopTween(bool runEndHandler)
        {
            _runEndHandler = runEndHandler;
            _active = false; // The tween will be garbage collected on the next update.
        }

        public void PauseTween()
        {
            _paused = true;
        }

        public void ResumeTween()
        {
            _paused = false;
        }

        public void Dispose()
        {
            _tweenObject = null;
            _propertyInfo = null;
            _methodObject = null;
            _methodInfo = null;
            if (_methodArgs != null)
                Array.Clear(_methodArgs, 0, _methodArgs.Length);
            _methodArgs = null;
            if (_propertiesList != null)
                Array.Clear(_propertiesList, 0,_propertiesList.Length);
            _propertiesList = null;
            if (_initialValues != null)
                Array.Clear(_initialValues, 0, _initialValues.Length);
            _initialValues = null;
            _active = false;
            _elapsedTime = 0;
            _duration = 0;
            _tweenTo = false;
            _ease = new Easing(Ease.Linear.EaseNone);
            _runEndHandler = false;
            _paused = false;
            UseTicks = false;
            m_delayTime = 0;
            m_elapsedDelayTime = 0;
        }

        public bool Active
        {
            get { return _active; }
        }

        public bool Paused
        {
            get { return _paused; }
        }

        public object TweenedObject
        {
            get { return _tweenObject; }
        }

        public bool IsType
        {
            get { return m_isType; }
        }
    }
}
