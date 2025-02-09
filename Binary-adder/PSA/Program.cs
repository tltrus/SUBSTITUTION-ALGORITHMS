using System;
using System.Collections.Generic;
using System.Data;

namespace PSA
{
    public class Program
    {
        // Code should be optimized
        
        static List<Pattern> patterns = new List<Pattern>();

        static int[,] matrix = new int[,]
            {
                { 0, 1, 0, 0, 1 }, // 9
		        { 0, 1, 1, 1, 1 }, // 15
		        { 0, 0, 1, 0, 1 }, // 5
		        { 0, 0, 0, 0, 0 }
            };

        //static int[,] matrix = new int[,]
        //    {
        //        { 1, 0, 0, 0, 0 }, // 16
        //        { 0, 1, 0, 1, 1 }, // 11
        //        { 0, 0, 1, 0, 0 }, // 4
        //        { 0, 0, 0, 0, 0 },
        //    };

        static int[,] busyMap = new int[,] // for storing visited cells, 0-not visited, 1-visited
            {
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
            };

        public static void Main()
        {
            // Draw matrix
            Console.WriteLine(" /--- Original matrix ---/");
            string m = BinMatrixToString(matrix);
            Console.WriteLine(m);
            Console.WriteLine();

            // Set
            SetPattern();

            // Calculation
            int incr = 0;
            while(incr < 10)
            {
                if (CheckSummationIsDone()) break;


                Summation();

                incr++;
            }

            Console.WriteLine($"SUM is {GetFinalSum()}");

            Console.ReadLine();

        }

        static void Summation()
        {
            // patterns
            for (int k = 0; k < 2; ++k)
            {
                // reset busy array
                busyMap = new int[,]
                {
                        { 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0 },
                };

                for (int i = 0; i < matrix.GetLength(0); ++i)
                {
                    for (int j = 0; j < matrix.GetLength(1); ++j)
                    {
                        // Are matrix and pattern Equal?
                        bool areArraysEqual = ArraysComparing(patterns[k].from, patterns[k].to, i, j, k);

                        if (areArraysEqual)
                        {
                            // FOR DEBUGGING

                            Console.WriteLine("pattern " + k + ". startRow = " + i + ", startCol = " + j);

                            //Draw matrix
                            var m = BinMatrixToString(matrix);
                            Console.WriteLine(m);
                        }
                    }
                }

                //Console.WriteLine(" /--- Pattern " + k + " end ---/");

                //// Draw matrix
                //var m = BinMatrixToString(matrix);
                //Console.WriteLine(m);
            }
        }

        static bool CheckSummationIsDone()
        {
            // Check rows from 1 to last.
            // "Summation is Done" means that all cells in rows[1..n] are 0.
            for (int i = 1; i < matrix.GetLength(0); ++i)
                for (int j = 0; j < matrix.GetLength(1); ++j)
                    if (matrix[i, j] == 1) return false;
            return true;
        }
        static int GetFinalSum()
        {
            // GET ROW FROM ARRAY
            int[] bin = GetArrayRow(matrix, 0);

            return BinToDec(String.Join("", bin));
        }

        static string BinMatrixToString(int[,] array)
        {
            string result = "";

            for (int i = 0; i < array.GetLength(0); ++i)
            {
                // GET ROW FROM ARRAY
                int[] bin = GetArrayRow(array, i);

                // Draw row
                result += String.Join("\t", bin);

                // Draw sum of row
                int sumDec = BinToDec(String.Join("", bin));

                result += "\t | " + sumDec.ToString() + "\n";
            }
            return result;
        }

        static int[] GetArrayRow(int[,] array, int row)
        {
            int len = array.GetLength(1);
            int[] result = new int[len];

            for (int i = 0; i < len; ++i)
            {
                result[i] = array[row, i];
            }
            return result;
        }

        static int BinToDec(string bin) => Convert.ToInt32(bin, 2);

