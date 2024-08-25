using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClothesShop.ViewModel
{
    public class HomeAdminViewModel
    {
        public int totalProduct { get; set; }
        public int totalOrder { get; set; }
        public decimal? totalRevenue { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public HomeAdminViewModel() { }
        public HomeAdminViewModel(int p, int o, decimal? r)
        {
            this.totalRevenue = r;
            this.totalProduct = p;
            this.totalOrder = o;
        }
    }
}