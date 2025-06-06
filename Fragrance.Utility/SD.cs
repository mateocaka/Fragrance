﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fragrance.Utility
{
    public static class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Company = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "DelayedPayment";
        public const string PaymentStatusRejected = "Rejected";

        public static readonly List<string> ScentProfiles = new List<string>
        {
            "Floral", "Woody", "Citrus", "Oriental", "Gourmand"
        };
        public static List<string> Genders = new List<string> { "Male", "Female", "Unisex" };
        public const string SessionCart = "SessionShoppingCart";

    }
}
