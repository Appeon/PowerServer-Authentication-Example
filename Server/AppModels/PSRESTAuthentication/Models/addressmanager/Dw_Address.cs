using DWNet.Data;
using SnapObjects.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSRESTAuthentication
{
    [DataWindow("dw_address", DwStyle.Grid)]
    [Table("address", Schema = "person")]
    #region DwSelectAttribute  
    [DwSelect("PBSELECT( VERSION(400) TABLE(NAME=\"person.address\" ) @(_COLUMNS_PLACEHOLDER_) WHERE(    EXP1 =\"\\\"person\\\".\\\"address\\\".\\\"addressid\\\"\"   OP =\">\"    EXP2 =\":id\" ) ) ORDER(NAME=\"person.address.addressid\" ASC=yes ) ARG(NAME = \"id\" TYPE = number)")]
    #endregion
    [DwParameter("id", typeof(double?))]
    [UpdateWhereStrategy(UpdateWhereStrategy.KeyAndConcurrencyCheckColumns)]
    [DwKeyModificationStrategy(UpdateSqlStrategy.DeleteThenInsert)]
    public class Dw_Address
    {
        [Key]
        [Identity]
        [SqlDefaultValue("autoincrement")]
        [DwColumn("person.address", "addressid")]
        public int? Addressid { get; set; }

        [ConcurrencyCheck]
        [DwColumn("person.address", "addressline1")]
        public string Addressline1 { get; set; }

        [ConcurrencyCheck]
        [DwColumn("person.address", "city")]
        public string City { get; set; }

        [ConcurrencyCheck]
        [DwColumn("person.address", "stateprovinceid")]
        public int? Stateprovinceid { get; set; }

        [ConcurrencyCheck]
        [DwColumn("person.address", "postalcode")]
        public string Postalcode { get; set; }

        [ConcurrencyCheck]
        [SqlDefaultValue("\"getdate\"()")]
        [DwColumn("person.address", "modifieddate", TypeName = "timestamp")]
        public DateTime? Modifieddate { get; set; }

    }

}