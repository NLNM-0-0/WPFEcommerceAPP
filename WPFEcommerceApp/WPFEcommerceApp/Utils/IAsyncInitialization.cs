using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp
{
    public interface IAsyncInitialization
    {
        Task InitializeAsync { get; }
    }
}
