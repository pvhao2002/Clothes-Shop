using ClothesShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClothesShop.ViewModel
{
    public class ProfileViewModel
    {
        public List<order> myOrder { get; set; }
        public user currentUser { get; set; }
        public ProfileViewModel() { }
        public ProfileViewModel(List<order> myOrder, user u)
        {
            this.myOrder = myOrder;
            this.currentUser = u;
        }
    }
}