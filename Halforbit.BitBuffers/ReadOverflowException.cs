using System;

namespace Halforbit.BitBuffers
{
    public class ReadOverflowException : Exception
    {
        public ReadOverflowException() { }

        public static void Assert(bool check)
        {
            if (!check) throw new ReadOverflowException();
        }
    }
}
