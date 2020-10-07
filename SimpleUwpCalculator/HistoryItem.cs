using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleUwpCalculator
{
    public class HistoryItem
    {
        public HistoryItem(string expression, decimal result)
        {
            Expression = expression;
            Result = result;
        }
        public string Expression { get; set; }
        public decimal Result { get; set; }
    }
}