        static void SetToBusy(int[,] p_from, int[,] p_to, int startRow, int startCol)
        {
            int patternRowLen = p_from.GetLength(0);
            int patternColLen = p_from.GetLength(1);

            int mainRowLen = matrix.GetLength(0);
            int mainColLen = matrix.GetLength(1);

            for (int i = 0; i < patternRowLen; ++i)
            {
                for (int j = 0; j < patternColLen; ++j)
                {
                    if (startRow + patternRowLen - 1 < mainRowLen && startCol + patternColLen - 1 < mainColLen) // check edges of scanning
                    {
                        if (matrix[i + startRow, j + startCol] == p_from[i, j])
                        {
                            // set to busy
                            busyMap[i + startRow, j + startCol] = 1; // set 1 for busy / 0 is not busy, 1 is busy

                            // replace
                            matrix[i + startRow, j + startCol] = p_to[i, j];
                        }
                    }
                }
            }
        }

        static bool ArraysComparing(int[,] p_from, int[,] p_to, int startRow, int startCol, int k)
        {
            int patternRowLen = p_from.GetLength(0);
            int patternColLen = p_from.GetLength(1);

            int mainRowLen = matrix.GetLength(0);
            int mainColLen = matrix.GetLength(1);

            bool result = false;

            int checksum = 0;
            for (int i = 0; i < patternRowLen; ++i)
            {
                for (int j = 0; j < patternColLen; ++j)
                {
                    if (startRow + patternRowLen - 1 < mainRowLen && startCol + patternColLen - 1 < mainColLen) // check edges of scanning
                    {
                        if (matrix[i + startRow, j + startCol] == p_from[i, j] && busyMap[i + startRow, j + startCol] != 1 || p_from[i, j] == -1) // -1 is not necesary cell
                        {
                            checksum++; // ok
                        }
                    }
                }
            }

            bool isLowRowEmpty = CheckLowCellisEmpty(startRow + patternRowLen, startCol, patternColLen);
            if (patterns[k].isCheckUnder) isLowRowEmpty = true;

            if (checksum == patterns[k].checkSum && isLowRowEmpty)
            {
                // if equal set to busy + replace
                SetToBusy(patterns[k].from, patterns[k].to, startRow, startCol);

                return true;
            }

            return false;
        }

        // check under pattern area. It should be 0
        static bool CheckLowCellisEmpty(int row, int col, int cols)
        {
            int mainRowLen = matrix.GetLength(0);
            if (row >= mainRowLen) return true;

            for (int j = col; j < cols; ++j)
            {
                if (matrix[row, j + col] == 1) // is under cell is 1
                {
                    return false;
                }
            }
            return true;
        }

        static void SetPattern()
        {
            // Add pattern 1
            Pattern pattern_1 = new Pattern(true, 4);
            var array = new int[,]
                {
                     { -1, 1 },
                     {  0, 1 }
                };
            pattern_1.AddToSource(array);

            array = new int[,]
                {
                     { -1, 0 },
                     {  1, 0 }
                };
            pattern_1.AddToReplace(array);

            // Add pattern 2
            Pattern pattern_2 = new Pattern(false, 2);
            array = new int[,]
                {
                    { 0 },
                    { 1 }
                };
            pattern_2.AddToSource(array);

            array = new int[,]
                {
                    { 1 },
                    { 0 }
                };
            pattern_2.AddToReplace(array);

            patterns.Add(pattern_1);
            patterns.Add(pattern_2);
        }

        class Pattern
        {
            public int[,] from, to;
            public bool isCheckUnder;
            public int checkSum;

            public Pattern(bool checkUnder, int checkSum)
            {
                isCheckUnder = checkUnder;
                this.checkSum = checkSum;
            }

            public void AddToSource(int[,] array)
            {
                from = new int[array.GetLength(0), array.GetLength(1)];
                
                Copy(array, from);
            }

            public void AddToReplace(int[,] array)
            {
                to = new int[array.GetLength(0), array.GetLength(1)];

                Copy(array, to);
            }

            private void Copy(int[,] source, int[,] to)
            {
                int sourceRowLen = source.GetLength(0);
                int sourceColLen = source.GetLength(1);

                for (int i = 0; i < sourceRowLen; ++i)
                {
                    for (int j = 0; j < sourceColLen; ++j)
                    {
                        to[i, j] = source[i, j];
                    }
                }

            }
        }
    }

}




