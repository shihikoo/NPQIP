using NPQIP.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class ReportViewModel
    {

        public ReportViewModel(ICollection<UserProfile> users, ICollection<Publication> publications)
        {
            RegistrationsProgressArray = GetProgressArray(users.Select(u => u.CreateDate??default(DateTime)).ToList());

            InternalReviewProgressArray = GetProgressArray(publications.Select(p => p.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString()).Min(rc => rc.ComplesionDate)).ToList());

            ExternalReviewProgressArray = GetProgressArray(publications.Where(p => p.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString()) >= 2)
                .Select(p => p.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString()).Skip(1).Min(rc => rc.ComplesionDate)).ToList());

            RegistrationsAverageArray = GetAverageArray(users.Select(u => u.CreateDate ?? default(DateTime)).ToList());

            InternalReviewAverageArray = GetAverageArray(publications.Select(p => p.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString()).Min(rc => rc.ComplesionDate)).ToList());

            ExternalReviewAverageArray = GetAverageArray(publications.Where(p => p.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString()) >= 2)
                .Select(p => p.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString()).Skip(1).Min(rc => rc.ComplesionDate)).ToList());

            var agreementRatios = publications.Where(
                p => p.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString()) >= 2)
                .Select(
                    p =>
                        CalculateAgreement(p.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString()))).ToList();

            ReviewQualityArray = GetReviewQualityArray(agreementRatios);
        }

        public double[,] RegistrationsProgressArray { get; set; }

        public double[,] ReviewQualityArray { get; set; }

        public double[,] InternalReviewProgressArray { get; set; }

        public double[,] ExternalReviewProgressArray { get; set; }

        public double[,] RegistrationsAverageArray { get; set; }

        public double[,] InternalReviewAverageArray { get; set; }

        public double[,] ExternalReviewAverageArray { get; set; }

        private static double CalculateAgreement(IEnumerable<ReviewCompletion> reviewCompletions)
        {
          var  SeniorReviews = reviewCompletions
        .OrderBy(rc => rc.ComplesionDate)
        .FirstOrDefault()
        .Reviews;

           var JuniorReviews = reviewCompletions
                    .OrderBy(rc => rc.ComplesionDate)
                    .Skip(1).FirstOrDefault()
                    .Reviews;

            var totalpoints = SeniorReviews.Count(r => r.Status == "Current" && r.OptionOptionID != null);

            var point = SeniorReviews.Select(correctRev => JuniorReviews
            .FirstOrDefault(r => r.ChecklistChecklistID == correctRev.ChecklistChecklistID && r.PublicationPublicationID == correctRev.PublicationPublicationID && r.OptionOptionID == correctRev.OptionOptionID))
            .Count(rev => rev != null);

            return (double)point * 100.0 / totalpoints;
        }

        private static double[,] GetProgressArray(IList<DateTime> RCDateTime)
        {
            if (RCDateTime == null) return null;

            //const double timeGrid = 30 * 24 * 3600;

            const double monthGrid = 1;

            var startDate = new DateTime(2015, 10, 01); // users.Min(rc => rc.CreateDate)??default(DateTime);

            var plotStartDate = new DateTime(2016, 01, 31,23,59,59);

            var plotEndDate = DateTime.Now;

            var nCut = (int)Math.Ceiling(((plotEndDate.Year - plotStartDate.Year)*12 + plotEndDate.Month - plotStartDate.Month+1) / monthGrid);  //(int)Math.Ceiling((endDate - startDate).TotalSeconds / timeGrid);

            var outputArray = new double[nCut, 2];

            for (var ii = 0; ii < nCut; ii++)
            {
                outputArray[ii, 0] = plotStartDate.AddMonths(ii).ToOADate(); // .AddSeconds(timeGrid * (ii + 1)).ToOADate();

                outputArray[ii, 1] = RCDateTime.Count(d => d >= startDate && d <= plotStartDate.AddMonths(ii));
            }

            return outputArray;
        }

        public static double[,] GetAverageArray(IList<DateTime> RCDateTime)
        {
            if (RCDateTime == null) return null;

            //const double timeGrid = 30 * 24 * 3600;

            const double monthGrid = 1;

            var startDate = new DateTime(2015, 10, 01); // users.Min(rc => rc.CreateDate)??default(DateTime);

            var plotStartDate = new DateTime(2016, 01, 31,23,59,59);

            var plotEndDate = DateTime.Now;

            var endDate = DateTime.Now;

            var nCut = (int)Math.Ceiling(((plotEndDate.Year - plotStartDate.Year) * 12 + plotEndDate.Month - plotStartDate.Month + 1) / monthGrid);  //(int)Math.Ceiling((endDate - startDate).TotalSeconds / timeGrid);

            //var nCut = (int)Math.Ceiling((endDate - startDate).TotalSeconds / timeGrid);

            var outputArray = new double[nCut, 2];

            for (var ii = 0; ii < nCut; ii++)
            {
                outputArray[ii, 0] = plotStartDate.AddMonths(ii).ToOADate(); // .AddSeconds(timeGrid * (ii + 1)).ToOADate();

                outputArray[ii, 1] = RCDateTime.Count(d => d >= plotStartDate.AddMonths(ii-1) && d <= plotStartDate.AddMonths(ii));

                //outputArray[ii, 0] = startDate.AddSeconds(timeGrid * (ii + 1)).ToOADate();

                //outputArray[ii, 1] = RCDateTime.Count(d => d >= startDate.AddSeconds(timeGrid * (ii)) && d <= startDate.AddSeconds(timeGrid * (ii + 1)));
            }

            return outputArray;
        }

        public static double[,] GetReviewQualityArray(IList<double> Ratio)
        {
            if (Ratio == null) return null;

            const double grid = 10;

            var lowLimit = 0;

            var highLimit = 100;

            var nCut = (int)Math.Ceiling((highLimit - lowLimit) / grid);

            var outputArray = new double[nCut, 2];

            for (var ii = 0; ii < nCut; ii++)
            {
                outputArray[ii, 0] = lowLimit + grid * (ii);

                outputArray[ii, 1] = Ratio.Count(d => d >= lowLimit + grid * (ii) && d <= lowLimit + grid * (ii + 1));
            }

            return outputArray;
        }
    }
}