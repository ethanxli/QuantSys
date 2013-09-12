using System;
using System.Reflection;
using System.Runtime.InteropServices;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.Office.Interop.Excel;

namespace QuantSys.Util
{
    public static class ExcelUtil
    {
        public static void Open(string filename, out object[,] data)
        {
            object misValue = Missing.Value;


            Console.WriteLine("Loading File " + filename + " ...");
            var xlApp = new Application();
            var xlWorkBook = xlApp.Workbooks.Open(filename, 0, true, 5, "", "", true, XlPlatform.xlWindows, "\t", false,
                false, 0, true, 1, 0);
            Console.WriteLine("Done.");

            var xlWorkSheet = (Worksheet) xlWorkBook.Worksheets.Item[1];

            data = ParseData(xlWorkSheet);


            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            ReleaseObject(xlWorkSheet);
            ReleaseObject(xlWorkBook);
            ReleaseObject(xlApp);
        }


        private static void ReleaseObject(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }


        public static object[,] ParseData(Worksheet w)
        {
            try
            {
                Range usedRange = w.UsedRange;
                var data = new object[usedRange.Rows.Count - 1, usedRange.Columns.Count];

                data = usedRange.get_Value(Type.Missing);

                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static DenseMatrix ToMatrix(this object[,] data, int rowStart, int rowEnd, int colStart, int colEnd,
            bool reverseColumns)
        {
            var d = new DenseMatrix(rowEnd - rowStart + 1, colEnd - colStart + 1);

            if (reverseColumns)
            {
                for (int i = rowEnd; i >= rowStart; i--)
                {
                    for (int j = colStart; j <= colEnd; j++)
                    {
                        d[rowEnd - i, j - colStart] = (double) data[i, j];
                    }
                }
            }
            else
            {
                for (int i = rowStart; i <= rowEnd; i++)
                {
                    for (int j = colStart; j <= colEnd; j++)
                    {
                        d[i - rowStart, j - colStart] = (double) data[i, j];
                    }
                }
            }

            return d;
        }
    }
}