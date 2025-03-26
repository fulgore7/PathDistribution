using Microsoft.Ajax.Utilities;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.SharePoint.Client;
using SPFramework.Data;
using SPFramework.Data.DAL;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;


namespace PathDistribution.Models.DAL
{
    public class AdminDAL: SPDAL
    {

#if DEBUG
        public AdminDAL() : base("CopathDistributionTestSPWH", ConnectionType.UseSecurityLoader)
        { }
#else
        public AdminDAL() : base("CopathDistributionProd", ConnectionType.UseSecurityLoader)
        { }
#endif

        public bool IsAdmin(string user)
        {
            return Query("SELECT ISNULL((SELECT [tblPaths].[bitIsAdmin] FROM [dbo].[tblPaths] INNER JOIN coplive.dbo.c_d_person c_d_person ON tblPaths.[pkPath] = c_d_person.[id] WHERE c_d_person.user_identifier = @UserName),0) AS bitIsAdmin")
                                .AddParameter("@UserName", user, System.Data.SqlDbType.VarChar, 50)
                                .ExecuteScalar<bool>();
        }

        public List<Pathologists> GetPathologists()
        {
            return StoredProcedure("uspSchedGetPathologists")
                    .FetchAll<Pathologists>();
        }

        public List<Pathologists> GetPathSchedPaths(DateTime start, DateTime end)
        {
            return StoredProcedure("rptSchedPath_PathData")
                    .AddParameter("@StartDate", start, System.Data.SqlDbType.Date)
                    .AddParameter("@EndDate", end, System.Data.SqlDbType.Date)
                    .FetchAll<Pathologists>();
        }

        public void UpsertPatholgist(Pathologists p)
        {
            StoredProcedure("uspSchedUpsertPathologists")
                .AddParameter("@pkPath", p.pkPath, System.Data.SqlDbType.VarChar, 15)
                .AddParameter("@bitHeme", p.bitHeme, System.Data.SqlDbType.Bit)
                .AddParameter("@bitCyto", p.bitCyto, System.Data.SqlDbType.Bit)
                .AddParameter("@bitSkin", p.bitSkin, System.Data.SqlDbType.Bit)
                .AddParameter("@bitGeneral", p.@bitGeneral, System.Data.SqlDbType.Bit)
                .AddParameter("@bitGU", p.bitGU, System.Data.SqlDbType.Bit)
                .AddParameter("@intGI", p.intGI, System.Data.SqlDbType.Int)
                .AddParameter("@bitActive", p.bitActive, System.Data.SqlDbType.Bit)
                .AddParameter("@intOrder", p.intOrder, System.Data.SqlDbType.Int)
                .Execute();
        }

        public void DeletePatholgist(string pkPath)
        {
            StoredProcedure("uspSchedDeletePathologist")
                .AddParameter("@pkPath", pkPath, System.Data.SqlDbType.VarChar, 15)
                .Execute();
        }

        public List<Pathologists> GetCopathPathologists(string filter)
        {
            return StoredProcedure("uspSchedGetCopathPathologists")
                    .AddParameter("@filter",filter,System.Data.SqlDbType.VarChar,50)
                    .FetchAll<Pathologists>();
        }

        public List<Assignments> GetAssignments()
        {
            return StoredProcedure("uspSchedGetAssignments")
                    .FetchAll<Assignments>();
        }

        public int UpsertAssignments(Assignments assignments)
        {
            return StoredProcedure("uspSchedUpsertAssignments")
                        .AddParameter("@pkAssignment", assignments.pkAssignment, System.Data.SqlDbType.Int)
                        .AddParameter("@chrAbbr", assignments.chrAbbr, System.Data.SqlDbType.VarChar, 10)
                        .AddParameter("@chrAssignment", assignments.chrAssignment, System.Data.SqlDbType.VarChar, 100)
                        .AddParameter("@intMaxSlideCount", assignments.intMaxSlideCount, System.Data.SqlDbType.Int)
                        .AddParameter("@intAssignmentType", (int)assignments.intAssignmentType, System.Data.SqlDbType.Int)
                        .AddParameter("@bitActive", assignments.bitActive, System.Data.SqlDbType.Bit)
                        .AddParameter("@intOrder", 0, System.Data.SqlDbType.Int)
                        .ExecuteScalar<int>();
        }

        public void DeleteAssignment(int pkAssignment)
        {
            StoredProcedure("uspSchedDeleteAssignment")
                .AddParameter("@pkAssignment", pkAssignment, System.Data.SqlDbType.Int)
                .Execute();
        }

