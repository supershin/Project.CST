namespace Project.ConstructionTracking.Web.Models
{
    public class DTParamModel
    {
        public int draw { get; set; }
        public int length { get; set; }
        public int start { get; set; }
        public int TotalRowCount { get; set; }
        public Dictionary<string, string> search { get; set; }
        public List<Dictionary<string, string>> order { get; set; }
        public List<Dictionary<string, string>> columns { get; set; }

        public string SortColumnName
        {
            get
            {
                string columnName = string.Empty;
                if (this.order != null)
                {
                    if (this.order.Count > 0)
                    {
                        if (columns != null)
                        {
                            int iColumn = Convert.ToInt32(order[0].Where(e => e.Key == "column").FirstOrDefault().Value);
                            columnName = columns[iColumn].Where(e => e.Key == "data").FirstOrDefault().Value;
                        }

                    }
                }

                return columnName;
            }
        }

        public string sortDirection
        {
            get
            {
                string dir = string.Empty;
                if (this.order != null)
                {
                    if (this.order.Count > 0)
                    {
                        dir = order[0].Where(e => e.Key == "dir").FirstOrDefault().Value;
                    }
                }

                return dir;
            }
        }
        public SortDirection iSortDirection
        {
            get
            {
                return (this.sortDirection.ToLower() == "asc") ? SortDirection.Ascending : SortDirection.Descending;
            }
        }
    }
    public enum SortDirection
    {
        Ascending = 0,
        Descending = 1
    }
}
