using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using WebSaleDistribute.Models;

namespace WebSaleDistribute.Core
{
    public static class DatabaseContext
    {
        public static List<ComboBoxDataModel> GetWarehouses(bool disableNonDefaults, bool visiableSelectAll, int defaultStore = 30)
        {
            // fetch stores by caching data
            var exceptedStores = new DataTable();
            exceptedStores.Columns.Add("Id");
            var rundate = DateTime.Now.GetPersianDateNumber();
            var stores = Connections.SaleBranch.SqlConn.Query("sp_GetAllStores",
                new { ExceptWHCodeList = exceptedStores, RunDate = rundate },
                commandType: CommandType.StoredProcedure);

            var comboData = new List<ComboBoxDataModel>();
            foreach(var store in stores)
            {
                if (!visiableSelectAll && store.WHCode?.ToString() == "0") continue;

                var val = store.WHCode?.ToString();
                comboData.Add(new ComboBoxDataModel()
                {
                    Value = val,
                    Text = store.WHName?.ToString(),
                    Enabled = (!disableNonDefaults || val == defaultStore.ToString()), // Enable all or disable all except default value
                    Selected = (val == defaultStore.ToString())
                });
            }

            return comboData;
        }
    }
}