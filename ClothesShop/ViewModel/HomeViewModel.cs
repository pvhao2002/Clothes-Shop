using ClothesShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClothesShop.ViewModel
{
    public class HomeViewModel
    {
        public List<category> categoryList;
        public List<slider> sliderList;
        public List<product> productList;

        public HomeViewModel() { }
        public HomeViewModel(List<slider> sliderList, List<product> productList = null)
        {
            this.sliderList = sliderList;
            this.productList = productList;
        }
    }
}