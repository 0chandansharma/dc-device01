
// The ZigPointF.cs file defines a class named ZigPointF, which appears to represent a point in a two-dimensional space with additional properties p, r, and t. However, some of the properties (r and t) are currently commented out in the code.
// It seems like the class is designed to represent a point with x and y coordinates, along with additional properties. However, the commented-out code suggests that there might be additional properties (r and t) that were considered but not implemented or used in the current version of the code.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZpenSample
{
    class ZigPointF
    {
        private float x;
        private float y;
        private int p;
      //  private float r;
      //  private long t;

        public void SetX(float x) {
            this.x = x;
        }
        public float GetX() {
            return x;
        }

        public void SetY(float y)
        {
            this.y = y;
        }
        public float GetY()
        {
            return y;
        }

        public void SetP(int p)
        {
            this.p = p;
        }
        public float GetP()
        {
            return p;
        }


        //   private void SetR(float r)
        //   {
        //       this.r = r;
        //   }
        //   private float GetR()
        //   {
        //       return r;
        //   }
        //
        //   private void SetT(long t)
        //   {
        //       this.t = t;
        //   }
        //   private long GetT()
        //   {
        //       return t;
        //   }
    }
}
