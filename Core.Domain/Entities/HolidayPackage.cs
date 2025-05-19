using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class HolidayPackage : Product
    {
        public HolidayPackage(string externalId, string name, decimal price, string description, string category, string provider, string duration, string imageUrl) : base(externalId, name, price, description, category, provider, duration, imageUrl)
        {
        }
    }
}
