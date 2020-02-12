using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamChecker.Source.ImageProcessing
{
    /// <summary>
    /// Class containing digits as 7-segment diplay.
    /// </summary>
    static class Digits
    {
        //       1
        //   +-------+
        //   +       +
        // 6 +   7   + 2
        //   +-------+
        //   +       +
        // 5 +       + 3
        //   +-------+  
        //       4


        ///<value> Structure containing digit as a key and segments' list as a value. 1 - on, 0 - off </value>
        public static Dictionary<int, List<int>> digits = new Dictionary<int, List<int>>() {
            {0, new List<int>() {1, 1, 1, 1, 1, 1, 0} },
            {1, new List<int>() {0, 1, 1, 0, 0, 0, 0} },
            {2, new List<int>() {1, 1, 0, 1, 1, 0, 1} },
            {3, new List<int>() {1, 1, 1, 1, 0, 0, 1} },
            {4, new List<int>() {0, 1, 1, 0, 0, 1, 1} },
            {5, new List<int>() {1, 0, 1, 1, 0, 1, 1} },
            {6, new List<int>() {1, 0, 1, 1, 1, 1, 1} },
            {7, new List<int>() {1, 1, 1, 0, 0, 0, 0} },
            {8, new List<int>() {1, 1, 1, 1, 1, 1, 1} },
            {9, new List<int>() {1, 1, 1, 1, 0, 1, 1} }
        };
        
    }
}
