using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProStickers.ViewModel.Master
{
   public class SalesReportViewModel
    {
        public string UserName { get; set; }
        public int NoOfDesigns { get; set; }
        public int NoOfOrder { get; set; }
        public double Amount { get; set; }
        public string UserID { get; set; }
    }
}
