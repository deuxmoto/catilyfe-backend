using System;

namespace CatiLyfe.Common.Utilities
{
    /// <summary>
    /// Math that nobody understands.
    /// </summary>
    public static class ComplexMath
    {
        /// <summary>
        /// Round down to the nearest power of two.
        /// </summary>
        /// <param name="input">The input value.</param>
        /// <returns>The nearest power of two.</returns>
        public static int RoundDownP2(int input)
        {
            if(input == 0)
            {
                return 0;
            }

            for(var i = 0; i < 32; i++)
            {
                if(input >> i == 1)
                {
                    return 1 << i;
                }
            }

            throw new Exception("Not possible....");
        }
    }
}
