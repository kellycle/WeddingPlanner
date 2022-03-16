using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner
{
    public class RestrictedDate : ValidationAttribute
    {
        public override bool IsValid(object value) 
        {
            DateTime date = (DateTime)value;
            return date > DateTime.Now;
        }
    }
}