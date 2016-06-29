using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace RepositoryParser.Core.Messages
{
    public class ChartMessageLevel3 : MessageBase
    {
        public List<string> AuthorsList { get; private set; }
        public string FilteringQuery { get; private set; }

        public ChartMessageLevel3(string generatedQuery)
        {
            FilteringQuery = generatedQuery;
        }
        public ChartMessageLevel3(List<string> authorsList, string generatedQuery)
        {
            FilteringQuery = generatedQuery;
            AuthorsList = authorsList;
        }
    }


    #region UserActivityMessages
    public class ChartMessageLevel3UserActivity : ChartMessageLevel3
    {
        public ChartMessageLevel3UserActivity(string generatedQuery) : base(generatedQuery)
        {
        }

        public ChartMessageLevel3UserActivity(List<string> authorsList, string generatedQuery) : base(authorsList, generatedQuery)
        {
        }
    }

    public class ChartMessageLevel3UserFrequencyCode : ChartMessageLevel3
    {
        public ChartMessageLevel3UserFrequencyCode(string generatedQuery) : base(generatedQuery)
        {
        }

        public ChartMessageLevel3UserFrequencyCode(List<string> authorsList, string generatedQuery) : base(authorsList, generatedQuery)
        {
        }
    }
    #endregion

    #region MonthActivity
    public class ChartMessageLevel3MonthActivity : ChartMessageLevel3
    {
        public ChartMessageLevel3MonthActivity(string generatedQuery) : base(generatedQuery)
        {
        }

        public ChartMessageLevel3MonthActivity(List<string> authorsList, string generatedQuery) : base(authorsList, generatedQuery)
        {
        }
    }
    #endregion

    #region DayActivity

    public class ChartMessageLevel3DayActivity : ChartMessageLevel3
    {
        public ChartMessageLevel3DayActivity(string generatedQuery) : base(generatedQuery)
        {
        }

        public ChartMessageLevel3DayActivity(List<string> authorsList, string generatedQuery) : base(authorsList, generatedQuery)
        {
        }
    }
    #endregion

    #region HourActivity
    public class ChartMessageLevel3HourActivity : ChartMessageLevel3
    {
        public ChartMessageLevel3HourActivity(string generatedQuery) : base(generatedQuery)
        {
        }

        public ChartMessageLevel3HourActivity(List<string> authorsList, string generatedQuery) : base(authorsList, generatedQuery)
        {
        }
    }

    #endregion

    #region WeekdayActivity
    public class ChartMessageLevel3WeekdayActivity: ChartMessageLevel3
    {
        public ChartMessageLevel3WeekdayActivity(string generatedQuery) : base(generatedQuery)
        {
        }

        public ChartMessageLevel3WeekdayActivity(List<string> authorsList, string generatedQuery) : base(authorsList, generatedQuery)
        {
        }
    }
    #endregion

}
