using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.Models
{
    public class Income: IEquatable<Income>
    {
        public int Id { get; set; }
        public double Summa { get; set; }
        public string DateOperation { get; set; }
        public int PurseId { get; set; }
        public int SourceOfIncomeId { get; set; }
        public string Comment { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public Purse Purse { get; set; }
        public SourceOfIncome SourceOfIncome { get; set; }

        public bool Equals(Income other)
        {
            //Check whether the compared object is null. 
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal. 
            return SourceOfIncomeId.Equals(other.SourceOfIncomeId);
        }
        public override int GetHashCode()
        {

            //Get hash code for the Name field if it is not null. 
            int hashSourceOfIncomeId = SourceOfIncomeId == null ? 0 : SourceOfIncomeId.GetHashCode();

            //Get hash code for the Code field. 
            //int hashProductCode = Code.GetHashCode();

            //Calculate the hash code for the product. 
            return hashSourceOfIncomeId;
        }
    }
}
