using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace linq_slideviews
{
    public class ParsingTask
    {
        public static IDictionary<int, SlideRecord> ParseSlideRecords(IEnumerable<string> lines)
        {
            return
                lines.Skip(1)
                    .Select(line => line.Split(';'))
                    .Select(ParseSlideRecord)
                    .Where(slideRecord => slideRecord != null)
                    .ToDictionary(slideRecord => slideRecord.SlideId);
        }

        public static IEnumerable<VisitRecord> ParseVisitRecords(
            IEnumerable<string> lines, IDictionary<int, SlideRecord> slides)
        {
            return lines.Skip(1).Select(line => ParseVisitRecord(slides, line));
        }

        private static VisitRecord ParseVisitRecord(IDictionary<int, SlideRecord> slides, string line)
        {
            try
            {
                var data = line.Split(';');
                var slideId = int.Parse(data[1]);
                return new VisitRecord(
                    int.Parse(data[0]),
                    slideId,
                    DateTime.ParseExact(
                        data[2] + ' ' + data[3],
                        "yyyy-MM-dd HH:mm:ss",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None), slides[slideId].SlideType);
            }
            catch (Exception e)
            {
                throw new FormatException($"Wrong line [{line}]", e);
            }
        }

        private static SlideRecord ParseSlideRecord(string[] data)
        {
            if (data.Length != 3)
                return null;

            int id;
            if (!int.TryParse(data[0], out id))
                return null;

            SlideType type;
            if (!Enum.TryParse(data[1], true, out type))
                return null;

            return new SlideRecord(id, type, data[2]);
        }
    }
}