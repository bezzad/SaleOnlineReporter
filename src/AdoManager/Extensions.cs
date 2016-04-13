using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoManager
{
    public static class Extensions
    {
        public static string ConvertToQuery(this Condition[] conditions)
        {
            if (conditions.Any())
            {
                var condStr = string.Join(Environment.NewLine,
                    conditions.Select(x => $"{x.OperateByBeforeConditions} {x.ToString()} "));

                return condStr.Substring(condStr.IndexOf(" ", StringComparison.Ordinal) + 1);
            }

            return "";
        }

        public static string ToSqlString(this object obj)
        {
            if (obj == null) return "";

            if (obj is string || obj is bool)
            {
                return $"\'{obj}\'";
            }

            return obj.ToString();
        }
    }
}
