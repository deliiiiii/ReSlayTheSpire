using UnityEngine;

namespace Violee
{
    public class BoxModel : MonoBehaviour
    {
        BoxData boxData;
        public static bool IsConnect(BoxData b, EBoxSide s1, EBoxSide s2)
        {
            return IsConnect(b, (byte)s1, (byte)s2);
        }
        static bool IsConnect(BoxData b,byte s1, byte s2)
        {
            var big = s1 > s2 ? s1 : s2;
            var small = s1 < s2 ? s1 : s2;
            var x = b.Walls & 0b1111;
            var y = b.Walls >> 4;
            var from = small;
            if (big == 8 && small == 1)
                from = 8;
            var sIsConnect = ((x & s1) | (x & s2)) == 0;
            var tIsConnect = (big, small) switch
            {
                (4, 1) => (y & 3) != 3
                          && (y & 12) != 12
                          && (y & 5) != 5
                          && (y & 10) != 10,
                (8, 2) => (y & 9) != 9
                          && (y & 6) != 6
                          && (y & 5) != 5
                          && (y & 10) != 10,
                _ => y == from || (y | from) != y,
            };
            return sIsConnect && tIsConnect;
        }
    }
}