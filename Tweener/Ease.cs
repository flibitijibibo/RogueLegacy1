using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tweener.Ease
{
    public class Back
    {
//        public static float EaseIn(float t, float b, float c, float d, float s)
        public static float EaseIn(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            return c * (t /= d) * t * ((s + 1) * t - s) + b;
        }

        public static float EaseInLarge(float t, float b, float c, float d)
        {
            float s = 1.70158f * 3;
            return c * (t /= d) * t * ((s + 1) * t - s) + b;
        }

        public static float EaseOut(float t, float b, float c, float d)
        {
            float s = 1.70158f;
			return c*((t=t/d-1)*t*((s+1)*t + s) + 1) + b;
		}

        public static float EaseOutLarge(float t, float b, float c, float d)
        {
            float s = 1.70158f * 3;
            return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
        }

		public static float EaseInOut(float t, float b, float c, float d)
        {
            float s = 1.70158f; 
			if ((t/=d/2) < 1) return c/2*(t*t*(((s*=(1.525f))+1)*t - s)) + b;
			return c/2*((t-=2)*t*(((s*=(1.525f))+1)*t + s) + 2) + b;
		}

        public static float EaseInOutLarge(float t, float b, float c, float d)
        {
            float s = 1.70158f * 3;
            if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + b;
            return c / 2 * ((t -= 2) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + b;
        }
    }

    public class Bounce 
    {
        public static float EaseOut(float t, float b, float c, float d)
        {
		    if ((t/=d) < (1/2.75f)) {
			    return c*(7.5625f*t*t) + b;
		    } else if (t < (2/2.75f)) {
			    return c*(7.5625f*(t-=(1.5f/2.75f))*t + .75f) + b;
		    } else if (t < (2.5f/2.75f)) {
			    return c*(7.5625f*(t-=(2.25f/2.75f))*t + .9375f) + b;
		    } else {
			    return c*(7.5625f*(t-=(2.625f/2.75f))*t + .984375f) + b;
		    }
	    }

	    public static float EaseIn(float t, float b, float c, float d)
        {
		    return c - EaseOut(d-t, 0, c, d) + b;
	    }

	    public static float EaseInOut(float t, float b, float c, float d)
        {
		    if (t < d/2) return EaseIn(t*2, 0, c, d) * .5f + b;
		    else return EaseOut(t*2-d, 0, c, d) * .5f + c*.5f + b;
	    }
	}

    public class Circ
    {
        public static float EaseIn(float t, float b, float c, float d)
        {
			return -c * (float)(Math.Sqrt(1 - (t/=d)*t) - 1) + b;
		}

		public static float EaseOut(float t, float b, float c, float d)
        {
            return c * (float)Math.Sqrt(1 - (t = t / d - 1) * t) + b;
		}
		
        public static float EaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return -c / 2 * (float)(Math.Sqrt(1 - t * t) - 1) + b;
            return c / 2 * (float)(Math.Sqrt(1 - (t -= 2) * t) + 1) + b;
		}
	}

    public class Cubic 
    {
        public static float EaseIn(float t, float b, float c, float d)
        {
			return c*(t/=d)*t*t + b;
		}
		
        public static float EaseOut(float t, float b, float c, float d)
        {
			return c*((t=t/d-1)*t*t + 1) + b;
		}
		
        public static float EaseInOut(float t, float b, float c, float d)
        {
			if ((t/=d/2) < 1) return c/2*t*t*t + b;
			return c/2*((t-=2)*t*t + 2) + b;
		}
	}

    public class Elastic
    {
        public static float EaseIn(float t, float b, float c, float d, float a, float p)
        {
            float s;
            if (t == 0) return b; if ((t /= d) == 1) return b + c; if (p == 0) p = d * .3f;
            if (a == 0 || a < Math.Abs(c)) { a = c; s = p / 4; }
            else s = (float)(p / (2 * Math.PI) * Math.Asin(c / a));
            return (float)(-(a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b);
        }

        public static float EaseOut(float t, float b, float c, float d, float a, float p)
        {
            float s;
            if (t == 0) return b; if ((t /= d) == 1) return b + c; if (p == 0) p = d * .3f;
            if (a == 0 || a < Math.Abs(c)) { a = c; s = p / 4; }
            else s = (float)(p / (2 * Math.PI) * Math.Asin(c / a));
            return (float)(a * Math.Pow(2, -10 * t) * Math.Sin((t * d - s) * (2 * Math.PI) / p) + c + b);
        }

        public static float EaseInOut(float t, float b, float c, float d, float a, float p)
        {
            float s;
            if (t == 0) return b; if ((t /= d / 2) == 2) return b + c; if (p == 0) p = d * (.3f * 1.5f);
            if (a == 0 || a < Math.Abs(c)) { a = c; s = p / 4; }
            else s = (float)(p / (2 * Math.PI) * Math.Asin(c / a));
            if (t < 1) return (float)(-.5f * (a * Math.Pow(2, 10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b);
            return (float)(a * Math.Pow(2, -10 * (t -= 1)) * Math.Sin((t * d - s) * (2 * Math.PI) / p) * .5f + c + b);
        }
    }

    public class Expo
    {
        public static float EaseIn(float t, float b, float c, float d)
        {
			return (t==0) ? b : (float)(c * Math.Pow(2, 10 * (t/d - 1)) + b);
		}

        public static float EaseOut(float t, float b, float c, float d)
        {
			return (t==d) ? b+c : (float)(c * (-Math.Pow(2, -10 * t/d) + 1) + b);
		}

        public static float EaseInOut(float t, float b, float c, float d)
        {
			if (t==0) return b;
			if (t==d) return b+c;
			if ((t/=d/2) < 1) return (float)(c/2 * Math.Pow(2, 10 * (t - 1)) + b);
			return (float)(c/2 * (-Math.Pow(2, -10 * --t) + 2) + b);
		}
	}

    //t is time, b is beginning position, c is the total change in position (NOT DELTACHANGE), and d is the total duration of the tween
    public class Linear 
    {
        public static float EaseNone(float t, float b, float c, float d)
        {
			return c*t/d + b;
		}

		public static float EaseIn(float t, float b, float c, float d)
        {
			return c*t/d + b;
		}
		
        public static float EaseOut(float t, float b, float c, float d)
        {
			return c*t/d + b;
		}

		public static float EaseInOut(float t, float b, float c, float d)
        {
			return c*t/d + b;
		}
	}

    public class Quad 
    {
        public static float EaseIn(float t, float b, float c, float d)
        {
			return c*(t/=d)*t + b;
		}

        public static float EaseOut(float t, float b, float c, float d)
        {
			return -c *(t/=d)*(t-2) + b;
		}

        public static float EaseInOut(float t, float b, float c, float d)
        {
			if ((t/=d/2) < 1) return c/2*t*t + b;
			return -c/2 * ((--t)*(t-2) - 1) + b;
		}
	}

    public class Quart 
    {
        public static float EaseIn(float t, float b, float c, float d)
        {
			return c*(t/=d)*t*t*t + b;
		}

        public static float EaseOut(float t, float b, float c, float d)
        {
			return -c * ((t=t/d-1)*t*t*t - 1) + b;
		}

        public static float EaseInOut(float t, float b, float c, float d)
        {
			if ((t/=d/2) < 1) return c/2*t*t*t*t + b;
			return -c/2 * ((t-=2)*t*t*t - 2) + b;
		}
	}

    public class Quint 
    {
        public static float EaseIn(float t, float b, float c, float d)
        {
			return c*(t/=d)*t*t*t*t + b;
		}

        public static float EaseOut(float t, float b, float c, float d)
        {
			return c*((t=t/d-1)*t*t*t*t + 1) + b;
		}

        public static float EaseInOut(float t, float b, float c, float d)
        {
			if ((t/=d/2) < 1) return c/2*t*t*t*t*t + b;
			return c/2*((t-=2)*t*t*t*t + 2) + b;
		}
	}

    public class Sine 
    {
        public static float EaseIn(float t, float b, float c, float d)
        {
			return (float)(-c * Math.Cos(t/d * (Math.PI/2)) + c + b);
		}

        public static float EaseOut(float t, float b, float c, float d)
        {
			return (float)(c * Math.Sin(t/d * (Math.PI/2)) + b);
		}

        public static float EaseInOut(float t, float b, float c, float d)
        {
			return (float)(-c/2 * (Math.Cos(Math.PI*t/d) - 1) + b);
		}
	}
}
