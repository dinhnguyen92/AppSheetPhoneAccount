using System;
using System.Collections.Generic;

namespace PhoneAccountService
{
    class AccNameCompare : IComparer<PhoneAcc>
    {
        public int Compare(PhoneAcc x, PhoneAcc y)
        {
            return String.Compare(x.name, y.name);
        }
    }
}