        public AssignmentConflicts GetAssignmentConflicts()
        {
            AssignmentConflicts ac = new AssignmentConflicts
            {
                Conflicts = StoredProcedure("uspSchedGetAssignmentConflicts")
                            .FetchAll<AssignmentConflict>()
            };
            return ac;
        }

        public void MoveAssignmentConflict(MoveAssignmentConflict mac)
        {
            StoredProcedure("uspSchedMoveAssignmentConflict")
                .AddParameter("@pkAssignmentConflict", mac.pkAssignmentConflict, System.Data.SqlDbType.Int)
                .AddParameter("@intNewConflictType", mac.intNewConflictType, System.Data.SqlDbType.Int)
                .Execute();
        }

        public List<CannedComment> GetCannedComments()
        {
            return StoredProcedure("uspSchedGetCannedComments")
                    .FetchAll<CannedComment>();
        }

        public int UpsertCannedComments(CannedComment cannedComment,bool bitDelete)
        {
            return StoredProcedure("uspSchedUpsertCannedComment")
                        .AddParameter("@pkComment", cannedComment.pkComment, System.Data.SqlDbType.Int)
                        .AddParameter("@chrComment", cannedComment.chrComment, System.Data.SqlDbType.VarChar, 100)
                        .AddParameter("@bitDelete", bitDelete, System.Data.SqlDbType.Bit)
                        .ExecuteScalar<int>();
        }

        public ScheduleData GetSchedules(string weekyear)
        {
            ScheduleData data = new ScheduleData
            {
                Assignments = new List<string>(),
                WeekEnd = new List<SelectListItem>(),
                OffTypes = new List<Tuple<string, string>>()
            };

            MultiResult rs = StoredProcedure("uspSchedGetSchedule")
                .AddParameter("@dateText",weekyear,System.Data.SqlDbType.VarChar,7)
                .Map(x => { data.Assignments.Add(x.GetString(0)); return x.GetString(0); })
                .Map<Schedule>()
                .Map<OffSchedule>()
                .Map<ScheduleComments>()
                .Map(x => { data.WeekEnd.Add(new SelectListItem() { Value = x.GetString(0), Text = x.GetString(0), Selected = x.GetBoolean(1) }); return x.GetString(0); })
                .Map<ScheduleIssues>()
                .Map(reader =>
                {
                    data.OffTypes.Add(new Tuple<string, string>(reader.GetString(0), reader.GetString(1)));
                    return null;
                })
                .FetchMultiple();
            
            data.Schedules = rs.FetchAll<Schedule>(1);
            data.OffSchedule = rs.FetchAll<OffSchedule>(2);
            data.Comments = rs.FetchAll<ScheduleComments>(3);
            data.Issues = rs.FetchAll<ScheduleIssues>(5);

            return data;
        }

        public List<PathSchedWeek> GetSchedWeekPathList(string chrAbbr, DateTime dteAssigned)
        {
            return StoredProcedure("uspSchedPathList")
                    .AddParameter("@chrAbbr", chrAbbr, System.Data.SqlDbType.VarChar, 10)
                    .AddParameter("@dteAssigned", dteAssigned, System.Data.SqlDbType.Date)
                    .FetchAll<PathSchedWeek>();
        }

        public List<PathOffScheduleWeek> GetSchedWeekPathOffList(string chrPath, DateTime dteAssigned)
        {
            return StoredProcedure("uspSchedGetWeekPathOffList")
                    .AddParameter("@chrPath", chrPath, System.Data.SqlDbType.VarChar, 10)
                    .AddParameter("@dteAssigned", dteAssigned, System.Data.SqlDbType.Date)
                    .FetchAll<PathOffScheduleWeek>();
        }

        public List<ScheduleComments> UpsertSchedComment(ScheduleComments sc)
        {
            return StoredProcedure("uspSchedUpsertComment")
                        .AddParameter("@pkComment", sc.pkComment, System.Data.SqlDbType.Int)
                        .AddParameter("@dteWeek", sc.dteWeek, System.Data.SqlDbType.Date)
                        .AddParameter("@chrComment", sc.chrComment, System.Data.SqlDbType.VarChar, 100)
                        .FetchAll<ScheduleComments>();
        }

        public void UpsertSchedPath(SchedulePath sp)
        {
            StoredProcedure("uspSchedUpsertPath")
                .AddParameter("@chrAbbr", sp.chrAbbr, System.Data.SqlDbType.VarChar, 10)
                .AddParameter("@dteAssigned", sp.dteAssigned, System.Data.SqlDbType.Date)
                .AddParameter("@chrPath", sp.chrPath, System.Data.SqlDbType.VarChar, 4)
                .Execute();
        }

