using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextEditorMVC.CustomClasses
{
    public class LevensteinZero
    {
        public int FollowingZerosCount { get; set; }

        public int Length { get; }

        public int XCoordinate { get; }

        public int YCoordinate { get; }

        public List<LevensteinZero> PreviousZeros { get; }

        public List<LevensteinZero> EndingZeros { get; }

        public LevensteinZero(int xCoordinate, int yCoordinate, List<LevensteinZero> otherZeros)
        {
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;

            try
            {
                EndingZeros = otherZeros[0].EndingZeros;
            }
            catch
            {
                EndingZeros = new List<LevensteinZero>();
            }

            PreviousZeros = new List<LevensteinZero>();
            Length = Math.Max(xCoordinate, yCoordinate);
            FollowingZerosCount = 0;

            foreach (LevensteinZero zero in otherZeros)
            {
                if(zero.XCoordinate< XCoordinate && zero.YCoordinate < YCoordinate)
                {
                    PreviousZeros.Add(zero);
                    if(zero.FollowingZerosCount == 0)
                    {
                        if (!EndingZeros.Remove(zero))
                        {
                            throw new Exception("Tried to remove non existing element.");
                        }
                    }
                    zero.FollowingZerosCount++;
                    if (zero.Length + Math.Max(XCoordinate - zero.XCoordinate, YCoordinate - zero.YCoordinate) -1 < Length)
                    {
                        Length = zero.Length + Math.Max(XCoordinate - zero.XCoordinate, YCoordinate - zero.YCoordinate) -1;
                    }
                }
            }
            EndingZeros.Add(this);
        }

        /// <summary>
        /// Call this method to remove all conections with its previous zeros.
        /// </summary>
        public void RemoveConnections()
        {
            foreach(LevensteinZero zero in PreviousZeros)
            {
                zero.FollowingZerosCount--;
                if (zero.FollowingZerosCount < 0)
                {
                    throw new ArgumentOutOfRangeException($"Zero point can not have negative {nameof(FollowingZerosCount)}.");
                }
                if(zero.FollowingZerosCount == 0)
                {
                    EndingZeros.Add(zero);
                }
            }
            EndingZeros.Remove(this);
        }
    }
}
