using System;

namespace Project.ConstructionTracking.Web.Models.MFormModel
{
    /// <summary>
    /// โครงสร้างแบบฟอร์มการตรวจงวดงานทั้ง 4 ระดับ ใช้สำหรับ Export Excel
    /// Lv.1 Form -> Lv.2 FormGroup -> Lv.3 FormPackage -> Lv.4 FormCheckList
    /// </summary>
    public class ExportFormStructureModel
    {
        public int FormTypeID { get; set; }
        public string? FormTypeName { get; set; }
        public string? FormTypeDesc { get; set; }
        public string? ProjectTypeName { get; set; }
        public List<ExportFormLevel> Forms { get; set; } = new List<ExportFormLevel>();

        public int TotalForm => Forms.Count;
        public int TotalGroup => Forms.Sum(f => f.Groups.Count);
        public int TotalPackage => Forms.Sum(f => f.Groups.Sum(g => g.Packages.Count));
        public int TotalCheckList => Forms.Sum(f => f.Groups.Sum(g => g.Packages.Sum(p => p.CheckLists.Count)));
    }

    public class ExportFormLevel
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Progress { get; set; }
        public int? DurationDay { get; set; }
        public List<string> QcList { get; set; } = new List<string>();
        public List<ExportGroupLevel> Groups { get; set; } = new List<ExportGroupLevel>();

        public int TotalPackage => Groups.Sum(g => g.Packages.Count);
        public int TotalCheckList => Groups.Sum(g => g.Packages.Sum(p => p.CheckLists.Count));
    }

    public class ExportGroupLevel
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public List<ExportPackageLevel> Packages { get; set; } = new List<ExportPackageLevel>();

        public int TotalCheckList => Packages.Sum(p => p.CheckLists.Count);
    }

    public class ExportPackageLevel
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public List<ExportCheckListLevel> CheckLists { get; set; } = new List<ExportCheckListLevel>();
    }

    public class ExportCheckListLevel
    {
        public int ID { get; set; }
        public string? Name { get; set; }
    }
}