        public void UpsertSchedOffPath(SchedulePath sp)
        {
            StoredProcedure("uspSchedUpsertOffPath")
                .AddParameter("@chrAbbr", sp.chrAbbr, System.Data.SqlDbType.VarChar, 10)
                .AddParameter("@dteAssigned", sp.dteAssigned, System.Data.SqlDbType.Date)
                .AddParameter("@chrPath", sp.chrPath, System.Data.SqlDbType.VarChar, 4)
                .Execute();
        }

        public EligibilityData GetEligibilities()
        {
            EligibilityData data = new EligibilityData
            {
                Assignments = new List<Tuple<string, string>>()
            };

            MultiResult result = StoredProcedure("uspSchedGetEligibilities")
                                    .Map<Pathologists>()
                                    .Map<Eligibility>()
                                    .Map(reader =>
                                    {
                                        data.Assignments.Add(new Tuple<string, string>(reader.GetString(0), reader.GetString(1)));
                                        return null;
                                    })
                                    .FetchMultiple();

            data.Paths = result.FetchAll<Pathologists>(0);
            data.Eligibilities = result.FetchAll<Eligibility>(1);

            return data;
        }

        public int UpdateEligibility(Eligibility e)
        {
            return StoredProcedure("uspSchedUpdateEligibility")
                        .AddParameter("@chrPath", e.chrPath, System.Data.SqlDbType.VarChar,10)
                        .AddParameter("@chrAbbr", e.chrAbbr, System.Data.SqlDbType.VarChar,10)
                        .AddParameter("@intEligibility", (int)e.intEligibility, System.Data.SqlDbType.TinyInt)
                        .AddParameter("@intCredited", e.intCredited, System.Data.SqlDbType.Int)
                        .ExecuteScalar<int>();
        }

        public List<VacationBalance> GetVacationBalances()
        {
            return StoredProcedure("uspSchedGetVacationBalances")
                    .Map<VacationBalance>()
                    .FetchAll<VacationBalance>();
        }
        //Returns a list of vacation schedules for the calendar view
        public List<PathScheduleDatesCal> GetVacationSchedulesCal(DateTime? start, DateTime? end)
        {

            PathScheduleDatesCal data = new PathScheduleDatesCal
            {
                Paths = new List<string>(),
                Assignments = new List<Tuple<string, string>>()
            };
            MultiResult rs = StoredProcedure("uspSchedGetVacationSchedulesCal")
                 .AddParameter("@start", start ?? null, System.Data.SqlDbType.Date)
                .AddParameter("@end", end ?? null, System.Data.SqlDbType.Date)
                .Map<PathScheduleDatesCal>()
                .Map(reader =>
                {
                    data.Paths.Add(reader.GetString(0));
                    return null;
                })
                .Map(reader =>
                {
                    data.Assignments.Add(new Tuple<string, string>(reader.GetString(0), reader.GetString(1)));
                    return null;
                })
                .Map<PTORequest>()
                .FetchMultiple();

            Parallel.Invoke(() =>  { data.PTORequests = rs.FetchAll<PTORequest>(5); });

            return data;



            //return StoredProcedure("uspSchedGetVacationSchedulesCal")
            //             .Map<PathScheduleDatesCal>()

        }
        public VacationSchedules GetVacationSchedules(DateTime? start, DateTime? end)
        {
            VacationSchedules data = new VacationSchedules
            {
                Paths = new List<string>(),
                Assignments = new List<Tuple<string, string>>()
            };

            MultiResult rs = StoredProcedure("uspSchedGetVacationSchedules")
                .AddParameter("@start",start ?? null, System.Data.SqlDbType.Date)
                .AddParameter("@end", end ?? null, System.Data.SqlDbType.Date)
                .Map(reader =>
                {
                    data.Dates = new Tuple<DateTime, DateTime>(reader.GetDateTime(0), reader.GetDateTime(1));
                    return null;
                })
                .Map(reader =>
                {
                    data.Paths.Add(reader.GetString(0));
                    return null;
                })
                .Map(reader =>
                {
                    data.Assignments.Add(new Tuple<string,string>(reader.GetString(0),reader.GetString(1)));
                    return null;
                })
                .Map<PathScheduleData>()
                .Map<PathScheduleDates>()
                .Map<PTORequest>()
                .FetchMultiple();

            Parallel.Invoke(() => { data.PathScheduleData = rs.FetchAll<PathScheduleData>(3); },
                            () =>
                            {
                                data.PathScheduleDates = rs.FetchAll<PathScheduleDates>(4)
                                                           .GroupBy(a => (a.dte, a.WeekNum, a.WkDay, a.isHoliday))
                                                           .Select(b => new PathScheduleDates()
                                                           {
                                                               dte = b.Key.dte,
                                                               WeekNum = b.Key.WeekNum,
                                                               WkDay = b.Key.WkDay,
                                                               isHoliday = b.Key.isHoliday,
                                                               OffAssignments = b.Select(d => new Tuple<string, string, string>(d.chrPath, d.chrValue, d.chrColor ?? string.Empty)).ToList()
                                                           }).ToList();
                            },
                            () => { data.PTORequests = rs.FetchAll<PTORequest>(5); });
            return data;
        }

