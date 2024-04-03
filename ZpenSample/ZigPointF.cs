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
