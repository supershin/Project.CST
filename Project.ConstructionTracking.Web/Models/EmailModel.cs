namespace Project.ConstructionTracking.Web.Models
{
    public class EmailModel
    {

        public string? Host { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int PORT { get; set; }
        public string? From { get; set; }
        public string? Sender { get; set; }
        private List<string> _To;
        public List<string> To
        {
            get
            {
                if (_To == null)
                {
                    _To = new List<string>();
                }
                return _To;
            }
            set { _To = value; }
        }

        private List<string> _Bcc;
        public List<string> Bcc
        {
            get
            {
                if (_Bcc == null)
                {
                    _Bcc = new List<string>();
                }
                return _Bcc;
            }
            set { _Bcc = value; }
        }

        private List<string> _CC;
        public List<string> CC
        {
            get
            {
                if (_CC == null)
                {
                    _CC = new List<string>();
                }
                return _CC;
            }
            set { _CC = value; }
        }

        public string? Body { get; set; }
        public string? Subject { get; set; }
    }

}
