namespace MCronberg.Sap.ConsoleOutput.Core
{

    // Inspired/copied from https://github.com/Robert-McGinley/TableParser    
    namespace TableParser
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Linq.Expressions;
        using System.Reflection;
        using System.Text;

        internal class TableParserHelper
        {
            private readonly bool showCounter;

            public TableParserHelper(bool showCounter = false)
            {
                this.showCounter = showCounter;
            }
            public string ToStringTable<T>(IEnumerable<T> values, string[] columnHeaders, params Func<T, object>[] valueSelectors)
            {
                return ToStringTable(values.ToArray(), columnHeaders, valueSelectors);
            }

            public string ToStringTable<T>(T[] values, string[] columnHeaders, params Func<T, object>[] valueSelectors)
            {


                var arrValues = new string[values.Length + 1, valueSelectors.Length];

                // Fill headers
                for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                {
                    arrValues[0, colIndex] = columnHeaders[colIndex];
                }

                // Fill table rows
                for (int rowIndex = 1; rowIndex < arrValues.GetLength(0); rowIndex++)
                {
                    for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                    {
                        object value = valueSelectors[colIndex].Invoke(values[rowIndex - 1]);

                        arrValues[rowIndex, colIndex] = value != null ? value.ToString() : "null";
                    }
                }

                return ToStringTable(arrValues);
            }

            public string ToStringTable(string[,] arrValues, int stringLength = 20)
            {
                int[] maxColumnsWidth = GetMaxColumnsWidth(arrValues);
                int countWidth = arrValues.GetLength(0).ToString().Length;
                int addShowCounter = showCounter ? countWidth + 3 : 0;
                var headerSpliter = new string('-', maxColumnsWidth.Sum(i => i + 3) - 1 + addShowCounter);

                var sb = new StringBuilder();
                int count = -1;
                for (int rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
                {
                    if (showCounter)
                    {
                        sb.Append(" | ");
                        if (rowIndex == 0)
                            sb.Append("#" + new string(' ', countWidth - 1));
                        else
                            sb.Append(count.ToString().PadRight(countWidth));
                    }

                    for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                    {
                        // Print cell
                        string cell = cut(arrValues[rowIndex, colIndex], stringLength);
                        cell = cell.PadRight(maxColumnsWidth[colIndex]);
                        sb.Append(" | ");
                        sb.Append(cell);
                    }
                    // Print end of line
                    sb.Append(" | ");
                    sb.AppendLine();

                    // Print splitter
                    if (rowIndex == 0)
                    {
                        sb.AppendFormat(" |{0}| ", headerSpliter);
                        sb.AppendLine();
                    }
                    count++;
                }

                return sb.ToString();

                string cut(string tzt, int length) {
                    if (tzt.Length < length)
                        return tzt;
                    return tzt.Substring(0, length - 3) + "...";
                }
            }

            private int[] GetMaxColumnsWidth(string[,] arrValues, int stringLength = 20)
            {
                var maxColumnsWidth = new int[arrValues.GetLength(1)];
                for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++)
                {
                    for (int rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++)
                    {
                        int newLength = arrValues[rowIndex, colIndex].Length<=stringLength? arrValues[rowIndex, colIndex].Length: stringLength;
                        int oldLength = maxColumnsWidth[colIndex];

                        if (newLength > oldLength)
                        {
                            maxColumnsWidth[colIndex] = newLength;
                        }
                    }
                }

                return maxColumnsWidth;
            }

            public string ToStringTable<T>(IEnumerable<T> values, params Expression<Func<T, object>>[] valueSelectors)
            {
                var headers = valueSelectors.Select(func => GetProperty(func).Name).ToArray();
                var selectors = valueSelectors.Select(exp => exp.Compile()).ToArray();
                return ToStringTable(values, headers, selectors);
            }

            private PropertyInfo GetProperty<T>(Expression<Func<T, object>> expresstion)
            {
                if (expresstion.Body is UnaryExpression)
                {
                    if ((expresstion.Body as UnaryExpression).Operand is MemberExpression)
                    {
                        return ((expresstion.Body as UnaryExpression).Operand as MemberExpression).Member as PropertyInfo;
                    }
                }

                if ((expresstion.Body is MemberExpression))
                {
                    return (expresstion.Body as MemberExpression).Member as PropertyInfo;
                }
                return null;
            }
        }
    }
}
