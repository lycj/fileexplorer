using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Visualstudiomagazine
{
    //http://stackoverflow.com/questions/1822687/c-3-0-tuple-equivalents-for-poor-men
    //http://visualstudiomagazine.com/articles/2007/04/01/generics-move-beyond-collections.aspx?sc_lang=en
    public struct Tuple<T1, T2> :
    IEquatable<Tuple<T1, T2>>
    {
        private readonly T1 first;
        public T1 First
        {
            get { return first; }
        }

        private readonly T2 second;
        public T2 Second
        {
            get { return second; }
        }

        public Tuple(T1 f, T2 s)
        {
            first = f;
            second = s;
        }

        #region IEquatable<Tuple<T1,T2>> Members
        public bool Equals(Tuple<T1, T2> other)
        {
            return first.Equals(other.first) &&
            second.Equals(other.second);
        }

        public override bool Equals(object obj)
        {
            if (obj is Tuple<T1, T2>)
                return this.Equals((Tuple<T1, T2>)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return first.GetHashCode() ^ second.GetHashCode();
        }
        #endregion
    }

}
