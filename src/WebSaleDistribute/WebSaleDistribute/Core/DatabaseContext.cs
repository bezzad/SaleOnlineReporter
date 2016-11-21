using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Core
{
    public static class DatabaseContext
    {
        public static List<ComboBoxDataModel> GetWarehouses(bool disableNonDefaults, bool visiableSelectAll)
        {
            var defaultValue = "30"; // TODO: this option must be declare dynamically not HARD CODE

            // fetch stores by caching data
            var exceptedStores = new DataTable();
            exceptedStores.Columns.Add("Id");

            var stores = Connections.SaleBranch.SqlConn.ExecuteReader("sp_GetAllStores",
                new { ExceptWHCodeList = exceptedStores, RunDate = DateTime.Now.GetPersianDate() },
                commandType: CommandType.StoredProcedure);

            var comboData = new List<ComboBoxDataModel>();
            while (stores.Read())
            {
                if (!visiableSelectAll && stores["WHCode"]?.ToString() == "0") continue;

                var val = stores["WHCode"]?.ToString();
                comboData.Add(new ComboBoxDataModel()
                {
                    Value = val,
                    Text = stores["WHName"]?.ToString(),
                    Enabled = (!disableNonDefaults || val == defaultValue), // Enable all or disable all except default value
                    Selected = (val == defaultValue)
                });
            }

            return comboData;
        }
    }
}