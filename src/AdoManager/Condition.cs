using System.Linq;

namespace AdoManager
{
    public class Condition
    {
        

        public Operators OperateByBeforeConditions { get; set; } = Operators.And;
        public string Key { get; set; }
        public Operators Operate { get; set; }
        public object[] Values { get; set; }
        public bool Not { get; set; }

        public Condition(bool not, string key, Operators opt,  params object[] values)
        {
            Key = key;
            Operate = opt;
            Values = values;
            Not = not;
        }

        public static Condition Factory(bool not, string key, Operators opt, params object[] values)
        {
            return new Condition(not, key, opt, values);
        }

        public static Condition Factory(string key, Operators opt, params object[] values)
        {
            return new Condition(false, key, opt, values);
        }


        public override string ToString()
        {
            string result = Not ? "NOT " : "";

            switch (Operate)
            {
                case Operators.Between:
                    result += $@"{Key} {Operate} {Values[0]} AND {Values[1]}";
                    break;
                case Operators.In:
                    var strValues = Values.Select(x => $"{x.ToSqlString()}");
                    result += $@"{Key} {Operate} ({string.Join(",", strValues)})";
                    break;
                case Operators.Like:
                    result += $@"{Key} {Operate} '%{Values[0]}%'";
                    break;
                case Operators.Equal:
                    result += $@"{Key} = {Values[0].ToSqlString()}";
                    break;
                case Operators.NotEqual:
                    result += $@"{Key} <> {Values[0].ToSqlString()}";
                    break;
                case Operators.GreaterThan:
                    result += $@"{Key} > {Values[0].ToSqlString()}";
                    break;
                case Operators.LessThan:
                    result += $@"{Key} < {Values[0].ToSqlString()}";
                    break;
                case Operators.GreaterThanOrEqualTo:
                    result += $@"{Key} >= {Values[0].ToSqlString()}";
                    break;
                case Operators.LessThanOrEqualTo:
                    result += $@"{Key} <= {Values[0].ToSqlString()}";
                    break;
                default:
                    result += $@"{Key} {Operate} {Values[0].ToSqlString()}";
                    break;
            }

            return result;
        }


        
    }
}
