﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Text;

namespace XNetCore.EF
{
    /// <summary>
    /// MsSql数据库操作
    /// </summary>
    class OracleExtension : DbContexExtension
    {

        public void UseDB(DbContextOptionsBuilder options, string constr)
        {
            options.UseOracle(constr);
        }
    }
}
