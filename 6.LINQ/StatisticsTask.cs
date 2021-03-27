using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews
{
	public class StatisticsTask
	{
		public static double GetMedianTimePerSlide(List<VisitRecord> visits, SlideType slideType){
			return visits
			       .ToLookup(x => x.UserId, x => x)
			       .Select(x => x
			                    .OrderBy(y => y.DateTime)
			                    .Bigrams()
			                    .Where(y => 
				                    y.Item1.SlideType == slideType
				                    && y.Item1.SlideId != y.Item2.SlideId)
			                    .Select(y => y.Item2.DateTime.Subtract(y.Item1.DateTime).TotalMinutes)
			                    .Where(y => y >= 1 && y <= 120)
			       )
			       .SelectMany(x => x)
			       .DefaultIfEmpty(0)
			       .Median();
		}
	}
}
