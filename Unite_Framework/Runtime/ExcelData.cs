using System.Data;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExcelDataReader;

namespace Unite
{
    public class ExcelReader
    {
        public string path;
        private DataSet data;
        public virtual int LoadData(int rowOffset, int colOffset) { return 1; }
        public virtual int SaveData(DataSet _data) { return 1; }
        public virtual DataTable GetTable(int index) { return data.Tables[index]; }
        public virtual DataTable GetTable(string name) { return data.Tables[name]; }
        public virtual DataRow GetRow(string table, int index)
        {
            return GetTable(table).Rows[index];
        }
    }
}