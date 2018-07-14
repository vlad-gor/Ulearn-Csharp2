// Вставьте сюда финальное содержимое файла StatisticsTask.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews
{
	public class StatisticsTask
	{
        private static double savedValue = 0;
        private static bool sameSlide = false;

        public static double GetMedianTimePerSlide(List<VisitRecord> visits, SlideType slideType)
		{
            return visits
                 .GroupBy(visit => visit.UserId)
                 .Select(group => group.OrderBy(x => x.DateTime)
                 .Bigrams())
                 .Select(bigram => GetAllTimes(bigram, slideType))
                 .SelectMany(x => x)
                 .Where(time=> time >= 1 && time <= 120)
                 .ToList()
                 .DefaultIfEmpty(0)
                 .Median();
        }

        private static IEnumerable<double> GetAllTimes(IEnumerable<Tuple<VisitRecord, VisitRecord>> visits, SlideType slideType)
        {
            return visits
                .Where(visit => visit.Item1.UserId.Equals(visit.Item2.UserId))
                .Where(x => CheckForSlideId(x) && x.Item1.SlideType == slideType)
                .Select(visit => visit.Item2.DateTime.Subtract(visit.Item1.DateTime).TotalMinutes + savedValue);
        }

        private static double GetTime(Tuple<VisitRecord, VisitRecord> visits)
        {
            if (!visits.Item1.UserId.Equals(visits.Item2.UserId)) return 0;
            return visits.Item2.DateTime.Subtract(visits.Item1.DateTime).TotalMinutes;
        }

        private static bool CheckForSlideId(Tuple<VisitRecord, VisitRecord> visit)
        {
            if (visit.Item1.SlideId.Equals(visit.Item2.SlideId))
            {
                sameSlide = true;
                savedValue += visit.Item2.DateTime.Subtract(visit.Item1.DateTime).TotalMinutes;
                return false;
            }
            else if (sameSlide)
            {
                sameSlide = false;
                return true;
            }
            savedValue = 0;
            return true;
        }
	}
}