        public int UpsertBalances(VacationBalance vb)
        {
            return StoredProcedure("uspSchedUpsertVacationBalance")
                        .AddParameter("@pkCredit", vb.pkCredit, System.Data.SqlDbType.Int)
                        .AddParameter("@chrPath", vb.chrPath, System.Data.SqlDbType.Char,3)
                        .AddParameter("@intYear", vb.intYear, System.Data.SqlDbType.Int)
                        .AddParameter("@intCredited", vb.intCredited, System.Data.SqlDbType.Int)
                        .AddParameter("@chrComment", vb.chrComments, System.Data.SqlDbType.VarChar, Int32.MaxValue)
                        .ExecuteScalar<int>();
        }

        //        public void UpsertPTO(string user, string startDate, string endDate, string offType, string comments)
        //        {
        //#if DEBUG
        //            user = "jzacks";
        //#endif
        //            StoredProcedure("uspSchedInsertPTO")
        //                .AddParameter("@user", user, System.Data.SqlDbType.VarChar, 50)
        //                .AddParameter("@dteStartDate", startDate, System.Data.SqlDbType.VarChar, 20)
        //                .AddParameter("@dteEndDate", endDate, System.Data.SqlDbType.VarChar, 20)                
        //                .AddParameter("@offType", offType, System.Data.SqlDbType.VarChar, 255)
        //                .AddParameter("@chrComments", comments, System.Data.SqlDbType.VarChar, 255)
        //                .Execute();
        //        }

        public bool CheckPTO()
        {
            return StoredProcedure("uspSchedCheckPTO")
                .ExecuteScalar<bool>();
        }

        //        public void AppRejPTO(string user, int intStatus, int pkPTO)
        //        {
        //            Console.WriteLine("Acc Rej:" + user + " " + intStatus + " " + pkPTO);

        //            //StoredProcedure("uspSchedUpdatePTO")
        //            //    .AddParameter("@intStatus", intStatus, System.Data.SqlDbType.Int)
        //            //    .AddParameter("@pkPTO", pkPTO, System.Data.SqlDbType.Int)
        //            //    .AddParameter("@dteStartDate", null, System.Data.SqlDbType.VarChar, 20)
        //            //    .AddParameter("@dteEndDate", null, System.Data.SqlDbType.VarChar, 20)
        //            //    .AddParameter("@offType", null, System.Data.SqlDbType.VarChar, 255)
        //            //    .AddParameter("@chrComments", null, System.Data.SqlDbType.VarChar, 255)
        //            //    .Execute();
        //        }

        public void UpsertPTO(int pkPTO, string chrPath, DateTime dteStart, DateTime dteEnd, int intStatus, string chrAbbr, string chrStatusChangedBy, DateTime dteStatusChanged, string chrComments)
        {
            StoredProcedure("uspSchedUpsertPTO")
                .AddParameter("@pkPTO", pkPTO, System.Data.SqlDbType.Int)
                .AddParameter("@chrPath", chrPath, System.Data.SqlDbType.VarChar,6)
                .AddParameter("@dteStart", dteStart, System.Data.SqlDbType.Date)
                .AddParameter("@dteEnd", dteEnd, System.Data.SqlDbType.Date)
                .AddParameter("@intStatus", intStatus, System.Data.SqlDbType.Int)
                .AddParameter("@chrAbbr", chrAbbr, System.Data.SqlDbType.VarChar, 10)
                .AddParameter("@chrStatusChangedBy", chrStatusChangedBy, System.Data.SqlDbType.VarChar, 50)
                .AddParameter("@dteStatusChanged", dteStatusChanged, System.Data.SqlDbType.DateTime)
                .AddParameter("@chrComments", chrComments, System.Data.SqlDbType.VarChar, 255)
                .Execute();
        }

        public string GetCopathAbbr(string user)
        {
            return StoredProcedure("uspSchedGetCopathAbbr")
                        .AddParameter("@chrUser", user, System.Data.SqlDbType.VarChar, 50)
                        .ExecuteScalar<string>();
                
        }
    }
}