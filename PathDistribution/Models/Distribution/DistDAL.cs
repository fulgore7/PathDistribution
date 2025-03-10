using SPFramework.Data;
using SPFramework.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathDistribution.Models.DAL
{
    public class DistDAL: SPDAL
    {

#if DEBUG
        public DistDAL() : base("CopathDistributionTestSPWH", ConnectionType.UseSecurityLoader)
        { }
#else
        public DistDAL() : base("CopathDistributionProd", ConnectionType.UseSecurityLoader)
        { }
#endif

        public void AddSlides(string userName, AddedSlide add)
        {
            StoredProcedure("uspAddSlides")
                .AddParameter("@dteAssigned", add.dteDateAssigned, System.Data.SqlDbType.Date)
                .AddParameter("@chrPath", add.chrPath, System.Data.SqlDbType.VarChar, 6)
                .AddParameter("@intSlideCount", add.intSlideCount, System.Data.SqlDbType.Int)
                .AddParameter("@chrUserName", userName, System.Data.SqlDbType.VarChar,255)
                .AddParameter("@chrComment", add.chrComment, System.Data.SqlDbType.VarChar, 255)
                .Execute();
        }

        public bool ResetDay(StartSettings settings)
        {
            return StoredProcedure("uspDeleteDay")
                .AddParameter("@Date", settings.ResetDist, System.Data.SqlDbType.Date)
                .ExecuteScalar<bool>();
        }

        public PathData GetPathData(StartSettings settings)
        {
            MultiResult rs = StoredProcedure("uspGetData")
                                .AddParameter("@StartAccession", settings.StartAccession, System.Data.SqlDbType.Date)
                                .AddParameter("@EndAccession", settings.EndAccession, System.Data.SqlDbType.Date)
                                .AddParameter("@intPriority", settings.Priority, System.Data.SqlDbType.Int)
                                .Map(x => {
                                    return new tblPathAssign()
                                    {
                                        dteAssigned = x.GetDateTime(0),
                                        chrName = x.GetString(1),
                                        chrAssignment = x.GetString(2),
                                        chrOverComment = x.GetString(3),
                                        bitOverride = x.GetBoolean(4)
                                    };
                                })
                                .Map(x => {
                                    return new tblPathAssign()
                                    {
                                        dteAssigned = x.GetDateTime(0),
                                        chrName = x.GetString(1),
                                        chrAssignment = x.GetString(2),
                                        chrOverComment = x.GetString(3),
                                        bitOverride = x.GetBoolean(4)
                                    };
                                })
                                .Map<PathDistribution>()
                                .Map<CaseDetails>()
                                .Map(x => { return x.GetString(0); })
                                .Map<Pathologist>()
                                .Map<TypeBreakdown>()
                                .Map<MinMax>()
                                .Map(x => { return x.GetString(0); })
                                .Map<AddedSlide>()
                                .FetchMultiple();

            PathData pa = new PathData();
            pa.PathAssign.AddRange(rs.FetchAll<tblPathAssign>(0));
            pa.OffPathAssign.AddRange(rs.FetchAll<tblPathAssign>(1));
            pa.OffPathAssign.OffAssignments = rs.FetchAll<string>(4);
            pa.PathDistribution = rs.FetchAll<PathDistribution>(2);
            pa.CaseDetails = rs.FetchAll<CaseDetails>(3);
            pa.TypeBreakdowns = rs.FetchAll<TypeBreakdown>(6);
            pa.cases.MinAndMax = rs.Fetch<MinMax>(7);
            pa.InvalidSchedule = rs.Fetch<string>(8);
            pa.AddedSlides = rs.FetchAll<AddedSlide>(9);

            //pa.AddedSlides = new List<AddedSlide>();

            //foreach (var item in pa.PathDistribution)
            //{
            //    pa.AddedSlides.Add(new AddedSlide() { dteDateAssigned = settings.Distribution, chrPath = item.chrPath, intSlideCount = item.AddedSlides });
            //}

            List<Pathologist> AllPaths = rs.FetchAll<Pathologist>(5);
            
            Parallel.ForEach(pa.CaseDetails, x =>
            {
                List<string> paths = x.pathList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (AllPaths.Exists(i => i.chrCopathAbbr == x.chrPath && i.bitConsultant == true) || x.PartGroup == "GI")
                {
                    paths = AllPaths.Where(i => paths.Contains(i.chrCopathAbbr)).Select(v => v.chrCopathAbbr).ToList();
                }
                else
                {
                    paths = AllPaths.Where(i => paths.Contains(i.chrCopathAbbr) && i.bitConsultant == false).Select(v => v.chrCopathAbbr).ToList();
                    //if (x.IsPlastic) paths.Add("VYR");
                }

                if (!paths.Contains(x.chrPath)) paths.Insert(0, x.chrPath);

                x.Paths = paths;
            });
            pa.PathAssign.Paths = AllPaths.Select(x => x.chrCopathAbbr).ToList();
            pa.OffPathAssign.Paths = AllPaths.Select(x => x.chrCopathAbbr).ToList();

            pa.Settings = settings;

            return pa;
        }
        
        public void CloneData(DateTime dte)
        {
            StoredProcedure("uspCloneData")
                .AddParameter("@dte", dte, System.Data.SqlDbType.Date)
                .Execute();
        }

        public void MoveCase(string userName, CaseMove cm)
        {
            StoredProcedure("uspMoveCase")
                .AddParameter("@dteAssigned", cm.dteDateAssigned, System.Data.SqlDbType.Date)
                .AddParameter("@chrCaseNumber", cm.chrCaseNumber, System.Data.SqlDbType.VarChar, 60)
                .AddParameter("@chrPath", cm.chrPath, System.Data.SqlDbType.VarChar, 6)
                .AddParameter("@chrComment", cm.chrComment, System.Data.SqlDbType.VarChar, 255)
                .AddParameter("@chrUserName", userName, System.Data.SqlDbType.VarChar, 255)
                .Execute();
        }

        public void ChangePathAssign(string userName, tblPathAssign pa)
        {
            StoredProcedure("uspChangePathAssign")
                .AddParameter("@dteAssigned", pa.dteAssigned, System.Data.SqlDbType.Date)
                .AddParameter("@chrAssignment", pa.chrAssignment, System.Data.SqlDbType.VarChar, 50)
                .AddParameter("@chrPath", pa.chrName, System.Data.SqlDbType.VarChar, 6)
                .AddParameter("@bitOverride", pa.bitOverride, System.Data.SqlDbType.Bit)
                .AddParameter("@chrComment", pa.chrOverComment, System.Data.SqlDbType.VarChar, 255)
                .AddParameter("@chrUserName", userName, System.Data.SqlDbType.VarChar, 255)
                .Execute();
        }

        public void ChangeOffPathAssign(string userName, tblPathAssign pa)
        {
            StoredProcedure("uspChangeOffPathAssign")
                .AddParameter("@dteAssigned", pa.dteAssigned, System.Data.SqlDbType.Date)
                .AddParameter("@chrAssignment", pa.chrAssignment, System.Data.SqlDbType.VarChar, 50)
                .AddParameter("@chrPath", pa.chrName, System.Data.SqlDbType.VarChar, 6)
                .AddParameter("@bitOverride", pa.bitOverride, System.Data.SqlDbType.Bit)
                .AddParameter("@chrComment", pa.chrOverComment, System.Data.SqlDbType.VarChar, 255)
                .AddParameter("@chrUserName", userName, System.Data.SqlDbType.VarChar, 255)
                .AddParameter("@bitDelete", pa.bitDelete, System.Data.SqlDbType.Bit)
                .Execute();
        }

        public void RunData(StartSettings settings, bool bitGetSchedule)
        {
#if DEBUG
            StoredProcedure("uspRunData_Test")
                .AddParameter("@dteStartAccession", settings.StartAccession, System.Data.SqlDbType.Date)
                .AddParameter("@dteEndAccession", settings.EndAccession, System.Data.SqlDbType.Date)
                .AddParameter("@intPriority", settings.Priority, System.Data.SqlDbType.Int)
                .AddParameter("@bitGetSchedule", bitGetSchedule, System.Data.SqlDbType.Bit)
                .Execute();
#else
            StoredProcedure("uspRunData")
                .AddParameter("@dteStartAccession", settings.StartAccession, System.Data.SqlDbType.Date)
                .AddParameter("@dteEndAccession", settings.EndAccession, System.Data.SqlDbType.Date)
                .AddParameter("@intPriority", settings.Priority, System.Data.SqlDbType.Int)
                .AddParameter("@bitGetSchedule", bitGetSchedule, System.Data.SqlDbType.Bit)
                .Execute();
#endif
        }

        public ProcessTypes IsProcessing(DateTime dte1, DateTime dte, Priorities Priority)
        {
            return (ProcessTypes)Enum.Parse(typeof(ProcessTypes), StoredProcedure("uspIsProcessing")
                                                                    .AddParameter("@dte1", dte1, System.Data.SqlDbType.Date)
                                                                    .AddParameter("@dte", dte, System.Data.SqlDbType.Date)
                                                                    .AddParameter("@Priority", Priority, System.Data.SqlDbType.Int)
                                                                    .ExecuteScalar<int>().ToString());
        }

        public void DownloadCopath(DateTime start, DateTime end)
        {
            StoredProcedure("uspDownloadCopath")
                .AddParameter("@StartAccession", start, System.Data.SqlDbType.Date)
                .AddParameter("@EndAccession", end, System.Data.SqlDbType.Date)
                .Execute();
        }

        public void Generate(StartSettings settings, string userName)
        {
            StoredProcedure("uspGenerate")
                .AddParameter("@dteStartAccession", settings.StartAccession, System.Data.SqlDbType.Date)
                .AddParameter("@dteEndAccession", settings.EndAccession, System.Data.SqlDbType.Date)
                .AddParameter("@intPriority", settings.Priority, System.Data.SqlDbType.Int)
                .AddParameter("@dteAssigned", settings.Distribution, System.Data.SqlDbType.Date)
                .AddParameter("@chrUserName", userName, System.Data.SqlDbType.VarChar, 255)
                .Execute();
        }

        public StartSettings GetDates()
        {
            return StoredProcedure("uspGetDates")
                        .Map(reader => {
                            return new StartSettings() {
                                StartAccession = reader.GetDateTime(0),
                                EndAccession = reader.GetDateTime(1),
                                Priority = (Priorities)Enum.Parse(typeof(Priorities), reader.GetInt32(3).ToString()),
                                ResetDist = reader.GetDateTime(2)
                            };
                        })
                        .Fetch<StartSettings>();
        }

        public List<OffType> GetOffTypes()
        {
            return StoredProcedure("uspGetOffTypes").FetchAll<OffType>();
        }
    }
}