using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AjeGroupCore.Helpers
{
    public class Helpers
    {
        public static string PhoneFormatter(string phone)
        {
            var myPhone = phone.Replace("(", string.Empty);
            myPhone = myPhone.Replace(")", string.Empty);
            myPhone = myPhone.Replace("-", string.Empty);
            myPhone = myPhone.Replace(" ", string.Empty);

            return myPhone;
        }


    }
}
