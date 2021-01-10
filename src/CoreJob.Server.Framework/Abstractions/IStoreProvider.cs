using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace CoreJob.Server.Framework.Abstractions
{
    public interface IStoreProvider
    {
        void OptionsAction(DbContextOptionsBuilder options);
    }
}
