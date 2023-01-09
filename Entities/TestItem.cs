using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
//using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlTransactUtility.Entities
{
    [Table(Name = "TestTable")]
    public class TestItem
    {
        private int _RowNum;
        private int _SerialNo;
        private string _Name;
        private string _Description;
        private float _MeasurementValue;
        private string _Status;
        private DateTime _Timestamp;

        [Column(DbType = "int")]
        public int RowNum
        {
            get { return _RowNum; }
            set { _RowNum = value; }
        }

        [Column(DbType = "int")]
        public int SerialNo
        {
            get { return _SerialNo; }
            set { _SerialNo = value; }
        }

        [Column(DbType = "nvarchar(16)")]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        [Column(DbType = "nvarchar(32)")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        [Column(DbType = "float")]
        public float MeasurementValue
        {
            get { return _MeasurementValue; }
            set { _MeasurementValue = value; }
        }

        [Column(DbType = "nvarchar(4)")]
        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        [Column(DbType = "datetime")]
        public DateTime Timestamp
        {
            get { return _Timestamp; }
            set { _Timestamp = value; }
        }
    }
}

